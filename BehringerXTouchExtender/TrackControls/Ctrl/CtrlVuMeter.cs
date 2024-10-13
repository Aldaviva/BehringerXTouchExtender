using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

internal class CtrlVuMeter(MidiClient midiClient, int trackId): VuMeter(midiClient, trackId) {

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int newValue = Math.Max(Math.Min(LightPosition.Value, LightCount), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (90 + TrackId);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount + 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}