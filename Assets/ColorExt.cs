using UnityEngine;

public static class ColorExtension
{
    public static Color LerpHSV(this Color color1, Color color2, float t)
    {
        Color.RGBToHSV(color1, out float h1, out float s1, out float v1);
        Color.RGBToHSV(color2, out float h2, out float s2, out float v2);

        float h = Mathf.LerpAngle(h1 * 360f, h2 * 360f, t) / 360f;
        float s = Mathf.Lerp(s1, s2, t);
        float v = Mathf.Lerp(v1, v2, t);

        return Color.HSVToRGB(h, s, v);
    }
}