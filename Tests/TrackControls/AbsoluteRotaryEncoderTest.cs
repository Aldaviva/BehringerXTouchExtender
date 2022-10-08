using BehringerXTouchExtender.Façades;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public class AbsoluteRotaryEncoderTest {

    internal readonly IInputDevice                    FromDevice = A.Fake<IInputDevice>();
    internal readonly IOutputDevice                   ToDevice   = A.Fake<IOutputDevice>();
    internal readonly DryWetMidiFaçade                MidiFaçade = A.Fake<DryWetMidiFaçade>();
    internal readonly AbsoluteBehringerXTouchExtender XTouch     = new();

    public AbsoluteRotaryEncoderTest() {
        XTouch.MidiFaçade = MidiFaçade;

        A.CallTo(() => MidiFaçade.GetInputDeviceByName(A<string>._)).Returns(FromDevice);
        A.CallTo(() => MidiFaçade.GetOutputDeviceByName(A<string>._)).Returns(ToDevice);
        A.CallTo(() => FromDevice.IsListeningForEvents).Returns(true);

        XTouch.Open();

        Fake.ClearRecordedCalls(ToDevice);
    }

    [Theory]
    [MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, double expectedPosition) {
        IAbsoluteRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);

        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) controlValue)));

        rotaryEncoder.RotationPosition.Value.Should().BeApproximately(expectedPosition, 0.01);
    }

    public static IEnumerable<object[]> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return new object[] { trackId, 0, 0 };
            yield return new object[] { trackId, 32, 0.25 };
            yield return new object[] { trackId, 64, 0.50 };
            yield return new object[] { trackId, 95, 0.75 };
            yield return new object[] { trackId, 127, 1.00 };
        }
    }

}