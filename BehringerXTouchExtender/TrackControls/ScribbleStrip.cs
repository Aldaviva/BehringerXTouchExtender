using System.Text;

namespace BehringerXTouchExtender.TrackControls;

internal abstract class ScribbleStrip(MidiClient midiClient, int trackId): WritableControl(midiClient), IScribbleStripInternal {

    private static readonly Encoding Encoding = Encoding.ASCII;

    public int TrackId { get; } = trackId;
    public abstract int TextColumnCount { get; }

    protected void WriteRightPaddedText(string text, byte[] destination, int destinationStartOffset) {
        int nonPaddingBytesWritten = Encoding.GetBytes(text, 0, Math.Min(text.Length, TextColumnCount), destination, destinationStartOffset);
        for (int destinationIndex = destinationStartOffset + nonPaddingBytesWritten; destinationIndex < destinationStartOffset + TextColumnCount; destinationIndex++) {
            destination[destinationIndex] = (byte) ' ';
        }
    }

}