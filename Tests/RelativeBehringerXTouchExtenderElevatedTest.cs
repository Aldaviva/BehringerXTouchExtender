#if RUN_ELEVATED_TESTS
using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using Tests.Helpers;
using Xunit.Abstractions;

namespace Tests;

/*
 * In the IDE, these elevated (advanced mocking) tests only run when both of the following conditions are met:
 *   1. JustMock Profiler must be installed and enabled in Visual Studio - this seems to have changed, and ReSharper now adds the JustMock profiler if the Test project refers to the JustMock assembly at all, regardless of JustMock's Enabled state.
 *   2. Tests must be launched with ReSharper/dotCover's "Cover" mode, not "Run" or "Debug" - this seems to be broken with VS 17.11.5, dotCover 2024.2.20241008.92351, and JustMock 2024.3.805.336 Q3: tests run elevated in Run, but in Cover they're not elevated and also 0 coverage is collected.
 * If either of these conditions are not fulfilled, these tests which require elevation will be skipped.
 *
 * In CI, these tests will always run because of Tests.runsettings
 */
public class BehringerXTouchExtenderElevatedTest {
    
    private readonly ITestOutputHelper               _testOutputHelper;
    private readonly InputDevice                     _fromDevice = Mock.Create<InputDevice>();
    private readonly OutputDevice                    _toDevice = Mock.Create<OutputDevice>();
    private readonly RelativeBehringerXTouchExtender _xtouch = new();

    public BehringerXTouchExtenderElevatedTest(ITestOutputHelper testOutputHelper) {
        _testOutputHelper = testOutputHelper;

        Mock.SetupStatic(typeof(InputDevice), StaticConstructor.Mocked);
        Mock.SetupStatic(typeof(OutputDevice), StaticConstructor.Mocked);

        Mock.Arrange(() => InputDevice.GetByName(Arg.AnyString)).Returns(_fromDevice);
        Mock.Arrange(() => OutputDevice.GetByName(Arg.AnyString)).Returns(_toDevice);

        Mock.Arrange(() => _fromDevice.Dispose()).DoNothing();
        Mock.Arrange(() => _toDevice.Dispose()).DoNothing();
    }

    [ElevatedFact]
    public void InputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        Mock.Arrange(() => InputDevice.GetByName(Arg.AnyString)).Throws<ArgumentException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [ElevatedFact]
    public void OutputDeviceNotFound() {
        Action thrower = () => _xtouch.Open();
        Mock.Arrange(() => OutputDevice.GetByName(Arg.AnyString)).Throws<ArgumentException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    [ElevatedFact]
    public void AlreadyInUse() {
        Action thrower = () => _xtouch.Open();
        Mock.Arrange(() => _toDevice.PrepareForEventsSending()).Throws<MidiDeviceException>();
        thrower.Should().Throw<DeviceNotFoundException>();
    }

    public static readonly IEnumerable<object[]> XtouchData = [
        [new RelativeBehringerXTouchExtender()],
        [new AbsoluteBehringerXTouchExtender()]
    ];

    [ElevatedTheory]
    [MemberData(nameof(XtouchData))]
    public void Open(IBehringerXTouchExtender<IRotaryEncoder> xTouch) {
        /*
         * We need to dispose of these theory data instances early, before the test method returns, to avoid an uncaught exception. If we don't, the following confusing sequence occurs.
         *
         * 1. The test method returns successfully.
         * 2. xTouch.MidiClient contains two non-null references to the InputDevice and OutputDevice mock instances (_fromDevice and _toDevice)
         * 3. xUnit notices that the MemberData value implements IDisposable, so it calls Dispose on the IBehringerXTouchExtender<IRotaryEncoder> instance
         *    (https://github.com/xunit/xunit/blob/2.4.2/src/xunit.execution/Sdk/Frameworks/Runners/XunitTheoryTestCaseRunner.cs#L152)
         * 4. That calls the real MidiClient.Dispose() method
         * 5. MidiClient tries to dispose both of the mocked MidiDevices, which would ideally do nothing because that's how the mocks were set up
         * 6. Because I believe the call to MidiDevice.Dispose() originated from xUnit's assembly/appdomain/whatever and not our Tests assembly, the JustMock instrumentation doesn't intercept the call
         *    to the mocked MidiDevice.Dispose(), allowing the real Dispose() method to be called instead of the mocked method
         * 7. The real MidiDevice.Dispose() method understandably throws an exception because it was never initialized correctly in the first place
         *
         * By calling Dispose early, before the Open() method returns, JustMock instrumentation is still in effect, so MidiClient happily calls the mocked MidiDevice.Dispose() methods instead of the
         * real method, then sets its references to null so future calls to Dispose() will be no-ops.
         */
        using (xTouch) {
            bool isListening = false;
            Mock.Arrange(() => _fromDevice.StartEventsListening()).DoInstead(() => isListening = true);
            Mock.Arrange(() => _fromDevice.IsListeningForEvents).Returns(() => isListening);

            xTouch.Open();

            Mock.Assert(() => InputDevice.GetByName("X-Touch-Ext"), Occurs.Once());
            Mock.Assert(() => OutputDevice.GetByName("X-Touch-Ext"), Occurs.Once());

            Mock.Assert(() => _toDevice.PrepareForEventsSending(), Occurs.Once());
            Mock.Assert(() => _fromDevice.StartEventsListening(), Occurs.Once());

            // All rec, solo, mute, and select button lights are turned off
            for (int noteId = 8; noteId < 32 + 8; noteId++) {
                Mock.Assert(() => _toDevice.SendEvent(Arg.Matches<NoteEvent>(midiEvent => NoteEventComparer.Instance.Equals(midiEvent,
                    new NoteOnEvent((SevenBitNumber) noteId, (SevenBitNumber) 0)))), Occurs.Once());
            }

            for (int trackId = 0; trackId < 8; trackId++) {
                // All VU meter lights are turned off
                Mock.Assert(() => _toDevice.SendEvent(Arg.Matches<ControlChangeEvent>(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                    new ControlChangeEvent((SevenBitNumber) (90 + trackId), (SevenBitNumber) 7)))), Occurs.Once());

                // All faders are moved to the bottom
                Mock.Assert(() => _toDevice.SendEvent(Arg.Matches<ControlChangeEvent>(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                    new ControlChangeEvent((SevenBitNumber) (70 + trackId), (SevenBitNumber) 0)))), Occurs.Once());

                // All rotary encoders are moved to the most counterclockwise position
                Mock.Assert(() => _toDevice.SendEvent(Arg.Matches<ControlChangeEvent>(midiEvent => ControlChangeEventComparer.Instance.Equals(midiEvent,
                    new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) 0)))), Occurs.Once());

                // All scribble strips are set to no text, light text color, dark background color, making them appear to be off
                Mock.Assert(() => _toDevice.SendEvent(Arg.Matches<SysExEvent>(midiEvent => SysExEventComparer.Instance.Equals(midiEvent,
                        new NormalSysExEvent(new byte[] { 0, 0x20, 0x32, 0x15, 0x4c, (byte) trackId, 0, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7 })))),
                    Occurs.Once());
            }
        }
    }
}

#endif