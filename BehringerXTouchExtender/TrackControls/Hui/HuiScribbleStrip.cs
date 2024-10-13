using BehringerXTouchExtender.Utilities;
using KoKo.Property;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Hui;

/// <summary>
/// <para>Scribble strips are the dot-matrix LCDs that show text at the top of each track.</para>
/// <para>For details about the data format used to set their text and colors, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips.</para>
/// </summary>
internal class HuiScribbleStrip: ScribbleStrip, IHuiScribbleStripInternal {

    /// <summary>
    /// Byte count of a scribble strip SysEx message, not including the leading 0xF0 but including the trailing 0xF7
    /// </summary>
    private const int SysExMessageLength = 12;

    private const int TEXT_COLUMN_COUNT = 4;
    public override int TextColumnCount => TEXT_COLUMN_COUNT;

    private static readonly FixedSizeArrayPool<byte> ArrayPool = new(SysExMessageLength, HuiBehringerXTouchExtender.TRACK_COUNT);

    public ConnectableProperty<string> Text { get; } = new(string.Empty);

    public HuiScribbleStrip(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        Text.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        byte[] payload = ArrayPool.Borrow();
        // leading 0xF0 is automatically prepended by NormalSysExEvent, so don't add it again here
        payload[0] = 0;
        payload[1] = 0;
        payload[2] = 0x66;
        payload[3] = 0x05;
        payload[4] = 0;
        payload[5] = 0x10;
        payload[6] = (byte) TrackId;
        WriteRightPaddedText(Text.Value, payload, 7);
        payload[11] = SysExEvent.EndOfEventByte;

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
        ArrayPool.Return(payload);
    }

}