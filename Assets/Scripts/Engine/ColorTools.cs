using UnityEngine;

public static class ColorTools
{
	public static Color ColorLerp(Color from, Color to, float percentile)
	{
		float r = from.r + ((to.r - from.r) * percentile);
		float g = from.g + ((to.g - from.g) * percentile);
		float b = from.b + ((to.b - from.b) * percentile);
		float a = from.a + ((to.a - from.a) * percentile);

		return new Color(r, g, b, a);
	}
}