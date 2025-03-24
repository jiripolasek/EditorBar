// ------------------------------------------------------------
// 
// Copyright (c) Jiří Polášek. All rights reserved.
// 
// ------------------------------------------------------------

#nullable enable

using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace JPSoftworks.EditorBar.Presentation;

public class BrightnessConverter : IValueConverter
{
    public double Factor { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is SolidColorBrush brush)
        {
            var color = brush.Color;
            var originalHsv = RgbToHsv(color);

            // check if new V is sufficiently different from the original; if the value is capped and near the original, then we have to change the direction (lighten/darken) to make the change more visible
            // let's say that 10 % is the threshold for "sufficiently different"; it the new value is within 10 % of the original, we will change the direction

            var newHsv = AdjustBrightness(originalHsv, this.Factor);
            var newColor = HsvToRgb(newHsv);
            return new SolidColorBrush(newColor);
        }

        return value;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }

    private static HsvColor AdjustBrightness(HsvColor hsv, double factor)
    {
        var (h, s, v) = hsv;

        // Make the colors brighter
        var newS = Math.Min(s * 1.2, 1);

        var deltaV = factor > 1 ? 0.15 : -0.1;
        var newV = v + deltaV;

        newV = Clamp(newV);

        return (h, newS, newV);
    }

    private static double Clamp(double value)
    {
        return value switch
        {
            < 0.0 => 0.0,
            > 1.0 => 1.0,
            _ => value
        };
    }


    private static HsvColor RgbToHsv(Color color)
    {
        var r = color.R / 255.0;
        var g = color.G / 255.0;
        var b = color.B / 255.0;

        var max = Math.Max(r, Math.Max(g, b));
        var min = Math.Min(r, Math.Min(g, b));
        var delta = max - min;

        double hue = 0;
        if (delta != 0)
        {
            if (max == r)
            {
                hue = (g - b) / delta % 6;
            }
            else if (max == g)
            {
                hue = ((b - r) / delta) + 2;
            }
            else
            {
                hue = ((r - g) / delta) + 4;
            }

            hue *= 60;
            if (hue < 0)
            {
                hue += 360;
            }
        }


        double saturation;
        if (color.R == color.G && color.R == color.B)
        {
            saturation = 0;
        }
        else
        {
            saturation = max == 0 ? 0 : delta / max;
        }

        return (hue, saturation, max);
    }

    private static Color HsvToRgb(HsvColor hsv)
    {
        var isGrayscale = hsv.S == 0;
        if (isGrayscale)
        {
            var v = (byte)(hsv.V * 255);
            return Color.FromRgb(v, v, v);
        }


        var c = hsv.V * hsv.S;
        var x = c * (1 - Math.Abs((hsv.H / 60 % 2) - 1));
        var m = hsv.V - c;

        double r = 0, g = 0, b = 0;
        if (hsv.H < 60)
        {
            (r, g, b) = (c, x, 0);
        }
        else if (hsv.H < 120)
        {
            (r, g, b) = (x, c, 0);
        }
        else if (hsv.H < 180)
        {
            (r, g, b) = (0, c, x);
        }
        else if (hsv.H < 240)
        {
            (r, g, b) = (0, x, c);
        }
        else if (hsv.H < 300)
        {
            (r, g, b) = (x, 0, c);
        }
        else
        {
            (r, g, b) = (c, 0, x);
        }

        return Color.FromRgb(
            (byte)((r + m) * 255),
            (byte)((g + m) * 255),
            (byte)((b + m) * 255)
        );
    }

    internal record struct HsvColor(double H, double S, double V)
    {
        public static implicit operator (double H, double S, double V)(HsvColor value)
        {
            return (value.H, value.S, value.V);
        }

        public static implicit operator HsvColor((double H, double S, double V) value)
        {
            return new HsvColor(value.H, value.S, value.V);
        }
    }
}