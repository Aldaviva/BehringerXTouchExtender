namespace BehringerXTouchExtender.Enums;

public enum ScribbleStripBackgroundColor: byte {

    /// <summary>
    /// Text is completely illegible when background is set to <c>Black</c>, and it looks like the LCD is off or broken. It is strongly recommended to use a <c>White</c> background and <c>Light</c> text color
    /// to achieve the appearance of white text on a black background. <c>Black</c> is really only useful if you don't want to show any text and instead want to make it look like the LCD is off.
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

public enum ScribbleStripTextColor: byte {

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