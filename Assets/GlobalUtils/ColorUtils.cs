using UnityEngine;
using System.Collections;

public static class ColorUtils
{
	public static Color Darkened(this Color color, float amount)
	{
		float h, s, v;
		Color.RGBToHSV(color, out h, out s, out v);
		v -= amount;
		return Color.HSVToRGB(h, s, v);
	}

	public static string GetHexCode(this Color color)
	{
		return string.Format("{0:X2}{1:X2}{2:X2}", ToByte(color.r), ToByte(color.g), ToByte(color.b));
	}

	private static byte ToByte(float f)
	{
		f = Mathf.Clamp01(f);
		return (byte)(f * 255);
	}
}
