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

	// a hsv version might be interesting...
}