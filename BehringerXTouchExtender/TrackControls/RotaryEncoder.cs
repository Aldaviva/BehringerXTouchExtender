using System.ComponentModel;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class RotaryEncoder: PressableButton {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public int LightCount => 13;

    public ConnectableProperty<int> LightPosition { get; } = new();

    protected RotaryEncoder(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += WriteStateToDevice;
    }

    internal override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int newValue = Math.Max(Math.Min(LightPosition.Value, LightCount - 1), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (80 + TrackId);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount - 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round(newValue * incrementWidth);

        // Console.WriteLine($"Setting rotary encoder {TrackId} light position to {controlValue}");
        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}

internal class RelativeRotaryEncoder: RotaryEncoder, IRelativeRotaryEncoder {

    public event IRelativeRotaryEncoder.RotaryEncoderRelativeRotationHandler? Rotated;

    public RelativeRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }

    internal void OnRotated(bool isClockwise) {
        Rotated?.Invoke(this, new IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs(isClockwise));
    }

}

internal class AbsoluteRotaryEncoder: RotaryEncoder, IAbsoluteRotaryEncoder {

    internal readonly SettableProperty<double> AbsoluteRotationPosition = new StoredProperty<double>();
    public Property<double> RotationPosition => AbsoluteRotationPosition;

    public AbsoluteRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }

}