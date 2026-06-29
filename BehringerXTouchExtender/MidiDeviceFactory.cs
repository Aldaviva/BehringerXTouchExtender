using Melanchall.DryWetMidi.Multimedia;
using System.Diagnostics.CodeAnalysis;

namespace BehringerXTouchExtender;

[ExcludeFromCodeCoverage]
internal class MidiDeviceFactory {

    public static readonly MidiDeviceFactory Instance = new();

    /// <inheritdoc cref="InputDevice.GetByName" />
    public virtual IInputDevice GetInputDeviceByName(string name) => InputDevice.GetByName(name);

    /// <inheritdoc cref="OutputDevice.GetByName" />
    public virtual IOutputDevice GetOutputDeviceByName(string name) => OutputDevice.GetByName(name);

}