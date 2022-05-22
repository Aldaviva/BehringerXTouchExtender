using BehringerXTouchExtender;

IDictionary<(int, IlluminatedButtonType), IlluminatedButtonState> buttonLightStates = new Dictionary<(int, IlluminatedButtonType), IlluminatedButtonState>();
IDictionary<int, bool>                                            touchingFaders    = new Dictionary<int, bool>();
IDictionary<int, int>                                             knobPositions     = new Dictionary<int, int>();

using IBehringerXTouchExtenderControlSurface<IRelativeRotaryEncoder> controller = BehringerXTouchExtenderControlSurface.CreateWithRelativeMode();

Console.WriteLine("Connecting to Behringer X-Touch Extender...");
controller.Open();
Console.WriteLine("Connected.");

for (int trackId = 1; trackId <= controller.TrackCount; trackId++) {
    IRelativeRotaryEncoder rotaryEncoder = controller.GetRotaryEncoder(trackId);
    rotaryEncoder.LightPosition.Connect(trackId - 1);
    rotaryEncoder.RotationPosition


    controller.GetVuMeter(trackId).LightPosition.Connect(trackId);
    controller.GetMuteButton(trackId).IlluminationState.Connect(IlluminatedButtonState.Blinking);
    controller.GetRecordButton(trackId).IlluminationState.Connect(IlluminatedButtonState.Blinking);
    controller.GetSoloButton(trackId).IlluminationState.Connect(IlluminatedButtonState.Blinking);
    controller.GetSelectButton(trackId).IlluminationState.Connect(IlluminatedButtonState.Blinking);

    double controlValue = (trackId - 1.0) / (controller.TrackCount - 1.0);
    controller.GetFader(trackId).DesiredPosition.Connect(controlValue);

    controller.GetScribbleStrip(trackId).TopText.Connect($"Track {trackId}");
    controller.GetScribbleStrip(trackId).BottomText.Connect(new string('.', trackId - 1));
    controller.GetScribbleStrip(trackId).TopTextColor.Connect(ScribbleStripTextColor.Dark);
    controller.GetScribbleStrip(trackId).BottomTextColor.Connect(ScribbleStripTextColor.Light);
    controller.GetScribbleStrip(trackId).BackgroundColor.Connect((ScribbleStripBackgroundColor) (trackId - 1));
}

// controller.MidiEventFromDevice += (sender, midiEvent) => {
//     switch (midiEvent) {
//         case ButtonPressed e:
//             Console.WriteLine($"Button {e.ButtonType} {e.TrackId} pressed");
//             if (e.ButtonType == PressableButtonType.Fader) {
//                 touchingFaders[e.TrackId] = true;
//             } else if (e.ButtonType.ToIlluminatedButtonType() is { } buttonToIlluminate) {
//                 (int, IlluminatedButtonType) key = (e.TrackId, buttonToIlluminate);
//                 IlluminatedButtonState newButtonState = buttonLightStates[key] switch {
//                     IlluminatedButtonState.Off      => IlluminatedButtonState.On,
//                     IlluminatedButtonState.On       => IlluminatedButtonState.Blinking,
//                     IlluminatedButtonState.Blinking => IlluminatedButtonState.Off,
//                     _                               => IlluminatedButtonState.On
//                 };
//                 buttonLightStates[key] = newButtonState;
//                 controller.SetButtonLight(e.TrackId, buttonToIlluminate, newButtonState);
//             }
//
//             break;
//         case ButtonReleased e:
//             Console.WriteLine($"Button {e.ButtonType} {e.TrackId} released");
//             if (e.ButtonType == PressableButtonType.Fader) {
//                 touchingFaders[e.TrackId] = false;
//             }
//
//             break;
//         case KnobRotatedRelative e:
//             Console.WriteLine($"Rotary encoder {e.TrackId} rotated {(e.DistanceRotatedClockwise > 0 ? "1 right" : "1 left")}");
//             int oldValue = knobPositions[e.TrackId];
//             int newValue = Math.Max(Math.Min(oldValue + e.DistanceRotatedClockwise, 12), 0);
//             knobPositions[e.TrackId] = newValue;
//             controller.RotateKnob(e.TrackId, newValue / 12.0);
//             break;
//         case SliderMoved e:
//             Console.WriteLine($"Fader {e.TrackId} moved to  {e.DistanceFromMinimumValue * 100:P0}");
//             if (!touchingFaders[e.TrackId]) {
//                 controller.MoveSlider(e.TrackId, e.DistanceFromMinimumValue);
//             }
//
//             break;
//     }
// };

// for (int trackId = 1; trackId < 8; trackId++) {
//     controller.RotateKnob(trackId, 0);
//     foreach (IlluminatedButtonType buttonType in Enums.GetValues<IlluminatedButtonType>()) {
//         buttonLightStates[(trackId, buttonType)] = IlluminatedButtonState.Blinking;
//         controller.SetButtonLight(trackId, buttonType, IlluminatedButtonState.Blinking);
//     }
//
//     double controlValue = (trackId - 1.0) / 7.0;
//     controller.RotateKnob(trackId, controlValue);
//     knobPositions[trackId] = (int) (controlValue * 12);
//
//     controller.MoveSlider(trackId, controlValue);
//     touchingFaders[trackId] = false;
//
//     controller.SetText(trackId, $"Track {trackId}", new string('.', trackId - 1), ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, (ScribbleStripBackgroundColor) (trackId - 1));
// }

Console.WriteLine("Press any key to exit.");
Console.ReadKey();