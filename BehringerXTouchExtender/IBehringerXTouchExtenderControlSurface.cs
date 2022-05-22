namespace BehringerXTouchExtender;

public interface IBehringerXTouchExtenderControlSurface<out TRotaryEncoder>: IDisposable where TRotaryEncoder: IRotaryEncoder {

    int TrackCount { get; }
    bool IsOpen { get; }

    /// <exception cref="LifecycleException">if <c>Open()</c> has already been called on this instance</exception>
    /// <exception cref="DeviceNotFoundException">if a connected MIDI input or output device named <c>X-Touch-Ext</c> cannot be found.</exception>
    void Open();

    IIlluminatedButton GetRecordButton(int trackId);
    IIlluminatedButton GetMuteButton(int   trackId);
    IIlluminatedButton GetSoloButton(int   trackId);
    IIlluminatedButton GetSelectButton(int trackId);
    TRotaryEncoder GetRotaryEncoder(int    trackId);
    IVuMeter GetVuMeter(int                trackId);
    IFader GetFader(int                    trackId);
    IScribbleStrip GetScribbleStrip(int    trackId);

}