using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class PressableButton: WritableControl, IPressableButtonInternal {

    public int TrackId { get; }

    private readonly StoredProperty<bool> _isPressed = new();
    public Property<bool> IsPressed { get; }

    protected PressableButton(MidiClient midiClient, int trackId): base(midiClient) {
        TrackId   = trackId;
        IsPressed = _isPressed;
    }

    public void OnButtonEvent(bool isPressed) {
        _isPressed.Value = isPressed;
    }

}