namespace BehringerXTouchExtender;

public static class Enums {

    public static IEnumerable<T> GetValues<T>() where T: Enum {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

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