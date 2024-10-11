using BehringerXTouchExtender;
using BehringerXTouchExtender.TrackControls;
using CSCore.CoreAudioAPI;
using KoKo.Property;
using Timer = System.Timers.Timer;

namespace Sample;

public static class AudioMeters {

    public static void Main() {
        using IBehringerXTouchExtender<IRelativeRotaryEncoder> device = BehringerXTouchExtenderFactory.CreateWithHuiMode();

        Console.WriteLine("Connecting to Behringer X-Touch Extender...");
        device.Open();
        Console.WriteLine("Connected.");

        using MMDeviceEnumerator    mmDeviceEnumerator    = new();
        using MMDevice              mmDevice              = mmDeviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
        using AudioMeterInformation audioMeterInformation = AudioMeterInformation.FromDevice(mmDevice);
        int                         audioChannelCount     = audioMeterInformation.MeteringChannelCount;

        int     vuMeterLightCount   = device.GetVuMeter(1).LightCount;
        int[][] ledPositionsBlitted = new int[2][];
        ledPositionsBlitted[0] = new int[vuMeterLightCount];
        ledPositionsBlitted[1] = new int[vuMeterLightCount];
        int ledPositionsBlitOffset = 0;
        ManuallyRecalculatedProperty<int[]> audioPeaks = new(() => {
            float[] peaks        = audioMeterInformation.GetChannelsPeakValues(audioChannelCount);
            int[]   ledPositions = ledPositionsBlitted[ledPositionsBlitOffset ^= 1];
            for (int i = 0; i < peaks.Length; i++) {
                ledPositions[i] = (int) Math.Round(peaks[i] * vuMeterLightCount);
            }

            return ledPositions;
        });

        for (int i = 0; i < device.TrackCount; i++) {
            int trackId = i; //create closure so it doesn't change between when a callback is defined and executed

            /*IRelativeRotaryEncoder rotaryEncoder              = device.GetRotaryEncoder(trackId);
            StoredProperty<int>    rotaryEncoderLightPosition = new(trackId);
            rotaryEncoder.LightPosition.Connect(rotaryEncoderLightPosition);
            rotaryEncoder.Rotated += (_, rotationArgs) =>
                rotaryEncoderLightPosition.Value = Math.Max(Math.Min(rotaryEncoderLightPosition.Value + (rotationArgs.IsClockwise ? 1 : -1), rotaryEncoder.LightCount - 1), 0);*/

            int audioChannel = trackId * audioChannelCount / device.TrackCount; //integer truncation is desired here
            device.GetVuMeter(trackId).LightPosition.Connect(DerivedProperty<int>.Create(audioPeaks, peaks => peaks[audioChannel]));

            /*IIlluminatedButton                     muteButton      = device.GetMuteButton(trackId);
            StoredProperty<IlluminatedButtonState> muteButtonState = new();
            muteButton.IlluminationState.Connect(muteButtonState);
            muteButton.IsPressed.PropertyChanged += (_, eventArgs) => {
                if (eventArgs.NewValue) {
                    muteButtonState.Value = muteButtonState.Value switch {
                        IlluminatedButtonState.Off => IlluminatedButtonState.On,
                        IlluminatedButtonState.On  => IlluminatedButtonState.Blinking,
                        _                          => IlluminatedButtonState.Off
                    };
                }
            };

            IIlluminatedButton recordButton = device.GetRecordButton(trackId);
            recordButton.IlluminationState.Connect(IlluminatedButtonState.Blinking);

            IIlluminatedButton                     soloButton      = device.GetSoloButton(trackId);
            StoredProperty<IlluminatedButtonState> soloButtonState = new();
            soloButton.IlluminationState.Connect(soloButtonState);
            soloButton.IsPressed.PropertyChanged += (_, eventArgs) => {
                if (eventArgs.NewValue) {
                    soloButtonState.Value = soloButtonState.Value switch {
                        IlluminatedButtonState.Off => IlluminatedButtonState.On,
                        IlluminatedButtonState.On  => IlluminatedButtonState.Blinking,
                        _                          => IlluminatedButtonState.Off
                    };
                }
            };

            IIlluminatedButton selectButton = device.GetSelectButton(trackId);
            selectButton.IlluminationState.Connect(DerivedProperty<IlluminatedButtonState>.Create(selectButton.IsPressed,
                isPressed => isPressed ? IlluminatedButtonState.On : IlluminatedButtonState.Off));

            IFader fader = device.GetFader(trackId);
            fader.IsPressed.PropertyChanged += (_, eventArgs) => {
                if (eventArgs.NewValue) {
                    Console.WriteLine($"User is touching Fader {trackId + 1}");
                }
            };
            fader.DesiredPosition.Connect(trackId / (device.TrackCount - 1.0));
            fader.ActualPosition.PropertyChanged += (_, eventArgs) => Console.WriteLine($"Fader {trackId + 1} moved to position {eventArgs.NewValue:P0}");

            device.GetScribbleStrip(trackId).TopText.Connect($"Track {trackId + 1}");
            device.GetScribbleStrip(trackId).BottomText.Connect(new string('.', trackId));
            device.GetScribbleStrip(trackId).TopTextColor.Connect(ScribbleStripTextColor.Dark);
            device.GetScribbleStrip(trackId).BottomTextColor.Connect(ScribbleStripTextColor.Light);
            device.GetScribbleStrip(trackId).BackgroundColor
                .Connect(trackId == 0 ? ScribbleStripBackgroundColor.White : (ScribbleStripBackgroundColor) trackId); //avoid black background because it makes text illegible*/
        }

        const int   audioPeakFps   = 30;
        using Timer audioPeakTimer = new(TimeSpan.FromSeconds(1.0 / audioPeakFps).TotalMilliseconds);
        audioPeakTimer.Elapsed += (_, _) => audioPeaks.Recalculate();
        audioPeakTimer.Start();

        CancellationTokenSource cts = new();
        Console.CancelKeyPress += (_, args) => {
            cts.Cancel();
            args.Cancel = true;
        };

        Console.WriteLine("Press Ctrl+C to exit.");
        cts.Token.WaitHandle.WaitOne();
    }

}