using BehringerXTouchExtender.Exceptions;
using BehringerXTouchExtender.TrackControls;

namespace BehringerXTouchExtender;

/// <summary>
/// <para>A client for communicating with a Behringer X-Touch Extender MIDI device over USB.</para>
/// <para>Create instances of this interface using the static methods on <see cref="BehringerXTouchExtenderFactory"/>.</para>
/// </summary>
/// <typeparam name="TRotaryEncoder">Whether the rotary encoders report relative (<see cref="IRelativeRotaryEncoder"/>) or absolute (<see cref="IAbsoluteRotaryEncoder"/>) rotation events</typeparam>
public interface IBehringerXTouchExtender<out TRotaryEncoder>: IDisposable where TRotaryEncoder: IRotaryEncoder {

    /// <summary>
    /// The number of tracks or channel columns on this device. Always returns <c>8</c>.
    /// </summary>
    int TrackCount { get; }

    /// <summary>
    /// <para>Whether or not this client has connected to the MIDI device. The device must be open in order to send commands and receive events from the device.</para>
    /// <para>To open a device, call <see cref="Open"/>.</para>
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    /// <para>Connects this client instance to the Behringer X-Touch Extender device that is turned on and plugged into this computer over USB.</para>
    /// <para>This client must be opened before you can send commands and receive events from the device.</para>
    /// <para>To determine whether an instance is open or not, call <see cref="IsOpen"/>.</para>
    /// </summary>
    /// <exception cref="LifecycleException">if <see cref="Open"/> has already been called on this instance</exception>
    /// <exception cref="DeviceNotFoundException">if a connected MIDI input or output device named <c>X-Touch-Ext</c> cannot be found</exception>
    void Open();

    /// <summary>
    /// Get a track's <c>REC</c> button.
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A button control that reads and writes values and events from the record button on the given track.</returns>
    IIlluminatedButton GetRecordButton(int trackId);

    /// <summary>
    /// Get a track's <c>MUTE</c> button.
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A button control that reads and writes values and events from the mute button on the given track.</returns>
    IIlluminatedButton GetMuteButton(int trackId);

    /// <summary>
    /// Get a track's <c>SOLO</c> button.
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A button control that reads and writes values and events from the solo button on the given track.</returns>
    IIlluminatedButton GetSoloButton(int trackId);

    /// <summary>
    /// Get a track's <c>SELECT</c> button.
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A button control that reads and writes values and events from the select button on the given track.</returns>
    IIlluminatedButton GetSelectButton(int trackId);

    /// <summary>
    /// <para>Get a track's rotary encoder, which is the knob you can turn.</para>
    /// <para>Depending on how the X-Touch Extender device was configured and the factory method you used from <see cref="BehringerXTouchExtenderFactory"/>, this will return either a <see cref="IRelativeRotaryEncoder"/> or <see cref="IAbsoluteRotaryEncoder"/>.</para>
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A relative or absolute rotary encoder control that reads and writes values and events from the rotary encoder on the given track.</returns>
    TRotaryEncoder GetRotaryEncoder(int trackId);

    /// <summary>
    /// <para>Get a track's VU (voice unit) meter, which is the vertical strip of eight red, yellow, and green LEDs.</para>
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A VU meter control that writes values to the VU meter on the given track.</returns>
    IVuMeter GetVuMeter(int trackId);

    /// <summary>
    /// <para>Get a track's fader, which is the slider knob that you can move forwards and backwards.</para>
    /// <para>Faders also have built-in motors that let you move the knobs programmatically.</para>
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A fader control that reads and writes values and events from the fader on the given track.</returns>
    IFader GetFader(int trackId);

    /// <summary>
    /// <para>Get a track's scribble strip, which is the LCD screen that can show text.</para>
    /// </summary>
    /// <param name="trackId">The 0-indexed track or channel number of the control, in the range [0, 7].</param>
    /// <returns>A scribble strip control that writes values to the LCD screen on the given track.</returns>
    IScribbleStrip GetScribbleStrip(int trackId);

}

/// <summary>
/// <para>A client for communicating with a Behringer X-Touch Extender which was set to <c>CtrlRel</c> mode in its onboard settings menu.</para>
/// <para>Create an instance of this interface using <see cref="BehringerXTouchExtenderFactory.CreateWithRelativeMode"/>.</para>
/// </summary>
public interface IRelativeBehringerXTouchExtender: IBehringerXTouchExtender<IRelativeRotaryEncoder> { }

/// <summary>
/// A client for communicating with a Behringer X-Touch Extender which was set to <c>Ctrl</c> mode in its onboard settings menu.
/// <para>Create an instance of this interface using <see cref="BehringerXTouchExtenderFactory.CreateWithAbsoluteMode"/>.</para>
/// </summary>
public interface IAbsoluteBehringerXTouchExtender: IBehringerXTouchExtender<IAbsoluteRotaryEncoder> { }