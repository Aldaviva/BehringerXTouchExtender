using BehringerXTouchExtender.Exceptions;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal class MidiClient: IDisposable {

    public IInputDevice?  FromDevice;
    public IOutputDevice? ToDevice;

    public bool IsOpen => ToDevice is not null && (FromDevice?.IsListeningForEvents ?? false);

    /// <exception cref="LifecycleException"></exception>
    public void AssertOpen() {
        if (!IsOpen) {
            throw new LifecycleException("IBehringerXTouchExtenderControlSurface instance has not been opened. Call Open() before calling this method.");
        }
    }

    public void Dispose() {
        ToDevice?.Dispose();
        ToDevice = null;

        FromDevice?.Dispose();
        FromDevice = null;
    }

}