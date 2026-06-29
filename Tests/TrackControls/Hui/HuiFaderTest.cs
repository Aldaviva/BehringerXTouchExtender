using BehringerXTouchExtender.TrackControls;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests.TrackControls.Hui;

public class HuiFaderTest: HuiTrackControlTest {

    [Theory, MemberData(nameof(MoveFaderData))]
    public void MoveFaderToDevice(int trackId, double desiredPosition, byte expectedHigherOrderBits, byte expectedLowerOrderBits) {
        IFader fader = XTouch.GetFader(trackId);
        if (desiredPosition == 0) {
            fader.DesiredPosition.Connect(1 - desiredPosition); // don't ignore unchanged values
        }
        fader.DesiredPosition.Connect(desiredPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) trackId, (SevenBitNumber) expectedHigherOrderBits),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (trackId + 32), (SevenBitNumber) expectedLowerOrderBits),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<TheoryDataRow<int, double, byte, byte>> MoveFaderData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 0, 0, 0);
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 0.5, 64, 0);
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 1, 127, 127);
        }
    }

    [Theory, MemberData(nameof(MoveFaderData))]
    public void PersistPositionOnRelease(int trackId, double desiredPosition, byte expectedHigherOrderBits, byte expectedLowerOrderBits) {
        IFader fader = XTouch.GetFader(trackId);
        ((PressableButton) fader).OnButtonEvent(true);
        ((StoredProperty<double>) fader.ActualPosition).Value = desiredPosition;
        ((PressableButton) fader).OnButtonEvent(false);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) trackId, (SevenBitNumber) expectedHigherOrderBits),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (trackId + 32), (SevenBitNumber) expectedLowerOrderBits),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Theory, MemberData(nameof(TrackIdData))]
    public void HandleButtonPress(int trackId) {
        IFader fader = XTouch.GetFader(trackId);
        fader.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 64)));
        fader.IsPressed.Value.Should().BeTrue();
    }

    [Theory, MemberData(nameof(MovedFaderData))]
    public void HandleFaderMoveFromDevice(int trackId, double expectedPosition, byte positionFromDevice) {
        IFader fader = XTouch.GetFader(trackId);
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) trackId, (SevenBitNumber) positionFromDevice)));
        fader.ActualPosition.Value.Should().BeApproximately(expectedPosition, 0.01);
    }

    public static IEnumerable<TheoryDataRow<int, double, byte>> MovedFaderData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, double, byte>(trackId, 0, 0);
            yield return new TheoryDataRow<int, double, byte>(trackId, 0.5, 64);
            yield return new TheoryDataRow<int, double, byte>(trackId, 1, 127);
        }
    }

}