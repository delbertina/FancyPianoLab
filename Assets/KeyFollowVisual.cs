using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(AudioSource))]
public class KeyFollowVisual : MonoBehaviour
{
    public Transform visualTarget;
    public Vector3 localAxis;
    public float resetSpeed = 5;
    public AudioClip noteSound;
    public float semitones;
    //public float followAngleThreshold = 180;

    private AudioSource noteAudioSource;
    private bool freeze = false;

    private Vector3 initialLocalPos;

    private Vector3 offset;
    private Transform pokeAttachTransform;

    private XRBaseInteractable interactable;
    private bool isFollowing = false;

    private static float pitchScale = Mathf.Pow(2f, 1.0f / 12f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        noteAudioSource = GetComponent<AudioSource>();
        noteAudioSource.pitch = Mathf.Pow(pitchScale, semitones);
        initialLocalPos = visualTarget.localPosition;

        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Follow);
        interactable.hoverExited.AddListener(Reset);
        interactable.selectEntered.AddListener(Freeze);
    }

    public void Follow(BaseInteractionEventArgs hover)
    {
        if (hover.interactorObject is XRPokeInteractor) {
            XRPokeInteractor interactor = (XRPokeInteractor)hover.interactorObject;

            Vector3 pokeDirection = (visualTarget.position - interactor.attachTransform.position).normalized;

            // Convert pokeDirection to local space of the visual target
            Vector3 localPokeDirection = visualTarget.InverseTransformDirection(pokeDirection);

            // Check if the poke is coming from the correct side (same direction as localAxis)
            float dot = Vector3.Dot(localPokeDirection, localAxis.normalized);
            if (dot < 0) // Poke is coming from the wrong direction (e.g. from below)
            {
                isFollowing = false;
                freeze = true;
                return;
            }

            isFollowing = true;
            freeze = false;

            pokeAttachTransform = interactor.attachTransform;
            offset = visualTarget.position - pokeAttachTransform.position;

            // float pokeAngle = Vector3.Angle(offset, visualTarget.TransformDirection(localAxis));

            // not really working, don't quite understand how to fix the issue with touch under
            //if(pokeAngle > followAngleThreshold)
            // {
            //    isFollowing = false;
            //     freeze = true;
            // }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (freeze)
        {
            return;
        }
        if (isFollowing)
        {
            Vector3 localTargetPosition = visualTarget.InverseTransformPoint(pokeAttachTransform.position + offset);
            Vector3 constrainedLocalTargetPosition = Vector3.Project(localTargetPosition, localAxis);

            visualTarget.position = visualTarget.TransformPoint(constrainedLocalTargetPosition);
        }
        else
        {
            visualTarget.localPosition = Vector3.Lerp(visualTarget.localPosition, initialLocalPos, Time.deltaTime * resetSpeed);
        }
    }

    public void Reset(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            isFollowing = false;
            freeze = false;
        }
    }

    public void Freeze(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor)
        {
            freeze = true;
            noteAudioSource.PlayOneShot(noteSound);
        }
    }
}
