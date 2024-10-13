namespace BehringerXTouchExtender.Enums;

/// <summary>
/// <para>Controls the hue of the LCD screens. Each of the eight screens can independently have its entire background color set to one of eight choices.</para>
/// <para>The background color also has an effect on the hue of the foreground text color:</para>
/// <para>If the text color is <see cref="ScribbleStripTextColor.Light"/>, then the text will be a very light version of the background color, while the negative space (all the pixels that don't make up the glyphs) will be a much darker version of the background color. For example, very light green text on a dark green background.</para>
/// <para>If the text color is <see cref="ScribbleStripTextColor.Dark"/>, then the text will be the darker version of the background color, while the negative space will be the very light version of the background color. For example, dark green text inside a very light green rectangle.</para>
/// <para>The contrast between light and dark pixels can be adjusted from the X-Touch Extender's setup menu. Turn on the device while holding Track 1 Select, then turn Rotary Encoder 8 to change LCD Contrast. The default value is 40%. Press Track 1 Select again to resume normal operation.</para>
/// <para>For a visual example of <see cref="ScribbleStripTextColor.Light"/> and <see cref="ScribbleStripTextColor.Dark"/> text on a <see cref="Magenta"/> background, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips#example</para>
/// </summary>
public enum ScribbleStripBackgroundColor: byte {

    /// <summary>
    /// Text is completely illegible when background is set to <see cref="Black"/>, and it looks like the LCD is off or broken. It is strongly recommended to use a <see cref="White"/> background and <see cref="ScribbleStripTextColor.Light"/> text color
    /// to achieve the appearance of white text on a black background. <see cref="Black"/> is really only useful if you don't want to show any text and instead want to make it look like the LCD is off.
    /// </summary>
    Black,

    /// <summary>
    /// Red color
    /// </summary>
    Red,

    /// <summary>
    /// Green color
    /// </summary>
    Green,

    /// <summary>
    /// Yellow color
    /// </summary>
    Yellow,

    /// <summary>
    /// Blue color
    /// </summary>
    Blue,

    /// <summary>
    /// Magenta color
    /// </summary>
    Magenta,

    /// <summary>
    /// Cyan color
    /// </summary>
    Cyan,

    /// <summary>
    /// <para>To show white text on a black background, set the text color to <see cref="ScribbleStripTextColor.Light"/> and the background color to <see cref="White"/>.</para>
    /// <para>To show black text on a white background, set the text color to <see cref="ScribbleStripTextColor.Dark"/> and the background color to <see cref="White"/>.</para>
    /// </summary>
    White

}

/// <summary>
/// <para>Allows you to invert the text and background lightness in the LCD screens. This can be controlled separately for the top and bottom halves of each of the eight screens.</para>
/// <para>If the text color is <see cref="Light"/>, then the text will be a very light version of the background color, while the negative space (all the pixels that don't make up the glyphs) will be a much darker version of the background color. For example, very light green text on a dark green background.</para>
/// <para>If the text color is <see cref="Dark"/>, then the text will be the darker version of the background color, while the negative space will be the very light version of the background color. For example, dark green text inside a very light green rectangle.</para>
/// <para>The contrast between light and dark pixels can be adjusted from the X-Touch Extender's setup menu. Turn on the device while holding Track 1 Select, then turn Rotary Encoder 8 to change LCD Contrast. The default value is 40%. Press Track 1 Select again to resume normal operation.</para>
/// <para>For a visual example of <see cref="Light"/> and <see cref="Dark"/> text on a <see cref="ScribbleStripBackgroundColor.Magenta"/> background, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips#example</para>
/// </summary>
public enum ScribbleStripTextColor: byte {

    /// <summary>
    /// <para>Text will be a very light version of the background color, while the negative space will be a much darker version of the background color.</para>
    /// <para>For example, very light green text on a dark green background.</para>
    /// </summary>
    Light,

    /// <summary>
    /// <para>Text will be the darker version of the background color, while the negative space will be a very light version of the background color.</para>
    /// <para>For example, dark green text inside a very light green rectangle.</para>
    /// </summary>
    Dark

}

/// <summary>
/// Whether a button that lights up is a <c>REC</c>, <c>SOLO</c>, <c>MUTE</c>, or <c>SELECT</c> button
/// </summary>
public enum IlluminatedButtonType {

    /// <summary>
    /// The <c>REC</c> buttons that can light up red
    /// </summary>
    Record,

    /// <summary>
    /// The <c>SOLO</c> buttons that can light up yellow
    /// </summary>
    Solo,

    /// <summary>
    /// The <c>MUTE</c> buttons that can light up red
    /// </summary>
    Mute,

    /// <summary>
    /// The <c>SELECT</c> buttons that can light up green
    /// </summary>
    Select

}

/// <summary>
/// Whether a button is lit up or not, or blinking
/// </summary>
public enum IlluminatedButtonState {

    /// <summary>
    /// The LED inside the button is not lit up
    /// </summary>
    Off,

    /// <summary>
    /// The LED inside the button is lit up
    /// </summary>
    On,

    /// <summary>
    /// The LED inside the button is repeatedly alternating between on and off. The period of the animation is 1 second, with 0.5 sec on and 0.5 off.
    /// </summary>
    Blinking

}

public enum RotaryEncoderFillMode {

    NoFill,
    FillCounterclockwise,
    FillToCenterAsymmetric,
    FillToCenterSymmetric

}

internal enum DeviceModel {

    XTouchExtender,
    XTouch

}