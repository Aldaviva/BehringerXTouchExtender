namespace BehringerXTouchExtender;

internal readonly struct Constants {

    /// <summary>
    /// Device ID is not 0x42 as documented.
    /// Thanks https://community.musictribe.com/t5/Recording/X-Touch-Extender-Scribble-Strip-Midi-Sysex-Command/td-p/251306
    /// </summary>
    internal const byte DeviceId = 0x15;

    internal const string DeviceName = "X-Touch-Ext";

}