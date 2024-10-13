using BehringerXTouchExtender.Enums;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiIlluminatedButton(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType): IlluminatedButton(midiClient, trackId, buttonType) {

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int buttonId = ButtonType switch {
            IlluminatedButtonType.Record => 7,
            IlluminatedButtonType.Solo   => 3,
            IlluminatedButtonType.Mute   => 2,
            IlluminatedButtonType.Select => 1
        };

        int controlValue = buttonId + (IlluminationState.Value == IlluminatedButtonState.On
            || (IlluminationState.Value == IlluminatedButtonState.Blinking && ((HuiMidiClient) MidiClient).BlinkingButtonsAreIlluminated) ? 64 : 0);

        MidiClient.AssertOpen();
        ((HuiMidiClient) MidiClient).BeforeWritingToTrackControl(TrackId);
        MidiClient.ToDevice?.SendEvent(new ControlChangeEvent((SevenBitNumber) 0x2c, (SevenBitNumber) controlValue));
    }

}