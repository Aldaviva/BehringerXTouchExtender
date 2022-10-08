using System.ComponentModel;
using BehringerXTouchExtender.Enums;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class IlluminatedButton: PressableButton, IIlluminatedButton {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public IlluminatedButtonType ButtonType { get; }

    public ConnectableProperty<IlluminatedButtonState> IlluminationState { get; } = new();

    public IlluminatedButton(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType) {
        _midiClient = midiClient;
        TrackId     = trackId;
        ButtonType  = buttonType;

        IlluminationState.PropertyChanged += WriteStateToDevice;
    }

    internal override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
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

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NoteOnEvent(noteId, velocity));
    }

}