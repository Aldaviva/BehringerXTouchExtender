using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;
using Tests.TrackControls.Hui;

namespace Tests.TrackControls;

public class HuiRotaryEncoderTest: HuiTrackControlTest {

    [Theory, MemberData(nameof(SetLightPositionData))]
    public void SetLightPosition(int trackId, int lightPosition, RotaryEncoderFillMode fill, bool illuminateBounds, byte expectedControlValue) {
        IHuiRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        if (lightPosition == 0) {
            //rotary encoder light is initialized to most counterclockwise when opening the client, so temporarily set a different value to cause the disabling event to be sent below
            rotaryEncoder.LightPosition.Connect(rotaryEncoder.LightCount - 1);
        }

        rotaryEncoder.Fill.Connect(fill);
        rotaryEncoder.IlluminateBounds.Connect(illuminateBounds);
        rotaryEncoder.LightPosition.Connect(lightPosition);

        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) expectedControlValue),
            ControlChangeEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    public static IEnumerable<TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>> SetLightPositionData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.NoFill, false, 0);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.NoFill, false, 1);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.NoFill, false, 2);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.NoFill, false, 3);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.NoFill, false, 4);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.NoFill, false, 5);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.NoFill, false, 6);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.NoFill, false, 7);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.NoFill, false, 8);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.NoFill, false, 9);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.NoFill, false, 10);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 11, RotaryEncoderFillMode.NoFill, false, 11);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillCounterClockwise, false, 32);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillCounterClockwise, false, 33);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillCounterClockwise, false, 34);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillCounterClockwise, false, 35);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillCounterClockwise, false, 36);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillCounterClockwise, false, 37);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillCounterClockwise, false, 38);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.FillCounterClockwise, false, 39);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.FillCounterClockwise, false, 40);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.FillCounterClockwise, false, 41);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.FillCounterClockwise, false, 42);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 11, RotaryEncoderFillMode.FillCounterClockwise, false, 43);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 17);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 18);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 19);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 20);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 21);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 22);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 23);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 24);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 25);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 26);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.FillToCenterAsymmetric, false, 27);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillToCenterSymmetric, false, 48);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillToCenterSymmetric, false, 49);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillToCenterSymmetric, false, 50);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillToCenterSymmetric, false, 51);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillToCenterSymmetric, false, 52);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillToCenterSymmetric, false, 53);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillToCenterSymmetric, false, 54);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.NoFill, true, 64);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.NoFill, true, 65);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.NoFill, true, 66);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.NoFill, true, 67);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.NoFill, true, 68);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.NoFill, true, 69);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.NoFill, true, 70);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.NoFill, true, 71);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.NoFill, true, 72);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.NoFill, true, 73);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.NoFill, true, 74);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 11, RotaryEncoderFillMode.NoFill, true, 75);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillCounterClockwise, true, 96);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillCounterClockwise, true, 97);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillCounterClockwise, true, 98);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillCounterClockwise, true, 99);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillCounterClockwise, true, 100);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillCounterClockwise, true, 101);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillCounterClockwise, true, 102);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.FillCounterClockwise, true, 103);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.FillCounterClockwise, true, 104);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.FillCounterClockwise, true, 105);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.FillCounterClockwise, true, 106);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 11, RotaryEncoderFillMode.FillCounterClockwise, true, 107);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 81);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 82);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 83);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 84);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 85);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 86);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 87);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 7, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 88);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 8, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 89);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 9, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 90);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 10, RotaryEncoderFillMode.FillToCenterAsymmetric, true, 91);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 0, RotaryEncoderFillMode.FillToCenterSymmetric, true, 112);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 1, RotaryEncoderFillMode.FillToCenterSymmetric, true, 113);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 2, RotaryEncoderFillMode.FillToCenterSymmetric, true, 114);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 3, RotaryEncoderFillMode.FillToCenterSymmetric, true, 115);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 4, RotaryEncoderFillMode.FillToCenterSymmetric, true, 116);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 5, RotaryEncoderFillMode.FillToCenterSymmetric, true, 117);
            yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, 6, RotaryEncoderFillMode.FillToCenterSymmetric, true, 118);
            /*
            foreach (bool illuminateBounds in (bool[]) [false, true]) {
                int illuminateBoundsAdder = illuminateBounds ? 64 : 0;

                foreach (RotaryEncoderFillMode fill in Enum.GetValues<RotaryEncoderFillMode>()) {
                    int fillAdder = fill switch {
                        RotaryEncoderFillMode.FillCounterClockwise   => 32,
                        RotaryEncoderFillMode.FillToCenterAsymmetric => 17,
                        RotaryEncoderFillMode.FillToCenterSymmetric  => 48,
                        _                                            => 0
                    };

                    int maxLightPosition = fill switch {
                        RotaryEncoderFillMode.FillToCenterAsymmetric => 10,
                        RotaryEncoderFillMode.FillToCenterSymmetric  => 6,
                        _                                            => 11
                    };
                    for (int lightPosition = 0; lightPosition <= maxLightPosition; lightPosition++) {

                        yield return new TheoryDataRow<int, int, RotaryEncoderFillMode, bool, byte>(trackId, lightPosition, fill, illuminateBounds,
                            (byte) (illuminateBoundsAdder + fillAdder + lightPosition));
                    }
                }
            }
        */
        }
    }

    [Theory, MemberData(nameof(TrackIdData))]
    public void HandleRotaryEncoderPress(int trackId) {
        IRelativeRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        rotaryEncoder.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 68)));
        rotaryEncoder.IsPressed.Value.Should().BeTrue();
    }

    [Theory, MemberData(nameof(ValueValidRangeData))]
    public void ValueValidRange(int trackId, RotaryEncoderFillMode fill, int expectedMin, int expectedMax) {
        IHuiRotaryEncoder rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        rotaryEncoder.Fill.Connect(fill);
        rotaryEncoder.MinPosition.Value.Should().Be(expectedMin);
        rotaryEncoder.MaxPosition.Value.Should().Be(expectedMax);
    }

    public static IEnumerable<TheoryDataRow<int, RotaryEncoderFillMode, int, int>> ValueValidRangeData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, RotaryEncoderFillMode, int, int>(trackId, RotaryEncoderFillMode.NoFill, 0, 11);
            yield return new TheoryDataRow<int, RotaryEncoderFillMode, int, int>(trackId, RotaryEncoderFillMode.FillCounterClockwise, 0, 11);
            yield return new TheoryDataRow<int, RotaryEncoderFillMode, int, int>(trackId, RotaryEncoderFillMode.FillToCenterAsymmetric, 0, 10);
            yield return new TheoryDataRow<int, RotaryEncoderFillMode, int, int>(trackId, RotaryEncoderFillMode.FillToCenterSymmetric, 0, 6);
        }
    }

    [Theory, MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, bool expectedIsClockwise, uint expectedDistance) {
        IHuiRotaryEncoder                                         rotaryEncoder = XTouch.GetRotaryEncoder(trackId);
        IRelativeRotaryEncoder.RotaryEncoderRelativeRotationArgs? actual        = null;
        rotaryEncoder.Rotated += (_, args) => actual = args;

        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (64 + trackId), (SevenBitNumber) controlValue)));

        actual!.Value.IsClockwise.Should().Be(expectedIsClockwise);
        actual!.Value.Distance.Should().Be(expectedDistance);
    }

    public static IEnumerable<TheoryDataRow<int, int, bool, uint>> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, bool, uint>(trackId, 1, false, 1);
            yield return new TheoryDataRow<int, int, bool, uint>(trackId, 65, true, 1);
            yield return new TheoryDataRow<int, int, bool, uint>(trackId, 2, false, 2);
            yield return new TheoryDataRow<int, int, bool, uint>(trackId, 66, true, 2);
        }
    }

}