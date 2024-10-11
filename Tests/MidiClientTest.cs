using BehringerXTouchExtender.Exceptions;
using Melanchall.DryWetMidi.Multimedia;

namespace Tests;

public class MidiClientTest {

    private readonly IInputDevice  fromDevice = A.Fake<IInputDevice>();
    private readonly IOutputDevice toDevice   = A.Fake<IOutputDevice>();
    private readonly MidiClient    midiClient = new();

    public MidiClientTest() {
        midiClient.FromDevice = fromDevice;
        midiClient.ToDevice   = toDevice;
    }

    [Fact]
    public void IsOpen() {
        new MidiClient().IsOpen.Should().BeFalse();

        A.CallTo(() => fromDevice.IsListeningForEvents).Returns(false);
        midiClient.IsOpen.Should().BeFalse();

        A.CallTo(() => fromDevice.IsListeningForEvents).Returns(true);
        midiClient.IsOpen.Should().BeTrue();
    }

    [Fact]
    public void AssertOpen() {
        Action thrower = () => midiClient.AssertOpen();

        A.CallTo(() => fromDevice.IsListeningForEvents).Returns(true);
        thrower.Should().NotThrow<LifecycleException>();

        A.CallTo(() => fromDevice.IsListeningForEvents).Returns(false);
        thrower.Should().Throw<LifecycleException>();
    }

    [Fact]
    public void Dispose() {
        midiClient.Dispose();

        A.CallTo(() => fromDevice.Dispose()).MustHaveHappened();
        A.CallTo(() => toDevice.Dispose()).MustHaveHappened();

        midiClient.FromDevice.Should().BeNull();
        midiClient.ToDevice.Should().BeNull();
    }

}