using BehringerXTouchExtender.Enums;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiRotaryEncoder: RotaryEncoder, IHuiRotaryEncoderInternal {

    public event EventHandler<IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs>? Rotated;

    public ConnectableProperty<bool> IlluminateBounds { get; } = new();
    public ConnectableProperty<RotaryEncoderFillMode> Fill { get; } = new();

    public override Property<int> MaxPosition { get; }

    public HuiRotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        MaxPosition = DerivedProperty<int>.Create(Fill, calculateMaxPosition);

        IlluminateBounds.PropertyChanged += WriteStateToDevice;
        Fill.PropertyChanged             += WriteStateToDevice;
    }

    public void OnRotated(bool isClockwise, uint distance) => Rotated?.Invoke(this, new IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs(isClockwise));

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int fillOffset = Fill.Value switch {
            RotaryEncoderFillMode.NoFill                 => 0,
            RotaryEncoderFillMode.FillCounterclockwise   => 32,
            RotaryEncoderFillMode.FillToCenterAsymmetric => 17,
            RotaryEncoderFillMode.FillToCenterSymmetric  => 48,
            _                                            => 0
        };

        int value = Math.Max(MinPosition.Value, Math.Min(LightPosition.Value, MaxPosition.Value)) + (IlluminateBounds.Value ? 64 : 0) + fillOffset;

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent((SevenBitNumber) (TrackId + 16), (SevenBitNumber) value));
    }

    private static int calculateMaxPosition(RotaryEncoderFillMode fill) => fill switch {
        RotaryEncoderFillMode.NoFill                 => 11,
        RotaryEncoderFillMode.FillCounterclockwise   => 11,
        RotaryEncoderFillMode.FillToCenterAsymmetric => 10,
        RotaryEncoderFillMode.FillToCenterSymmetric  => 6,
        _                                            => 11
    };

}