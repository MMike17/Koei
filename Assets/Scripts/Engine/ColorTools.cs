using UnityEngine;

// static class containing usefull methods for color manipulations
public static class ColorTools
{
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

	static float[] ReturnHSV(Color color)
	{
		float[] values = new float[3];
		Color.RGBToHSV(color, out values[0], out values[1], out values[2]);

		return values;
	}
}