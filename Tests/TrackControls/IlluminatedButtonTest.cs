using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests.TrackControls;

public class IlluminatedButtonTest: AbstractTrackControlTest {

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleRecordButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) (8 + trackId), (SevenBitNumber) 127)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleSoloButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) 127)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleMuteButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) (24 + trackId), (SevenBitNumber) 127)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void HandleSelectButtonPress(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IsPressed.Value.Should().BeFalse();
        FromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) (32 + trackId), (SevenBitNumber) 127)));
        button.IsPressed.Value.Should().BeTrue();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (8 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (8 + trackId), (SevenBitNumber) 64), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetRecordButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetRecordButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (8 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();

        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (8 + trackId), (SevenBitNumber) 0), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) 64), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSoloButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetSoloButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();

        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (16 + trackId), (SevenBitNumber) 0), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (24 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (24 + trackId), (SevenBitNumber) 64), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetMuteButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetMuteButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (24 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();

        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (24 + trackId), (SevenBitNumber) 0), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightOn(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);

        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (32 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightBlinking(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.Blinking);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (32 + trackId), (SevenBitNumber) 64), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void SetSelectButtonLightOff(int trackId) {
        IIlluminatedButton button = XTouch.GetSelectButton(trackId);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (32 + trackId), (SevenBitNumber) 127), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();

        button.IlluminationState.Connect(IlluminatedButtonState.Off);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) (32 + trackId), (SevenBitNumber) 0), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void InvalidEnumValues() {
        IIlluminatedButton button = new IlluminatedButton(XTouch.MidiClient, 0, (IlluminatedButtonType) 4);

        Action thrower = () => button.IlluminationState.Connect(IlluminatedButtonState.On);
        thrower.Should().Throw<ArgumentOutOfRangeException>();

        button = XTouch.GetRecordButton(0);
        button.IlluminationState.Connect(IlluminatedButtonState.On);
        button.IlluminationState.Connect((IlluminatedButtonState) 3);
        A.CallTo(() => ToDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) 8, (SevenBitNumber) 0), NoteEventComparer.Instance)))
            .MustHaveHappenedOnceExactly();
    }

}