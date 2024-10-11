using BehringerXTouchExtender.TrackControls.Ctrl;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiRotaryEncoder: RelativeRotaryEncoder {

    public HuiRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        //TODO
    }

}