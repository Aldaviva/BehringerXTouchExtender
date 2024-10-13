using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.Exceptions;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal class MidiClient: IDisposable {

    public IInputDevice?  FromDevice;
    public IOutputDevice? ToDevice;
    public DeviceModel?   DeviceModel;

    public bool IsOpen => ToDevice is not null && (FromDevice?.IsListeningForEvents ?? false);

    /// <exception cref="LifecycleException"></exception>
    public void AssertOpen() {
        if (!IsOpen) {
            throw new LifecycleException("IBehringerXTouchExtenderControlSurface instance has not been opened. Call Open() before calling this method.");
        }
    }

    /// <inheritdoc />
    public void Dispose() {
        ToDevice?.Dispose();
        ToDevice = null;

        FromDevice?.Dispose();
        FromDevice = null;

        DeviceModel = null;
    }

}

internal class HuiMidiClient: MidiClient {

    private volatile int _mostRecentOutputTrackId = -1;

    public bool BlinkingButtonsAreIlluminated;

    /// <summary>
    /// Call this before writing to a track control.
    /// </summary>
    public void BeforeWritingToTrackControl(int trackId) {
        if (_mostRecentOutputTrackId != trackId) {
            ToDevice?.SendEvent(new ControlChangeEvent((SevenBitNumber) 0x0c, (SevenBitNumber) trackId));
            _mostRecentOutputTrackId = trackId;
        }
    }

}