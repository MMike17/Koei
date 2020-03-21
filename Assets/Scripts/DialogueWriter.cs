using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
[DisallowMultipleComponent]
public class DialogueWriter : MonoBehaviour
{
	[Header("Settings")]
	public float writingSpeed;
	public int trailLength;
	public Color highlightColor;

	[Header("Assign in Inspector")]
	[TextArea]
	public string line;

	public bool isDone => string.IsNullOrEmpty(line) || writingIndex - trailLength > line.Length;
	public float timeBeforeEnd => 1 / writingSpeed * line.Length;

	TextMeshProUGUI textComponent;
	Color[] trailColors;
	int writingIndex;
	float timer;
	bool start;

	void Awake()
	{
		textComponent = GetComponent<TextMeshProUGUI>();
	}

	void Update()
	{
		if(start)
		{
			if(string.IsNullOrEmpty(line))
			{
				Debug.LogError("line to write is empty", gameObject);
				return;
			}

			string result = string.Empty;

			if(writingIndex - trailLength > line.Length)
			{
				textComponent.text = line;
				return;
			}
			else
			{
				// add non highlighted chars
				if(writingIndex - trailLength >= 0)
				{
					result += line.Substring(0, writingIndex - trailLength);
				}

				// add highlighted chars
				for (int i = 0; i < trailLength; i++)
				{
					int index = writingIndex - trailLength + i;

					if(index >= 0 && index < line.Length)
						result += "<color=#" + ColorUtility.ToHtmlStringRGB(trailColors[i]) + ">" + line.Substring(index, 1) + "</color>";
				}

				// add invisible chars
				if(writingIndex < line.Length)
				{
					result += "<color=#" + ColorUtility.ToHtmlStringRGBA(Color.clear) + ">" + line.Substring(writingIndex) + "</color>";
				}

				// apply line
				textComponent.text = result;

				// timer and index incrementation
				timer += Time.deltaTime;

				if(timer >= 1 / writingSpeed)
				{
					timer = 0;
					writingIndex++;
				}
			}
		}
	}

	public void Play()
	{
		if(start)
			return;

		writingIndex = 0;
		timer = 0;

		ComputeColors();

		start = true;
	}

	public void Play(string toWrite)
	{
		line = toWrite;

		Play();
	}

	public void Play(Color highlight)
	{
		highlightColor = highlight;

		Play();
	}

	public void Play(string toWrite, Color highlight)
	{
		line = toWrite;
		highlightColor = highlight;

		Play();
	}

	public void Play(string toWrite, float speed, int length, Color highlight, Color text)
	{
		highlightColor = highlight;
		textComponent.color = text;

		line = toWrite;
		writingSpeed = speed;
		trailLength = length;

		Play();
	}

	public void Reset()
	{
		start = false;
		textComponent.text = string.Empty;

		writingIndex = 0;
		timer = 0;
	}

	public void Finish()
	{
		writingIndex = line.Length + trailLength;
	}

	void ComputeColors()
	{
		Awake();
		trailColors = new Color[trailLength];

		for (int i = 0; i < trailLength; i++)
		{
			trailColors[i] = ColorTools.ColorLerp(textComponent.color, highlightColor, (float) i / trailLength);
		}
	}
}