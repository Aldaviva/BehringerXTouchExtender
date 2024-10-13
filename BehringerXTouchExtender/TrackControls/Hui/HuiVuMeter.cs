using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiVuMeter(MidiClient midiClient, int trackId): VuMeter(midiClient, trackId) {

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        SevenBitNumber aftertouchValue = (SevenBitNumber) (LightPosition.Value switch {
            <= 0               => 0,
            <= 5 and var value => value + 1,
            var value          => 2 * (value - 6) + 8
        });

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new NoteAftertouchEvent((SevenBitNumber) TrackId, aftertouchValue));
    }

}