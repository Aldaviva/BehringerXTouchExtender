using System.ComponentModel;
using System.Text;
using KoKo.Property;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender.TrackControls;

internal class ScribbleStrip: WritableControl, IScribbleStrip {

    private readonly MidiClient _midiClient;

    public int TextColumnCount => 7;
    public int TrackId { get; }
    public ConnectableProperty<string> TopText { get; } = new(string.Empty);
    public ConnectableProperty<string> BottomText { get; } = new(string.Empty);
    public ConnectableProperty<ScribbleStripTextColor> TopTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripTextColor> BottomTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; } = new();

    public ScribbleStrip(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        TopText.PropertyChanged         += WriteStateToDevice;
        BottomText.PropertyChanged      += WriteStateToDevice;
        TopTextColor.PropertyChanged    += WriteStateToDevice;
        BottomTextColor.PropertyChanged += WriteStateToDevice;
        BackgroundColor.PropertyChanged += WriteStateToDevice;
    }

    internal override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        byte[] payload = new byte[22];
        // leading 0xF0 is automatically prepended by NormalSysExEvent, so don't add it again here
        payload[0] = 0;
        payload[1] = 0x20;
        payload[2] = 0x32;
        payload[3] = Constants.DeviceId;
        payload[4] = 0x4C;
        payload[5] = (byte) TrackId;
        payload[6] = (byte) ((int) BackgroundColor.Value | ((int) TopTextColor.Value << 4) | ((int) BottomTextColor.Value << 5));

        byte[] topTextBytes    = Enumerable.Repeat((byte) ' ', TextColumnCount).ToArray();
        byte[] bottomTextBytes = Enumerable.Repeat((byte) ' ', TextColumnCount).ToArray();
        Encoding.ASCII.GetBytes(TopText.Value, 0, Math.Min(TopText.Value.Length, TextColumnCount), topTextBytes, 0);
        Encoding.ASCII.GetBytes(BottomText.Value, 0, Math.Min(BottomText.Value.Length, TextColumnCount), bottomTextBytes, 0);

        for (int column = 0; column < TextColumnCount; column++) {
            payload[7 + column]                   = topTextBytes[column];
            payload[7 + column + TextColumnCount] = bottomTextBytes[column];
        }

        payload[21] = SysExEvent.EndOfEventByte;

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
    }

}