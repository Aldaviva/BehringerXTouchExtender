using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

internal class HuiVuMeter: WritableControl, IVuMeterInternal {

    private readonly MidiClient _midiClient;

    public int TrackId { get; }
    public int LightCount { get; } = 8;

    public ConnectableProperty<int> LightPosition { get; } = new();

    public HuiVuMeter(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        SevenBitNumber aftertouchValue = (SevenBitNumber) (LightPosition.Value switch {
            <= 0               => 0,
            <= 5 and var value => value + 1,
            var value          => 2 * (value - 6) + 8
        });

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NoteAftertouchEvent((SevenBitNumber) TrackId, aftertouchValue));
    }

}