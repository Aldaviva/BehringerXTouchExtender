using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests.TrackControls;

public class AbsoluteRotaryEncoderTest {

    private readonly IInputDevice                    _fromDevice = A.Fake<IInputDevice>();
    private readonly IOutputDevice                   _toDevice   = A.Fake<IOutputDevice>();
    private readonly AbsoluteBehringerXTouchExtender _xTouch     = new();

    public AbsoluteRotaryEncoderTest() {
        _xTouch.MidiClient.FromDevice = _fromDevice;
        _xTouch.MidiClient.ToDevice   = _toDevice;

        _xTouch.SubscribeToEventsFromDevice();

        A.CallTo(() => _fromDevice.IsListeningForEvents).Returns(true);
    }

    [Theory, MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, double expectedPosition) {
        IAbsoluteRotaryEncoder rotaryEncoder = _xTouch.GetRotaryEncoder(trackId);

        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) controlValue)));

        rotaryEncoder.RotationPosition.Value.Should().BeApproximately(expectedPosition, 0.01);
    }

    public static IEnumerable<TheoryDataRow<int, int, double>> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < BehringerXTouchExtender.BehringerXTouchExtender.TRACK_COUNT; trackId++) {
            yield return new TheoryDataRow<int, int, double>(trackId, 0, 0);
            yield return new TheoryDataRow<int, int, double>(trackId, 32, 0.25);
            yield return new TheoryDataRow<int, int, double>(trackId, 64, 0.50);
            yield return new TheoryDataRow<int, int, double>(trackId, 95, 0.75);
            yield return new TheoryDataRow<int, int, double>(trackId, 127, 1.00);
        }
    }

}