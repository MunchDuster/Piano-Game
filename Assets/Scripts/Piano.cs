using UnityEngine;
using System.Collections;

public class Piano : MonoBehaviour
{
	public int noteOffset = 0;

	public Oscillator oscillator;

	public class Key
	{
		public int noteNumber;
		public Transform transform;
		public AudioSource source;

		public Key(int noteNumber, Transform transform)
		{
			this.noteNumber = noteNumber;
			this.transform = transform;

			renderer = transform.gameObject.GetComponent<Renderer>();
			oldColor = renderer.material.color;
		}

		private bool wasPlaying = false;
		private Oscillator.Note note;
		private Renderer renderer;
		private Color oldColor;

		public void Update(int noteOffset)
		{
			float move = MidiInput.GetKey(noteNumber + noteOffset);
			bool isPlaying = move > 0;
			Debug.Log(isPlaying);
			if(isPlaying && !wasPlaying) Play(move, noteOffset);
			else if(!isPlaying && wasPlaying) UnPlay();
			wasPlaying = isPlaying;
		}

		private void Play(float move, int noteOffset)
		{
			transform.localRotation = Quaternion.Euler(move * 7, 0, 0);
			renderer.material.color = Color.red;
			note = new Oscillator.Note(noteNumber + noteOffset, move);

			Debug.Log(note);
		}
		private void UnPlay()
		{
			transform.localRotation = Quaternion.Euler(0, 0, 0);
			renderer.material.color = oldColor;
			note.Release();
		}
	}

	public Transform[] keyTransforms;


	public Key[] keys = new Key[88];


	// Start is called before the first frame update
	void Start()
	{
		SetupKeys();
	}

	void SetupKeys()
	{
		int index = 0;
		foreach(Transform keyTransform in keyTransforms)
		{
			int noteNumber = GetNoteNumberFromNoteName(keyTransform.gameObject.name);
			keys[index++] = new Key(noteNumber, keyTransform);
		}
	}

	static string[] noteNamesRaw = new string[] { "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#" };

	string GetNoteNameFromNumber(int noteNumber)
	{
		int octave = noteNumber / 12;
		int note = noteNumber % 12;

		return noteNamesRaw[note] + octave.ToString(); //Example: A4
	}

	int GetNoteNumberFromNoteName(string noteName)
	{
		bool hasSharp = noteName[1] == '#';

		int noteNameRawLength = hasSharp ? 2 : 1;
		int octaveNumberLength = noteName.Length - noteNameRawLength;

		string noteNameRaw = noteName.Substring(0,noteNameRawLength);
		string octaveNumber = noteName.Substring(noteNameRawLength,octaveNumberLength);
			
		int note = System.Array.IndexOf(noteNamesRaw, noteNameRaw);
		int octave = int.Parse(octaveNumber);

		return note + octave * 12;
	}

	void Update()
	{
		foreach(Key key in keys)
		{
			key.Update(noteOffset);
		}
	}
}
