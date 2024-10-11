using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class WritableControl: IWritableControl {

    public abstract void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null);

}