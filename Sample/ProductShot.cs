using BehringerXTouchExtender;
using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;

namespace Sample;

public static class ProductShot {

    /// <summary>
    /// <para>Recreates the control state shown in the official product photograph on https://www.behringer.com/product.html?modelCode=0808-AAH</para>
    /// <para>Photo: https://cdn-media.empowertribe.com/9640d351590b4927b54183d8b8c05743/Image_BE_0808-AAH_X-TOUCH-EXTENDER_Top_XL.png</para>
    /// </summary>
    public static void Main() {
        using var device = BehringerXTouchExtenderFactory.CreateWithHuiMode();
        device.Open();

        int[]    ctrlRotaryEncoderLightPositions = [2, 2, 0, 0, 6, 10, 12, 12];
        int[]    huiRotaryEncoderLightPositions  = [1, 1, 0, 0, 5, 9, 10, 10];
        string[] scribbleStripBottomText         = ["-32", "-32", "-64", "-64", "C", "+32", "+64", "+64"];
        ScribbleStripTextColor[] scribbleStripTextColors = [
            ScribbleStripTextColor.Dark, ScribbleStripTextColor.Dark, ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripTextColor.Light, ScribbleStripTextColor.Light,
            ScribbleStripTextColor.Light, ScribbleStripTextColor.Light
        ];
        ScribbleStripBackgroundColor[] scribbleStripBackgroundColors = [
            ScribbleStripBackgroundColor.Yellow, ScribbleStripBackgroundColor.Yellow, ScribbleStripBackgroundColor.Red, ScribbleStripBackgroundColor.Red, ScribbleStripBackgroundColor.Blue,
            ScribbleStripBackgroundColor.Blue, ScribbleStripBackgroundColor.Green, ScribbleStripBackgroundColor.Green
        ];
        int[]    vuMeterLightPositions = [7, 4, 3, 1, 7, 5, 4, 5];
        bool[]   recordButtonLightIsOn = [true, false, false, true, true, false, false, false];
        bool[]   soloButtonLightIsOn   = [false, true, false, true, false, false, false, true];
        bool[]   muteButtonLightIsOn   = [false, false, true, false, false, true, true, false];
        bool[]   selectButtonLightIsOn = [true, true, false, true, true, false, false, true];
        double[] faderPositions        = [0.5, 0.72, 0.72, 0.5, 0.375, 0.25, 0.375, 0.5];

        for (int trackId = 0; trackId < device.TrackCount; trackId++) {
            IRotaryEncoder rotaryEncoder = device.GetRotaryEncoder(trackId);
            if (rotaryEncoder is IHuiRotaryEncoder huiRotaryEncoder) {
                huiRotaryEncoder.Fill.Connect(RotaryEncoderFillMode.FillToCenterAsymmetric);
                rotaryEncoder.LightPosition.Connect(huiRotaryEncoderLightPositions[trackId]);
            } else {
                rotaryEncoder.LightPosition.Connect(ctrlRotaryEncoderLightPositions[trackId]);
            }

            IScribbleStrip trackScribbleStrip = device.GetScribbleStrip(trackId);
            switch (trackScribbleStrip) {
                case ICtrlScribbleStrip scribbleStrip:
                    scribbleStrip.TopText.Connect("PAN");
                    scribbleStrip.BottomText.Connect(scribbleStripBottomText[trackId]);
                    scribbleStrip.TopTextColor.Connect(scribbleStripTextColors[trackId]);
                    scribbleStrip.BottomTextColor.Connect(scribbleStripTextColors[trackId]);
                    scribbleStrip.BackgroundColor.Connect(scribbleStripBackgroundColors[trackId]);
                    break;
                case IHuiScribbleStrip scribbleStrip:
                    scribbleStrip.Text.Connect(scribbleStripBottomText[trackId]);
                    break;
            }

            device.GetVuMeter(trackId).LightPosition.Connect(vuMeterLightPositions[trackId]);

            device.GetRecordButton(trackId).IlluminationState.Connect(recordButtonLightIsOn[trackId] ? IlluminatedButtonState.On : IlluminatedButtonState.Off);
            device.GetSoloButton(trackId).IlluminationState.Connect(soloButtonLightIsOn[trackId] ? IlluminatedButtonState.On : IlluminatedButtonState.Off);
            device.GetMuteButton(trackId).IlluminationState.Connect(muteButtonLightIsOn[trackId] ? IlluminatedButtonState.On : IlluminatedButtonState.Off);
            device.GetSelectButton(trackId).IlluminationState.Connect(selectButtonLightIsOn[trackId] ? IlluminatedButtonState.On : IlluminatedButtonState.Off);
            device.GetFader(trackId).DesiredPosition.Connect(faderPositions[trackId]);
        }
    }

}