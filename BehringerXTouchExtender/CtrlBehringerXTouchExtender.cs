using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal abstract class CtrlBehringerXTouchExtender<TRotaryEncoder>: BehringerXTouchExtender<TRotaryEncoder, ICtrlScribbleStrip> where TRotaryEncoder: IRotaryEncoder {

    internal override MidiClient MidiClient { get; } = new();

    private readonly ICtrlScribbleStripInternal[] _scribbleStrips = new ICtrlScribbleStripInternal[TRACK_COUNT];

    protected CtrlBehringerXTouchExtender() {

        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            RecordButtons[trackId]   = new CtrlIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Record);
            SoloButtons[trackId]     = new CtrlIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Solo);
            MuteButtons[trackId]     = new CtrlIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Mute);
            SelectButtons[trackId]   = new CtrlIlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Select);
            VuMeters[trackId]        = new CtrlVuMeter(MidiClient, trackId);
            Faders[trackId]          = new CtrlFader(MidiClient, trackId);
            _scribbleStrips[trackId] = new CtrlScribbleStrip(MidiClient, trackId);
            //rotary encoders are constructed in concrete subclasses
        }
    }

    public override void Open() {
        base.Open();
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            _scribbleStrips[trackId].WriteStateToDevice();
        }
    }

    public override ICtrlScribbleStrip GetScribbleStrip(int trackId) {
        ValidateTrackId(trackId);
        return _scribbleStrips[trackId];
    }

    protected override void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs args) {
        switch (args.Event) {
            case NoteOnEvent e:
                OnEventReceivedFromDevice(e);
                break;
            case ControlChangeEvent e:
                OnEventReceivedFromDevice(e);
                break;
            default:
                break;
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void OnEventReceivedFromDevice(NoteOnEvent incomingEvent) {
        SevenBitNumber noteId    = incomingEvent.NoteNumber;
        bool           isPressed = incomingEvent.Velocity == SevenBitNumber.MaxValue;

        switch (noteId) {
            case < 0 + TRACK_COUNT:
                ((IPressableButtonInternal) GetRotaryEncoder(noteId - 0)).OnButtonEvent(isPressed);
                break;
            case < 8 + TRACK_COUNT:
                RecordButtons[noteId - 8].OnButtonEvent(isPressed);
                break;
            case < 16 + TRACK_COUNT:
                SoloButtons[noteId - 16].OnButtonEvent(isPressed);
                break;
            case < 24 + TRACK_COUNT:
                MuteButtons[noteId - 24].OnButtonEvent(isPressed);
                break;
            case < 32 + TRACK_COUNT:
                SelectButtons[noteId - 32].OnButtonEvent(isPressed);
                break;
            case >= 110 and < 110 + TRACK_COUNT:
                Faders[noteId - 110].OnButtonEvent(isPressed);
                break;
            default:
                break;
        }
    }

    private void OnEventReceivedFromDevice(ControlChangeEvent incomingEvent) {
        int controlNumber = incomingEvent.ControlNumber;

        switch (controlNumber) {
            case >= 80 and < 80 + TRACK_COUNT:
                OnRotaryEncoderRotationEventReceivedFromDevice(controlNumber - 80, incomingEvent.ControlValue);
                break;
            case >= 70 and < 70 + TRACK_COUNT:
                Faders[controlNumber - 70].OnFaderMoved((double) incomingEvent.ControlValue / SevenBitNumber.MaxValue);
                break;
            default:
                break;
        }

    }

    protected abstract void OnRotaryEncoderRotationEventReceivedFromDevice(int trackId, SevenBitNumber incomingEventControlValue);

}