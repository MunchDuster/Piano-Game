using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour
{
	private float increment;
	private float phase;
	private float gain;

	public WaveSettings waveSettings;

	public WaveType waveType;

	public Wave.WaveFunction waveFunction;

	// Start is called before the first frame update
	private void Awake()
	{
		UpdateWaveType();
		Note.notes = new List<Note>();
	}

	public void UpdateWaveType()
	{
		waveFunction = Wave.waveFunctions[waveType];
	}

	[System.Serializable]
	public class Note
	{
		public static List<Note> notes;

		public float frequency;
		public float strength;
		

		[HideInInspector] public float phase;
		[HideInInspector] public float amplitude;

		private float timeReleased;
		private float timePressed;
		private float pressStrength;
		private bool keyHeld = true;
		private float maxAmplitude = 0;

		public bool UpdateAmplitude(WaveSettings waveSettings)
		{
			if(keyHeld)
			{
				float timeSincePressed = Time.timeSinceLevelLoad - timePressed;
				amplitude = Mathf.Lerp(0, waveSettings.volume * pressStrength, timeSincePressed * waveSettings.gainSpeed);
				maxAmplitude = amplitude;
			}
			else
			{

				float timeSinceReleased = Time.timeSinceLevelLoad - timeReleased;

				//Check if delete note
				if(timeSinceReleased >= waveSettings.fallSpeed)
				{
					notes.Remove(this);
					return true;
				}

				amplitude = Mathf.Lerp(maxAmplitude, 0, timeSinceReleased * waveSettings.fallSpeed);
			}

			//if(keyHeld) amplitude = Mathf.Lerp(amplitude, waveSettings.volume * strength, waveSettings.gainSpeed * Time.deltaTime);
			//else
			

			//amplitude = Mathf.Lerp(waveSettings.volume, 0,  Mathf.Clamp01(timeSinceReleased / waveSettings.fallSpeed));
			return false;
		}

		public void Release()
		{
			keyHeld = false;
			timeReleased = Time.timeSinceLevelLoad;
		}
		public void UpdateWave(ref float[] data, int channels, Wave.WaveFunction waveFunction, WaveSettings waveSettings)
		{
			data = waveFunction(data, channels, this, waveSettings);
		}

		public Note(int noteNumber, float pressStrength)
		{
			frequency = GetNoteFrequency(noteNumber);
			timePressed = Time.timeSinceLevelLoad;
			this.pressStrength = pressStrength;
			notes.Add(this);
		}

		private float GetNoteFrequency(int noteNumber)
		{
			float pow = (noteNumber - 69f) / 12f;
			return 440f * Mathf.Pow(2f, pow);
		}
	}

	// Update is called every frame
	private void Update()
	{
		for (int i = 0; i < Note.notes.Count; i++)
		{
			if(Note.notes[i].UpdateAmplitude(waveSettings))
			{
				i--;
			}
		}
	}

	private void OnAudioFilterRead(float[] data, int channels)
	{
		if(Note.notes == null) return;
		for (int i = 0; i < Note.notes.Count; i++)
		{
			Note.notes[i].UpdateWave(ref data, channels, waveFunction, waveSettings);
		}
	}
}
