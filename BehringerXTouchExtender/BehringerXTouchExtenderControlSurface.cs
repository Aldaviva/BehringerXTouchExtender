using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

public static class BehringerXTouchExtenderControlSurface {

    public static BehringerXTouchExtenderControlSurface<IRelativeRotaryEncoder> CreateWithRelativeMode() {
        return new RelativeBehringerXTouchExtender();
    }

    public static BehringerXTouchExtenderControlSurface<IAbsoluteRotaryEncoder> CreateWithAbsoluteMode() {
        return new AbsoluteBehringerXTouchExtender();
    }

}

public abstract class BehringerXTouchExtenderControlSurface<TRotaryEncoder>: IBehringerXTouchExtenderControlSurface<TRotaryEncoder> where TRotaryEncoder: IRotaryEncoder {

    protected const int trackCount = 8;

    public int TrackCount => trackCount;

    internal readonly MidiClient MidiClient = new();

    private readonly  IlluminatedButtonImpl[] _recordButtons  = new IlluminatedButtonImpl[trackCount];
    private readonly  IlluminatedButtonImpl[] _soloButtons    = new IlluminatedButtonImpl[trackCount];
    private readonly  IlluminatedButtonImpl[] _muteButtons    = new IlluminatedButtonImpl[trackCount];
    internal readonly RotaryEncoder[]         _rotaryEncoders = new RotaryEncoder[trackCount];
    private readonly  IlluminatedButtonImpl[] _selectButtons  = new IlluminatedButtonImpl[trackCount];

    // internal readonly TRotaryEncoder[]        RotaryEncoders  = new TRotaryEncoder[trackCount];
    private readonly VuMeter[]       _vuMeters       = new VuMeter[trackCount];
    private readonly Fader[]         _faders         = new Fader[trackCount];
    private readonly ScribbleStrip[] _scribbleStrips = new ScribbleStrip[trackCount];

    // public event IBehringerXTouchExtenderControlSurface<TRotaryEncoder>.MidiEventFromDeviceHandler? MidiEventFromDevice;

    protected BehringerXTouchExtenderControlSurface() {

        for (int trackId = 1; trackId < trackCount; trackId++) {
            _recordButtons[trackId - 1]  = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Record);
            _soloButtons[trackId - 1]    = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Solo);
            _muteButtons[trackId - 1]    = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Mute);
            _selectButtons[trackId - 1]  = new IlluminatedButtonImpl(MidiClient, trackId, IlluminatedButtonType.Select);
            _rotaryEncoders[trackId - 1] = new RotaryEncoder(MidiClient, trackId);
            _vuMeters[trackId - 1]       = new VuMeter(MidiClient, trackId);
            _faders[trackId - 1]         = new Fader(MidiClient, trackId);
            _scribbleStrips[trackId - 1] = new ScribbleStrip(MidiClient, trackId);
        }
    }

    public IIlluminatedButton GetRecordButton(int trackId) {
        ValidateTrackId(trackId);
        return _recordButtons[trackId - 1];
    }

    public IIlluminatedButton GetMuteButton(int trackId) {
        ValidateTrackId(trackId);
        return _muteButtons[trackId - 1];
    }

    public IIlluminatedButton GetSoloButton(int trackId) {
        ValidateTrackId(trackId);
        return _soloButtons[trackId - 1];
    }

    public IIlluminatedButton GetSelectButton(int trackId) {
        ValidateTrackId(trackId);
        return _selectButtons[trackId - 1];
    }

    public abstract TRotaryEncoder GetRotaryEncoder(int trackId);

    public IVuMeter GetVuMeter(int trackId) {
        ValidateTrackId(trackId);
        return _vuMeters[trackId - 1];
    }

    public IFader GetFader(int trackId) {
        ValidateTrackId(trackId);
        return _faders[trackId - 1];
    }

    public IScribbleStrip GetScribbleStrip(int trackId) {
        ValidateTrackId(trackId);
        return _scribbleStrips[trackId - 1];
    }

    public bool IsOpen => MidiClient.IsOpen;

    /// <exception cref="LifecycleException"></exception>
    public void Open() {
        if (IsOpen) {
            throw new LifecycleException("This IMidiControlSurface instance has already been opened. Call MidiControlSurface.IsOpen");
        }

        ICollection<InputDevice> inputDevices = InputDevice.GetAll();
        Console.WriteLine("Input devices:");
        foreach (InputDevice inputDevice in inputDevices) {
            Console.WriteLine($"- {inputDevice.Name}");
            foreach (InputDeviceProperty property in Enums.GetValues<InputDeviceProperty>()) {
                Console.WriteLine($"    {property}:\t{inputDevice.GetProperty(property)}");
            }
        }

        MidiClient.FromDevice = inputDevices.FirstOrDefault(device => device.Name == "X-Touch-Ext") ??
            throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to receive MIDI messages from.");

        ICollection<OutputDevice> outputDevices = OutputDevice.GetAll();
        Console.WriteLine("Output devices:");
        foreach (OutputDevice outputDevice in outputDevices) {
            Console.WriteLine($"- {outputDevice.Name}");
            foreach (OutputDeviceProperty property in Enums.GetValues<OutputDeviceProperty>()) {
                Console.WriteLine($"    {property}:\t{outputDevice.GetProperty(property)}");
            }
        }

        MidiClient.ToDevice = outputDevices.FirstOrDefault(device => device.Name == "X-Touch-Ext") ??
            throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to send MIDI messages to.");

        MidiClient.FromDevice.EventReceived += OnEventReceivedFromDevice;

        MidiClient.ToDevice.PrepareForEventsSending();
        MidiClient.FromDevice.StartEventsListening();
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
            case >= 0x00 and <= 0x00 + trackCount:
                trackId = noteId - 0x00 + 1;
                _rotaryEncoders[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x08 and <= 0x08 + trackCount:
                trackId = noteId - 0x08 + 1;
                _recordButtons[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x10 and <= 0x10 + trackCount:
                trackId = noteId - 0x10 + 1;
                _soloButtons[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x18 and <= 0x18 + trackCount:
                trackId = noteId - 0x18 + 1;
                _muteButtons[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x20 and <= 0x20 + trackCount:
                trackId = noteId - 0x20 + 1;
                _selectButtons[trackId - 1].OnButtonEvent(isPressed);
                break;
            case >= 0x68 and <= 0x68 + trackCount:
                trackId = noteId - 0x68 + 1;
                _faders[trackId - 1].OnButtonEvent(isPressed);
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
                trackId = controlNumber - 80 + 1;
                OnRotaryEncoderRotationEventReceivedFromDevice(trackId, incomingEvent.ControlValue);
                break;
            case >= 70 and < 70 + trackCount:
                trackId = controlNumber - 70 + 1;
                double newValue = (double) incomingEvent.ControlValue / SevenBitNumber.MaxValue;
                _faders[trackId - 1].Position.Value = newValue;
                break;
            default:
                break;
        }

    }

    protected abstract void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue);

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /*public void SetButtonLight(int trackId, IlluminatedButtonType buttonType, IlluminatedButtonState illuminatedButtonState) {
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
    }*/

    // /// <exception cref="LifecycleException"></exception>
    // /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public void RotateKnob(int trackId, int distanceFromMinimumValue) {
    //     ChangeControl(80, trackId, distanceFromMinimumValue);
    // }
    //
    // /// <exception cref="LifecycleException"></exception>
    // /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public void MoveSlider(int trackId, double distanceFromMinimumValue) {
    //     ChangeControl(70, trackId, distanceFromMinimumValue);
    // }
    //
    // /// <exception cref="LifecycleException"></exception>
    // /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public void SetMeterLevel(int trackId, int distanceFromMinimumValue) {
    //     ChangeControl(90, trackId, distanceFromMinimumValue);
    // }

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    // public void SetText(int                          trackId,
    //                     string                       topText         = "",
    //                     string                       bottomText      = "",
    //                     ScribbleStripTextColor       topTextColor    = ScribbleStripTextColor.Light,
    //                     ScribbleStripTextColor       bottomTextColor = ScribbleStripTextColor.Light,
    //                     ScribbleStripBackgroundColor backgroundColor = ScribbleStripBackgroundColor.Black) {
    //     _midiClient.AssertOpen();
    //     ValidateTrackId(trackId);
    //
    //     const int textColumnCount = 7;
    //
    //     byte[] payload = new byte[22];
    //     // 0xF0 is automatically prepended by NormalSysExEvent
    //     payload[0] = 0;
    //     payload[1] = 0x20;
    //     payload[2] = 0x32;
    //     payload[3] = DeviceId;
    //     payload[4] = 0x4C;
    //     payload[5] = (byte) (trackId - 1);
    //     payload[6] = (byte) ((int) backgroundColor | ((int) topTextColor << 4) | ((int) bottomTextColor << 5));
    //
    //     byte[] topTextBytes    = Enumerable.Repeat((byte) ' ', textColumnCount).ToArray();
    //     byte[] bottomTextBytes = Enumerable.Repeat((byte) ' ', textColumnCount).ToArray();
    //     Encoding.ASCII.GetBytes(topText, 0, textColumnCount, topTextBytes, 0);
    //     Encoding.ASCII.GetBytes(bottomText, 0, textColumnCount, bottomTextBytes, 0);
    //
    //     for (int column = 0; column < textColumnCount; column++) {
    //         payload[7 + column]                   = topTextBytes[column];
    //         payload[7 + column + textColumnCount] = bottomTextBytes[column];
    //     }
    //
    //     payload[21] = SysExEvent.EndOfEventByte;
    //
    //     _midiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
    // }

    /// <exception cref="LifecycleException"></exception>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    // private void ChangeControl(int track1ControlId, int trackId, double distanceFromMinimumValue) {
    //     _midiClient.AssertOpen();
    //     ValidateTrackId(trackId);
    //
    //     if (distanceFromMinimumValue is < 0 or > 1) {
    //         throw new ArgumentOutOfRangeException(nameof(distanceFromMinimumValue), distanceFromMinimumValue,
    //             $"Parameter distanceFromMinimumValue value {distanceFromMinimumValue} is out of bounds. Valid values are in the range [0.0, 1.0].");
    //     }
    //
    //     SevenBitNumber controlId    = (SevenBitNumber) (track1ControlId + trackId - 1);
    //     SevenBitNumber controlValue = (SevenBitNumber) Math.Round(distanceFromMinimumValue * 127);
    //
    //     _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    // }

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected static void ValidateTrackId(int trackId) {
        if (trackId is < 1 or > trackCount) {
            throw new ArgumentOutOfRangeException(nameof(trackId), trackId, $"Parameter trackId value {trackId} is out of bounds. Valid values are in the range [1, {trackCount}].");
        }
    }

    public void Dispose() {
        if (MidiClient.FromDevice is not null) {
            MidiClient.FromDevice.EventReceived -= OnEventReceivedFromDevice;
        }

        MidiClient.Dispose();
    }

}

internal class RelativeBehringerXTouchExtender: BehringerXTouchExtenderControlSurface<IRelativeRotaryEncoder> {

    public override IRelativeRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId - 1];
    }

    protected override void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue) {
        int relativeDistanceMoved = (int) incomingEventControlValue switch {
            65 => 1,
            1  => -1,
            _  => 0
        };
        GetRotaryEncoder(trackId).RotationPosition.Value = relativeDistanceMoved;
    }

}

internal class AbsoluteBehringerXTouchExtender: BehringerXTouchExtenderControlSurface<IAbsoluteRotaryEncoder> {

    public override IAbsoluteRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId - 1];
    }

    protected override void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue) {
        double newValue = (double) incomingEventControlValue / SevenBitNumber.MaxValue;
        GetRotaryEncoder(trackId).RotationPosition.Value = newValue;
    }

}