using UnityEngine;

// class representing an exchange between the player and the character
[CreateAssetMenu(fileName = "Dialogue", menuName = "Koei/Dialogue")]
public class Dialogue : ScriptableObject
{
	[TextArea(1, 10)]
	public string playerQuestion, characterAnswer;
	public bool hasClue;
	public Clue clue;

	public Dialogue[] nextDialogues;

	bool done;

	public void Reset()
	{
		done = false;
	}

	// will be called recursively along dialogues untill explorations of all branches are done
	public bool IsDone()
	{
		return done;
	}

	public void SetAsDone()
	{
		done = true;
	}
}