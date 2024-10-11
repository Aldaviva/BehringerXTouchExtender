using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls;

public class VuMeterTest: AbstractTrackControlTest {

    [Theory]
    [MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, int expectedControlValue) {
        IVuMeter vuMeter = XTouch.GetVuMeter(trackId);
        vuMeter.LightPosition.Connect(vuMeter.LightCount); //meter is initialized to off when opening the client, so temporarily set a different value to cause the disabling event to be sent below

        vuMeter.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (90 + trackId), (SevenBitNumber) expectedControlValue),
                ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<object[]> SetLightPositionData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return [trackId, 0, 7];
            yield return [trackId, 1, 21];
            yield return [trackId, 2, 35];
            yield return [trackId, 3, 49];
            yield return [trackId, 4, 64];
            yield return [trackId, 5, 78];
            yield return [trackId, 6, 92];
            yield return [trackId, 7, 106];
            yield return [trackId, 8, 120];
        }
    }

}