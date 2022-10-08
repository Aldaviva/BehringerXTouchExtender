using System.ComponentModel;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class Fader: PressableButton, IFader {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public ConnectableProperty<double> DesiredPosition { get; } = new();

    private readonly StoredProperty<double> _actualPosition = new();
    public Property<double> ActualPosition => _actualPosition;

    public Fader(MidiClient midiClient, int trackId) {
        TrackId     = trackId;
        _midiClient = midiClient;

        DesiredPosition.PropertyChanged += WriteStateToDevice;
    }

    internal void OnFaderMoved(double newPosition) {
        _actualPosition.Value = newPosition;
    }

    internal override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        double newValue = Math.Max(Math.Min(DesiredPosition.Value, 1), 0);

        if (!IsPressed.Value) { //don't slide the fader out from under the user's finger
            SevenBitNumber controlId    = (SevenBitNumber) (70 + TrackId);
            SevenBitNumber controlValue = (SevenBitNumber) Math.Round(newValue * SevenBitNumber.MaxValue);

            _midiClient.AssertOpen();
            _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));

            OnFaderMoved(newValue);
        }
    }

}