using Melanchall.DryWetMidi.Multimedia;

/*
 * This namespace is excluded from code coverage using BehringerXTouchExtender.sln.DotSettings (dotCover) and coverlet.runsettings (dotnet test, CI)
 */
namespace BehringerXTouchExtender.Façades;

internal class DryWetMidiFaçade {

    public virtual IInputDevice? GetInputDeviceByName(string name) {
        return InputDevice.GetByName(name);
    }

    public virtual IOutputDevice? GetOutputDeviceByName(string name) {
        return OutputDevice.GetByName(name);
    }

}