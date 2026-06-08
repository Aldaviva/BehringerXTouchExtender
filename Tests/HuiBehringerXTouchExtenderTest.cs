using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.Exceptions;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests;

public class HuiBehringerXTouchExtenderTest: IDisposable {

    private readonly IInputDevice               _fromDevice    = A.Fake<IInputDevice>();
    private readonly IOutputDevice              _toDevice      = A.Fake<IOutputDevice>();
    private readonly MidiDeviceFactory          _deviceFactory = A.Fake<MidiDeviceFactory>();
    private readonly HuiBehringerXTouchExtender _xtouch        = new();

    public HuiBehringerXTouchExtenderTest() {
        _xtouch.MidiClient.FromDevice = _fromDevice;
        _xtouch.MidiClient.ToDevice   = _toDevice;
        _xtouch.MidiDeviceFactory     = _deviceFactory;
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
    public void InvalidTrackId(int trackId) {
        Action thrower = () => _xtouch.GetRecordButton(trackId);
        thrower.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Fact]
    public void Disposal() {
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
    public void IgnoreUnrecognizedControlChangeEvent() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 72, SevenBitNumber.MinValue)));
    }

    [Fact]
    public async Task Open() {
        bool isListening = false;
        _xtouch.MidiClient.Dispose();
        A.CallTo(() => _deviceFactory.GetInputDeviceByName(A<string>._)).Returns(_fromDevice);
        A.CallTo(() => _deviceFactory.GetOutputDeviceByName(A<string>._)).Returns(_toDevice);
        A.CallTo(() => _fromDevice.StartEventsListening()).Invokes(() => isListening = true);
        A.CallTo(() => _fromDevice.IsListeningForEvents).ReturnsLazily(() => isListening);

        try {
            _xtouch.GetRecordButton(0).IlluminationState.Connect(IlluminatedButtonState.Blinking);
        } catch (LifecycleException) {}

        _xtouch.Open();

        A.CallTo(() => _toDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] { 0x00, 0x00, 0x66, 0x14, 0x00, 0xF7 }), SysExEventComparer.Instance)))
            .MustHaveHappened();

        await Task.Delay(4500, TestContext.Current.CancellationToken);

        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) 0x2c, (SevenBitNumber) 71), ControlChangeEventComparer.Instance)))
            .MustHaveHappened();
        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) 0x2c, (SevenBitNumber) 7), ControlChangeEventComparer.Instance)))
            .MustHaveHappened();
    }

    public void Dispose() {
        _xtouch.Dispose();
        GC.SuppressFinalize(this);
    }

}