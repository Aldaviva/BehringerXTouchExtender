using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

internal class CtrlFader(MidiClient midiClient, int trackId): Fader(midiClient, trackId) {

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        double newValue = Math.Max(Math.Min(DesiredPosition.Value, 1), 0);

        if (!IsPressed.Value) { //don't slide the fader out from under the user's finger
            SevenBitNumber controlId    = (SevenBitNumber) (70 + TrackId);
            SevenBitNumber controlValue = (SevenBitNumber) Math.Round(newValue * SevenBitNumber.MaxValue);

            MidiClient.AssertOpen();
            MidiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));

            OnFaderMoved(newValue);
        }
    }

}