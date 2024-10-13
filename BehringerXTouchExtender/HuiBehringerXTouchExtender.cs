using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Hui;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal class HuiBehringerXTouchExtender: BehringerXTouchExtender<IHuiRotaryEncoder, IHuiScribbleStrip>, IHuiBehringerXTouchExtender {

    private static readonly NormalSysExEvent HealthCheckEvent = new([0x00, 0x00, 0x66, 0x14, 0x00, SysExEvent.EndOfEventByte]);

    private readonly IHuiRotaryEncoderInternal[]       _rotaryEncoders = new IHuiRotaryEncoderInternal[TRACK_COUNT];
    private readonly IHuiScribbleStripInternal[]       _scribbleStrips = new IHuiScribbleStripInternal[TRACK_COUNT];
    private readonly Timer                             _healthCheckTimer;
    private readonly Timer                             _blinkingButtonTimer;
    private readonly IList<IIlluminatedButtonInternal> _illuminatedButtons;

    private volatile uint _mostRecentEventTrackId;

    private HuiMidiClient HuiMidiClient { get; } = new();
    internal override MidiClient MidiClient => HuiMidiClient;

    public HuiBehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            RecordButtons[trackId]   = new HuiIlluminatedButton(HuiMidiClient, trackId, IlluminatedButtonType.Record);
            SoloButtons[trackId]     = new HuiIlluminatedButton(HuiMidiClient, trackId, IlluminatedButtonType.Solo);
            MuteButtons[trackId]     = new HuiIlluminatedButton(HuiMidiClient, trackId, IlluminatedButtonType.Mute);
            SelectButtons[trackId]   = new HuiIlluminatedButton(HuiMidiClient, trackId, IlluminatedButtonType.Select);
            Faders[trackId]          = new HuiFader(HuiMidiClient, trackId);
            _scribbleStrips[trackId] = new HuiScribbleStrip(HuiMidiClient, trackId);
            VuMeters[trackId]        = new HuiVuMeter(HuiMidiClient, trackId);
            _rotaryEncoders[trackId] = new HuiRotaryEncoder(HuiMidiClient, trackId);
        }

        _healthCheckTimer    = new Timer(SendHealthCheck);
        _blinkingButtonTimer = new Timer(BlinkButtons);
        _illuminatedButtons  = RecordButtons.Concat(SoloButtons).Concat(MuteButtons).Concat(SelectButtons).OrderBy(button => button.TrackId).ToList();
    }

    public override void Open() {
        base.Open();

        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _scribbleStrips[trackId].WriteStateToDevice();
            _rotaryEncoders[trackId].WriteStateToDevice();
        }

        _healthCheckTimer.Change(0, 3000);
        _blinkingButtonTimer.Change(0, 500);
    }

    private void SendHealthCheck(object? state = null) {
        MidiClient.ToDevice?.SendEvent(HealthCheckEvent);
    }

    public override IHuiRotaryEncoder GetRotaryEncoder(int trackId) {
        ValidateTrackId(trackId);
        return _rotaryEncoders[trackId];
    }

    public override IHuiScribbleStrip GetScribbleStrip(int trackId) {
        ValidateTrackId(trackId);
        return _scribbleStrips[trackId];
    }

    /// <exception cref="ArgumentOutOfRangeException">if the control change refers to an unknown control</exception>
    protected override void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs args) {
        if (args.Event is ControlChangeEvent { ControlNumber: var control, ControlValue: var value } evt) {
            switch (control) {
                case 15:
                    // stateful track select for next event
                    _mostRecentEventTrackId = value;
                    break;
                case >= 64 and < 72:
                    bool isClockwise = value >= 64;
                    _rotaryEncoders[control - 64].OnRotated(isClockwise, value - (isClockwise ? 64u : 0));
                    break;
                case 47:
                    bool isPressed = value >= 64;
                    IPressableButtonInternal[]? buttons = (value - (isPressed ? 64 : 0)) switch {
                        0 => Faders,
                        1 => SelectButtons,
                        2 => MuteButtons,
                        3 => SoloButtons,
                        4 => _rotaryEncoders,
                        7 => RecordButtons,
                        _ => null
                    };
                    buttons?[_mostRecentEventTrackId].OnButtonEvent(isPressed);
                    break;
                case < 8:
                    Faders[control].OnFaderMoved((double) value / SevenBitNumber.MaxValue);
                    break;
                case >= 32 and < 40:
                    // fader velocity, ignore
                    break;
                default:
                    Console.WriteLine($"Received unknown control change event with channel {evt.Channel}, control {control}, and value {value}"); //TODO
                    break;
            }
        } else {
            Console.WriteLine($"Received unknown MIDI event from device: {args.Event.EventType}");
        }
    }

    private void BlinkButtons(object? state = null) {
        HuiMidiClient.BlinkingButtonsAreIlluminated = !HuiMidiClient.BlinkingButtonsAreIlluminated;
        foreach (IIlluminatedButtonInternal button in _illuminatedButtons.Where(button => button.IlluminationState.Value == IlluminatedButtonState.Blinking)) {
            button.WriteStateToDevice(this);
        }
    }

    protected override void Dispose(bool disposing) {
        if (disposing) {
            _healthCheckTimer.Dispose();
            _blinkingButtonTimer.Dispose();
        }
        base.Dispose(disposing);
    }

}