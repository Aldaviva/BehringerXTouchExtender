using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls.Hui;

public abstract class HuiTrackControlTest {

    internal readonly IInputDevice  FromDevice = A.Fake<IInputDevice>();
    internal readonly IOutputDevice ToDevice   = A.Fake<IOutputDevice>();

    internal readonly HuiBehringerXTouchExtender XTouch = new();

    protected HuiTrackControlTest() {
        XTouch.MidiClient.FromDevice = FromDevice;
        XTouch.MidiClient.ToDevice   = ToDevice;

        XTouch.SubscribeToEventsFromDevice();

        A.CallTo(() => FromDevice.IsListeningForEvents).Returns(true);
    }

    public static readonly TheoryData<int> TrackIdData = RelativeTrackControlTest.TrackIdData;

}