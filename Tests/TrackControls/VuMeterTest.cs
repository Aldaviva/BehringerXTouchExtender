using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls;

public class VuMeterTest: RelativeTrackControlTest {

    [Theory, MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, byte expectedControlValue) {
        IVuMeter vuMeter = XTouch.GetVuMeter(trackId);
        vuMeter.LightPosition.Connect(vuMeter.LightCount); //meter is initialized to off when opening the client, so temporarily set a different value to cause the disabling event to be sent below

        vuMeter.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (90 + trackId), (SevenBitNumber) expectedControlValue),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<TheoryDataRow<int, int, byte>> SetLightPositionData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, byte>(trackId, 0, 7);
            yield return new TheoryDataRow<int, int, byte>(trackId, 1, 21);
            yield return new TheoryDataRow<int, int, byte>(trackId, 2, 35);
            yield return new TheoryDataRow<int, int, byte>(trackId, 3, 49);
            yield return new TheoryDataRow<int, int, byte>(trackId, 4, 64);
            yield return new TheoryDataRow<int, int, byte>(trackId, 5, 78);
            yield return new TheoryDataRow<int, int, byte>(trackId, 6, 92);
            yield return new TheoryDataRow<int, int, byte>(trackId, 7, 106);
            yield return new TheoryDataRow<int, int, byte>(trackId, 8, 120);
        }
    }

}