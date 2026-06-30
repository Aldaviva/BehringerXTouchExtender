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
    public void IgnoreFaderVelocityControlChangeEvent() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 32, SevenBitNumber.MinValue)));
    }

    [Fact]
    public void IgnoreUnrecognizedButtonPressEvent() {
        _xtouch.SubscribeToEventsFromDevice();
        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) 47, (SevenBitNumber) 8)));
    }

    [Fact]
    public void Open() {
        using AutoResetEvent blinked     = new(false);
        int                  blinks      = 0;
        bool                 isListening = false;

        _xtouch.MidiClient.Dispose();
        A.CallTo(() => _deviceFactory.GetInputDeviceByName(A<string>._)).Returns(_fromDevice);
        A.CallTo(() => _deviceFactory.GetOutputDeviceByName(A<string>._)).Returns(_toDevice);
        A.CallTo(() => _fromDevice.StartEventsListening()).Invokes(() => isListening = true);
        A.CallTo(() => _fromDevice.IsListeningForEvents).ReturnsLazily(() => isListening);
        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.Matches(midiEvent => midiEvent.ControlNumber == 0x2c))).Invokes((MidiEvent midiEvent) => {
            int? nextState = (byte) ((ControlChangeEvent) midiEvent).ControlValue switch {
                71 => 1,
                7  => 2,
                _  => null
            };

            if (nextState != null) {
                Interlocked.CompareExchange(ref blinks, nextState.Value, nextState.Value - 1);
                blinked.Set();
            }
        });

        try {
            _xtouch.GetRecordButton(0).IlluminationState.Connect(IlluminatedButtonState.Blinking);
        } catch (LifecycleException) {}

        _xtouch.Open();

        A.CallTo(() => _toDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] { 0x00, 0x00, 0x66, 0x14, 0x00, 0xF7 }), SysExEventComparer.Instance)))
            .MustHaveHappened();

        while (blinks < 2) {
            blinked.WaitOne();
        }

        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) 0x2c, (SevenBitNumber) 71), ControlChangeEventComparer.Instance)))
            .MustHaveHappenedOnceOrMore();
        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.IsEqualTo(new ControlChangeEvent((SevenBitNumber) 0x2c, (SevenBitNumber) 7), ControlChangeEventComparer.Instance)))
            .MustHaveHappened(BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT + 1, Times.OrMore);
    }

    [Fact]
    public void BlinkButtonsIgnoresDeviceErrors() {
        using CountdownEvent blinkLatch = new(2);

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

        A.CallTo(() => _toDevice.SendEvent(A<ControlChangeEvent>.That.Matches(midiEvent => midiEvent.ControlNumber == (SevenBitNumber) 0x0c || midiEvent.ControlNumber == 0x2c)))
            .Invokes(() => {
                try {
                    blinkLatch.Signal();
                } catch (InvalidOperationException) {}
            }).Throws<MidiDeviceException>();

        blinkLatch.Wait(TestContext.Current.CancellationToken);
    }

    [Fact]
    public void HealthCheckIgnoresDeviceErrors() {
        bool isListening = false;
        _xtouch.MidiClient.Dispose();
        A.CallTo(() => _deviceFactory.GetInputDeviceByName(A<string>._)).Returns(_fromDevice);
        A.CallTo(() => _deviceFactory.GetOutputDeviceByName(A<string>._)).Returns(_toDevice);
        A.CallTo(() => _fromDevice.StartEventsListening()).Invokes(() => isListening = true);
        A.CallTo(() => _fromDevice.IsListeningForEvents).ReturnsLazily(() => isListening);
        byte[] expectedHealthCheck = [0, 0, 0x66, 0x14, 0, 0xF7];
        A.CallTo(() => _toDevice.SendEvent(A<SysExEvent>.That.Matches(midiEvent => midiEvent.Data.SequenceEqual(expectedHealthCheck)))).Throws<MidiDeviceException>();

        try {
            _xtouch.GetRecordButton(0).IlluminationState.Connect(IlluminatedButtonState.Blinking);
        } catch (LifecycleException) {}

        _xtouch.Open();
    }

    public void Dispose() {
        _xtouch.Dispose();
        GC.SuppressFinalize(this);
    }

}