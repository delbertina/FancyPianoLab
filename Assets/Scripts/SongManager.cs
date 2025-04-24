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

    int[] songNotes = new int[] { 0, 1, 2, 3, 4 }; // Array of note indices to spawn
    int[] noteDurations = new int[] { 1, 1, 2, 1, 1 }; // Array of note indices to spawn

    int currentNoteIndex = 0; // Index of the current note to spawn
    int nextNoteTime = 4; // Time to spawn the next note


    float noteSpeed = 1f; // Speed of the notes
    [SerializeField] public Dictionary<string, Vector3> noteSpawnPoint = new Dictionary<string, Vector3>();

    float time = 0f;

    List<GameObject> spawnedNotes = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GameObject newNote = Instantiate(notes[currentNoteIndex].notePrefab, notes[currentNoteIndex].spawnPoint.position, notes[currentNoteIndex].spawnPoint.rotation);
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
            GameObject newNote = Instantiate(notes[currentNoteIndex].notePrefab, notes[currentNoteIndex].spawnPoint.position, notes[currentNoteIndex].spawnPoint.rotation);
            newNote.SetActive(true);
            spawnedNotes.Add(newNote);

            nextNoteTime += noteDurations[currentNoteIndex];

            currentNoteIndex++;
        }


    }
}
