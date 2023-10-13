using System.Collections.Generic;
using UnityEngine;

public class SongPlayer : MonoBehaviour
{
    [SerializeField] private Piano piano;
    [SerializeField] private AudioSynthesizer audioSynth;

    private int noteNumber = 30;
    private void Update()
    {
        AddKeyListener(KeyCode.A, "C2");
        AddKeyListener(KeyCode.S, "D2");
        AddKeyListener(KeyCode.D, "E2");
        AddKeyListener(KeyCode.F, "F2");
        AddKeyListener(KeyCode.G, "G2");
        AddKeyListener(KeyCode.H, "A3");
        AddKeyListener(KeyCode.J, "B3");
        AddKeyListener(KeyCode.K, "C3");
        
        AddKeyListener(KeyCode.W, "C#2");
        AddKeyListener(KeyCode.E, "D#2");
        AddKeyListener(KeyCode.T, "F#2");
        AddKeyListener(KeyCode.Y, "G#2");
        AddKeyListener(KeyCode.U, "A#3");
    }

    private void AddKeyListener(KeyCode keycode, string noteName)
    {
        if (Input.GetKeyDown(keycode)) PlayNote(new Note(noteName));
        else if (Input.GetKeyUp(keycode)) StopPlayingNote(new Note(noteName));
    }

    private void PlayNote(Note note)
    {
        piano.PlayNote(note);
        audioSynth.PlayNote(note);
    }
    private void StopPlayingNote(Note note)
    {
        piano.StopPlayingNote(note);
        audioSynth.StopPlayingNote(note);
    }
}