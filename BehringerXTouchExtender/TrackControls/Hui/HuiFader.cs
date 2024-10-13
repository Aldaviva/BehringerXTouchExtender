using KoKo.Events;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiFader: Fader {

    private const double MAX_WRITABLE_VALUE = (1 << 14) - 1;

    public HuiFader(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        IsPressed.PropertyChanged += PersistPositionOnRelease;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        if (!IsPressed.Value) { //don't slide the fader out from under the user's finger
            OnFaderMoved(WriteDesiredPosition(DesiredPosition.Value));
        }
    }

    private double WriteDesiredPosition(double desiredPosition) {
        double newValue     = Math.Max(Math.Min(desiredPosition, 1), 0);
        int    controlValue = (int) Math.Round(newValue * MAX_WRITABLE_VALUE);

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent((SevenBitNumber) TrackId, (SevenBitNumber) (controlValue >> 7)));
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent((SevenBitNumber) (32 + TrackId), (SevenBitNumber) (controlValue & SevenBitNumber.MaxValue)));
        return newValue;
    }

    private void PersistPositionOnRelease(object sender, KoKoPropertyChangedEventArgs<bool> args) {
        if (!args.NewValue) {
            WriteDesiredPosition(ActualPosition.Value);
        }
    }

}