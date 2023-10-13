using System;
using System.Linq;
using UnityEngine;

public struct Note
{
    private static readonly string[] SemitoneNames =
    {
        "A", "A#", "B", "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#"
    };

    private int octave;
    private int semitone;

    public static int GetNoteNumber(Note note)
    {
        return GetNoteNumber(note.octave, note.semitone);
    }

    public static int GetNoteNumber(int octave, int semitone)
    {
        //The number of semitones from A0, AKA OverallSemitones
        return octave * 12 + semitone;
    }

    public static float GetFrequency(Note note)
    {
        //Starting from A0
        return 55f * Mathf.Pow(2, GetNoteNumber(note) / 12f);
    }

    public static string GetNoteName(Note note)
    {
        return note.octave + SemitoneNames[note.semitone];
    }

    public static int GetNumberFromName(string name)
    {
        string[] nameSplit = name.ToCharArray().Select(c => c.ToString()).ToArray(); //Split string into array of chars, but they are still strings
        bool nameIsTwoChars = nameSplit.Length == 2;
        
        //Semitone
        string semitoneName = nameIsTwoChars ? nameSplit[0] : nameSplit[0] + nameSplit[1];
        int semitone = Array.IndexOf(SemitoneNames, semitoneName);
        if (semitone == -1)
            Debug.LogError("UNPARSABLE VALUE, CAN'T GET SEMITONE FROM NOTE NAME: " + name);
        
        //Octave
        string octaveText = nameIsTwoChars ? nameSplit[1] : nameSplit[2];
        if (!int.TryParse(octaveText, out int octave))
            Debug.LogError("UNPARSABLE VALUE, CAN'T GET OCTAVVE FROM NOTE NAME: " + name);

        return GetNoteNumber(octave, semitone);
    }

    public static implicit operator int(Note note)
    {
        return GetNoteNumber(note);
    }

    public static Note[] GetNotes()
    {
        Note[] notes = new Note[88];
        for (int noteNumber = 0; noteNumber < 88; noteNumber++)
            notes[noteNumber] = new Note(noteNumber);
        return notes;
    }

    public Note(string name)
    {
        int noteNumber = GetNumberFromName(name);
        octave = noteNumber / 12;
        semitone = noteNumber % 12;
    }
    public Note(int noteNumber)
    {
        octave = noteNumber / 12;
        semitone = noteNumber % 12;
    }
}