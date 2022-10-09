using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public abstract class AbstractTrackControlTest {

    internal readonly IInputDevice                    FromDevice = A.Fake<IInputDevice>();
    internal readonly IOutputDevice                   ToDevice   = A.Fake<IOutputDevice>();
    internal readonly RelativeBehringerXTouchExtender XTouch     = new();

    protected AbstractTrackControlTest() {
        XTouch.MidiClient.FromDevice = FromDevice;
        XTouch.MidiClient.ToDevice   = ToDevice;

        XTouch.SubscribeToEventsFromDevice();

        A.CallTo(() => FromDevice.IsListeningForEvents).Returns(true);
    }

    public static IEnumerable<object[]> TrackIdData = Enumerable.Range(0, 8).Select(i => new object[] { i });

}