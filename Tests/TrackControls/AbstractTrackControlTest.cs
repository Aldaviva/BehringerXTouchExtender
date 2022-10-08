using BehringerXTouchExtender.Façades;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public abstract class AbstractTrackControlTest {

    internal readonly IInputDevice                    FromDevice = A.Fake<IInputDevice>();
    internal readonly IOutputDevice                   ToDevice   = A.Fake<IOutputDevice>();
    internal readonly DryWetMidiFaçade                MidiFaçade = A.Fake<DryWetMidiFaçade>();
    internal readonly RelativeBehringerXTouchExtender XTouch     = new();

    protected AbstractTrackControlTest() {
        XTouch.MidiFaçade = MidiFaçade;

        A.CallTo(() => MidiFaçade.GetInputDeviceByName(A<string>._)).Returns(FromDevice);
        A.CallTo(() => MidiFaçade.GetOutputDeviceByName(A<string>._)).Returns(ToDevice);
        A.CallTo(() => FromDevice.IsListeningForEvents).Returns(true);

        XTouch.Open();

        Fake.ClearRecordedCalls(ToDevice);
    }

    public static IEnumerable<object[]> TrackIdData = Enumerable.Range(0, 8).Select(i => new object[] { i });

}