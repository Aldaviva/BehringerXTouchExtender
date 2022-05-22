using KoKo.Events;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class RotaryEncoder: PressableButton, IRelativeRotaryEncoder, IAbsoluteRotaryEncoder {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public int LightCount => 13;

    public ConnectableProperty<int> LightPosition { get; } = new();

    protected RotaryEncoder(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += OnLightPositionChanged;
    }

    private void OnLightPositionChanged(object sender, KoKoPropertyChangedEventArgs<int> args) {
        int newValue = Math.Max(Math.Min(args.NewValue, LightCount - 1), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (80 + TrackId - 1);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / LightCount;
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

    Property<int> IRotaryEncoder<int>.RotationPosition => throw new NotImplementedException("abstract");

    Property<double> IRotaryEncoder<double>.RotationPosition => throw new NotImplementedException("abstract");

}

internal class RelativeRotaryEncoder: RotaryEncoder, IRelativeRotaryEncoder {

    internal readonly SettableProperty<int> RelativeRotationPosition = new StoredProperty<int>();
    Property<int> IRotaryEncoder<int>.RotationPosition => RelativeRotationPosition;

    public RelativeRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }

}

internal class AbsoluteRotaryEncoder: RotaryEncoder, IAbsoluteRotaryEncoder {

    internal readonly SettableProperty<double> AbsoluteRotationPosition = new StoredProperty<double>();
    Property<double> IRotaryEncoder<double>.RotationPosition => AbsoluteRotationPosition;

    public AbsoluteRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }

}