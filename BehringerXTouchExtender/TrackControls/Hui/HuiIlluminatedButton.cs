using BehringerXTouchExtender.Enums;
using KoKo.Property;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiIlluminatedButton: PressableButton, IIlluminatedButtonInternal {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public IlluminatedButtonType ButtonType { get; }

    public ConnectableProperty<IlluminatedButtonState> IlluminationState { get; } = new();

    public HuiIlluminatedButton(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType) {
        _midiClient = midiClient;
        TrackId     = trackId;
        ButtonType  = buttonType;

        IlluminationState.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        //TODO
    }

}