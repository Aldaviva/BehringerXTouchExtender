using BehringerXTouchExtender.TrackControls;
using Melanchall.DryWetMidi.Core;
using Tests.Helpers;

namespace Tests.TrackControls.Hui;

public class HuiScribbleStripTest: HuiTrackControlTest {

    [Theory]
    [MemberData(nameof(TrackIdData))]
    public void Render(int trackId) {
        IHuiScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(trackId);
        scribbleStrip.Text.Connect("Hola");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x00, 0x66, 0x05, 0x00, 0x10, (byte) trackId, 0x48, 0x6f, 0x6c, 0x61, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappened();
    }

    [Fact]
    public void ShortTextIsPadded() {
        IHuiScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.Text.Connect("Hi");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x00, 0x66, 0x05, 0x00, 0x10, 0x00, 0x48, 0x69, 0x20, 0x20, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void LongTextIsTruncated() {
        IHuiScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.Text.Connect("Hello");

        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x00, 0x66, 0x05, 0x00, 0x10, 0x00, 0x48, 0x65, 0x6c, 0x6c, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

    [Fact]
    public void NonAsciiCharactersAreConverted() {
        IHuiScribbleStrip scribbleStrip = XTouch.GetScribbleStrip(0);
        scribbleStrip.Text.Connect("Bÿ☃💩");

        // Technically this is converting the single grapheme cluster/rune 💩 into two ASCII bytes because it's encoded in UTF-16 with two code units instead of one (as a surrogate pair).
        // Ideally it would be converted into just "?" instead of "??" since it's only one grapheme cluster, but whatever.
        A.CallTo(() => ToDevice.SendEvent(A<NormalSysExEvent>.That.IsEqualTo(new NormalSysExEvent(new byte[] {
            0x00, 0x00, 0x66, 0x05, 0x00, 0x10, 0x00, 0x42, 0x3f, 0x3f, 0x3f, 0xf7
        }), SysExEventComparer.Instance))).MustHaveHappenedOnceExactly();
    }

}