using System.ComponentModel;
using System.Text;
using KoKo.Events;
using KoKo.Property;
using Melanchall.DryWetMidi.Common;
using Melanchall.DryWetMidi.Core;

namespace BehringerXTouchExtender;

public interface IBehringerXTouchExtenderControlSurface<out TRotaryEncoder>: IDisposable where TRotaryEncoder: IRotaryEncoder {

    int TrackCount { get; }
    bool IsOpen { get; }

    /// <exception cref="DeviceNotFoundException"></exception>
    void Open();

    // delegate void MidiEventFromDeviceHandler(object sender, IMidiEventFromDevice midiEvent);
    // event MidiEventFromDeviceHandler MidiEventFromDevice;

    IIlluminatedButton GetRecordButton(int trackId);
    IIlluminatedButton GetMuteButton(int   trackId);
    IIlluminatedButton GetSoloButton(int   trackId);
    IIlluminatedButton GetSelectButton(int trackId);
    TRotaryEncoder GetRotaryEncoder(int    trackId);
    IVuMeter GetVuMeter(int                trackId);
    IFader GetFader(int                    trackId);
    IScribbleStrip GetScribbleStrip(int    trackId);

    // void SetButtonLight(int trackId, IlluminatedButtonType buttonType, IlluminatedButtonState illuminatedButtonState);
    //
    // void RotateKnob(int    trackId, int    distanceFromMinimumValue);
    // void MoveSlider(int    trackId, double distanceFromMinimumValue);
    // void SetMeterLevel(int trackId, int    distanceFromMinimumValue);
    //
    // void SetText(int                          trackId,
    //              string                       topText         = "",
    //              string                       bottomText      = "",
    //              ScribbleStripTextColor       topTextColor    = ScribbleStripTextColor.Light,
    //              ScribbleStripTextColor       bottomTextColor = ScribbleStripTextColor.Light,
    //              ScribbleStripBackgroundColor backgroundColor = ScribbleStripBackgroundColor.Black);

}

public interface ITrackControl {

    int TrackId { get; }

}

public interface IScribbleStrip: ITrackControl {

    int TextColumnCount { get; }
    SettableProperty<string> TopText { get; }
    SettableProperty<string> BottomText { get; }
    SettableProperty<ScribbleStripTextColor> TopTextColor { get; }
    SettableProperty<ScribbleStripTextColor> BottomTextColor { get; }
    SettableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; }

}

internal class ScribbleStrip: IScribbleStrip {

    /// <summary>
    /// Device ID is not 0x42 as documented.
    /// Thanks https://community.musictribe.com/t5/Recording/X-Touch-Extender-Scribble-Strip-Midi-Sysex-Command/td-p/251306
    /// </summary>
    private const byte DeviceId = 0x15;

    private readonly MidiClient _midiClient;

    public int TextColumnCount => 7;
    public int TrackId { get; }
    public SettableProperty<string> TopText { get; } = new StoredProperty<string>(string.Empty);
    public SettableProperty<string> BottomText { get; } = new StoredProperty<string>(string.Empty);
    public SettableProperty<ScribbleStripTextColor> TopTextColor { get; } = new StoredProperty<ScribbleStripTextColor>();
    public SettableProperty<ScribbleStripTextColor> BottomTextColor { get; } = new StoredProperty<ScribbleStripTextColor>();
    public SettableProperty<ScribbleStripBackgroundColor> BackgroundColor { get; } = new StoredProperty<ScribbleStripBackgroundColor>();

    public ScribbleStrip(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        TopText.PropertyChanged         += OnPropertyChanged;
        BottomText.PropertyChanged      += OnPropertyChanged;
        TopTextColor.PropertyChanged    += OnPropertyChanged;
        BottomTextColor.PropertyChanged += OnPropertyChanged;
        BackgroundColor.PropertyChanged += OnPropertyChanged;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        byte[] payload = new byte[22];
        // 0xF0 is automatically prepended by NormalSysExEvent
        payload[0] = 0;
        payload[1] = 0x20;
        payload[2] = 0x32;
        payload[3] = DeviceId;
        payload[4] = 0x4C;
        payload[5] = (byte) (TrackId - 1);
        payload[6] = (byte) ((int) BackgroundColor.Value | ((int) TopTextColor.Value << 4) | ((int) BottomTextColor.Value << 5));

        byte[] topTextBytes    = Enumerable.Repeat((byte) ' ', TextColumnCount).ToArray();
        byte[] bottomTextBytes = Enumerable.Repeat((byte) ' ', TextColumnCount).ToArray();
        Encoding.ASCII.GetBytes(TopText.Value, 0, TextColumnCount, topTextBytes, 0);
        Encoding.ASCII.GetBytes(BottomText.Value, 0, TextColumnCount, bottomTextBytes, 0);

        for (int column = 0; column < TextColumnCount; column++) {
            payload[7 + column]                   = topTextBytes[column];
            payload[7 + column + TextColumnCount] = bottomTextBytes[column];
        }

        payload[21] = SysExEvent.EndOfEventByte;

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NormalSysExEvent(payload));
    }

}

public interface IPressableButton: ITrackControl {

    Property<bool> IsPressed { get; }

}

public interface IIlluminatedButton: IPressableButton {

    IlluminatedButtonType ButtonType { get; }
    SettableProperty<IlluminatedButtonState> IlluminationState { get; }

}

internal abstract class PressableButton: IPressableButton {

    public abstract int TrackId { get; }

    internal readonly StoredProperty<bool> _isPressed = new();
    public Property<bool> IsPressed { get; }

    protected PressableButton() {
        IsPressed = _isPressed;
    }

    internal void OnButtonEvent(bool isPressed) {
        _isPressed.Value = isPressed;
    }

}

internal class IlluminatedButtonImpl: PressableButton, IIlluminatedButton {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public IlluminatedButtonType ButtonType { get; }

    public SettableProperty<IlluminatedButtonState> IlluminationState { get; } = new StoredProperty<IlluminatedButtonState>();

    public IlluminatedButtonImpl(MidiClient midiClient, int trackId, IlluminatedButtonType buttonType) {
        _midiClient = midiClient;
        TrackId     = trackId;
        ButtonType  = buttonType;

        IlluminationState.PropertyChanged += OnIlluminationStateChanged;
    }

    private void OnIlluminationStateChanged(object sender, KoKoPropertyChangedEventArgs<IlluminatedButtonState> args) {
        SevenBitNumber noteId = (SevenBitNumber) (TrackId - 1 + ButtonType switch {
            IlluminatedButtonType.Record => 8,
            IlluminatedButtonType.Solo   => 16,
            IlluminatedButtonType.Mute   => 24,
            IlluminatedButtonType.Select => 32,
            _                            => throw new ArgumentOutOfRangeException(nameof(ButtonType), ButtonType, null)
        });

        SevenBitNumber velocity = (SevenBitNumber) (args.NewValue switch {
            IlluminatedButtonState.Off      => SevenBitNumber.MinValue,
            IlluminatedButtonState.On       => SevenBitNumber.MaxValue,
            IlluminatedButtonState.Blinking => 64,
            _                               => SevenBitNumber.MinValue
        });

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new NoteOnEvent(noteId, velocity));
    }

}

public interface IRotaryEncoder: IPressableButton {

    int LightCount { get; }

    SettableProperty<int> LightPosition { get; }

}

public interface IRotaryEncoder<T>: IRotaryEncoder {

    SettableProperty<T> RotationPosition { get; }

}

public interface IRelativeRotaryEncoder: IRotaryEncoder<int> { }
public interface IAbsoluteRotaryEncoder: IRotaryEncoder<double> { }

internal class RotaryEncoder: PressableButton, IRelativeRotaryEncoder, IAbsoluteRotaryEncoder {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public int LightCount => 13;

    public SettableProperty<int> LightPosition { get; } = new StoredProperty<int>();

    public RotaryEncoder(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += OnLightPositionChanged;
    }

    private void OnLightPositionChanged(object sender, KoKoPropertyChangedEventArgs<int> args) {
        int newValue = Math.Max(Math.Min(args.NewValue, LightCount - 1), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (80 + TrackId - 1);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / LightCount;
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

    SettableProperty<int> IRotaryEncoder<int>.RotationPosition { get; } = new StoredProperty<int>();

    SettableProperty<double> IRotaryEncoder<double>.RotationPosition { get; } = new StoredProperty<double>();

}

// internal class RotaryEncoder<T>: RotaryEncoder, IRelativeRotaryEncoder, IAbsoluteRotaryEncoder {
//
//     public RotaryEncoder(MidiClient midiClient, int trackId): base(midiClient, trackId) { }
//
//     // public SettableProperty<T> RotationPosition { get; } = new StoredProperty<T>();
//
//     // private T offset = default;
//
// }

// public class OffsetStoredProperty<T>: StoredProperty<T> {
//
//     public override T Value {
//         get { }
//         set { }
//     }
//
// }

public interface IVuMeter {

    SettableProperty<int> LightPosition { get; }
    int LightCount { get; }

}

internal class VuMeter: IVuMeter {

    private readonly MidiClient _midiClient;

    public int TrackId { get; set; }
    public int LightCount => 8;

    public SettableProperty<int> LightPosition { get; } = new StoredProperty<int>();

    public VuMeter(MidiClient midiClient, int trackId) {
        _midiClient = midiClient;
        TrackId     = trackId;

        LightPosition.PropertyChanged += OnLightPositionChanged;
    }

    private void OnLightPositionChanged(object sender, KoKoPropertyChangedEventArgs<int> args) {
        int newValue = Math.Max(Math.Min(args.NewValue, LightCount), 0);

        SevenBitNumber controlId      = (SevenBitNumber) (90 + TrackId - 1);
        double         incrementWidth = (double) SevenBitNumber.MaxValue / (LightCount + 1);
        SevenBitNumber controlValue   = (SevenBitNumber) Math.Round((0.5 + newValue) * incrementWidth);

        _midiClient.AssertOpen();
        _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
    }

}

public interface IFader: IPressableButton {

    SettableProperty<double> Position { get; }

}

internal class Fader: PressableButton, IFader {

    private readonly MidiClient _midiClient;

    public override int TrackId { get; }
    public SettableProperty<double> Position { get; } = new StoredProperty<double>();

    public Fader(MidiClient midiClient, int trackId) {
        TrackId     = trackId;
        _midiClient = midiClient;

        Position.PropertyChanged += OnPositionChanged;
    }

    private void OnPositionChanged(object sender, KoKoPropertyChangedEventArgs<double> args) {
        double newValue = Math.Max(Math.Min(args.NewValue, 1), 0);

        if (!IsPressed.Value) { //don't slide the fader out from under the user's finger
            SevenBitNumber controlId    = (SevenBitNumber) (70 + TrackId - 1);
            SevenBitNumber controlValue = (SevenBitNumber) Math.Round(newValue * SevenBitNumber.MaxValue);

            _midiClient.AssertOpen();
            _midiClient.ToDevice?.SendEvent(new ControlChangeEvent(controlId, controlValue));
        }
    }

}