using BehringerXTouchExtender.Exceptions;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests;

public class RelativeBehringerXTouchExtenderTest {

    private readonly IInputDevice                    _fromDevice = A.Fake<IInputDevice>();
    private readonly IOutputDevice                   _toDevice   = A.Fake<IOutputDevice>();
    private readonly RelativeBehringerXTouchExtender _xtouch     = new();

    public RelativeBehringerXTouchExtenderTest() {
        _xtouch.MidiClient.FromDevice = _fromDevice;
        _xtouch.MidiClient.ToDevice   = _toDevice;
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

    [Fact]
    public void IgnoreUnrecognizedMidiEventType() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOffEvent(SevenBitNumber.MinValue, SevenBitNumber.MinValue)));
    }

    [Fact]
    public void IgnoreUnrecognizedNoteOnEvent() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new NoteOnEvent((SevenBitNumber) 119, SevenBitNumber.MinValue)));
    }

    [Fact]
    public void IgnoreUnrecognizedControlChangeEvent() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent(SevenBitNumber.MinValue, SevenBitNumber.MinValue)));
    }

}