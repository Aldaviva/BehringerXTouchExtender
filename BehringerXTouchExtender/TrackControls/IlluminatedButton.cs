using BehringerXTouchExtender.Enums;
using KoKo.Property;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class IlluminatedButton: PressableButton, IIlluminatedButtonInternal {

    public IlluminatedButtonType ButtonType { get; }
    public ConnectableProperty<IlluminatedButtonState> IlluminationState { get; } = new();

    protected IlluminatedButton(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType): base(midiClient, trackId) {
        ButtonType = buttonType;

        IlluminationState.PropertyChanged += WriteStateToDevice;
    }

}