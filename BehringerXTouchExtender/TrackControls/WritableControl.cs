using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class WritableControl(MidiClient midiClient): MidiControl(midiClient), IWritableControl {

    public abstract void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null);

}