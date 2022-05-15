using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal class MidiClient: IDisposable {

    public IInputDevice?  FromDevice;
    public IOutputDevice? ToDevice;

    public bool IsOpen => ToDevice is not null && (FromDevice?.IsListeningForEvents ?? false);

    /// <exception cref="LifecycleException"></exception>
    public void AssertOpen() {
        if (!IsOpen) {
            throw new LifecycleException("IMidiControlSurface class instance has not been opened. Call IMidiControlSurface.Open() before calling this method.");
        }
    }

    public void Dispose() {
        (ToDevice as IDisposable)?.Dispose();
        ToDevice = null;

        (FromDevice as IDisposable)?.Dispose();
        FromDevice = null;
    }

}