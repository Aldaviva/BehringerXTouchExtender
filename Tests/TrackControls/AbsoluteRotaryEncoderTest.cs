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

    [Theory]
    [MemberData(nameof(RelativeRotaryEncoderRotationData))]
    public void HandleRotaryEncoderRotation(int trackId, int controlValue, double expectedPosition) {
        IAbsoluteRotaryEncoder rotaryEncoder = _xTouch.GetRotaryEncoder(trackId);

        _fromDevice.EventReceived += Raise.With(new MidiEventReceivedEventArgs(new ControlChangeEvent((SevenBitNumber) (80 + trackId), (SevenBitNumber) controlValue)));

        rotaryEncoder.RotationPosition.Value.Should().BeApproximately(expectedPosition, 0.01);
    }

    public static IEnumerable<object[]> RelativeRotaryEncoderRotationData() {
        for (int trackId = 0; trackId < 8; trackId++) {
            yield return [trackId, 0, 0];
            yield return [trackId, 32, 0.25];
            yield return [trackId, 64, 0.50];
            yield return [trackId, 95, 0.75];
            yield return [trackId, 127, 1.00];
        }
    }

}