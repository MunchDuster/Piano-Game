using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[RequireComponent(typeof(AudioSource))]
public class AudioSynthesizer : MonoBehaviour
{
    private class PlayNoteWave
    {
        private static readonly double SamplingFrequency = 48000;
        
        public readonly Note Note;
        
        private double[] _phases;
        private double[] _harmonicsGains;
        private double _gain;
        private readonly double _gainIncrement;
        private readonly double _fadeDecrement;
        private readonly double _baseFrequency;
        private readonly float _volume;
        private bool _isFading = true;
        private Random _random = new (86018326);

        public PlayNoteWave(Note note, float volume, float gainTime, float fadeTime, double[] harmonics)
        {
            Note = note;
            _phases = new double[5];
            _harmonicsGains = harmonics;
            _gain = 0;
            _baseFrequency = Note.GetFrequency(note);
            _volume = volume;
            _gainIncrement = _volume / (gainTime * SamplingFrequency);
            _fadeDecrement = _volume / (fadeTime * SamplingFrequency);
        }

        public void Stop() { _isFading = true; }
        public void Play() { _isFading = false; }

        public double Update()
        {
            _gain = _isFading ? Math.Max(_gain - _fadeDecrement, 0.0) : Math.Min(_gain + _gainIncrement, _volume);
           
            double value = 0;
            AddHarmonics(ref value);
            
            //Add some noise
            AddNoise(ref value, 0.01f);
            
            return value;
        }

        private void AddHarmonics(ref double value)
        {
            //A bunch of octaves with decreasing strength
            double max = 0;
            for (int i = 0; i < _phases.Length; i++)
            {
                double frequency = (i+2) * _baseFrequency;
                double phaseIncrement = frequency * 2.0 * Mathf.PI / SamplingFrequency;
                
                _phases[i] = (_phases[i] + phaseIncrement) % (2 * Mathf.PI);

                double gainMultiplier = _harmonicsGains[i];
                max += gainMultiplier;
                value += Math.Sin(_phases[i]) * gainMultiplier * _gain;
            }
            
            //Scale sum of waves to max of 1
            value /= max;
        }

        private void AddNoise(ref double value, double noiseScale)
        {
            value += 2 * (_random.NextDouble() - 0.5) * noiseScale * _gain;
        }
    }
    
    [SerializeField] private float volume = 0.1f;     // Volume of the audio clip
    [SerializeField] private float gainTime = 0.1f;
    [SerializeField] private float fadeTime = 0.1f;

    [SerializeField] private double[] harmonics = { 0.7, 0.25, 0.5, 0.4, 0.05, 0.3, 0.1, 0.1, 0.2 };

    private AudioSource audioSource;

    private readonly Dictionary<int, PlayNoteWave> waves = new();

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        foreach (Note note in Note.GetNotes())
        {
            PlayNoteWave wave = new(note, volume, gainTime, fadeTime, harmonics);
            waves.Add(note, wave);
        }
    }
    public void PlayNote(Note note)
    {
        waves[note].Play();
    }
    public void StopPlayingNote(Note note)
    {
        waves[note].Stop(); //Will start note fading, when ready will call remove note
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (waves.Count == 0) return;
        
        for (int i = 0; i < data.Length; i += channels)
        {
            float sample = 0;
            int[] keys = waves.Keys.ToArray();
            foreach (int key in keys)
            {
                sample += (float)waves[key].Update();
            }
            
            sample /= waves.Count;
            for (int channel = 0; channel < channels; channel++)
            {
                data[i + channel] = sample;
            }
        }
    }
}
