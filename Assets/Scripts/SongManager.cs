using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using static SongManager;

public class SongManager : MonoBehaviour
{


    private InputDevice _leftHand;
    [SerializeField] public Slider speedSlider; // Reference to the speed slider in the UI
    [SerializeField] public Dropdown songDropdown; // Reference to the dropdown in the UI

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

    int[] songNotesA = new int[] { 5, 3, 3, 4, 2, 2, 1, 2, 3, 4, 5, 5, 5, 3, 3, 4, 4, 2, 2, 1, 2, 1 }; // Array of note indices to spawn
    float[] noteDurationsA = new float[] { 1, 0.5f, 0.5f, 1, .5f, .5f, .5f, .5f, .5f, .5f, 2, .5f, .5f, .5f, .5f, .5f, .5f, .5f, .5f, 1, 1, 2 }; // Array of note indices to spawn

    int[] songNotesB = new int[] { 3, 3, 3, 3, 3, 3, 3, 5, 1, 2, 3, 4, 4, 4, 4, 4, 3, 3, 3, 3, 3, 2, 2, 3, 2, 5 }; // Array of note indices to spawn
    float[] noteDurationsB = new float[] { 1, 1, 2, 1, 1, 2, 1, 1, 1, 1, 2, 1, 1, 1, 1, 1, 1, 1, .5f, .5f, 1, 1, 1, 1, 2, 2 }; // Array of note indices to spawn

    int[] songNotes; 
    float[] noteDurations;

    int currentNoteIndex = 1000; // Index of the current note to spawn
    float nextNoteTime = 4; // Time to spawn the next note


    [SerializeField] float noteSpeed = 1f; // Speed of the notes
    [SerializeField] public Dictionary<string, Vector3> noteSpawnPoint = new Dictionary<string, Vector3>();

    float time = 0f;

    List<GameObject> spawnedNotes = new List<GameObject>();

    [ContextMenu("Start song in 5s")]
    public void StartSong()
    {
        nextNoteTime = 2f;
        time = 0f;
        currentNoteIndex = 0;
        noteSpeed = speedSlider.value; // Update the note speed based on the slider value
        Debug.Log(songDropdown.value);
        if (songDropdown.value == 0)
        {
            songNotes = songNotesA;
            noteDurations = noteDurationsA;
        }
        else if (songDropdown.value == 1)
        {
            songNotes = songNotesB;
            noteDurations = noteDurationsB;
        }
        else
        {
            Debug.Log("Invalid song selected");
            return;
        }
     

    }
    // Start is called before the first frame update
    void Start()
    {
        // grab the left‐hand tracked device when we start
        //_leftHand = InputDevices.GetDeviceAtXRNode(XRNode.LeftHand);
        TryInitializeLeftHand();
        //GameObject newNote = Instantiate(notes[songNotes[currentNoteIndex] - 1].notePrefab, notes[songNotes[currentNoteIndex] - 1].spawnPoint.position, notes[songNotes[currentNoteIndex] - 1].spawnPoint.rotation);
        //newNote.SetActive(true);
        //spawnedNotes.Add(newNote);

        songNotes = songNotesA;
        noteDurations = noteDurationsA;
    }

    // Update is called once per frame
    void Update()
    {
        //noteSpeed = speedSlider.value; // Update the note speed based on the slider value

        time += Time.deltaTime;

        foreach (GameObject note in spawnedNotes)
        {
            // Move the note down the screen
            note.transform.Translate(Vector3.down * Time.deltaTime);
            //note.transform.position += Vector3.down * Time.deltaTime;
            // Check if the note has gone off the screen
            /*if (note.transform.position.y < 10f)
            {
                Destroy(note);
                spawnedNotes.Remove(note);
            }*/
        }

        if (time > nextNoteTime)
        {
            //Debug.Log("Time: " + time);
            //Debug.Log("Next note time: " + nextNoteTime);
            if (currentNoteIndex >= songNotes.Length)
            {
                // All notes have been spawned
                //spawnedNotes.Clear();
                return;
            }
           
            // Spawn the next note
            GameObject newNote = Instantiate(notes[songNotes[currentNoteIndex] - 1].notePrefab, notes[songNotes[currentNoteIndex] - 1].spawnPoint.position, notes[songNotes[currentNoteIndex] - 1].spawnPoint.rotation);
            newNote.SetActive(true);
            //Adjust size of note
            // 1) Grab the only child
            Transform child = newNote.transform.GetChild(0);

            // 2) Detach it, preserving world transform
            child.SetParent(null, worldPositionStays: true);

            

            //spawnedNotes.Add(child.gameObject);

            // 3) Scale the parent
            newNote.transform.localScale = new Vector3(newNote.transform.localScale.x, noteDurations[currentNoteIndex] / noteSpeed * .95f, newNote.transform.localScale.z ); // newNote.transform.parent.lossyScale.z);

            // 4) Re-attach the child, preserving its world transform again
            //child.SetParent(newNote.transform, worldPositionStays: true);

            float halfLength = noteDurations[currentNoteIndex] / noteSpeed * 0.5f;
            //newNote.transform.localPosition = new Vector3(newNote.transform.position.x, 4 + halfLength, newNote.transform.position.z);
            //newNote.transform.position += Vector3.up * halfLength;
            newNote.transform.Translate(Vector3.up * halfLength);

            child.position = new Vector3(newNote.transform.position.x, newNote.transform.position.y, newNote.transform.position.z);
            child.Translate(Vector3.back * .02f);
            spawnedNotes.Add(child.gameObject);


            
            spawnedNotes.Add(newNote);

            nextNoteTime += noteDurations[currentNoteIndex] / noteSpeed;
            //Debug.Log("Time Added: " + noteDurations[currentNoteIndex] / noteSpeed);
            currentNoteIndex++;

        }


    }

    private void TryInitializeLeftHand()
    {
        var devices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.LeftHand, devices);
        if (devices.Count > 0)
        {
            _leftHand = devices[0];
            Debug.Log($" Left-hand device found: {_leftHand.name}");
        }
        else
        {
            Debug.LogWarning(" Left-hand XRNode device not found");
        }
    }

    public void SetSpeed(float newValue)
    {
        noteSpeed = newValue;
        Debug.Log("Speed set to: " + noteSpeed);
    }
}
