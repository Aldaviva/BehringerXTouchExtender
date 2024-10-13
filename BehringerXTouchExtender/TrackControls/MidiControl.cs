namespace BehringerXTouchExtender.TrackControls;

internal abstract class MidiControl(MidiClient midiClient) {

    protected readonly MidiClient MidiClient = midiClient;

}