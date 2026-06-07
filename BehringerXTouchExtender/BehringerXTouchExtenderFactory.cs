namespace BehringerXTouchExtender;

/// <summary>
/// <para>Primary entry point into the <c>BehringerXTouchExtender</c> library.</para>
/// <para>Creates instances of <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}"/> that let you communicate with a Behringer X-Touch Extender MIDI device over USB.</para>
/// <para>The method that you should call depends on the device's configured operation mode, either <c>Ctrl</c>, <c>CtrlRel</c>, or <c>HUI</c>.</para>
/// <para>See <see cref="CreateWithAbsoluteMode"/>, <see cref="CreateWithRelativeMode"/>, and <see cref="CreateWithHuiMode"/> for descriptions of these three modes.</para>
/// </summary>
public static class BehringerXTouchExtenderFactory {

    /// <summary>
    /// <para>Create a client for a Behringer X-Touch Extender in Relative mode, which is where the rotary encoder knobs report rotation in terms of which direction they were turned.</para>
    /// 
    /// <para>For example, turning the knob clockwise will send one or more MIDI events to your computer indicating that the knob was turned 1 detent clockwise, and turning it farther will send more
    /// such events.</para>
    /// 
    /// <para>Ensure that the X-Touch Extender's hardware control mode is set to <c>HUI</c> when using this method. To set the hardware control mode of an X-Touch Extender:</para>
    /// 
    /// <list type="number">
    /// <item><description>Turn off the device.</description></item>
    /// <item><description>Press and hold the Select button on Track 1.</description></item>
    /// <item><description>Turn on the device.</description></item>
    /// <item><description>Release the Select button on Track 1.</description></item>
    /// <item><description>The LCD screen on Track 1 will show the current control mode. <c>Ctrl</c> is the MIDI Controller mode used by <see cref="CreateWithAbsoluteMode"/>. <c>CtrlRel</c> is the Relative MIDI Controller mode
    /// used by <see cref="CreateWithRelativeMode"/>. <c>HUI</c> is the Mackie Human User Interface mode used by <see cref="CreateWithHuiMode"/>. <c>MC</c> is not supported by this library.</description></item>
    /// <item><description>Set the control mode by turning the rotary encoder knob on Track 1 until the desired mode is shown on the LCD screen on Track 1.</description></item>
    /// <item><description>To save your changes and begin using the device, press the Select button on Track 1.</description></item>
    /// </list>
    /// </summary>
    public static IRelativeBehringerXTouchExtender CreateWithRelativeMode() {
        return new RelativeBehringerXTouchExtender();
    }

    /// <summary>
    /// <para>Create a client for a Behringer X-Touch Extender in Absolute mode, which is where the rotary encoder knobs report rotation in terms of how far it has cumulatively been turned from a
    /// fixed starting point.</para>
    /// 
    /// <para>For example, turning the knob clockwise will send one or more MIDI events to your computer indicating that the knob was turned to a certain distance in the range [0,1], where 0 is the starting point and the farthest possible counterclockwise value.</para>
    ///
    /// <para>Ensure that the X-Touch Extender's hardware control mode is set to <c>HUI</c> when using this method. To set the hardware control mode of an X-Touch Extender:</para>
    /// 
    /// <list type="number">
    /// <item><description>Turn off the device.</description></item>
    /// <item><description>Press and hold the Select button on Track 1.</description></item>
    /// <item><description>Turn on the device.</description></item>
    /// <item><description>Release the Select button on Track 1.</description></item>
    /// <item><description>The LCD screen on Track 1 will show the current control mode. <c>Ctrl</c> is the MIDI Controller mode used by <see cref="CreateWithAbsoluteMode"/>. <c>CtrlRel</c> is the Relative MIDI Controller mode
    /// used by <see cref="CreateWithRelativeMode"/>. <c>HUI</c> is the Mackie Human User Interface mode used by <see cref="CreateWithHuiMode"/>. <c>MC</c> is not supported by this library.</description></item>
    /// <item><description>Set the control mode by turning the rotary encoder knob on Track 1 until the desired mode is shown on the LCD screen on Track 1.</description></item>
    /// <item><description>To save your changes and begin using the device, press the Select button on Track 1.</description></item>
    /// </list>
    /// </summary>
    public static IAbsoluteBehringerXTouchExtender CreateWithAbsoluteMode() {
        return new AbsoluteBehringerXTouchExtender();
    }

    /// <summary>
    /// <para>Create a client for a Behringer X-Touch Extender in HUI mode, which lets multiple lights in the same track control be illuminated at once, but restricts the scribble strips to more primitive display capabilities. Its rotary encoder inputs are the same as Relative mode.</para>
    /// 
    /// <para>For example, HUI mode allows the VU meter lights below the selected value to also illuminate, not just the selected light by itself. This also applies to the rotary encoder lights.</para>
    /// 
    /// <para>Ensure that the X-Touch Extender's hardware control mode is set to <c>HUI</c> when using this method. To set the hardware control mode of an X-Touch Extender:</para>
    /// 
    /// <list type="number">
    /// <item><description>Turn off the device.</description></item>
    /// <item><description>Press and hold the Select button on Track 1.</description></item>
    /// <item><description>Turn on the device.</description></item>
    /// <item><description>Release the Select button on Track 1.</description></item>
    /// <item><description>The LCD screen on Track 1 will show the current control mode. <c>Ctrl</c> is the MIDI Controller mode used by <see cref="CreateWithAbsoluteMode"/>. <c>CtrlRel</c> is the Relative MIDI Controller mode
    /// used by <see cref="CreateWithRelativeMode"/>. <c>HUI</c> is the Mackie Human User Interface mode used by <see cref="CreateWithHuiMode"/>. <c>MC</c> is not supported by this library.</description></item>
    /// <item><description>Set the control mode by turning the rotary encoder knob on Track 1 until the desired mode is shown on the LCD screen on Track 1.</description></item>
    /// <item><description>To save your changes and begin using the device, press the Select button on Track 1.</description></item>
    /// </list>
    /// </summary>
    public static IHuiBehringerXTouchExtender CreateWithHuiMode() {
        return new HuiBehringerXTouchExtender();
    }

}