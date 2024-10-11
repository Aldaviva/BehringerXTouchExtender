using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal abstract class BehringerXTouchExtender<TRotaryEncoder>: IBehringerXTouchExtender<TRotaryEncoder> where TRotaryEncoder: IRotaryEncoder {

    private const string DeviceName = "X-Touch-Ext";

    protected internal const int TRACK_COUNT = 8;
    public int TrackCount => TRACK_COUNT;

    internal readonly MidiClient MidiClient = new();

    protected readonly IIlluminatedButtonInternal[] RecordButtons  = new IIlluminatedButtonInternal[TRACK_COUNT];
    protected readonly IIlluminatedButtonInternal[] SoloButtons    = new IIlluminatedButtonInternal[TRACK_COUNT];
    protected readonly IIlluminatedButtonInternal[] MuteButtons    = new IIlluminatedButtonInternal[TRACK_COUNT];
    protected readonly IIlluminatedButtonInternal[] SelectButtons  = new IIlluminatedButtonInternal[TRACK_COUNT];
    protected readonly IVuMeterInternal[]           VuMeters       = new IVuMeterInternal[TRACK_COUNT];
    protected readonly IFaderInternal[]             Faders         = new IFaderInternal[TRACK_COUNT];
    protected readonly IScribbleStripInternal[]     ScribbleStrips = new IScribbleStripInternal[TRACK_COUNT];

    public IIlluminatedButton GetRecordButton(int trackId) {
        ValidateTrackId(trackId);
        return RecordButtons[trackId];
    }

    public IIlluminatedButton GetMuteButton(int trackId) {
        ValidateTrackId(trackId);
        return MuteButtons[trackId];
    }

    public IIlluminatedButton GetSoloButton(int trackId) {
        ValidateTrackId(trackId);
        return SoloButtons[trackId];
    }

    public IIlluminatedButton GetSelectButton(int trackId) {
        ValidateTrackId(trackId);
        return SelectButtons[trackId];
    }

    public abstract TRotaryEncoder GetRotaryEncoder(int trackId);

    public IVuMeter GetVuMeter(int trackId) {
        ValidateTrackId(trackId);
        return VuMeters[trackId];
    }

    public IFader GetFader(int trackId) {
        ValidateTrackId(trackId);
        return Faders[trackId];
    }

    public IScribbleStrip GetScribbleStrip(int trackId) {
        ValidateTrackId(trackId);
        return ScribbleStrips[trackId];
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
            RecordButtons[trackId].WriteStateToDevice();
            SelectButtons[trackId].WriteStateToDevice();
            SoloButtons[trackId].WriteStateToDevice();
            MuteButtons[trackId].WriteStateToDevice();
            VuMeters[trackId].WriteStateToDevice();
            Faders[trackId].WriteStateToDevice();
            ScribbleStrips[trackId].WriteStateToDevice();
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

    protected abstract void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs e);

    protected abstract void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue);

    /// <exception cref="ArgumentOutOfRangeException"></exception>
    protected static void ValidateTrackId(int trackId) {
        if (trackId is < 0 or >= TRACK_COUNT) {
            throw new ArgumentOutOfRangeException(nameof(trackId), trackId, $"Parameter trackId value {trackId} is out of bounds. Valid values are in the range [0, {TRACK_COUNT - 1}].");
        }
    }

    protected virtual void Dispose(bool disposing) {
        if (disposing) {
            if (MidiClient.FromDevice is not null) {
                MidiClient.FromDevice.EventReceived -= OnEventReceivedFromDevice;
            }
            MidiClient.Dispose();
        }
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

}