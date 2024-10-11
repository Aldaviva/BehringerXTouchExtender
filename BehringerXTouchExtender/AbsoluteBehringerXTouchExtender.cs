using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using Melanchall.DryWetMidi.Common;

namespace BehringerXTouchExtender;

internal class AbsoluteBehringerXTouchExtender: CtrlBehringerXTouchExtender<IAbsoluteRotaryEncoder>, IAbsoluteBehringerXTouchExtender {

    private readonly AbsoluteRotaryEncoder[] _rotaryEncoders = new AbsoluteRotaryEncoder[TRACK_COUNT];

    public AbsoluteBehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _rotaryEncoders[trackId] = new AbsoluteRotaryEncoder(MidiClient, trackId);
        }
    }

    public override void Open() {
        base.Open();
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _rotaryEncoders[trackId].WriteStateToDevice();
        }
    }

    public override IAbsoluteRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId];
    }

    protected override void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue) {
        double newValue = (double) incomingEventControlValue / SevenBitNumber.MaxValue;
        _rotaryEncoders[trackId].AbsoluteRotationPosition.Value = newValue;
    }

}