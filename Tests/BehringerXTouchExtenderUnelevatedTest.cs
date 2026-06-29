using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;

namespace Tests;

public class BehringerXTouchExtenderUnelevatedTest {

    private readonly RelativeBehringerXTouchExtender _xtouch = new() { MidiDeviceFactory = A.Fake<MidiDeviceFactory>() };

    [Fact]
    public void InputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _xtouch.MidiDeviceFactory.GetInputDeviceByName(A<string>._)).Throws<ArgumentException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Fact]
    public void OutputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _xtouch.MidiDeviceFactory.GetOutputDeviceByName(A<string>._)).Throws<ArgumentException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Fact]
    public void AlreadyInUse() {
        IOutputDevice outputDevice = A.Fake<IOutputDevice>();

        Action thrower = () => _xtouch.Open();
        A.CallTo(() => _xtouch.MidiDeviceFactory.GetOutputDeviceByName(A<string>._)).Returns(outputDevice);
        A.CallTo(() => outputDevice.PrepareForEventsSending()).Throws<MidiDeviceException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [Theory, MemberData(nameof(OpenData))]
    public void Open(IBehringerXTouchExtender<IRotaryEncoder, IScribbleStrip> xTouch) {
        IOutputDevice     toDevice          = A.Fake<IOutputDevice>();
        IInputDevice      fromDevice        = A.Fake<IInputDevice>();
        MidiDeviceFactory midiDeviceFactory = A.Fake<MidiDeviceFactory>();
        ((BehringerXTouchExtender.BehringerXTouchExtender) xTouch).MidiDeviceFactory = midiDeviceFactory;

        A.CallTo(() => midiDeviceFactory.GetInputDeviceByName(A<string>._)).Returns(fromDevice);
        A.CallTo(() => midiDeviceFactory.GetOutputDeviceByName(A<string>._)).Returns(toDevice);

        bool isListening = false;
        A.CallTo(() => fromDevice.StartEventsListening()).Invokes(() => isListening = true);
        A.CallTo(() => fromDevice.IsListeningForEvents).ReturnsLazily(() => isListening);

        xTouch.Open();

        A.CallTo(() => midiDeviceFactory.GetInputDeviceByName("X-Touch-Ext")).MustHaveHappenedOnceExactly();
        A.CallTo(() => midiDeviceFactory.GetOutputDeviceByName("X-Touch-Ext")).MustHaveHappenedOnceExactly();
        A.CallTo(() => toDevice.PrepareForEventsSending()).MustHaveHappenedOnceExactly();
        A.CallTo(() => fromDevice.StartEventsListening()).MustHaveHappenedOnceExactly();

        // All rec, solo, mute, and select button lights are turned off
        for (int noteId = 8; noteId < 32 + 8; noteId++) {
            A.CallTo(() => toDevice.SendEvent(A<NoteEvent>.That.Matches(midiEvent => NoteEventComparer.Instance.Equals(midiEvent,
                new NoteOnEvent((SevenBitNumber) noteId, (SevenBitNumber) 0))))).MustHaveHappenedOnceExactly();
        }

        for (int trackId = 0; trackId < 8; trackId++) {
            // All VU meter lights are turned off
            A.CallTo(() => toDevice.SendEvent(A<ControlChangeEvent>.That.Matches(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                new ControlChangeEvent((SevenBitNumber) (90 + trackId), (SevenBitNumber) 7))))).MustHaveHappenedOnceExactly();

            // All faders are moved to the bottom
            A.CallTo(() => toDevice.SendEvent(A<ControlChangeEvent>.That.Matches(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                new ControlChangeEvent((SevenBitNumber) (70 + trackId), (SevenBitNumber) 0))))).MustHaveHappenedOnceExactly();

            // All rotary encoders are moved to the most counterclockwise position
            A.CallTo(() => toDevice.SendEvent(A<ControlChangeEvent>.That.Matches(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) 0))))).MustHaveHappenedOnceExactly();

            // All scribble strips are set to no text, light text color, dark background color, making them appear to be off
            A.CallTo(() => toDevice.SendEvent(A<SysExEvent>.That.Matches(midiEvent => SysExEventComparer.Instance.Equals(midiEvent,
                    new NormalSysExEvent(new byte[] { 0, 0x20, 0x32, 0x15, 0x4c, (byte) trackId, 0, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7 })))))
                .MustHaveHappenedOnceExactly();
        }
    }

    public static TheoryData<IBehringerXTouchExtender<IRotaryEncoder, IScribbleStrip>> OpenData => [
        new RelativeBehringerXTouchExtender(),
        new AbsoluteBehringerXTouchExtender()
    ];

}