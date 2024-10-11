using BehringerXTouchExtender.Enums;
using BehringerXTouchExtender.TrackControls;
using BehringerXTouchExtender.TrackControls.Ctrl;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;

namespace BehringerXTouchExtender;

internal abstract class CtrlBehringerXTouchExtender<TRotaryEncoder>: BehringerXTouchExtender<TRotaryEncoder> where TRotaryEncoder: IRotaryEncoder {

    protected CtrlBehringerXTouchExtender() {
        for (int trackId = 0; trackId < TRACK_COUNT; trackId++) {
            RecordButtons[trackId]  = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Record);
            SoloButtons[trackId]    = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Solo);
            MuteButtons[trackId]    = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Mute);
            SelectButtons[trackId]  = new IlluminatedButton(MidiClient, trackId, IlluminatedButtonType.Select);
            VuMeters[trackId]       = new VuMeter(MidiClient, trackId);
            Faders[trackId]         = new Fader(MidiClient, trackId);
            ScribbleStrips[trackId] = new ScribbleStrip(MidiClient, trackId);
            //rotary encoders are constructed in concrete subclasses
        }
    }

    protected override void OnEventReceivedFromDevice(object sender, MidiEventReceivedEventArgs e) {
        switch (e.Event.EventType) {
            case MidiEventType.NoteOn:
                OnEventReceivedFromDevice((NoteOnEvent) e.Event);
                break;
            case MidiEventType.ControlChange:
                OnEventReceivedFromDevice((ControlChangeEvent) e.Event);
                break;
            default:
                break;
        }
    }

    // ReSharper disable once SuggestBaseTypeForParameter
    private void OnEventReceivedFromDevice(NoteOnEvent incomingEvent) {
        int            trackId;
        SevenBitNumber noteId    = incomingEvent.NoteNumber;
        bool           isPressed = incomingEvent.Velocity == SevenBitNumber.MaxValue;

        switch (noteId) {
            case < 0x00 + TRACK_COUNT:
                trackId = noteId - 0x00;
                ((RotaryEncoder) (object) GetRotaryEncoder(trackId)).OnButtonEvent(isPressed);
                break;
            case < 0x08 + TRACK_COUNT:
                trackId = noteId - 0x08;
                RecordButtons[trackId].OnButtonEvent(isPressed);
                break;
            case < 0x10 + TRACK_COUNT:
                trackId = noteId - 0x10;
                SoloButtons[trackId].OnButtonEvent(isPressed);
                break;
            case < 0x18 + TRACK_COUNT:
                trackId = noteId - 0x18;
                MuteButtons[trackId].OnButtonEvent(isPressed);
                break;
            case < 0x20 + TRACK_COUNT:
                trackId = noteId - 0x20;
                SelectButtons[trackId].OnButtonEvent(isPressed);
                break;
            case >= 0x6E and < 0x6E + TRACK_COUNT:
                trackId = noteId - 0x6E;
                Faders[trackId].OnButtonEvent(isPressed);
                break;
            default:
                break;
        }
    }

    private void OnEventReceivedFromDevice(ControlChangeEvent incomingEvent) {
        int controlNumber = incomingEvent.ControlNumber;
        int trackId;

        switch (controlNumber) {
            case >= 80 and < 80 + TRACK_COUNT:
                trackId = controlNumber - 80;
                OnRotaryEncoderRotationEventReceivedFromDevice(trackId, incomingEvent.ControlValue);
                break;
            case >= 70 and < 70 + TRACK_COUNT:
                trackId = controlNumber - 70;
                double newValue = (double) incomingEvent.ControlValue / SevenBitNumber.MaxValue;
                Faders[trackId].OnFaderMoved(newValue);
                break;
            default:
                break;
        }

    }

}