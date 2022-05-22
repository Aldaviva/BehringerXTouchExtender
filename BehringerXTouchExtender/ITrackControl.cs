using KoKo.Property;

namespace BehringerXTouchExtender;

public interface ITrackControl {

    int TrackId { get; }

}

public interface IScribbleStrip: ITrackControl {

    int TextColumnCount { get; }
    ConnectableProperty<string> TopText { get; }
    ConnectableProperty<string> BottomText { get; }
    ConnectableProperty<ScribbleStripTextColor> TopTextColor { get; }
    ConnectableProperty<ScribbleStripTextColor> BottomTextColor { get; }
    ConnectableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; }

}

public interface IPressableButton: ITrackControl {

    Property<bool> IsPressed { get; }

}

public interface IIlluminatedButton: IPressableButton {

    IlluminatedButtonType ButtonType { get; }
    ConnectableProperty<IlluminatedButtonState> IlluminationState { get; }

}

public interface IRotaryEncoder: IPressableButton {

    int LightCount { get; }

    ConnectableProperty<int> LightPosition { get; }

}

public interface IRelativeRotaryEncoder: IRotaryEncoder {

    delegate void RotaryEncoderRelativeRotationHandler(object sender, RotaryEncoderRelativeRotationArgs args);
    event RotaryEncoderRelativeRotationHandler Rotated;

    record RotaryEncoderRelativeRotationArgs(bool IsClockwise) {

        public bool IsClockwise { get; } = IsClockwise;

    }

}

public interface IAbsoluteRotaryEncoder: IRotaryEncoder {

    Property<double> RotationPosition { get; }

}

public interface IVuMeter: ITrackControl {

    ConnectableProperty<int> LightPosition { get; }
    int LightCount { get; }

}

public interface IFader: IPressableButton {

    ConnectableProperty<double> DesiredPosition { get; }
    Property<double> ActualPosition { get; }

}