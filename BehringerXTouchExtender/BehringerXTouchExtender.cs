using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal abstract class BehringerXTouchExtender<TRotaryEncoder>: IBehringerXTouchExtender<TRotaryEncoder> where TRotaryEncoder: IRotaryEncoder {

    private const string DeviceName = "X-Touch-Ext";

    protected const int TRACK_COUNT = 8;
    public int TrackCount => TRACK_COUNT;

    internal readonly MidiClient MidiClient = new();

    private readonly IlluminatedButton[] _recordButtons  = new IlluminatedButton[TRACK_COUNT];
    private readonly IlluminatedButton[] _soloButtons    = new IlluminatedButton[TRACK_COUNT];
    private readonly IlluminatedButton[] _muteButtons    = new IlluminatedButton[TRACK_COUNT];
    private readonly IlluminatedButton[] _selectButtons  = new IlluminatedButton[TRACK_COUNT];
    private readonly VuMeter[]           _vuMeters       = new VuMeter[TRACK_COUNT];
    private readonly Fader[]             _faders         = new Fader[TRACK_COUNT];
    private readonly ScribbleStrip[]     _scribbleStrips = new ScribbleStrip[TRACK_COUNT];

    protected BehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _recordButtons[trackId]  = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Record);
            _soloButtons[trackId]    = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Solo);
            _muteButtons[trackId]    = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Mute);
            _selectButtons[trackId]  = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Select);
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
    /// <exception cref="DeviceNotFoundException">if no MIDI input or output device named <c>X-Touch-Ext</c> is connected, or if it's already in use by another client. One situation in which the X-Touch Extender may not appear to be connected is when its firmware is too old (e.g. 1.00) and your CPU microarchitecture is AMD Zen2 or later (Ryzen 3000 or later); in this case you should upgrade the X-Touch Extender firmware to version 1.21 or later (download from https://www.behringer.com/product.html?modelCode=0808-AAH); you can upgrade the firmware by turning on the device while holding the Track 8 Rec button, running MIDI-OX on your computer connected with USB, highlighting the X-Touch-Ext entries in Options › MIDI Devices, then selecting the downloaded .syx file using Actions › Send › SysEx File, waiting for the upgrade to finish, and finally turning the device off and on again.</exception>
    public virtual void Open() {
        if (IsOpen) {
            throw new LifecycleException("This IMidiControlSurface instance has already been opened. Call MidiControlSurface.IsOpen");
        }

        MidiClient.Dispose();

        try {
            try {
                MidiClient.FromDevice = InputDevice.GetByName(DeviceName);
            } catch (ArgumentException e) {
                throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to receive MIDI messages from.", e);
            }

            try {
                MidiClient.ToDevice = OutputDevice.GetByName(DeviceName);
            } catch (ArgumentException e) {
                throw new DeviceNotFoundException("Could not find connected Behringer X-Touch Extender to send MIDI messages to.", e);
            }

            SubscribeToEventsFromDevice();

            MidiClient.ToDevice.PrepareForEventsSending();
            MidiClient.FromDevice.StartEventsListening();
        } catch (MidiDeviceException e) {
            throw new DeviceNotFoundException("Found a Behringer X-Touch Extender, but could not connect to it, possibly because it is already in use by another process on this computer.", e);
        }

        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _recordButtons[trackId].WriteStateToDevice();
            _selectButtons[trackId].WriteStateToDevice();
            _soloButtons[trackId].WriteStateToDevice();
            _muteButtons[trackId].WriteStateToDevice();
            _vuMeters[trackId].WriteStateToDevice();
            _faders[trackId].WriteStateToDevice();
            _scribbleStrips[trackId].WriteStateToDevice();
        }
    }

    /// <summary>
    /// This is a separate method from <see cref="Open"/> to facilitate unit testing. We want callbacks to work without all the initialization logic running for each track control.
    /// </summary>
    internal void SubscribeToEventsFromDevice() {
        if (MidiClient.FromDevice != null) {
            MidiClient.FromDevice.EventReceived += OnEventReceivedFromDevice;
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

    // ReSharper disable once SuggestBaseTypeForParameter
    private void OnEventReceivedFromDevice(NoteOnEvent incomingEvent) {
        int            trackId;
        SevenBitNumber noteId    = incomingEvent.NoteNumber;
        bool           isPressed = incomingEvent.Velocity == SevenBitNumber.MaxValue;

        switch (noteId) {
            case >= 0x00 and < 0x00 + TRACK_COUNT:
                trackId = noteId - 0x00;
                ((RotaryEncoder) (object) GetRotaryEncoder(trackId)).OnButtonEvent(isPressed);
                break;
            case >= 0x08 and < 0x08 + TRACK_COUNT:
                trackId = noteId - 0x08;
                _recordButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x10 and < 0x10 + TRACK_COUNT:
                trackId = noteId - 0x10;
                _soloButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x18 and < 0x18 + TRACK_COUNT:
                trackId = noteId - 0x18;
                _muteButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x20 and < 0x20 + TRACK_COUNT:
                trackId = noteId - 0x20;
                _selectButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x6E and < 0x6E + TRACK_COUNT:
                trackId = noteId - 0x6E;
                _faders[trackId].OnButtonEvent(isPressed);
                break;
            default:
                break;
        }
    }

    private void OnEventReceivedFromDevice(ControlChangeEvent incomingEvent) {
        int controlNumber = incomingEvent.ControlNumber;
        int trackId;

        switch (controlNumber) {
            case >= 80 and < 80 + TRACK_COUNT:
                trackId = controlNumber - 80;
                OnRotaryEncoderRotationEventReceivedFromDevice(trackId, incomingEvent.ControlValue);
                break;
            case >= 70 and < 70 + TRACK_COUNT:
                trackId = controlNumber - 70;
                double newValue = (double) incomingEvent.ControlValue / SevenBitNumber.MaxValue;
                _faders[trackId].OnFaderMoved(newValue);
                break;
            default:
                break;
        }

    }

    protected abstract void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue);

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected static void ValidateTrackId(int trackId) {
        if (trackId is < 0 or >= TRACK_COUNT) {
            throw new ArgumentOutOfRangeException(nameof(trackId), trackId, $"Parameter trackId value {trackId} is out of bounds. Valid values are in the range [0, {TRACK_COUNT - 1}].");
        }
    }

    public void Dispose() {
        if (MidiClient.FromDevice is not null) {
            MidiClient.FromDevice.EventReceived -= OnEventReceivedFromDevice;
        }

        MidiClient.Dispose();
    }

}