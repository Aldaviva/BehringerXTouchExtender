using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls;

public class ScribbleStripTest: RelativeTrackControlTest {

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void Render(int trackId) {
        ICtrlScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(trackId);
        scribbleStrip.TopText.Connect("Hello");
        scribbleStrip.BottomText.Connect("World");
        scribbleStrip.TopTextColor.Connect(ScribbleStripTextColor.Light);
        scribbleStrip.BottomTextColor.Connect(ScribbleStripTextColor.Dark);
        scribbleStrip.BackgroundColor.Connect(ScribbleStripBackgroundColor.Magenta);

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x20, 0x32, 0x15, 0x4c, (byte) trackId, 0x25, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x20, 0x20, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappened();
    }

    [Theory]
    [MemberData(nameof(EncodeColorData))]
    public void EncodeColor(ScribbleStripTextColor topTextColor, ScribbleStripTextColor bottomTextColor, ScribbleStripBackgroundColor backgroundColor, byte expected) {
        ICtrlScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopTextColor.Connect(topTextColor);
        scribbleStrip.BottomTextColor.Connect(bottomTextColor);
        scribbleStrip.BackgroundColor.Connect(backgroundColor);
        scribbleStrip.TopText.Connect("sendnow");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.Matches(sysex => sysex.Data[6].Equals(expected)))).MustHaveHappened();
    }

    public static readonly TheoryData<ScribbleStripTextColor, ScribbleStripTextColor, ScribbleStripBackgroundColor, byte> EncodeColorData = new() {
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Black, 0x00 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Red, 0x01 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Green, 0x02 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Yellow, 0x03 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Blue, 0x04 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Magenta, 0x05 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Cyan, 0x06 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.White, 0x07 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Black, 0x10 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Red, 0x11 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Green, 0x12 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Yellow, 0x13 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Blue, 0x14 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Magenta, 0x15 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Cyan, 0x16 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.White, 0x17 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Black, 0x20 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Red, 0x21 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Green, 0x22 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Yellow, 0x23 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Blue, 0x24 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Magenta, 0x25 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Cyan, 0x26 },
        { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.White, 0x27 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Black, 0x30 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Red, 0x31 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Green, 0x32 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Yellow, 0x33 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Blue, 0x34 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Magenta, 0x35 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Cyan, 0x36 },
        { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.White, 0x37 }
    };

    [Fact]
    public void ShortTextIsPadded() {
        ICtrlScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("a");
        scribbleStrip.BottomText.Connect("");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x61, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LongTextIsTruncated() {
        ICtrlScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("Behringer");
        scribbleStrip.BottomText.Connect("X-Touch Extender");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x42, 0x65, 0x68, 0x72, 0x69, 0x6E, 0x67, 0x58, 0x2D, 0x54, 0x6F, 0x75, 0x63, 0x68, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappened();
    }

    [Fact]
    public void NonAsciiCharactersAreConverted() {
        ICtrlScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("Bÿ☃💩");

        // Technically this is converting the single grapheme cluster/rune 💩 into two ASCII bytes because it's encoded in UTF-16 with two code units instead of one (as a surrogate pair).
        // Ideally it would be converted into just "?" instead of "??" since it's only one grapheme cluster, but whatever.
        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x42, 0x3f, 0x3f, 0x3f, 0x3f, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [InlineData(DeviceModel.XTouchExtender, 0x15)]
    [InlineData(DeviceModel.XTouch, 0x14)]
    internal void DeviceId(DeviceModel deviceModel, byte expectedDeviceIdByte) {
        XTouch.MidiClient.DeviceModel = deviceModel;

        XTouch.GetScribbleStrip(0).TopText.Connect("A");
        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x20, 0x32, expectedDeviceIdByte, 0x4c, 0x00, 0x00, (byte) 'A', 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

}