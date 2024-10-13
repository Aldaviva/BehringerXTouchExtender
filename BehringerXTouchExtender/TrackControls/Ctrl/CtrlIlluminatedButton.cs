using BehringerXTouchExtender.Enums;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

internal class CtrlIlluminatedButton(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType): IlluminatedButton(midiClient, trackId, buttonType) {

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        SevenBitNumber noteId = (SevenBitNumber) (TrackId + ButtonType switch {
            IlluminatedButtonType.Record => 8,
            IlluminatedButtonType.Solo   => 16,
            IlluminatedButtonType.Mute   => 24,
            IlluminatedButtonType.Select => 32,
            _                            => throw new ArgumentOutOfRangeException(nameof(ButtonType), ButtonType, null)
        });

        SevenBitNumber velocity = (SevenBitNumber) (IlluminationState.Value switch {
            IlluminatedButtonState.Off      => SevenBitNumber.MinValue,
            IlluminatedButtonState.On       => SevenBitNumber.MaxValue,
            IlluminatedButtonState.Blinking => 64,
            _                               => SevenBitNumber.MinValue
        });

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new NoteOnEvent(noteId, velocity));
    }

}