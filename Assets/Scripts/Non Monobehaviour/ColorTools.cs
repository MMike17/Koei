using System.Collections.Generic;
using UnityEngine;

// static class containing usefull methods for color manipulations
public static class ColorTools
{
	public enum Value
	{
		H,
		S,
		V,
		HS,
		HV,
		SV,
		HSV
	}

	// lerps color "from" to color "to" of "percentile" percentile
	public static Color ColorLerp(Color from, Color to, float percentile)
	{
		float r = from.r + ((to.r - from.r) * percentile);
		float g = from.g + ((to.g - from.g) * percentile);
		float b = from.b + ((to.b - from.b) * percentile);
		float a = from.a + ((to.a - from.a) * percentile);

		return new Color(r, g, b, a);
	}

	// lerps color "from" to color "to" of "percentile" percentile using hsv
	public static Color ColorLerpHSV(Color from, Color to, float percentile)
	{
		float[] from_values = ReturnHSV(from);
		float[] to_values = ReturnHSV(to);

		to_values[0] = from_values[0] + ((to_values[0] - from_values[0]) * percentile);
		to_values[1] = from_values[1] + ((to_values[1] - from_values[1]) * percentile);
		to_values[2] = from_values[2] + ((to_values[2] - from_values[2]) * percentile);

		return Color.HSVToRGB(to_values[0], to_values[1], to_values[2]);
	}

	// shifts individual values of an HSV color
	public static Color LerpColorValues(Color color, Value lerpValues, int[] offsets)
	{
		float[] colorValues = ReturnHSV(color);

		switch(lerpValues)
		{
			case Value.H:
				colorValues[0] += (float) offsets[0] / 100;
				break;
			case Value.S:
				colorValues[1] += (float) offsets[0] / 100;
				break;
			case Value.V:
				colorValues[2] += (float) offsets[0] / 100;
				break;

			case Value.HS:
				colorValues[0] += (float) offsets[0] / 100;
				colorValues[1] += (float) offsets[1] / 100;
				break;
			case Value.HV:
				colorValues[0] += (float) offsets[0] / 100;
				colorValues[2] += (float) offsets[1] / 100;
				break;
			case Value.SV:
				colorValues[1] += (float) offsets[0] / 100;
				colorValues[2] += (float) offsets[1] / 100;
				break;

			case Value.HSV:
				colorValues[0] += (float) offsets[0] / 100;
				colorValues[1] += (float) offsets[1] / 100;
				colorValues[2] += (float) offsets[2] / 100;
				break;
		}

		return Color.HSVToRGB(colorValues[0], colorValues[1], colorValues[2]);
	}

	static float[] ReturnHSV(Color color)
	{
		float[] values = new float[3];
		Color.RGBToHSV(color, out values[0], out values[1], out values[2]);

		return values;
	}
}