namespace BehringerXTouchExtender.Exceptions;

public abstract class ControlSurfaceException: Exception {

    protected ControlSurfaceException(string message, Exception? cause = null): base(message, cause) { }

}

public class LifecycleException: ControlSurfaceException {

    public LifecycleException(string message): base(message) { }

}

public class DeviceNotFoundException: ControlSurfaceException {

    public DeviceNotFoundException(string message): base(message) { }
    public DeviceNotFoundException(string message, Exception cause): base(message, cause) { }

}