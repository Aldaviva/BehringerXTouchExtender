using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Core;

namespace Tests.TrackControls;

public class ScribbleStripTest: AbstractTrackControlTest {

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void Render(int trackId) {
        IScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(trackId);
        scribbleStrip.TopText.Connect("Hello");
        scribbleStrip.BottomText.Connect("World");
        scribbleStrip.TopTextColor.Connect(ScribbleStripTextColor.Light);
        scribbleStrip.BottomTextColor.Connect(ScribbleStripTextColor.Dark);
        scribbleStrip.BackgroundColor.Connect(ScribbleStripBackgroundColor.Magenta);

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(
            new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x32, 0x15, 0x4c, (byte) trackId, 0x25, 0x48, 0x65, 0x6c, 0x6c, 0x6f, 0x20, 0x20, 0x57, 0x6f, 0x72, 0x6c, 0x64, 0x20, 0x20, 0xf7 }),
            SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Theory]
    [MemberData(nameof(EncodeColorData))]
    public void EncodeColor(ScribbleStripTextColor topTextColor, ScribbleStripTextColor bottomTextColor, ScribbleStripBackgroundColor backgroundColor, byte expected) {
        IScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopTextColor.Connect(topTextColor);
        scribbleStrip.BottomTextColor.Connect(bottomTextColor);
        scribbleStrip.BackgroundColor.Connect(backgroundColor);
        scribbleStrip.TopText.Connect("sendnow");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.Matches(sysex => sysex.Data[6].Equals(expected)))).MustHaveHappened();
    }

    public static readonly IEnumerable<object[]> EncodeColorData = new[] {
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Black, (byte) 0x00 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Red, (byte) 0x01 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Green, (byte) 0x02 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Yellow, (byte) 0x03 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Blue, (byte) 0x04 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Magenta, (byte) 0x05 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Cyan, (byte) 0x06 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.White, (byte) 0x07 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Black, (byte) 0x10 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Red, (byte) 0x11 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Green, (byte) 0x12 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Yellow, (byte) 0x13 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Blue, (byte) 0x14 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Magenta, (byte) 0x15 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.Cyan, (byte) 0x16 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripBackgroundColor.White, (byte) 0x17 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Black, (byte) 0x20 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Red, (byte) 0x21 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Green, (byte) 0x22 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Yellow, (byte) 0x23 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Blue, (byte) 0x24 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Magenta, (byte) 0x25 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Cyan, (byte) 0x26 },
        new object[] { ScribbleStripTextColor.Light, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.White, (byte) 0x27 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Black, (byte) 0x30 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Red, (byte) 0x31 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Green, (byte) 0x32 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Yellow, (byte) 0x33 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Blue, (byte) 0x34 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Magenta, (byte) 0x35 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.Cyan, (byte) 0x36 },
        new object[] { ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripBackgroundColor.White, (byte) 0x37 }
    };

    [Fact]
    public void ShortTextIsPadded() {
        IScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("a");
        scribbleStrip.BottomText.Connect("");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(
            new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x61, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7 }),
            SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LongTextIsTruncated() {
        IScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("Behringer");
        scribbleStrip.BottomText.Connect("X-Touch Extender");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(
            new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x42, 0x65, 0x68, 0x72, 0x69, 0x6E, 0x67, 0x58, 0x2D, 0x54, 0x6F, 0x75, 0x63, 0x68, 0xf7 }),
            SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void NonAsciiCharactersAreConverted() {
        IScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.TopText.Connect("Bÿ☃💩");

        // Technically this is converting the single grapheme cluster/rune 💩 into two ASCII bytes because it's encoded in UTF-16 with two code units instead of one (as a surrogate pair).
        // Ideally it would be converted into just "?" instead of "??" since it's only one grapheme cluster, but whatever.
        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(
            new NormalSysExEvent(new byte[] { 0x00, 0x20, 0x32, 0x15, 0x4c, 0x00, 0x00, 0x42, 0x3f, 0x3f, 0x3f, 0x3f, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0xf7 }),
            SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

}