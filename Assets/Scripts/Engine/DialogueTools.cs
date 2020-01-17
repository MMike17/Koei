using UnityEngine;

// utilities for dialogues
public static class DialogueTools
{
	public static string HighlightString(string line, Color textColor, Color highlight, int charIndex, int highlightLength)
	{
		if(charIndex >= line.Length + highlightLength)
			return line;

		string result = string.Empty, after = string.Empty;

		// computes actaul trail length
		int actualTrailLength = Mathf.Min(charIndex + 1, highlightLength);

		// fills the color slots in
		Color[] trailColors = new Color[actualTrailLength];

		for (int i = 0; i < trailColors.Length; i++) // 0 => text color ; last => highlight color
		{
			trailColors[i] = ColorTools.ColorLerp(textColor, highlight, (float) (i + 1) / actualTrailLength);
		}

		// computes length of before
		int beforeLength = charIndex - (actualTrailLength - 1);

		// feeds in old chars if length > 0
		if(beforeLength > 0 && beforeLength < line.Length)
		{
			result = line.Substring(0, beforeLength);
		}

		// feeds after if index is less than line length (there are invisible chars)
		if(charIndex < line.Length)
		{
			int start = Mathf.Max(beforeLength + actualTrailLength, 0);
			after = "<color=#00000000>" + line.Substring(start) + "</color>";
		}
		else
		{

		}

		// colors chars that have to be shown
		for (int i = 0; i < actualTrailLength; i++)
		{
			int index = charIndex - (actualTrailLength - 1) + i;

			// if chars are inside the showable bracket
			if(index <= line.Length - 1 && index >= 0)
			{
				result += "<color=#" + ColorUtility.ToHtmlStringRGB(trailColors[i]) + ">" + line.Substring(charIndex + 1 - actualTrailLength + i, 1) + "</color>";
			}
		}

		result += after;

		return result;
	}
}