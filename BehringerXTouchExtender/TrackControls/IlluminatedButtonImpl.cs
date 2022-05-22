using KoKo.Events;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class IlluminatedButtonImpl: PressableButton, IIlluminatedButton {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public IlluminatedButtonType ButtonType { get; }

    public ConnectableProperty<IlluminatedButtonState> IlluminationState { get; } = new();

    public IlluminatedButtonImpl(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType) {
        _midiClient = midiClient;
        TrackId     = trackId;
        ButtonType  = buttonType;

        IlluminationState.PropertyChanged += OnIlluminationStateChanged;
    }

    private void OnIlluminationStateChanged(object sender, KoKoPropertyChangedEventArgs<IlluminatedButtonState> args) {
        SevenBitNumber noteId = (SevenBitNumber) (TrackId - 1 + ButtonType switch {
            IlluminatedButtonType.Record => 8,
            IlluminatedButtonType.Solo   => 16,
            IlluminatedButtonType.Mute   => 24,
            IlluminatedButtonType.Select => 32,
            _                            => throw new ArgumentOutOfRangeException(nameof(ButtonType), ButtonType, null)
        });

        SevenBitNumber velocity = (SevenBitNumber) (args.NewValue switch {
            IlluminatedButtonState.Off      => SevenBitNumber.MinValue,
            IlluminatedButtonState.On       => SevenBitNumber.MaxValue,
            IlluminatedButtonState.Blinking => 64,
            _                               => SevenBitNumber.MinValue
        });

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NoteOnEvent(noteId, velocity));
    }

}