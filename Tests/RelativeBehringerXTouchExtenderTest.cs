using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.Façades;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests;

public class RelativeBehringerXTouchExtenderTest {

    private readonly IInputDevice                    _fromDevice = A.Fake<IInputDevice>();
    private readonly IOutputDevice                   _toDevice   = A.Fake<IOutputDevice>();
    private readonly DryWetMidiFaçade                _midiFaçade = A.Fake<DryWetMidiFaçade>();
    private readonly RelativeBehringerXTouchExtender _xtouch     = new();

    public RelativeBehringerXTouchExtenderTest() {
        _xtouch.MidiClient.FromDevice = _fromDevice;
        _xtouch.MidiClient.ToDevice   = _toDevice;
        _xtouch.MidiFaçade            = _midiFaçade;

        A.CallTo(() => _midiFaçade.GetInputDeviceByName(A<string>._)).Returns(_fromDevice);
        A.CallTo(() => _midiFaçade.GetOutputDeviceByName(A<string>._)).Returns(_toDevice);
    }

    [Fact]
    public void TrackCount() {
        _xtouch.TrackCount.Should().Be(8);
    }

    [Fact]
    public void IsNotOpen() {
        _xtouch.IsOpen.Should().BeFalse();
    }

    [Fact]
    public void IsOpen() {
        A.CallTo(() => _fromDevice.IsListeningForEvents).Returns(true);
        _xtouch.IsOpen.Should().BeTrue();
    }

    [Fact]
    public void OpenTwice() {
        A.CallTo(() => _fromDevice.IsListeningForEvents).Returns(true);
        Action thrower = () => _xtouch.Open();
        thrower.Should().Throw<LifecycleException>();
    }

    [Fact]
    public void InputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _midiFaçade.GetInputDeviceByName(A<string>._)).Returns(null);
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Fact]
    public void OutputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _midiFaçade.GetOutputDeviceByName(A<string>._)).Returns(null);
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Fact]
    public void AlreadyInUse() {
        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _fromDevice.StartEventsListening()).Throws<MidiDeviceException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Fact]
    public void Open() {
        A.CallTo(() => _fromDevice.IsListeningForEvents).Returns(true);
        _xtouch.MidiClient.Dispose();
        _xtouch.Open();

        A.CallTo(() => _fromDevice.StartEventsListening()).MustHaveHappened();
        A.CallTo(() => _toDevice.PrepareForEventsSending()).MustHaveHappened();

        // All rec, solo, mute, and select button lights are turned off
        for (int noteId = 8; noteId < 32 + 8; noteId++) {
            A.CallTo(() => _toDevice.SendEvent(A<NoteOnEvent>.That.IsEqualTo(new NoteOnEvent((SevenBitNumber) noteId, (SevenBitNumber) 0), NoteEventComparer.Instance))).MustHaveHappenedOnceExactly();
        }

        for (int trackId = 0; trackId < 8; trackId++) {
            // All VU meter lights are turned off
            A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (90 + trackId), (SevenBitNumber) 7), ControlChangeEventComparer.Instance)))
                .MustHaveHappenedOnceExactly();

            // All faders are moved to the bottom
            A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (70 + trackId), (SevenBitNumber) 0), ControlChangeEventComparer.Instance)))
                .MustHaveHappenedOnceExactly();

            // All rotary encoders are moved to the most counterclockwise position
            A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) 0), ControlChangeEventComparer.Instance)))
                .MustHaveHappenedOnceExactly();

            // All scribble strips are set to no text, light text color, dark background color, making them appear to be off
            A.CallTo(() => _toDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(
                new NormalSysExEvent(new byte[] { 0, 0x20, 0x32, 0x15, 0x4c, (byte) trackId, 0, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7 }),
                SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
        }
    }

    [Theory]
    [InlineData(-2)]
    [InlineData(-1)]
    [InlineData(8)]
    [InlineData(9)]
    public void TheoryMethodName(int trackId) {
        Action thrower = () => _xtouch.GetRecordButton(trackId);
        thrower.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Dispose() {
        _xtouch.Dispose();
        _xtouch.MidiClient.FromDevice.Should().BeNull();
        _xtouch.MidiClient.ToDevice.Should().BeNull();
    }

}