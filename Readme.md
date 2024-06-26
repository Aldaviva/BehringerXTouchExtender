🎚️ BehringerXTouchExtender
===

[![Nuget](https://img.shields.io/nuget/v/BehringerXTouchExtender?logo=nuget)](https://www.nuget.org/packages/BehringerXTouchExtender/) [![GitHub Workflow Status](https://img.shields.io/github/actions/workflow/status/Aldaviva/BehringerXTouchExtender/dotnetpackage.yml?branch=master&logo=github)](https://github.com/Aldaviva/BehringerXTouchExtender/actions/workflows/dotnetpackage.yml) [![Testspace](https://img.shields.io/testspace/tests/Aldaviva/Aldaviva:BehringerXTouchExtender/master?passed_label=passing&failed_label=failing&logo=data%3Aimage%2Fsvg%2Bxml%3Bbase64%2CPHN2ZyB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciIHZpZXdCb3g9IjAgMCA4NTkgODYxIj48cGF0aCBkPSJtNTk4IDUxMy05NCA5NCAyOCAyNyA5NC05NC0yOC0yN3pNMzA2IDIyNmwtOTQgOTQgMjggMjggOTQtOTQtMjgtMjh6bS00NiAyODctMjcgMjcgOTQgOTQgMjctMjctOTQtOTR6bTI5My0yODctMjcgMjggOTQgOTQgMjctMjgtOTQtOTR6TTQzMiA4NjFjNDEuMzMgMCA3Ni44My0xNC42NyAxMDYuNS00NFM1ODMgNzUyIDU4MyA3MTBjMC00MS4zMy0xNC44My03Ni44My00NC41LTEwNi41UzQ3My4zMyA1NTkgNDMyIDU1OWMtNDIgMC03Ny42NyAxNC44My0xMDcgNDQuNXMtNDQgNjUuMTctNDQgMTA2LjVjMCA0MiAxNC42NyA3Ny42NyA0NCAxMDdzNjUgNDQgMTA3IDQ0em0wLTU1OWM0MS4zMyAwIDc2LjgzLTE0LjgzIDEwNi41LTQ0LjVTNTgzIDE5Mi4zMyA1ODMgMTUxYzAtNDItMTQuODMtNzcuNjctNDQuNS0xMDdTNDczLjMzIDAgNDMyIDBjLTQyIDAtNzcuNjcgMTQuNjctMTA3IDQ0cy00NCA2NS00NCAxMDdjMCA0MS4zMyAxNC42NyA3Ni44MyA0NCAxMDYuNVMzOTAgMzAyIDQzMiAzMDJ6bTI3NiAyODJjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjY3IDE0LjY3LTEwNiA0NHMtNDQgNjUtNDQgMTA3YzAgNDEuMzMgMTQuNjcgNzYuODMgNDQgMTA2LjVTNjY2LjY3IDU4NCA3MDggNTg0em0tNTU3IDBjNDIgMCA3Ny42Ny0xNC44MyAxMDctNDQuNXM0NC02NS4xNyA0NC0xMDYuNWMwLTQyLTE0LjY3LTc3LjY3LTQ0LTEwN3MtNjUtNDQtMTA3LTQ0Yy00MS4zMyAwLTc2LjgzIDE0LjY3LTEwNi41IDQ0UzAgMzkxIDAgNDMzYzAgNDEuMzMgMTQuODMgNzYuODMgNDQuNSAxMDYuNVMxMDkuNjcgNTg0IDE1MSA1ODR6IiBmaWxsPSIjZmZmIi8%2BPC9zdmc%2B)](https://aldaviva.testspace.com/spaces/191394) [![Coveralls](https://img.shields.io/coveralls/github/Aldaviva/BehringerXTouchExtender?logo=coveralls)](https://coveralls.io/github/Aldaviva/BehringerXTouchExtender?branch=master)

*Send and receive events with a [Behringer X-Touch Extender](https://www.behringer.com/product.html?modelCode=0808-AAH) DAW MIDI control surface over USB.*

<!-- MarkdownTOC autolink="true" bracket="round" autoanchor="false" levels="1,2,3" bullets="1.,-,-" -->

1. [Quick Start](#quick-start)
1. [Prerequisites](#prerequisites)
    - [MIDI control mode](#midi-control-mode)
    - [Firmware upgrade](#firmware-upgrade)
1. [Installation](#installation)
1. [Connection](#connection)
1. [Track identifiers](#track-identifiers)
1. [Reactive data](#reactive-data)
1. [Usage](#usage)
    - [Rotary encoders](#rotary-encoders)
    - [Scribble strips](#scribble-strips)
    - [VU meters](#vu-meters)
    - [Record, solo, mute, and select buttons](#record-solo-mute-and-select-buttons)
    - [Faders](#faders)
    - [Disposal](#disposal)
1. [References](#references)
1. [Acknowledgements](#acknowledgements)

<!-- /MarkdownTOC -->

![Behringer X-Touch Extender](https://raw.githubusercontent.com/Aldaviva/BehringerXTouchExtender/master/.github/images/xtouch-extender.webp)

## Quick Start

```bat
dotnet new console
dotnet add package BehringerXTouchExtender
```

```cs
// Program.cs
using BehringerXTouchExtender;

using var device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
device.Open();

var scribbleStrip = device.GetScribbleStrip(0);
scribbleStrip.TopText.Connect("Hello");
scribbleStrip.BottomText.Connect("World");
scribbleStrip.TopTextColor.Connect(ScribbleStripTextColor.Light);
scribbleStrip.BottomTextColor.Connect(ScribbleStripTextColor.Dark);
scribbleStrip.BackgroundColor.Connect(ScribbleStripBackgroundColor.Magenta);
```

```bat
dotnet run
```

![Hello World](https://raw.githubusercontent.com/Aldaviva/BehringerXTouchExtender/master/.github/images/helloworld.jpg)

## Prerequisites

- A [Behringer X-Touch Extender](https://www.behringer.com/product.html?modelCode=0808-AAH)
- A Microsoft .NET runtime that supports [.NET Standard 2.0 or later](https://docs.microsoft.com/en-us/dotnet/standard/net-standard?tabs=net-standard-2-0#net-standard-versions):
    - [.NET 5.0 or later](https://dotnet.microsoft.com/en-us/download/dotnet)
    - [.NET Core 2.0 or later](https://dotnet.microsoft.com/en-us/download/dotnet)
    - [.NET Framework 4.6.1 or later](https://dotnet.microsoft.com/en-us/download/dotnet-framework)

### MIDI control mode

You must manually set the X-Touch Extender to use absolute or relative MIDI control mode. The other two modes, HUI and MC, are not supported by this library.

1. Turn on the X-Touch Extender while holding the leftmost Select button
1. Turn the leftmost rotary encoder knob until the LCD shows `Ctrl` (**absolute** MIDI control mode) or `CtrlRel` (**relative** MIDI control mode)
1. Press the leftmost Select button
1. Remember which mode you chose when you [connect](#connection) to the device

Once configured, the X-Touch Extender will persist this control mode setting until you change it again, even after being turned off or unplugged. You don't have to set it every time you turn the device on.

### Firmware upgrade

If your computer has an AMD Zen2 (Ryzen 3000 series) or later CPU, then you must install X-Touch Extender firmware 1.21 or later to fix the [broken USB connection](https://community.amd.com/t5/drivers-software/behringer-x-touch-usb-driver-issues/m-p/199495).

1. [Download the firmware](https://mediadl.musictribe.com/download/software/behringer/X-TOUCH/X-TOUCH-EXT_Firmware_V1.22.zip)
1. Extract the `.syx` file from the `.zip` file
1. Turn on the X-Touch Extender while holding the rightmost Record button
1. Download and run [MIDI-OX](http://www.midiox.com/moxdown.htm) on an unaffected (e.g. Intel) computer connected to the X-Touch Extender over USB
1. Select Options › MIDI Devices and highlight the `X-Touch-Ext` entries
1. Select Actions › Send › SysEx File and choose the `.syx` file from step 2
1. Wait for the upgrade to finish
1. Reboot the X-Touch Extender using its power switch

## Installation

You can install this library into your project from [NuGet Gallery](https://www.nuget.org/packages/BehringerXTouchExtender):
- `dotnet add package BehringerXTouchExtender`
- `Install-Package BehringerXTouchExtender`
- Go to Project › Manage NuGet Packages in Visual Studio and search for `BehringerXTouchExtender`

## Connection

1. Use `BehringerXTouchExtenderFactory` to create a device instance you can use. The choice of which factory method to call depends on the device's [configured MIDI control mode](#midi-control-mode).
    - If you set your X-Touch Extender to `Ctrl` mode:
        ```cs
        using BehringerXTouchExtender;

        using var device = BehringerXTouchExtenderFactory.CreateWithAbsoluteMode();
        ```
    - If you set your X-Touch Extender to `CtrlRel` mode:
        ```cs
        using BehringerXTouchExtender;

        using var device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
        ```
1. Once the X-Touch Extender is powered on and connected over USB, open the connection.
    ```cs
    device.Open();
    ```

## Track identifiers

The X-Touch Extender is divided into eight vertical banks of controls, called tracks or channels. In this library, they are numbered from 0 on the left to 7 on the right (**0-indexed**).

On the physical device, the painted legends are 1-indexed, but 0-indexing is easier and less confusing to use in software.

Any time this library takes or provides a track ID, it's 0-indexed.

In most of the code examples below, the `trackId` argument used is `0`.

## Reactive data

This library handles data using **[KoKo](https://github.com/Aldaviva/KoKo/) `Property` objects**. These encapsulate values that can change, including automatically in response to other Properties, and they automatically fire change events.

If you have your own Property that you want to drive a control on the X-Touch Extender, you can connect it to this library.

```cs
using KoKo;

var scribbleStrip = device.GetScribbleStrip(0);
var greeting = new StoredProperty<string>("Hello");
scribbleStrip.TopText.Connect(greeting);
// The device will show "Hello"

greeting.Value = "Hola";
// The device will automatically update to show "Hola"
```

KoKo Properties are used by all values on all controls in this library, not just scribble strips. This is useful if the desired value actually depends on other values, because they can all be automatically calculated without any manual change event firing or listening.

```cs
Property<CultureInfo> cultureInfo;
var greeting = DerivedProperty.Create<string>(cultureInfo, culture => culture.Name switch {
    "es" => "Hola",
    _ => "Hello"
});
scribbleStrip.TopText.Connect(greeting);
// The device will automatically update the greeting shown whenever the cultureInfo property changes
```

These Properties are used for reading changing data as well as writing it. You can read the immediate value, as well as registering for events whenever the Property changes in the future.

```cs
var button = device.GetRecordButton(0);

if(button.IsPressed.Value){
    Console.WriteLine("Button is pressed");
}

button.IsPressed.PropertyChanged += (sender, args) => {
    if(args.NewValue){
        Console.WriteLine("Button was pressed");
    }
};
```

If you don't want to create reactive properties, you can connect the device's properties to constant values whenever you want them to change.

```cs
scribbleStrip.BottomText.Connect("World");
```

## Usage

### Rotary encoders

```cs
IRotaryEncoder rotaryEncoder = device.GetRotaryEncoder(0);
```

#### Illuminating lights

There are thirteen orange lights on each rotary encoder. *Exactly one* of them is illuminated at any given time. You can't turn them all off at the same time. Set the **`LightPosition`** property to change which light is illuminated.

They are numbered from `0` (most counter-clockwise) to `12` (most clockwise). Values outside this range are clipped to stay within `[0, 12]`.

The number of lights `13` is available programmatically in the `IRotaryEncoder.LightCount` property.

```cs
rotaryEncoder.LightPosition.Connect(0);
// Most counter-clockwise light illuminates
```

#### Detecting presses

You can click in on the knob and the rotary encoder's **`IsPressed`** Property will change to `true` (pressed) or `false` (not pressed).

```cs
Console.WriteLine($"Rotary encoder is currently {(rotaryEncoder.IsPressed.Value ? "pressed" : "not pressed")}");

rotaryEncoder.IsPressed.PropertyChanged += (sender, args) => 
        Console.WriteLine($"Rotary encoder was {(args.NewValue ? "pressed" : "released")}");
```

#### Detecting rotation

The available Properties and their values for a rotary encoder depend on whether you created your `IBehringerXTouchExtender` instance using either `BehringerXTouchExtenderFactory.CreateWithAbsoluteMode()` or `.CreateWithRelativeMode()`.

This control mode must match the [configured mode on the physical X-Touch Extender](#midi-control-mode) (`Ctrl` or `CtrlRel`, respectively).

##### Absolute control mode

Available when you [set the X-Touch Extender's control mode](#midi-control-mode) to `Ctrl` and called `BehringerXTouchExtenderFactory.CreateWithAbsoluteMode()`.

When the knob is rotated, the rotary encoder will update its **`RotationPosition`** Property with the new position of the knob, from `0.0` (most counter-clockwise) to `1.0` (most clockwise).

If you keep turning the knob while it is at one of its numeric limits, the `RotationPosition` value will stay in the range `[0.0, 1.0]`.

```cs
using IAbsoluteBehringerXTouchExtender device = BehringerXTouchExtenderFactory.CreateWithAbsoluteMode();
device.Open();
IAbsoluteRotaryEncoder rotaryEncoder = device.GetRotaryEncoder(0);

rotaryEncoder.RotationPosition.PropertyChanged += (sender, args) => 
        Console.WriteLine($"Knob was turned to {args.NewValue:P0}");
```

##### Relative control mode

Available when you [set the X-Touch Extender's control mode](#midi-control-mode) to `CtrlRel` and called `BehringerXTouchExtenderFactory.CreateWithRelativeMode()`.

When the knob is rotated, the rotary encoder will emit a **`Rotated`** event that tells you in which direction it was rotated. It does not tell you how far it was rotated, instead, it sends more `Rotated` events as you continue to turn the knob.

Each `Rotated` event corresponds to turning the knob until you feel the next physical detent. One complete 360° rotation is 24 detents, with 15° between each one.

```cs
using IRelativeBehringerXTouchExtender device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
device.Open();
IRelativeRotaryEncoder rotaryEncoder = device.GetRotaryEncoder(0);

rotaryEncoder.Rotated += (sender, args) =>
        Console.WriteLine($"Knob was turned 15° {(args.IsClockwise ? "clockwise" : "counter-clockwise")}");
```

### Scribble strips

These are the LCD screens at the top of each track column. They are so named because they are digital replacements for putting a strip of tape on your analog mixer and scribbling the channel name on them with a marker.

The eight X-Touch Extender scribble strips can each show two lines of seven ASCII characters each. The background color can be `Black`, `Red`, `Green`, `Yellow`, `Blue`, `Magenta`, `Cyan`, or `White`. The text color can be light or dark, with the negative space inverted, and can be set independently for both rows.

I recommend not using a `Black` background, because it's completely illegible and the LCD appears to be off or broken, even with `Light` text. Instead, to show white text on a black background, you should set the background color to `White` and the text color to `Light`.

```cs
IScribbleStrip scribbleStrip = device.GetScribbleStrip(0);
scribbleStrip.TopText.Connect("Hello");
scribbleStrip.BottomText.Connect("World");
scribbleStrip.TopTextColor.Connect(ScribbleStripTextColor.Light);
scribbleStrip.BottomTextColor.Connect(ScribbleStripTextColor.Dark);
scribbleStrip.BackgroundColor.Connect(ScribbleStripBackgroundColor.Magenta);
```

For low-level protocol details of this control, see the [Scribble strip RAW MIDI usage](https://github.com/Aldaviva/BehringerXTouchExtender/wiki/Scribble-strips#raw-midi-usage).

### VU meters

There are eight lights on each VU meter, *at most one* of which can be illuminated at any time. They can also all be turned off.

The lights are numbered from `1` at the bottom to `8` at the top, with `0` representing all lights being turned off. You can change which one is illuminated by setting the **`LightPosition`** Property.

The bottom four lights (`1`–`4`) are green, the next three (`5`–`7`) are orange, and the top light (`8`) is red.

The maximum value `8` is available programmatically in the `IVuMeter.LightCount` property.

```cs
IVuMeter vuMeter = device.GetVuMeter(0);
vuMeter.LightPosition.Connect(vuMeter.LightCount);
// Top red light illuminates
```

### Record, solo, mute, and select buttons

```cs
IIlluminatedButton recordButton = device.GetRecordButton(0);
IIlluminatedButton soloButton   = device.GetSoloButton(0);
IIlluminatedButton muteButton   = device.GetMuteButton(0);
IIlluminatedButton selectButton = device.GetSelectButton(0);
```

#### Illuminating lights

When you set a button's **`IlluminationState`** Property to `On`, the button lights up with a color that depends on the kind of button it is. The record and mute buttons light up red, the solo buttons light up yellow, and the select buttons light up green.

You can also use the `Blinking` value to make a button blink with a repeating pattern of 0.5 seconds on, 0.5 seconds off. The `Off` value turns off the light. When they are off, all of the buttons are gray.

```cs
button.IlluminationState.Connect(IlluminatedButtonState.On);
```

```cs
// Light up whenever button is pressed
button.IlluminationState.Connect(DerivedProperty<IlluminatedButtonState>.Create(button.IsPressed, isPressed => 
        isPressed ? IlluminatedButtonState.On : IlluminatedButtonState.Off));
```

#### Detecting presses

You can press the buttons and their **`IsPressed`** Properties will change to `true` (pressed) or `false` (not pressed).

### Faders

Faders are the vertical sliders on each track. They are motorized, so you can move them programmatically to any given position. You can also move them manually with your fingers and detect where they went.

The finger rest knobs have capacitive touch sensors, so the device can sense when you are touching the fader knob, which is useful for preventing the device from moving the fader out from under you when you don't expect it and possibly hurting your hand.

```cs
IFader fader = device.GetFader(0);
```

#### Detecting presses

You can touch the fader knob and its **`IsPressed`** Property will change to `true` (touching) or `false` (not touching).

```cs
fader.IsPressed.PropertyChanged += (sender, args) => 
        Console.WriteLine($"{(args.NewValue ? "Touching" : "Not touching")} fader");
```

#### Requesting movement

The range of motion for each fader is from `0.0` at the bottom to `1.0` at the top. If you try to move the fader to a value outside this range, it will be clipped to stay within the range `[0.0, 1.0]`.

Move a fader by changing the value of its **`DesiredPosition`** Property.

The printed legends on the physical device go from `-∞` at the bottom to `10` at the top, with the `0` legend corresponding to a logical value of about `0.75` in this library.

When you read the value of this Property, it returns the most recent position to which you programmatically requested the fader to move, which may be out-of-date, rather than its current position. To get its current position instead, use `ActualPosition` (see [Detecting movement](#detecting-movement)).

```cs
fader.DesiredPosition.Connect(1.0);
// Fader moves all the way to the top of its travel distance.
```

If the fader is being pressed when you change the `DesiredPosition` value, the change will be automatically ignored so that the knob moving doesn't surprise or hurt the user.

#### Detecting movement

There is a separate **`ActualPosition`** Property that shows where the fader is currently located, which is different from the `DesiredPosition` Property that you use to actuate the motor. These are two separate Properties instead of one in order to prevent infinite event loops, and to make it possible to subscribe to only the changes events that you want.

This property will change in response to both manual (finger) and automatic (motor) movement, so it will always have an up-to-date value for the fader's position.

```cs
fader.ActualPosition.PropertyChanged += (sender, args) =>
        Console.WriteLine($"Fader moved to {args.NewValue:P0}");
```

### Disposal

When you're done using an `IBehringerXTouchExtender<>` instance, you should [dispose](https://learn.microsoft.com/en-us/dotnet/standard/garbage-collection/using-objects) of it to cleanly close the MIDI connection to the device. You can do this explicitly by calling `IBehringerXTouchExtender.Dispose()`, implicitly with a [`using` statement](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement) or [declaration](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/using-statement), or implicitly with a dependency injection framework that manages the lifecycle of your components.

```cs
public void ExplicitlyDispose() {
    var device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
    device.Open();
    // use device here
    device.Dispose();
}
```

```cs
public void ImplicitlyDisposeWithUsingDeclaration() {
    using var device = BehringerXTouchExtenderFactory.CreateWithRelativeMode();
    device.Open();
    // use device here
    // when control exits the ImplicitlyDisposeWithUsingDeclaration method, device will be disposed
}
```

```cs
public void ImplicitlyDisposeWithUsingStatement() {
    using (var device = BehringerXTouchExtenderFactory.CreateWithRelativeMode()) {
        device.Open();
        // use device here
        // when control exits the using block, device will be disposed
    }
}
```

## References
- [Product page: Behringer X-Touch Extender](https://www.behringer.com/product.html?modelCode=0808-AAH)
- [Documentation: Quick Start Guide](https://mediadl.musictribe.com/media/PLM/data/docs/P0CCR/QSG_BE_0808-AAH_X-TOUCH%20EXTENDER_WW.pdf)
- [Firmware: 1.22 ZIP](https://mediadl.musictribe.com/download/software/behringer/X-TOUCH/X-TOUCH-EXT_Firmware_V1.22.zip)

## Acknowledgements

- [Maxim Dobroselsky](https://github.com/melanchall) for the [DryWetMIDI](https://github.com/melanchall/drywetmidi) library that controls MIDI devices from .NET
- The person on the now-deleted MusicTribe forums who correctly [answered a question](https://community.musictribe.com/t5/Recording/X-Touch-Extender-Scribble-Strip-Midi-Sysex-Command/td-p/251306) about the 5th byte of the scribble strip SysEx message, the value of which must be the message length `0x15` instead of the incorrectly documented device ID `0x42`
