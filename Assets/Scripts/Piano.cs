using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piano : MonoBehaviour
{
    private class PlayingNote
    {
        public delegate void onUpdate(float deltaTime);
        public static onUpdate OnUpdate;
        
        public Note Note;
        public Transform Transform;
        
        private bool _keyGoingDown;
        private bool _keyGoingUp;
        private float _keyDownSpeed;
        private float _keyUpSpeed;
        private float _angle;
        
        private void AnimateKeyGoingDown(float deltaTime)
        {
            _angle += _keyDownSpeed * deltaTime;
            if (_angle <= 6)
            {
                Transform.localRotation = Quaternion.Euler(_angle,0,0);
            }
            else
            {
                Transform.localRotation = Quaternion.Euler(6,0,0);
                OnUpdate -= AnimateKeyGoingDown;
                _keyGoingDown = false;
            }
        }
        private void AnimateKeyGoingUp(float deltaTime)
        {
            _angle -= _keyUpSpeed * deltaTime;
            if (_angle >= 0)
            {
                Transform.localRotation = Quaternion.Euler(_angle,0,0);
            }
            else
            {
                Transform.localRotation = Quaternion.Euler(0,0,0);
                OnUpdate -= AnimateKeyGoingUp;
                _keyGoingUp = false;
            }
        }

        public PlayingNote(Note note, Transform transform, float keyDownSpeed, float keyUpSpeed)
        {
            _keyDownSpeed = keyDownSpeed;
            _keyUpSpeed = keyUpSpeed;
            Note = note;
            Transform = transform;
            PressNote();
        }

        public void PressNote()
        {
            if (_keyGoingUp)
            {
                OnUpdate -= AnimateKeyGoingUp;
                _keyGoingUp = false;
            }

            OnUpdate += AnimateKeyGoingDown;
            _keyGoingDown = true;
        }

        public void ReleaseNote()
        {
            if (_keyGoingDown)
            {
                OnUpdate -= AnimateKeyGoingDown;
                _keyGoingDown = false;
            }
            
            OnUpdate += AnimateKeyGoingUp;
            _keyGoingUp = true;
        }
    }
   
    [SerializeField] private float keyDownSpeed = 180f;
    [SerializeField] private float keyUpSpeed = 180f;
    [SerializeField] private Transform[] keyTransforms;
    
    private Dictionary<Note, Transform> _keys = new();
    private Dictionary<Note, PlayingNote> _playingNotes = new();
    
    void Start()
    {
        InitPianoKeys();
    }

    void Update()
    {
        PlayingNote.OnUpdate?.Invoke(Time.deltaTime);
    }

    void InitPianoKeys()
    {
        Debug.Log("Initializing piano keys...");
        for (int i = 0; i < keyTransforms.Length; i++)
        {
            string noteName = keyTransforms[i].gameObject.name;
            Note note = new Note(noteName);
            
            if(_keys.ContainsKey(note))
                Debug.Log("Piano keys already contains: " + _keys[note] + " " + noteName);
            _keys.Add(note, keyTransforms[i]);
        }
    }

    public void PlayNote(Note note)
    {
        if (_playingNotes.ContainsKey(note)) 
            _playingNotes[note].PressNote();
        else
            _playingNotes.Add(note, new PlayingNote(note, _keys[note], keyDownSpeed, keyUpSpeed));
    }

    public void StopPlayingNote(Note note)
    {
        _playingNotes[note].ReleaseNote();
    }
}