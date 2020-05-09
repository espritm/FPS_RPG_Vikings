using UnityEngine;

public class PlayerFootSteps : MonoBehaviour
{
    private AudioSource footStepSound;

    [SerializeField]
    private AudioClip[] footStepsClips;

    private CharacterController characterController;

    private PlayerMovement movementController;

    [HideInInspector]
    public float volumeMin, volumeMax;

    private float accumulatedDistance;

    [HideInInspector]
    public float stepDistance;


    void Awake()
    {
        footStepSound = GetComponent<AudioSource>();

        characterController = GetComponentInParent<CharacterController>();

        movementController = GetComponentInParent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckToPlayFootStepSound();
    }

    void CheckToPlayFootStepSound()
    {
        //If we are not on the ground, we are in the air, so no footstep soudn needed.
        if (!characterController.isGrounded)
            return;

        //If we are mooving
        if (characterController.velocity.sqrMagnitude > 0)
        {
            accumulatedDistance += Time.deltaTime;
            //accumulatedDistance += movementController.GetCurrentWalkVelocity();

            if (accumulatedDistance > stepDistance)
            {
                footStepSound.volume = Random.Range(volumeMin, volumeMax);
                footStepSound.clip = footStepsClips[Random.Range(0, footStepsClips.Length)]; //Range (int a, int b) : a is included but not b
                footStepSound.Play();

                accumulatedDistance = 0f;
            }
        }
        else
        {
            accumulatedDistance = 0;
        }
    }
}
