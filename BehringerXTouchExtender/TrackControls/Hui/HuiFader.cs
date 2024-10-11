using KoKo.Property;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiFader: PressableButton, IFaderInternal {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public ConnectableProperty<double> DesiredPosition { get; } = new();

    private readonly StoredProperty<double> _actualPosition = new();
    public Property<double> ActualPosition => _actualPosition;

    public HuiFader(MidiClient midiClient, int trackId) {
        TrackId     = trackId;
        _midiClient = midiClient;

        DesiredPosition.PropertyChanged += WriteStateToDevice;
    }

    public void OnFaderMoved(double newPosition) {
        _actualPosition.Value = newPosition;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        //TODO
    }

}