using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests.TrackControls;

public class RelativeRotaryEncoderTest: RelativeTrackControlTest {

    [Theory, MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, byte expectedControlValue) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        if (lightPosition == 0) {
            //rotary encoder light is initialized to most counterclockwise when opening the client, so temporarily set a different value to cause the disabling event to be sent below
            rotaryEncoder.LightPosition.Connect(rotaryEncoder.LightCount - 1);
        }

        rotaryEncoder.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) expectedControlValue),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<TheoryDataRow<int, int, byte>> SetLightPositionData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, byte>(trackId, 0, 0);
            yield return new TheoryDataRow<int, int, byte>(trackId, 1, 11);
            yield return new TheoryDataRow<int, int, byte>(trackId, 2, 21);
            yield return new TheoryDataRow<int, int, byte>(trackId, 3, 32);
            yield return new TheoryDataRow<int, int, byte>(trackId, 4, 42);
            yield return new TheoryDataRow<int, int, byte>(trackId, 5, 53);
            yield return new TheoryDataRow<int, int, byte>(trackId, 6, 64);
            yield return new TheoryDataRow<int, int, byte>(trackId, 7, 74);
            yield return new TheoryDataRow<int, int, byte>(trackId, 8, 85);
            yield return new TheoryDataRow<int, int, byte>(trackId, 9, 95);
            yield return new TheoryDataRow<int, int, byte>(trackId, 10, 106);
            yield return new TheoryDataRow<int, int, byte>(trackId, 11, 116);
            yield return new TheoryDataRow<int, int, byte>(trackId, 12, 127);
        }
    }

    [Theory, MemberData(nameof(TrackIdData))]
    public void HandleRotaryEncoderPress(int trackId) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        rotaryEncoder.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) trackId, (SevenBitNumber) 127)));
        rotaryEncoder.IsPressed.Value.Should().BeTrue();
    }

    [Theory, MemberData(nameof(TrackIdData))]
    public void ValueValidRange(int trackId) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        rotaryEncoder.MinPosition.Value.Should().Be(0);
        rotaryEncoder.MaxPosition.Value.Should().Be(13);
    }

    [Theory, MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, bool expectedIsClockwise) {
        IRelativeRotaryEncoder                                    rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs? actual        = null;
        rotaryEncoder.Rotated += (_, args) => actual = args;

        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) controlValue)));

        actual!.Value.IsClockwise.Should().Be(expectedIsClockwise);
        actual.Value.Distance.Should().Be(1);
    }

    public static IEnumerable<TheoryDataRow<int, int, bool>> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, bool>(trackId, 1, false);
            yield return new TheoryDataRow<int, int, bool>(trackId, 65, true);
        }
    }

}