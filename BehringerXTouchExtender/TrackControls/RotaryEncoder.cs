using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class RotaryEncoder: PressableButton, IRotaryEncoder {

    protected const int LIGHT_COUNT = 13;

    private static readonly Property<int> MIN_POSITION = new StoredProperty<int>();

    public int LightCount => LIGHT_COUNT;

    public ConnectableProperty<int> LightPosition { get; } = new();

    public Property<int> MinPosition => MIN_POSITION;
    public abstract Property<int> MaxPosition { get; }

    protected RotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        LightPosition.PropertyChanged += WriteStateToDevice;
    }

}