namespace BehringerXTouchExtender.Exceptions;

/// <summary>
/// Superclass of all application-specific exceptions thrown by this library.
/// </summary>
public abstract class ControlSurfaceException: Exception {

    /// <summary>
    /// Superclass of all application-specific exceptions thrown by this library.
    /// </summary>
    protected ControlSurfaceException(string message, Exception? cause = null): base(message, cause) { }

}

/// <summary>
/// Thrown if you try to <see cref="IBehringerXTouchExtender{TRotaryEncoder}.Open"/> an <see cref="IBehringerXTouchExtender{TRotaryEncoder}"/> instance while it's already open (while <see cref="IBehringerXTouchExtender{TRotaryEncoder}.IsOpen"/> is <c>true</c>).
/// </summary>
public class LifecycleException: ControlSurfaceException {

    /// <summary>
    /// Thrown if you try to <see cref="IBehringerXTouchExtender{TRotaryEncoder}.Open"/> an <see cref="IBehringerXTouchExtender{TRotaryEncoder}"/> instance while it's already open (while <see cref="IBehringerXTouchExtender{TRotaryEncoder}.IsOpen"/> is <c>true</c>).
    /// </summary>
    public LifecycleException(string message): base(message) { }

}

/// <summary>
/// Thrown when a <see cref="IBehringerXTouchExtender{TRotaryEncoder}"/> tries to open a connection to a Behringer X-Touch Extender connected over USB, and the connection fails because no such device was found, or it was already in use by another process on the same computer.
/// </summary>
public class DeviceNotFoundException: ControlSurfaceException {

    /// <summary>
    /// Thrown when a <see cref="IBehringerXTouchExtender{TRotaryEncoder}"/> tries to open a connection to a Behringer X-Touch Extender connected over USB, and the connection fails because no such device was found, or it was already in use by another process on the same computer.
    /// </summary>
    public DeviceNotFoundException(string message, Exception cause): base(message, cause) { }

}