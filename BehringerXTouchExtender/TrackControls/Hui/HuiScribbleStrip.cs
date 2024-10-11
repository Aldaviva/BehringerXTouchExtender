using BehringerXTouchExtender.Enums;
using KoKo.Property;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

/// <summary>
/// <para>Scribble strips are the dot-matrix LCDs that show text at the top of each track.</para>
/// <para>For details about the data format used to set their text and colors, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips.</para>
/// </summary>
internal class HuiScribbleStrip: WritableControl, IScribbleStripInternal {

    private const int TEXT_COLUMN_COUNT = 7;

    private readonly MidiClient _midiClient;

    public int TextColumnCount { get; } = TEXT_COLUMN_COUNT;
    public int TrackId { get; }
    public ConnectableProperty<string> TopText { get; } = new(string.Empty);
    public ConnectableProperty<string> BottomText { get; } = new(string.Empty);
    public ConnectableProperty<ScribbleStripTextColor> TopTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripTextColor> BottomTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; } = new();

    public HuiScribbleStrip(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        TopText.PropertyChanged         += WriteStateToDevice;
        BottomText.PropertyChanged      += WriteStateToDevice;
        TopTextColor.PropertyChanged    += WriteStateToDevice;
        BottomTextColor.PropertyChanged += WriteStateToDevice;
        BackgroundColor.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        //TODO
    }

}