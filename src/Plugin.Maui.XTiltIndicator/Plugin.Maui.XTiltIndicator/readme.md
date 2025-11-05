# Plugin.Maui.XTiltIndicator

**XPitchIndicator** is a MAUI control for displaying the device's tilt (pitch) in both horizontal and vertical axes. It supports two display modes: **Gauge** (semi-circular arc with indicator) and **Bars** (horizontal/vertical bars). Ideal for flight simulators, technical instruments, HUDs, or any app that needs to visualize device orientation.

---

![Overlay Screenshot](https://raw.githubusercontent.com/vankraster/Plugin.Maui.XTiltIndicator/refs/heads/master/demos/images/v1.png)



## Features

- Supports **HorizontalPitch** and **VerticalPitch**.
- Display modes: **Gauge** or **Bars**.
- Customizable colors for the indicator and zero line.
- Optional numeric display of pitch value.
- Smooth motion with easing (`lerp`) for natural transitions.
- Built on **GraphicsView** for optimal performance.
- Uses the device accelerometer to calculate pitch angles.

---

## Installation

Install via NuGet:

```bash
dotnet add package Plugin.Maui.XTiltIndicator --version 1.0.0
```

---

## Configuration

You can customize the control by setting its bindable properties in XAML or C#.

### Example XAML

```xml
<Grid BackgroundColor="White">
    <xctrls:XPitchIndicator
        HorizontalOptions="Fill"
        VerticalOptions="Fill"
        PitchColor="Red"
        PitchZeroColor="Lime"
        HorizontalPitch="Gauge"
        VerticalPitch="Gauge"
        ShowHorizontalPitchValue="False"/>
        
    <Label Text="Your content" HorizontalOptions="Center" />
</Grid>
```

### Example C# Usage

```csharp
using Plugin.Maui.XTiltIndicator;

var pitchIndicator = new XPitchIndicator
{
    HorizontalOptions = LayoutOptions.Fill,
    VerticalOptions = LayoutOptions.Fill,
    PitchColor = Colors.Red,
    PitchZeroColor = Colors.Lime,
    HorizontalPitch = TPitchType.Gauge,
    VerticalPitch = TPitchType.Gauge,
    ShowHorizontalPitchValue = false
};

var grid = new Grid { BackgroundColor = Colors.White };
grid.Children.Add(pitchIndicator);
grid.Children.Add(new Label 
{ 
    Text = "Your content", 
    HorizontalOptions = LayoutOptions.Center 
});

Content = grid;
```

---

### Bindable Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `PitchColor` | `Color` | `Lime` | Sets the color of the pitch indicator and major tick marks. Example: `Red`. |
| `PitchZeroColor` | `Color` | `Red` | Sets the color of the zero line for horizontal and vertical scales. Example: `Lime`. |
| `HorizontalPitch` | `TPitchType` | `Gauge` | Display mode for horizontal pitch. Options: `Gauge` (semi-circle) or `Bars`. |
| `VerticalPitch` | `TPitchType` | `Gauge` | Display mode for vertical pitch. Options: `Gauge` or `Bars`. |
| `ShowHorizontalPitchValue` | `bool` | `true` | Show numeric horizontal pitch value above the semicircular gauge. Set to `False` to hide. |

---

## Smooth Motion / Easing

The control applies a **linear interpolation (lerp)** to smooth the movement of the indicator when pitch values change. This creates a natural, analog-style motion, avoiding abrupt jumps from sensor updates.

---

## Notes

- On emulators, accelerometer values may not be exactly zero due to simulation offsets.
- Tested on Android, iOS, and Windows MAUI apps.
- For best results, test on a real device.

---

## License

MIT License. Free to use and modify.