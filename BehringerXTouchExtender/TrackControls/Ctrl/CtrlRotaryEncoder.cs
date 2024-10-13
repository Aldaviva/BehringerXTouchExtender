using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

internal abstract class CtrlRotaryEncoder(MidiClient midiClient, int trackId): RotaryEncoder(midiClient, trackId) {

    private static readonly Property<int> MAX_POSITION = new StoredProperty<int>(LIGHT_COUNT);
    public override Property<int> MaxPosition => MAX_POSITION;

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int newValue = Math.Max(Math.Min(LightPosition.Value, LightCount - 1), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (80 + TrackId);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount - 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round(newValue * incrementWidth);

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}

internal class RelativeRotaryEncoder(MidiClient midiClient, int trackId): CtrlRotaryEncoder(midiClient, trackId), IRelativeRotaryEncoderInternal {

    public event EventHandler<IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs>? Rotated;

    public void OnRotated(bool isClockwise, uint distance) => Rotated?.Invoke(this, new IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs(isClockwise, distance));

}

internal class AbsoluteRotaryEncoder(MidiClient midiClient, int trackId): CtrlRotaryEncoder(midiClient, trackId), IAbsoluteRotaryEncoderInternal {

    public SettableProperty<double> AbsoluteRotationPosition { get; } = new StoredProperty<double>();
    public Property<double> RotationPosition => AbsoluteRotationPosition;

}