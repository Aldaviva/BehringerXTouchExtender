using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests.TrackControls.Hui;

public class HuiIlluminatedButtonTest: HuiTrackControlTest {

    private static readonly SevenBitNumber SetLightControlNumber = (SevenBitNumber) 0x2c;

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleRecordButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 71)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleSoloButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 67)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleMuteButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 66)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleSelectButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 15, (SevenBitNumber) trackId)));
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 65)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 71), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 7), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 7), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 67), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 3), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 3), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 66), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 2), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);

        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 2), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 65), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 1), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent(SetLightControlNumber, (SevenBitNumber) 1), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

}