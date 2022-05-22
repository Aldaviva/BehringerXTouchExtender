namespace BehringerXTouchExtender;

public static class Enums {

    public static IlluminatedButtonType? ToIlluminatedButtonType(this PressableButtonType buttonType) {
        return buttonType switch {
            PressableButtonType.Record => IlluminatedButtonType.Record,
            PressableButtonType.Solo   => IlluminatedButtonType.Solo,
            PressableButtonType.Mute   => IlluminatedButtonType.Mute,
            PressableButtonType.Select => IlluminatedButtonType.Select,
            _                          => null
        };
    }

    public static PressableButtonType ToPressableButtonType(this IlluminatedButtonType buttonType) {
        return buttonType switch {
            IlluminatedButtonType.Record => PressableButtonType.Record,
            IlluminatedButtonType.Solo   => PressableButtonType.Solo,
            IlluminatedButtonType.Mute   => PressableButtonType.Mute,
            IlluminatedButtonType.Select => PressableButtonType.Select,
            _                            => throw new ArgumentOutOfRangeException(nameof(buttonType), buttonType, null)
        };
    }

}

public enum ScribbleStripBackgroundColor {

    /// <summary>
    /// Text is completely illegible when background is set to <c>Black</c>, and it looks like the LCD is off or broken. It is strongly recommended to use a <c>White</c> background and <c>Light</c> text color
    /// to achieve the apperance of white text on a black background. <c>Black</c> is really only useful if you don't want to show any text and instead want to make it look like the LCD is off.
    /// </summary>
    Black,
    Red,
    Green,
    Yellow,
    Blue,
    Magenta,
    Cyan,
    White

}

public enum ScribbleStripTextColor {

    Light,
    Dark

}

public enum PressableButtonType {

    RotaryEncoder,
    Record,
    Solo,
    Mute,
    Select,
    Fader

}

public enum IlluminatedButtonType {

    Record,
    Solo,
    Mute,
    Select

}

public enum IlluminatedButtonState {

    Off,
    On,
    Blinking

}

public enum MidiControlMode {

    Absolute,
    Relative

}