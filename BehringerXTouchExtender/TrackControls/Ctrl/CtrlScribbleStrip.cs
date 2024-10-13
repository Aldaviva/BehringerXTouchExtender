using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.Utilities;
using KoKo.Property;
using Melanchall.DryWetMidi.Core;
using System.ComponentModel;

namespace BehringerXTouchExtender.TrackControls.Ctrl;

/// <summary>
/// <para>Scribble strips are the dot-matrix LCDs that show text at the top of each track.</para>
/// <para>For details about the data format used to set their text and colors, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips.</para>
/// </summary>
internal class CtrlScribbleStrip: ScribbleStrip, ICtrlScribbleStripInternal {

    /// <summary>
    /// Byte count of a scribble strip SysEx message, not including the leading 0xF0 but including the trailing 0xF7
    /// </summary>
    private const int SysExMessageLength = 22;

    private const int TEXT_COLUMN_COUNT = 7;

    private static readonly FixedSizeArrayPool<byte> ArrayPool = new(SysExMessageLength, RelativeBehringerXTouchExtender.TRACK_COUNT);

    public override int TextColumnCount => TEXT_COLUMN_COUNT;
    public ConnectableProperty<string> TopText { get; } = new(string.Empty);
    public ConnectableProperty<string> BottomText { get; } = new(string.Empty);
    public ConnectableProperty<ScribbleStripTextColor> TopTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripTextColor> BottomTextColor { get; } = new();
    public ConnectableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; } = new();

    /// <summary>
    /// <para>Scribble strips are the dot-matrix LCDs that show text at the top of each track.</para>
    /// <para>For details about the data format used to set their text and colors, refer to https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips.</para>
    /// </summary>
    public CtrlScribbleStrip(MidiClient midiClient, int trackId): base(midiClient, trackId) {
        TopText.PropertyChanged         += WriteStateToDevice;
        BottomText.PropertyChanged      += WriteStateToDevice;
        TopTextColor.PropertyChanged    += WriteStateToDevice;
        BottomTextColor.PropertyChanged += WriteStateToDevice;
        BackgroundColor.PropertyChanged += WriteStateToDevice;
    }

    public override void WriteStateToDevice(object? sender = null, PropertyChangedEventArgs? args = null) {
        byte[] payload = ArrayPool.Borrow();
        // leading 0xF0 is automatically prepended by NormalSysExEvent, so don't add it again here
        payload[0] = 0;    // Behringer manufacturer ID (https://electronicmusic.fandom.com/wiki/List_of_MIDI_Manufacturer_IDs#Europe)
        payload[1] = 0x20; // Behringer manufacturer ID
        payload[2] = 0x32; // Behringer manufacturer ID
        /*
         * Device ID, NOT 0x42 as documented! Thanks https://community.musictribe.com/t5/Recording/X-Touch-Extender-Scribble-Strip-Midi-Sysex-Command/td-p/251306.
         * According to https://mediadl.musictribe.com/download/software/behringer/X-TOUCH/Document_BE_X-TOUCH-X-TOUCH-EXTENDER-MIDI-Mode-Implementation.pdf, this value is 0x14 on the X-Touch (as opposed to 0x15 on the X-Touch Extender).
         */
        payload[3] = MidiClient.DeviceModel switch {
            DeviceModel.XTouch              => 0x14,
            DeviceModel.XTouchExtender or _ => 0x15
        };
        payload[4] = 0x4C;
        payload[5] = (byte) TrackId;
        payload[6] = (byte) ((int) BackgroundColor.Value | ((int) TopTextColor.Value << 4) | ((int) BottomTextColor.Value << 5));
        WriteRightPaddedText(TopText.Value, payload, 7);
        WriteRightPaddedText(BottomText.Value, payload, 14);
        payload[21] = SysExEvent.EndOfEventByte;

        MidiClient.AssertOpen();
        MidiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
        ArrayPool.Return(payload);
    }

}