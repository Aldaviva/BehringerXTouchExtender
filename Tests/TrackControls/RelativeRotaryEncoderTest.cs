using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests.TrackControls;

public class RelativeRotaryEncoderTest: AbstractTrackControlTest {

    [Theory]
    [MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, int expectedControlValue) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        if (lightPosition == 0) {
            //rotary encoder light is initialized to most counterclockwise when opening the client, so temporarily set a different value to cause the disabling event to be sent below
            rotaryEncoder.LightPosition.Connect(rotaryEncoder.LightCount - 1);
        }

        rotaryEncoder.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) expectedControlValue),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<object[]> SetLightPositionData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return [trackId, 0, 0];
            yield return [trackId, 1, 11];
            yield return [trackId, 2, 21];
            yield return [trackId, 3, 32];
            yield return [trackId, 4, 42];
            yield return [trackId, 5, 53];
            yield return [trackId, 6, 64];
            yield return [trackId, 7, 74];
            yield return [trackId, 8, 85];
            yield return [trackId, 9, 95];
            yield return [trackId, 10, 106];
            yield return [trackId, 11, 116];
            yield return [trackId, 12, 127];
        }
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleRotaryEncoderPress(int trackId) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        rotaryEncoder.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) trackId, (SevenBitNumber) 127)));
        rotaryEncoder.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, bool expectedIsClockwise) {
        IRelativeRotaryEncoder rotaryEncoder     = XTouch.GetRotaryEncoder(trackId);
        bool?                  actualIsClockwise = null;
        rotaryEncoder.Rotated += (_, args) => actualIsClockwise = args.IsClockwise;

        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) controlValue)));

        actualIsClockwise.Should().Be(expectedIsClockwise);
    }

    public static IEnumerable<object[]> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return [trackId, 1, false];
            yield return [trackId, 65, true];
        }
    }

}