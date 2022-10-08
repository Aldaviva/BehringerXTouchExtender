using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public class FaderTest: AbstractTrackControlTest {

    [Theory]
    [MemberData(nameof(MoveFaderData))]
    public void MoveFaderToDevice(int trackId, double inputPosition, int expectedPositionToDevice) {
        IFader fader = XTouch.GetFader(trackId);
        fader.DesiredPosition.Connect(1); //fader is initialized to 0 when opening the client, so temporarily set a different value to cause the disabling event to be sent below

        fader.DesiredPosition.Connect(inputPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (70 + trackId), (SevenBitNumber) expectedPositionToDevice),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<object[]> MoveFaderData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return new object[] { trackId, 0.00, 0 };
            yield return new object[] { trackId, 0.25, 32 };
            yield return new object[] { trackId, 0.50, 64 };
            yield return new object[] { trackId, 0.75, 95 };
            yield return new object[] { trackId, 1.00, 127 };
        }
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleRecordButtonPress(int trackId) {
        IFader fader = XTouch.GetFader(trackId);
        fader.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) (110 + trackId), (SevenBitNumber) 127)));
        fader.IsPressed.Value.Should().BeTrue();
    }

    [Fact]
    public void DontMoveWhenUserIsTouchingFader() {
        IFader fader = XTouch.GetFader(0);
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) 110, (SevenBitNumber) 127)));
        fader.IsPressed.Value.Should().BeTrue();

        fader.DesiredPosition.Connect(1);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>._)).MustNotHaveHappened();
    }

    [Theory]
    [MemberData(nameof(MoveFaderData))]
    public void HandleFaderMoveFromDevice(int trackId, double expectedPosition, int positionFromDevice) {
        IFader fader = XTouch.GetFader(trackId);
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (70 + trackId), (SevenBitNumber) positionFromDevice)));
        fader.ActualPosition.Value.Should().BeApproximately(expectedPosition, 0.01);
    }

}