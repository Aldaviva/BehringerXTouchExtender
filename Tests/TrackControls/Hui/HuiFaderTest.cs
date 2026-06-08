using BehringerXTouchExtender.TrackControls;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls.Hui;

public class HuiFaderTest: HuiTrackControlTest {

    [Theory] [MemberData(nameof(MoveFaderData))]
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
        for (int trackId = 0; trackId < HuiBehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 0, 0, 0);
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 0.5, 64, 0);
            yield return new TheoryDataRow<int, double, byte, byte>(trackId, 1, 127, 127);
        }
    }

    [Theory] [MemberData(nameof(MoveFaderData))]
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

}