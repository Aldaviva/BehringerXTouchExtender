namespace BehringerXTouchExtender.Exceptions;

/// <summary>
/// Superclass of all application-specific exceptions thrown by this library.
/// </summary>
public abstract class ControlSurfaceException: Exception {

    /// <summary>
    /// Superclass of all application-specific exceptions thrown by this library.
    /// </summary>
    protected ControlSurfaceException(string message, Exception? cause = null): base(message, cause) {}

}

/// <summary>
/// Thrown if you try to <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}.Open"/> an <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}"/> instance while it's already open (while <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}.IsOpen"/> is <c>true</c>).
/// </summary>
public sealed class LifecycleException: ControlSurfaceException {

    /// <summary>
    /// Thrown if you try to <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}.Open"/> an <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}"/> instance while it's already open (while <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}.IsOpen"/> is <c>true</c>).
    /// </summary>
    public LifecycleException(string message): base(message) {}

}

/// <summary>
/// Thrown when a <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}"/> tries to open a connection to a Behringer X-Touch Extender connected over USB, and the connection fails because no such device was found, or it was already in use by another process on the same computer.
/// </summary>
public sealed class DeviceNotFoundException: ControlSurfaceException {

    /// <summary>
    /// Thrown when a <see cref="IBehringerXTouchExtender{TRotaryEncoder,TScribbleStrip}"/> tries to open a connection to a Behringer X-Touch Extender connected over USB, and the connection fails because no such device was found, or it was already in use by another process on the same computer.
    /// </summary>
    public DeviceNotFoundException(string message, Exception cause): base(message, cause) {}

}