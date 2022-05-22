using KoKo.Events;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class VuMeter: IVuMeter {

    private readonly MidiClient _midiClient;

    public int TrackId { get; }
    public int LightCount => 8;

    public ConnectableProperty<int> LightPosition { get; } = new();

    public VuMeter(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += OnLightPositionChanged;
    }

    private void OnLightPositionChanged(object sender, KoKoPropertyChangedEventArgs<int> args) {
        int newValue = Math.Max(Math.Min(args.NewValue, LightCount), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (90 + TrackId - 1);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount + 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}