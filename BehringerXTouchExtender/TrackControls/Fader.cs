using KoKo.Events;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class Fader: PressableButton, IFader {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public ConnectableProperty<double> DesiredPosition { get; } = new();

    private readonly StoredProperty<double> _actualPosition = new();
    public Property<double> ActualPosition => _actualPosition;

    public Fader(MidiClient midiClient, int trackId) {
        TrackId     = trackId;
        _midiClient = midiClient;

        DesiredPosition.PropertyChanged += MoveFader;
    }

    private void MoveFader(object sender, KoKoPropertyChangedEventArgs<double> args) {
        double newValue = Math.Max(Math.Min(args.NewValue, 1), 0);

        if (!IsPressed.Value) { //don't slide the fader out from under the user's finger
            //TODO ignore events caused when we tell the fader to automatically move
            SevenBitNumber controlId    = (SevenBitNumber) (70 + TrackId - 1);
            SevenBitNumber controlValue = (SevenBitNumber) Math.Round(newValue * SevenBitNumber.MaxValue);

            _midiClient.AssertOpen();
            _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
            Console.WriteLine($"User wants to automatically move fader {TrackId} to position {newValue:N3} and their finger is not touching the fader, sending command to device.");
        } else {
            Console.WriteLine($"User wants to automatically move fader {TrackId} to position {newValue:N3} but their finger is touching the fader, not sending command to device.");
        }
    }

    internal void OnFaderMoved(double newPosition) {
        _actualPosition.Value = newPosition;
    }

}