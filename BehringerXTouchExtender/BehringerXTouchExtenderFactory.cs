namespace BehringerXTouchExtender;

public class BehringerXTouchExtenderFactory {

    /// <summary>
    /// <para>Create a client for a Behringer X-Touch Extender in Relative mode, which is where the rotary encoder knobs report rotation in terms of which direction they were turned.</para>
    /// 
    /// <para>For example, turning the knob clockwise will send one or more MIDI events to your computer indicating that the knob was turned 1 detent clockwise, and turning it farther will send more
    /// such events.</para>
    /// 
    /// <para>Ensure that the X-Touch Extender's hardware control mode is set to <c>CtrlRel</c> when using this method.</para>
    /// 
    /// <para>To set the hardware control mode of an X-Touch Extender:</para>
    /// 
    /// <list type="number">
    /// <item><description>Turn off the device.</description></item>
    /// <item><description>Press and hold the Select button on Track 1.</description></item>
    /// <item><description>Turn on the device.</description></item>
    /// <item><description>Release the Select button on Track 1.</description></item>
    /// <item><description>The LCD screen on Track 1 will show the current control mode. <c>MC</c> (Mackie Control) and <c>HUI</c> (Mackie Human User Interface) modes are used by Digital Audio
    /// Workstations and are not supported by this library. <c>Ctrl</c> is the MIDI Controller mode used by <see cref="CreateWithAbsoluteMode"/>. <c>CtrlRel</c> is the Relative MIDI Controller mode
    /// used by <see cref="CreateWithRelativeMode"/>.</description></item>
    /// <item><description>Set the control mode by turning the rotary encoder knob on Track 1 until the desired mode is shown on the LCD screen on Track 1.</description></item>
    /// <item><description>To save your changes and begin using the device, press the Select button on Track 1.</description></item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public static IRelativeBehringerXTouchExtender CreateWithRelativeMode() {
        return new RelativeBehringerXTouchExtender();
    }

    /// <summary>
    /// <para>Create a client for a Behringer X-Touch Extender in Absolute mode, which is where the rotary encoder knobs report rotation in terms of how far it has cumulatively been turned from a
    /// fixed starting point.</para>
    /// 
    /// <para>For example, turning the knob clockwise will send one or more MIDI events to your computer indicating that the knob was turned to a certain distance in the range [0,1], where 0 is the starting point and the farthest possible counterclockwise value.</para>
    ///
    /// <para>Ensure that the X-Touch Extender's hardware control mode is set to <c>Ctrl</c> when using this method.</para>
    /// 
    /// <para>To set the control mode of an X-Touch Extender:</para>
    /// 
    /// <list type="number">
    /// <item><description>Turn off the device.</description></item>
    /// <item><description>Press and hold the Select button on Track 1.</description></item>
    /// <item><description>Turn on the device.</description></item>
    /// <item><description>Release the Select button on Track 1.</description></item>
    /// <item><description>The LCD screen on Track 1 will show the current control mode. <c>MC</c> (Mackie Control) and <c>HUI</c> (Mackie Human User Interface) modes are used by Digital Audio
    /// Workstations and are not supported by this library. <c>Ctrl</c> is the MIDI Controller mode used by <see cref="CreateWithAbsoluteMode"/>. <c>CtrlRel</c> is the Relative MIDI Controller mode
    /// used by <see cref="CreateWithRelativeMode"/>.</description></item>
    /// <item><description>Set the control mode by turning the rotary encoder knob on Track 1 until the desired mode is shown on the LCD screen on Track 1.</description></item>
    /// <item><description>To save your changes and begin using the device, press the Select button on Track 1.</description></item>
    /// </list>
    /// </summary>
    /// <returns></returns>
    public static IAbsoluteBehringerXTouchExtender CreateWithAbsoluteMode() {
        return new AbsoluteBehringerXTouchExtender();
    }

}