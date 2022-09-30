using UnityEngine;
using System.Collections.Generic;

public enum WaveType
{
	Sine,
	Square
}

[System.Serializable]
public class WaveSettings
{
	public float volume = 0.1f;
	public float samplingFrequency = 48000;
	public float gainSpeed = 1;
	public float fallSpeed = 1;

	public void UpdatePhase(ref Oscillator.Note note)
	{
		float increment = note.frequency * 2f * Mathf.PI / samplingFrequency;
		note.phase = (note.phase + increment) % (2 * Mathf.PI);

	}
}
public static class Wave
{
	public delegate float[] WaveFunction(float[] data, int channels, Oscillator.Note note, WaveSettings info);

	public static WaveFunction sineWave = (float[] data, int channels, Oscillator.Note note, WaveSettings settings) =>
	{
		for (int i = 0; i < data.Length; i += channels)
		{
			settings.UpdatePhase(ref note);

			float delta = note.amplitude * Mathf.Sin(note.phase);

			for(int c = 0; c < channels; c++)
			{
				data[i + c] += delta;
			}
		}

		return data;
	};
	public static WaveFunction squareWave = (float[] data, int channels, Oscillator.Note note, WaveSettings settings) =>
	{
		for (int i = 0; i < data.Length; i += channels)
		{
			settings.UpdatePhase(ref note);

			float delta = note.amplitude * Mathf.Sign(Mathf.Sin(note.phase));

			for(int c = 0; c < channels; c++)
			{
				data[i + c] += delta;
			}
		}

		return data;

	};

	public static Dictionary<WaveType, WaveFunction> waveFunctions = new Dictionary<WaveType, WaveFunction>()
	{
		{WaveType.Sine, sineWave},
		{WaveType.Square, squareWave},
	};
}