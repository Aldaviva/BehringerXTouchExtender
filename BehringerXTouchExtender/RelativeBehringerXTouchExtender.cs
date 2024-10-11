using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using Melanchall.DryWetMidi.Common;

namespace BehringerXTouchExtender;

internal class RelativeBehringerXTouchExtender: CtrlBehringerXTouchExtender<IRelativeRotaryEncoder>, IRelativeBehringerXTouchExtender {

    private readonly RelativeRotaryEncoder[] _rotaryEncoders = new RelativeRotaryEncoder[TRACK_COUNT];

    public RelativeBehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _rotaryEncoders[trackId] = new RelativeRotaryEncoder(MidiClient, trackId);
        }
    }

    public override void Open() {
        base.Open();
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _rotaryEncoders[trackId].WriteStateToDevice();
        }
    }

    public override IRelativeRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId];
    }

    protected override void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue) {
        if ((int) incomingEventControlValue is 1 or 65) {
            bool isClockwise = incomingEventControlValue == 65;
            _rotaryEncoders[trackId].OnRotated(isClockwise);
        }
    }

}