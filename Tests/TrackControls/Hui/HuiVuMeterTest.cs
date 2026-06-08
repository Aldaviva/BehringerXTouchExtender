using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls.Hui;

public class HuiVuMeterTest: HuiTrackControlTest {

    [Theory] [MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, byte expectedControlValue) {
        IVuMeter vuMeter = XTouch.GetVuMeter(trackId);
        if (lightPosition == 0) {
            vuMeter.LightPosition.Connect(1); // don't ignore 0 test
        }
        vuMeter.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<NoteAftertouchEvent>.That.IsEqualTo(new NoteAftertouchEvent((SevenBitNumber) trackId, (SevenBitNumber) expectedControlValue),
            NoteAftertouchEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<TheoryDataRow<int, int, byte>> SetLightPositionData() {
        for (int trackId = 0; trackId < HuiBehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, byte>(trackId, 0, 0);
            yield return new TheoryDataRow<int, int, byte>(trackId, 1, 2);
            yield return new TheoryDataRow<int, int, byte>(trackId, 2, 3);
            yield return new TheoryDataRow<int, int, byte>(trackId, 3, 4);
            yield return new TheoryDataRow<int, int, byte>(trackId, 4, 5);
            yield return new TheoryDataRow<int, int, byte>(trackId, 5, 6);
            yield return new TheoryDataRow<int, int, byte>(trackId, 6, 8);
            yield return new TheoryDataRow<int, int, byte>(trackId, 7, 10);
            yield return new TheoryDataRow<int, int, byte>(trackId, 8, 12);
        }
    }

}