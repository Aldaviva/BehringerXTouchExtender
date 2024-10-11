using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

internal class VuMeter: WritableControl, IVuMeterInternal {

    private readonly MidiClient _midiClient;

    public int TrackId { get; }
    public int LightCount { get; } = 8;

    public ConnectableProperty<int> LightPosition { get; } = new();

    public VuMeter(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        int newValue = Math.Max(Math.Min(LightPosition.Value, LightCount), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (90 + TrackId);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount + 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}