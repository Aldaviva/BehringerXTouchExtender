namespace BehringerXTouchExtender;

// public interface IMidiEventFromDevice {
//
//     int TrackId { get; }
//
// }
//
// public readonly struct ButtonPressed: IMidiEventFromDevice {
//
//     public int TrackId { get; }
//     public PressableButtonType ButtonType { get; }
//
//     public ButtonPressed(int trackId, PressableButtonType buttonType) {
//         TrackId    = trackId;
//         ButtonType = buttonType;
//     }
//
// }
//
// public readonly struct ButtonReleased: IMidiEventFromDevice {
//
//     public int TrackId { get; }
//     public PressableButtonType ButtonType { get; }
//
//     public ButtonReleased(int trackId, PressableButtonType buttonType) {
//         TrackId    = trackId;
//         ButtonType = buttonType;
//     }
//
// }
//
// public readonly struct KnobRotatedRelative: IMidiEventFromDevice {
//
//     public int TrackId { get; }
//     public int DistanceRotatedClockwise { get; }
//
//     public KnobRotatedRelative(int trackId, int distanceRotatedClockwise) {
//         TrackId                  = trackId;
//         DistanceRotatedClockwise = distanceRotatedClockwise;
//     }
//
// }
//
// public readonly struct KnobRotatedAbsolute: IMidiEventFromDevice {
//
//     public int TrackId { get; }
//     public double DistanceFromMinimumValue { get; }
//
//     public KnobRotatedAbsolute(int trackId, double distanceFromMinimumValue) {
//         TrackId                  = trackId;
//         DistanceFromMinimumValue = distanceFromMinimumValue;
//     }
//
// }
//
// public readonly struct SliderMoved: IMidiEventFromDevice {
//
//     public int TrackId { get; }
//     public double DistanceFromMinimumValue { get; }
//
//     public SliderMoved(int trackId, double distanceFromMinimumValue) {
//         TrackId                  = trackId;
//         DistanceFromMinimumValue = distanceFromMinimumValue;
//     }
//
// }