using BehringerXTouchExtender.Enums;
using KoKo.Property;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls;

/// <summary>
/// <para>Any of the inputs or outputs on the device which belong to a numbered track or channel column, for example, the record button on track 3.</para>
/// </summary>
public interface ITrackControl {

    /// <summary>
    /// <para>The 0-indexed track number of the control.</para>
    /// <para>The Behringer X-Touch Extender has 8 tracks. This library numbers them from <c>0</c> (on the left side) to <c>7</c> (on the right).</para>
    /// <para>The number of tracks is available in <see cref="IBehringerXTouchExtender{TRotaryEncoder}.TrackCount"/> if you want to avoid hard-coding the magic value <c>8</c>.</para>
    /// </summary>
    int TrackId { get; }

}

public interface IScribbleStrip: ITrackControl {

    /// <summary>
    /// The maximum number of characters that can appear in each row of text.
    /// </summary>
    int TextColumnCount { get; }

}

/// <summary>
/// <para>An LCD screen that can show custom text and colors.</para>
/// <para>For raw protocol details, see https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips</para>
/// </summary>
public interface ICtrlScribbleStrip: IScribbleStrip {

    /// <summary>
    /// <para>The text that appears in the top half of the screen.</para>
    /// <para>Must be characters representable with 7-bit ASCII; unrepresentable characters will be converted to one or more <c>?</c> characters.</para>
    /// <para>The maximum length is <c>7</c> characters. If you set the property value to fewer than 7 characters, the text will be left-aligned in the screen with empty space on the right. If you set the value to more than 7 characters, the extra characters will be truncated.</para>
    /// <para>Defaults to the empty string.</para>
    /// </summary>
    ConnectableProperty<string> TopText { get; }

    /// <summary>
    /// <para>The text that appears in the bottom half of the screen.</para>
    /// <para>Must be characters representable with 7-bit ASCII; unrepresentable characters will be converted to one or more <c>?</c> characters.</para>
    /// <para>The maximum length is <c>7</c> characters. If you set the property value to fewer than 7 characters, the text will be left-aligned in the screen with empty space on the right. If you set the value to more than 7 characters, the extra characters will be truncated.</para>
    /// <para>Defaults to the empty string.</para>
    /// </summary>
    ConnectableProperty<string> BottomText { get; }

    /// <summary>
    /// <para>Whether the top row of text is <see cref="ScribbleStripTextColor.Light"/> with a dark background, or <see cref="ScribbleStripTextColor.Dark"/> with a light background.</para>
    /// <para>See <see cref="ScribbleStripTextColor"/> and <see cref="ScribbleStripBackgroundColor"/> for more details.</para>
    /// <para>Defaults to <see cref="ScribbleStripTextColor.Light"/>.</para>
    /// </summary>
    ConnectableProperty<ScribbleStripTextColor> TopTextColor { get; }

    /// <summary>
    /// <para>Whether the bottom row of text is <see cref="ScribbleStripTextColor.Light"/> with a dark background, or <see cref="ScribbleStripTextColor.Dark"/> with a light background.</para>
    /// <para>See <see cref="ScribbleStripTextColor"/> and <see cref="ScribbleStripBackgroundColor"/> for more details.</para>
    /// <para>Defaults to <see cref="ScribbleStripTextColor.Light"/>.</para>
    /// </summary>
    ConnectableProperty<ScribbleStripTextColor> BottomTextColor { get; }

    /// <summary>
    /// <para>Chooses the hue of the LCD screen.</para>
    /// <para>Note that choosing <see cref="ScribbleStripBackgroundColor.Black"/> will make it look like the screen is off, regardless of the text you set and its text color.</para>
    /// <para>See <see cref="ScribbleStripTextColor"/> and <see cref="ScribbleStripBackgroundColor"/> for more details.</para>
    /// <para>Defaults to <see cref="ScribbleStripBackgroundColor.Black"/>.</para>
    /// </summary>
    ConnectableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; }

}

public interface IHuiScribbleStrip: IScribbleStrip {

    ConnectableProperty<string> Text { get; }

}

/// <summary>
/// A control that the device can detect when you press or touch it
/// </summary>
public interface IPressableButton: ITrackControl {

    /// <summary>
    /// <para><c>true</c> when you are pressing down or (for <see cref="IFader"/>s) touching the control, and <c>false</c> otherwise.</para>
    /// <para>Defaults to <c>false</c>.</para>
    /// </summary>
    Property<bool> IsPressed { get; }

}

/// <summary>
/// A button that can light up with a built-in LED.
/// </summary>
public interface IIlluminatedButton: IPressableButton {

    /// <summary>
    /// The type of button (record, solo, mute, or select).
    /// </summary>
    IlluminatedButtonType ButtonType { get; }

    /// <summary>
    /// <para>Whether the button's LED is on, off, or blinking.</para>
    /// <para>For more details, see <see cref="IlluminatedButtonState"/>.</para>
    /// </summary>
    ConnectableProperty<IlluminatedButtonState> IlluminationState { get; }

}

/// <summary>
/// <para>A knob you can turn or press, with a ring of 13 orange LEDs surrounding it.</para>
/// <para>Clicking in on the top of the knob will update the <see cref="IPressableButton.IsPressed"/> property.</para>
/// </summary>
public interface IRotaryEncoder: IPressableButton {

    /// <summary>
    /// <c>13</c>, the number of LEDs around each rotary encoder.
    /// </summary>
    int LightCount { get; }

    /// <summary>
    /// <para>Which of the LEDs is currently illuminated, where <c>0</c> is the most counterclockwise light and <c>12</c> is the most clockwise.</para>
    /// <para>Exactly one will be lit up at any given time.</para>
    /// <para>Defaults to <c>0</c>.</para>
    /// </summary>
    ConnectableProperty<int> LightPosition { get; }

    Property<int> MinPosition { get; }
    Property<int> MaxPosition { get; }

}

/// <summary>
/// <para>A knob you can turn or press, with a ring of orange LEDs surrounding it.</para>
/// <para>Clicking in on the top of the knob will update the <see cref="IPressableButton.IsPressed"/> property.</para>
/// <para>Rotation events are reported with their direction: clockwise or counter-clockwise.</para>
/// <para>This type of rotary encoder is used in <c>CtrlRel</c> and <c>HUI</c> modes.</para>
/// </summary>
public interface IRelativeRotaryEncoder: IRotaryEncoder {

    /// <summary>
    /// <para>Event fired when you turn the knob of a rotary encoder. The event describes the direction you turned it.</para>
    /// <para><c>CtrlRel</c> mode: There is no angular distance reported. Instead, to express how far the knob was turned, multiple events will be fired, one for each detent. There are 24 detents per complete 360° rotation (15° apart).</para>
    /// <para><c>HUI</c> mode: Rotation events are batched and report an angular distance, which is a positive number starting at 1 and increasing for higher rotation speeds.</para>
    /// </summary>
    event EventHandler<RotaryEncoderRelativeRotationArgs> Rotated;

    /// <summary>
    /// Whether the rotary encoder was turned clockwise or counterclockwise.
    /// </summary>
    /// <param name="isClockwise"><c>true</c> if the rotary encoder was turned clockwise, or <c>false</c> if it was turned counterclockwise</param>
    /// <param name="distance">Absolute value of how far the encoder was rotated, in an arbitrary angular unit. In <c>CtrlRel</c> mode, this is always 1, but in <c>HUI</c> mode it may be more than 1 for fast rotations. A distance of 1 means 1 detent, which is 15°.</param>
    readonly struct RotaryEncoderRelativeRotationArgs(bool isClockwise, uint distance = 1) {

        /// <summary>
        /// <c>true</c> if the rotary encoder was turned clockwise, or <c>false</c> if it was turned counterclockwise
        /// </summary>
        public bool IsClockwise { get; } = isClockwise;

        /// <summary>
        /// <para>How far (in arbitrary angular units) the rotary encoder was rotated. Never 0, and always positive, regardless of the value of <see cref="IsClockwise"/>.</para>
        /// <para>In <c>CtrlRel</c> mode, this is always 1, which represents a rotation of 1 detent, or 15°. Fast rotations result in multiple events being fired.</para>
        /// <para>In <c>HUI</c> mode, this will be 1 for slow rotations, but for faster rotations (more than one detent per reporting interval) the value will be greater than 1. This batching allows the device to batch updates in the face of frequent changes without having to send a deluge of events of distance 1.</para>
        /// </summary>
        public uint Distance { get; } = distance;

    }

}

/// <summary>
/// <para>A knob you can turn or press, with a ring of orange LEDs surrounding it.</para>
/// <para>Clicking in on the top of the knob will update the <see cref="IPressableButton.IsPressed"/> property.</para>
/// <para>Rotation events are reported with their distance from their original angle when the device was powered on.</para>
/// <para>This type of rotary encoder is used when you call <see cref="BehringerXTouchExtenderFactory.CreateWithAbsoluteMode"/> and set the X-Touch Extender to use <c>Ctrl</c> mode.</para>
/// </summary>
public interface IAbsoluteRotaryEncoder: IRotaryEncoder {

    /// <summary>
    /// <para>How far the knob was rotated from its starting point, which was set to <c>0.0</c> when the device was powered on.</para>
    /// <para>The range of possible values is [0.0, 1.0].</para>
    /// <para>If you try to rotate the encoder past this range, the values will be clipped to stay inside this range.</para>
    /// </summary>
    Property<double> RotationPosition { get; }

}

/// <summary>
/// <para>A VU (voice unit) meter, which is the column of eight LEDs.</para>
/// <para>At most one LED can be illuminated at any given time. They can also all be turned off.</para>
/// <para>The bottom four LEDs are green, the next three are yellow, and the top one is red.</para>
/// </summary>
public interface IVuMeter: ITrackControl {

    /// <summary>
    /// <c>8</c>, the number of LEDs in each track's VU meter.
    /// </summary>
    int LightCount { get; }

    /// <summary>
    /// <para>Which of the eight LEDs is illuminated.</para>
    /// <para>To turn all LEDs on this control off, set the value to <c>0</c>. The default value is <c>0</c>.</para>
    /// <para>The bottom green light has position <c>1</c>, going up to the top red light at position <c>8</c>.</para>
    /// </summary>
    ConnectableProperty<int> LightPosition { get; }

}

/// <summary>
/// <para>The slider knob that moves up and down.</para>
/// <para>Not only can you move it with your finger, but the X-Touch Extender can also move them automatically to a specified position.</para>
/// <para>It can also detect when you are touching the fader with your finger, exposed in the <see cref="IPressableButton.IsPressed"/> property.</para>
/// </summary>
public interface IFader: IPressableButton {

    /// <summary>
    /// <para>Use this property to request that the X-Touch Extender move the motorized fader to the given position automatically.</para>
    /// <para>Values are between <c>0.0</c> at the bottom and <c>1.0</c> at the top. The default value is <c>0.0</c>.</para>
    /// <para>If you change this property while the user's finger is touching the fader knob, your change will be ignored so the user isn't startled or injured by the unexpected rapid movement.</para>
    /// <para>This value will not update when the user manually moves the fader, because that would cause an infinite loop of events. To get the current position, use <see cref="ActualPosition"/>.</para>
    /// </summary>
    ConnectableProperty<double> DesiredPosition { get; }

    /// <summary>
    /// <para>The current position of the fader, whether it was set automatically by the motorized sliders or manually by the user's finger.</para>
    /// <para>Values are between <c>0.0</c> at the bottom and <c>1.0</c> at the top. The default value is <c>0.0</c>.</para>
    /// <para>To move the fader with the built-in motors, change the <see cref="DesiredPosition"/> value.</para>
    /// </summary>
    Property<double> ActualPosition { get; }

}

internal interface IWritableControl {

    void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null);

}

internal interface IScribbleStripInternal: IScribbleStrip, IWritableControl;

internal interface ICtrlScribbleStripInternal: ICtrlScribbleStrip, IWritableControl;

internal interface IHuiScribbleStripInternal: IHuiScribbleStrip, IWritableControl;

internal interface IFaderInternal: IFader, IPressableButtonInternal, IWritableControl {

    void OnFaderMoved(double newPosition);

}

internal interface IVuMeterInternal: IVuMeter, IWritableControl;

internal interface IIlluminatedButtonInternal: IIlluminatedButton, IPressableButtonInternal, IWritableControl;

internal interface IPressableButtonInternal: IPressableButton {

    void OnButtonEvent(bool isPressed);

}

internal interface IRelativeRotaryEncoderInternal: IRelativeRotaryEncoder, IPressableButtonInternal, IWritableControl {

    void OnRotated(bool isClockwise, uint distance);

}

internal interface IAbsoluteRotaryEncoderInternal: IAbsoluteRotaryEncoder, IPressableButtonInternal, IWritableControl {

    SettableProperty<double> AbsoluteRotationPosition { get; }

}

public interface IHuiRotaryEncoder: IRelativeRotaryEncoder {

    ConnectableProperty<bool> IlluminateBounds { get; }
    ConnectableProperty<RotaryEncoderFillMode> Fill { get; }

}

internal interface IHuiRotaryEncoderInternal: IHuiRotaryEncoder, IRelativeRotaryEncoderInternal;