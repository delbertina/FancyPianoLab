using System.Collections.Generic;
using UnityEngine;

public class SongManager : MonoBehaviour
{




    [System.Serializable]
    public class NoteObject
    {
        public string noteName;
        public GameObject notePrefab;
        public Transform spawnPoint;
    }

    [System.Serializable]
    public class Note
    {
        public int noteObjectIndex;
        public float noteDuration;
    }

    [SerializeField] public List<NoteObject> notes = new List<NoteObject>();
    //[SerializeField] public List<Note> songNotes = new List<Note>();

    int[] songNotes = new int[] { 5, 3, 3, 4, 2, 2, 1, 2, 3, 4, 5, 5, 5, 3, 3, 4, 4, 2, 2, 1, 2, 1 }; // Array of note indices to spawn
    float[] noteDurations = new float[] { 1, 0.5f, 0.5f, 1, .5f, .5f, .5f, .5f, .5f, .5f, 2, .5f, .5f, .5f, .5f, .5f, .5f, .5f, .5f, 1, 1, 2 }; // Array of note indices to spawn

    int currentNoteIndex = 0; // Index of the current note to spawn
    float nextNoteTime = 4; // Time to spawn the next note


    float noteSpeed = 1f; // Speed of the notes
    [SerializeField] public Dictionary<string, Vector3> noteSpawnPoint = new Dictionary<string, Vector3>();

    float time = 0f;

    List<GameObject> spawnedNotes = new List<GameObject>();

    [ContextMenu("Start song in 5s")]
    private void StartSong()
    {
        nextNoteTime = 5f;
        time = 0f;
        currentNoteIndex = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject newNote = Instantiate(notes[songNotes[currentNoteIndex] - 1].notePrefab, notes[songNotes[currentNoteIndex] - 1].spawnPoint.position, notes[songNotes[currentNoteIndex] - 1].spawnPoint.rotation);
        newNote.SetActive(true);
        spawnedNotes.Add(newNote);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        foreach (GameObject note in spawnedNotes)
        {
            // Move the note down the screen
            note.transform.position += Vector3.down * noteSpeed * Time.deltaTime;
            // Check if the note has gone off the screen
            /*if (note.transform.position.y < 10f)
            {
                Destroy(note);
                spawnedNotes.Remove(note);
            }*/
        }

        if (time > nextNoteTime)
        {
            if (currentNoteIndex >= songNotes.Length)
            {
                // All notes have been spawned
                return;
            }
            // Spawn the next note
            GameObject newNote = Instantiate(notes[songNotes[currentNoteIndex] - 1].notePrefab, notes[songNotes[currentNoteIndex] - 1].spawnPoint.position, notes[songNotes[currentNoteIndex] - 1].spawnPoint.rotation);
            newNote.SetActive(true);
            spawnedNotes.Add(newNote);

            nextNoteTime += noteDurations[currentNoteIndex];

            currentNoteIndex++;
        }


    }
}
