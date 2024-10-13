using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class VuMeter: WritableControl, IVuMeterInternal {

    public int TrackId { get; }
    public int LightCount { get; } = 8;
    public ConnectableProperty<int> LightPosition { get; } = new();

    protected VuMeter(MidiClient midiClient, int trackId): base(midiClient) {
        TrackId = trackId;

        LightPosition.PropertyChanged += WriteStateToDevice;
    }

}