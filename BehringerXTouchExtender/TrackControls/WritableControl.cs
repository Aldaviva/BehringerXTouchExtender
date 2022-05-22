using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class WritableControl {

    internal abstract void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null);

}