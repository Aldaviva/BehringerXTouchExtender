using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

public abstract class BehringerXTouchExtender<TRotaryEncoder>: IBehringerXTouchExtender<TRotaryEncoder>
    where TRotaryEncoder: IRotaryEncoder {

    protected const int trackCount = 8;
    public int TrackCount => trackCount;

    internal readonly MidiClient MidiClient = new();

    private readonly IlluminatedButtonImpl[] _recordButtons  = new IlluminatedButtonImpl[trackCount];
    private readonly IlluminatedButtonImpl[] _soloButtons    = new IlluminatedButtonImpl[trackCount];
    private readonly IlluminatedButtonImpl[] _muteButtons    = new IlluminatedButtonImpl[trackCount];
    private readonly IlluminatedButtonImpl[] _selectButtons  = new IlluminatedButtonImpl[trackCount];
    private readonly VuMeter[]               _vuMeters       = new VuMeter[trackCount];
    private readonly Fader[]                 _faders         = new Fader[trackCount];
    private readonly ScribbleStrip[]         _scribbleStrips = new ScribbleStrip[trackCount];

    protected BehringerXTouchExtender() {
        AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        Console.CancelKeyPress              += OnProcessExit;

        for (int trackId = 0; trackId < trackCount; trackId++) {
            _recordButtons[trackId]  = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Record);
            _soloButtons[trackId]    = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Solo);
            _muteButtons[trackId]    = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Mute);
            _selectButtons[trackId]  = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Select);
            _vuMeters[trackId]       = new VuMeter(MidiClient, trackId);
            _faders[trackId]         = new Fader(MidiClient, trackId);
            _scribbleStrips[trackId] = new ScribbleStrip(MidiClient, trackId);
            //rotary encoders are constructed in concrete subclasses
        }
    }

    public IIlluminatedButton GetRecordButton(int trackId) {
        ValidateTrackId(trackId);
        return _recordButtons[trackId];
    }

    public IIlluminatedButton GetMuteButton(int trackId) {
        ValidateTrackId(trackId);
        return _muteButtons[trackId];
    }

    public IIlluminatedButton GetSoloButton(int trackId) {
        ValidateTrackId(trackId);
        return _soloButtons[trackId];
    }

    public IIlluminatedButton GetSelectButton(int trackId) {
        ValidateTrackId(trackId);
        return _selectButtons[trackId];
    }

    public abstract TRotaryEncoder GetRotaryEncoder(int trackId);

    public IVuMeter GetVuMeter(int trackId) {
        ValidateTrackId(trackId);
        return _vuMeters[trackId];
    }

    public IFader GetFader(int trackId) {
        ValidateTrackId(trackId);
        return _faders[trackId];
    }

    public IScribbleStrip GetScribbleStrip(int trackId) {
        ValidateTrackId(trackId);
        return _scribbleStrips[trackId];
    }

    public bool IsOpen => MidiClient.IsOpen;

    /// <exception cref="LifecycleException">if <c>Open()</c> has already been called on this instance</exception>
    /// <exception cref="DeviceNotFoundException">if no MIDI input or output device named <c>X-Touch-Ext</c> is connected, or if it's already in use by another client. One situation in which the X-Touch Extender may not appear to be connected is when its firmware is too old (e.g. 1.00) and your CPU microarchitecture is AMD Zen2 or later (Ryzen 3000 or later); in this case you should upgrade the X-Touch Extender firmware to version 1.21 or later (download from https://www.behringer.com/product.html?modelCode=P0CCR); you can upgrade the firmware by turning on the device while holding the Track 8 Rec button, running MIDI-OX on your computer connected with USB, highlighting the X-Touch-Ext entries in Options › MIDI Devices, then selecting the downloaded .syx file using Actions › Send › SysEx File, waiting for the upgrade to finish, and finally turning the device off and on again.</exception>
    public virtual void Open() {
        if (IsOpen) {
            throw new LifecycleException("This IMidiControlSurface instance has already been opened. Call MidiControlSurface.IsOpen");
        }

        // ICollection<InputDevice> inputDevices = InputDevice.GetAll();
        // Console.WriteLine("Input devices:");
        // foreach (InputDevice inputDevice in inputDevices) {
        //     Console.WriteLine($"- {inputDevice.Name}");
        //     foreach (InputDeviceProperty property in InputDevice.GetSupportedProperties()) {
        //         Console.WriteLine($"    {property}:\t{inputDevice.GetProperty(property)}");
        //     }
        // }

        MidiClient.FromDevice = InputDevice.GetByName(Constants.DeviceName) ??
            throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to receive MIDI messages from.");

        // ICollection<OutputDevice> outputDevices = OutputDevice.GetAll();
        // Console.WriteLine("Output devices:");
        // foreach (OutputDevice outputDevice in outputDevices) {
        //     Console.WriteLine($"- {outputDevice.Name}");
        //     foreach (OutputDeviceProperty property in OutputDevice.GetSupportedProperties()) {
        //         Console.WriteLine($"    {property}:\t{outputDevice.GetProperty(property)}");
        //     }
        // }

        MidiClient.ToDevice = OutputDevice.GetByName(Constants.DeviceName) ??
            throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to send MIDI messages to.");

        MidiClient.FromDevice.EventReceived += OnEventReceivedFromDevice;

        try {
            MidiClient.ToDevice.PrepareForEventsSending();
            MidiClient.FromDevice.StartEventsListening();
        } catch (MidiDeviceException e) {
            throw new DeviceNotFoundException("Found a MIDI device, but could not connect to it, possibly because it is already in use by another process on this computer.", e);
        }

        for (int trackId = 0; trackId < trackCount; trackId++) {
            _recordButtons[trackId].WriteStateToDevice();
            _selectButtons[trackId].WriteStateToDevice();
            _soloButtons[trackId].WriteStateToDevice();
            _muteButtons[trackId].WriteStateToDevice();
            _vuMeters[trackId].WriteStateToDevice();
            _faders[trackId].WriteStateToDevice();
            _scribbleStrips[trackId].WriteStateToDevice();
        }
    }

    private void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs e) {
        switch (e.Event.EventType) {
            case MidiEventType.NoteOn:
                OnEventReceivedFromDevice((NoteOnEvent) e.Event);
                break;
            case MidiEventType.ControlChange:
                OnEventReceivedFromDevice((ControlChangeEvent) e.Event);
                break;
            default:
                break;
        }
    }

    private void OnEventReceivedFromDevice(NoteOnEvent incomingEvent) {
        int            trackId;
        SevenBitNumber noteId    = incomingEvent.NoteNumber;
        bool           isPressed = incomingEvent.Velocity == SevenBitNumber.MaxValue;

        switch (noteId) {
            case >= 0x00 and < 0x00 + trackCount:
                trackId = noteId - 0x00;
                ((RotaryEncoder) (object) GetRotaryEncoder(trackId)).OnButtonEvent(isPressed);
                // _rotaryEncoders[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x08 and < 0x08 + trackCount:
                trackId = noteId - 0x08;
                _recordButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x10 and < 0x10 + trackCount:
                trackId = noteId - 0x10;
                _soloButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x18 and < 0x18 + trackCount:
                trackId = noteId - 0x18;
                _muteButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x20 and < 0x20 + trackCount:
                trackId = noteId - 0x20;
                _selectButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x6E and < 0x6E + trackCount:
                trackId = noteId - 0x6E;
                _faders[trackId].OnButtonEvent(isPressed);
                break;
            default:
                break;
        }
    }

    // protected abstract void OnRotaryEncoderPressEventReceivedFromDevice(int trackId, bool isPressed);

    private void OnEventReceivedFromDevice(ControlChangeEvent incomingEvent) {
        int controlNumber = incomingEvent.ControlNumber;
        int trackId;

        switch (controlNumber) {
            case >= 80 and < 80 + trackCount:
                trackId = controlNumber - 80;
                OnRotaryEncoderRotationEventReceivedFromDevice(trackId, incomingEvent.ControlValue);
                break;
            case >= 70 and < 70 + trackCount:
                trackId = controlNumber - 70;
                double newValue = (double) incomingEvent.ControlValue / SevenBitNumber.MaxValue;
                // _faders[trackId - 1]..Value = newValue;
                _faders[trackId].OnFaderMoved(newValue);
                // Console.WriteLine($"Detected that fader {trackId} on device moved to position {newValue:N3}");
                break;
            default:
                break;
        }

    }

    protected abstract void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue);

    /*
    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetButtonLight(int trackId, IlluminatedButtonType buttonType, IlluminatedButtonState illuminatedButtonState) {
        AssertOpen();
        ValidateTrackId(trackId);

        SevenBitNumber noteId = (SevenBitNumber) (trackId - 1 + buttonType switch {
            IlluminatedButtonType.Record => 8,
            IlluminatedButtonType.Solo   => 16,
            IlluminatedButtonType.Mute   => 24,
            IlluminatedButtonType.Select => 32,
            _                            => throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null)
        });

        SevenBitNumber velocity = (SevenBitNumber) (illuminatedButtonState switch {
            IlluminatedButtonState.Off      => SevenBitNumber.MinValue,
            IlluminatedButtonState.On       => SevenBitNumber.MaxValue,
            IlluminatedButtonState.Blinking => 64,
            _                               => throw new ArgumentOutOfRangeException(nameof(illuminatedButtonState), illuminatedButtonState, null)
        });

        _midiClient.ToDevice?.SendEvent(new NoteOnEvent(noteId, velocity));
    }

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void RotateKnob(int trackId, int distanceFromMinimumValue) {
        ChangeControl(80, trackId, distanceFromMinimumValue);
    }
    
    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void MoveSlider(int trackId, double distanceFromMinimumValue) {
        ChangeControl(70, trackId, distanceFromMinimumValue);
    }
    
    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetMeterLevel(int trackId, int distanceFromMinimumValue) {
        ChangeControl(90, trackId, distanceFromMinimumValue);
    }

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public void SetText(int                          trackId,
                        string                       topText         = "",
                        string                       bottomText      = "",
                        ScribbleStripTextColor       topTextColor    = ScribbleStripTextColor.Light,
                        ScribbleStripTextColor       bottomTextColor = ScribbleStripTextColor.Light,
                        ScribbleStripBackgroundColor backgroundColor = ScribbleStripBackgroundColor.Black) {
        _midiClient.AssertOpen();
        ValidateTrackId(trackId);
    
        const int textColumnCount = 7;
    
        byte[] payload = new byte[22];
        // 0xF0 is automatically prepended by NormalSysExEvent
        payload[0] = 0;
        payload[1] = 0x20;
        payload[2] = 0x32;
        payload[3] = DeviceId;
        payload[4] = 0x4C;
        payload[5] = (byte) (trackId - 1);
        payload[6] = (byte) ((int) backgroundColor | ((int) topTextColor << 4) | ((int) bottomTextColor << 5));
    
        byte[] topTextBytes    = Enumerable.Repeat((byte) ' ', textColumnCount).ToArray();
        byte[] bottomTextBytes = Enumerable.Repeat((byte) ' ', textColumnCount).ToArray();
        Encoding.ASCII.GetBytes(topText, 0, textColumnCount, topTextBytes, 0);
        Encoding.ASCII.GetBytes(bottomText, 0, textColumnCount, bottomTextBytes, 0);
    
        for (int column = 0; column < textColumnCount; column++) {
            payload[7 + column]                   = topTextBytes[column];
            payload[7 + column + textColumnCount] = bottomTextBytes[column];
        }
    
        payload[21] = SysExEvent.EndOfEventByte;
    
        _midiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
    }

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    private void ChangeControl(int track1ControlId, int trackId, double distanceFromMinimumValue) {
        _midiClient.AssertOpen();
        ValidateTrackId(trackId);
    
        if (distanceFromMinimumValue is < 0 or > 1) {
            throw new ArgumentOutOfRangeException(nameof(distanceFromMinimumValue), distanceFromMinimumValue,
                $"Parameter distanceFromMinimumValue value {distanceFromMinimumValue} is out of bounds. Valid values are in the range [0.0, 1.0].");
        }
    
        SevenBitNumber controlId    = (SevenBitNumber) (track1ControlId + trackId - 1);
        SevenBitNumber controlValue = (SevenBitNumber) Math.Round(distanceFromMinimumValue * 127);
    
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }*/

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected static void ValidateTrackId(int trackId) {
        if (trackId is < 0 or >= trackCount) {
            throw new ArgumentOutOfRangeException(nameof(trackId), trackId, $"Parameter trackId value {trackId} is out of bounds. Valid values are in the range [0, {trackCount - 1}].");
        }
    }

    public void Dispose() {
        if (MidiClient.FromDevice is not null) {
            MidiClient.FromDevice.EventReceived -= OnEventReceivedFromDevice;
        }

        MidiClient.Dispose();
    }

    private void OnProcessExit(object sender, EventArgs e) {
        Dispose();
    }

}