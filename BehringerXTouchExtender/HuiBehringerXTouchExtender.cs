using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using BehringerXTouchExtender.TrackControls.Hui;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal class HuiBehringerXTouchExtender: BehringerXTouchExtender<IRelativeRotaryEncoder>, IRelativeBehringerXTouchExtender {

    private static readonly byte[] HealthCheckPayload = [0x00, 0x00, 0x66, 0x14, 0x00, SysExEvent.EndOfEventByte];

    private readonly RelativeRotaryEncoder[] _rotaryEncoders = new RelativeRotaryEncoder[TRACK_COUNT];
    private readonly Timer                   _healthCheckTimer;

    public HuiBehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            RecordButtons[trackId]   = new HuiIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Record);
            SoloButtons[trackId]     = new HuiIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Solo);
            MuteButtons[trackId]     = new HuiIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Mute);
            SelectButtons[trackId]   = new HuiIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Select);
            Faders[trackId]          = new HuiFader(MidiClient, trackId);
            ScribbleStrips[trackId]  = new HuiScribbleStrip(MidiClient, trackId);
            VuMeters[trackId]        = new HuiVuMeter(MidiClient, trackId);
            _rotaryEncoders[trackId] = new HuiRotaryEncoder(MidiClient, trackId);
        }

        _healthCheckTimer = new Timer(SendHealthCheck);
    }

    public override void Open() {
        base.Open();

        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _rotaryEncoders[trackId].WriteStateToDevice();
        }

        SendHealthCheck();
        _healthCheckTimer.Change(6000, 6000);
    }

    private void SendHealthCheck(object? state = null) {
        MidiClient.ToDevice?.SendEvent(new NormalSysExEvent(HealthCheckPayload));
    }

    public override IRelativeRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId];
    }

    protected override void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs e) {
        //TODO
    }

    protected override void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue) {
        //TODO
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            _healthCheckTimer.Dispose();
        }
        base.Dispose(disposing);
    }

}