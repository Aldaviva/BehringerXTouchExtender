using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class PressableButton: WritableControl, IPressableButton {

    public abstract int TrackId { get; }

    private readonly StoredProperty<bool> _isPressed = new();
    public Property<bool> IsPressed { get; }

    protected PressableButton() {
        IsPressed = _isPressed;
    }

    internal void OnButtonEvent(bool isPressed) {
        _isPressed.Value = isPressed;
    }

}