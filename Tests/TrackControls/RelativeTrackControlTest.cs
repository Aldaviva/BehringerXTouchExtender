using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public abstract class RelativeTrackControlTest {

    internal readonly IInputDevice  FromDevice = A.Fake<IInputDevice>();
    internal readonly IOutputDevice ToDevice   = A.Fake<IOutputDevice>();

    internal readonly RelativeBehringerXTouchExtender XTouch = new();

    protected RelativeTrackControlTest() {
        XTouch.MidiClient.FromDevice = FromDevice;
        XTouch.MidiClient.ToDevice   = ToDevice;

        XTouch.SubscribeToEventsFromDevice();

        A.CallTo(() => FromDevice.IsListeningForEvents).Returns(true);
    }

    public static readonly TheoryData<int> TrackIdData = new(Enumerable.Range(0, RelativeBehringerXTouchExtender.TRACK_COUNT));

}