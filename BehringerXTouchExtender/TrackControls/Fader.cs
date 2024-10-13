using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class Fader: PressableButton, IFaderInternal {

    private readonly StoredProperty<double> _actualPosition = new();

    public ConnectableProperty<double> DesiredPosition { get; } = new();
    public Property<double> ActualPosition => _actualPosition;

    protected Fader(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        DesiredPosition.PropertyChanged += WriteStateToDevice;
    }

    public void OnFaderMoved(double newPosition) {
        _actualPosition.Value = newPosition;
    }

}