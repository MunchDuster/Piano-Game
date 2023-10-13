
    public record PlayNote
    {
        public Note Note;
        public int DurationMilliseconds;
        public int startMilliseconds;

        public int EndTimeMilliseconds => startMilliseconds + DurationMilliseconds;
        
        public static implicit operator int(PlayNote playNote)
        {
            return Note.GetNoteNumber(playNote.Note);
        }
        
        public PlayNote(Note note, int startMilliseconds, int durationMilliseconds)
        {
            this.Note = note;
            this.startMilliseconds = startMilliseconds;
            this.DurationMilliseconds = durationMilliseconds;
        }
    }
