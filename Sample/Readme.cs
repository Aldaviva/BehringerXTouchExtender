using BehringerXTouchExtender;
using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;

namespace Sample;

public class Readme {

    public void Text() {
        using IRelativeBehringerXTouchExtender device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();

        IRelativeRotaryEncoder rotaryEncoder = device.GetRotaryEncoder(0);
        rotaryEncoder.LightPosition.Connect(0);
        int rotaryEncoderLightCount = rotaryEncoder.LightCount;
        rotaryEncoder.IsPressed.PropertyChanged += (sender, args) => Console.WriteLine($"Rotary encoder was {(args.NewValue ? "pressed" : "released")}");
        rotaryEncoder.Rotated                   += (sender, args) => Console.WriteLine($"Rotary encoder was rotated 15° {(args.IsClockwise ? "clockwise" : "counter-clockwise")}");

        IIlluminatedButton button = device.GetRecordButton(0);
        button.IlluminationState.Connect(IlluminatedButtonState.On);

        IFader fader = device.GetFader(0);
        fader.IsPressed.PropertyChanged      += (sender, args) => Console.WriteLine($"{(args.NewValue ? "Touching" : "Not touching")} fader");
        fader.ActualPosition.PropertyChanged += (sender, args) => Console.WriteLine($"Fader moved to {args.NewValue:P0}");
    }

}