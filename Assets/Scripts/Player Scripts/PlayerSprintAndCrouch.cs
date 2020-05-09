using System;
using UnityEngine;

public class PlayerSprintAndCrouch : MonoBehaviour
{
    private PlayerMovement playerMovement;

    public float sprintSpeed = 10f;
    public float moveSpeed = 5f;
    public float crouchSpeed = 2f;

    private Transform lookRoot;
    private float standHeight = 1.6f;
    private float crouchHeight = 1f;

    private bool isCrouching;
    private bool isSprinting;
    private bool isForcedToWalk;

    private bool isCrouchButtonPressed;
    private bool isSprintButtonPressed;

    private PlayerFootSteps playerFootSteps;

    private float sprintVolume = 1f;
    private float crouchVolume = 0.1f;
    private float walkVolumeMin = 0.2f, walkVolumeMax = 0.6f;

    private float walkStepDistance = 0.5f;
    private float sprintStepDistance = 0.2f;
    private float crouchStepDistance = 0.75f;

    private PlayerStats playerStats;

    private float sprintValue = 100f;
    public float sprintTreshold = 10f;

    private CharacterController characterController;

    private PlayerAudio playerAudio;

    void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();

        //Transform is our Player gameobject. His first child is our lookRoot gameobject. We access to it using GetChild(0);
        lookRoot = transform.GetChild(0);

        playerFootSteps = GetComponentInChildren<PlayerFootSteps>();

        playerStats = GetComponent<PlayerStats>();

        characterController = GetComponentInParent<CharacterController>();

        playerAudio = GetComponentInChildren<PlayerAudio>();
    }

    void Start()
    {
        playerFootSteps.volumeMin = walkVolumeMin;
        playerFootSteps.volumeMax = walkVolumeMax;
        playerFootSteps.stepDistance = walkStepDistance;
    }


    void Update()
    {
        Sprint();
        Crouch();
    }

    void Sprint()
    {
        //If we have stamina, we can sprint
        if (sprintValue > 0f)
        {
            //If we are crouching, we are not able to sprint !
            if ((Input.GetKeyDown(KeyCode.LeftShift) || (isSprintButtonPressed && !isSprinting)) && !isCrouching && !isForcedToWalk)
            {
                isSprintButtonPressed = false;

                //Start sprinting
                StartSprint();
            }
        }

        //Decrease or increase sprint stamina 
        ManageStaminaRegeneration();

        //If player stop sprint OR if stamina is <= 0, stop sprint
        if (((Input.GetKeyUp(KeyCode.LeftShift) || isSprintButtonPressed && isSprinting) && !isCrouching) || sprintValue <= 0f)
        {
            isSprintButtonPressed = false;

            //Stop sprinting
            StopSprint();
        }
    }

    void Crouch()
    {
        //If we press C
        if (Input.GetKeyDown(KeyCode.C) || isCrouchButtonPressed)
        {
            isCrouchButtonPressed = false;

            if (isCrouching)
            {
                //Need to stand up

                //Reset camera position / footstep volume and distance, and if not walking reset player movement speed
                StopCrouch();
            }
            else
            {
                //Need to crouch

                //Moove camera position / player movement speed / footstep volume and distance
                StartCrouch();
            }
        }
    }

    void ManageStaminaRegeneration()
    {
        if (isSprinting && characterController.velocity.sqrMagnitude > 0)
        {
            //Decrease stamina
            sprintValue -= sprintTreshold * Time.deltaTime;
            if (sprintValue <= 0)
                sprintValue = 0;

            //Update UI
            playerStats.DisplayStaminaStats(sprintValue);
        }
        else if (sprintValue < 100f)
        {
            //Increase stamina andupdate UI
            sprintValue += (sprintTreshold / 2f) * Time.deltaTime;
            if (sprintValue >= 100f)
                sprintValue = 100f;

            //Update UI
            playerStats.DisplayStaminaStats(sprintValue);
        }

        //Play sound if user is low stamina (breath)
        if (sprintValue <= 45 && !playerAudio.IsPlaying())
            playerAudio.PlayLowStaminaSound();
    }

    void StartSprint()
    {
        isSprinting = true;
        playerMovement.speed = sprintSpeed;

        playerFootSteps.stepDistance = sprintStepDistance;
        playerFootSteps.volumeMin = sprintVolume;
        playerFootSteps.volumeMax = sprintVolume;
    }

    void StopSprint()
    {
        isSprinting = false;
        playerMovement.speed = moveSpeed;

        playerFootSteps.stepDistance = walkStepDistance;
        playerFootSteps.volumeMin = walkVolumeMin;
        playerFootSteps.volumeMax = walkVolumeMax;
    }

    void StartCrouch()
    {
        isCrouching = true;

        lookRoot.localPosition = new Vector3(0f, crouchHeight, 0f);

        playerFootSteps.stepDistance = crouchStepDistance;
        playerFootSteps.volumeMin = crouchVolume;
        playerFootSteps.volumeMax = crouchVolume;

        playerMovement.speed = crouchSpeed;
    }

    void StopCrouch()
    {
        isCrouching = false;

        //Reset camera position
        lookRoot.localPosition = new Vector3(0f, standHeight, 0f);
        
        //Reset footstep volume
        playerFootSteps.volumeMin = walkVolumeMin;
        playerFootSteps.volumeMax = walkVolumeMax;

        //Reset movement speed if player is not forced to walk and footstep distance
        if (!isForcedToWalk)
        {
            playerMovement.speed = moveSpeed;

            //Reset footstep distance 
            playerFootSteps.stepDistance = walkStepDistance;
        }
    }

    //For instance when aiming.
    public void ForceWalk()
    {
        isForcedToWalk = true;
        
        if (isSprinting)
            StopSprint();

        playerMovement.speed = crouchSpeed;
        playerFootSteps.stepDistance = crouchStepDistance;
    }

    public void ReleaseForceWalk()
    {
        isForcedToWalk = false;

        if (isCrouching)
        {
            ;//nothing to do
        }
        else
        {
            playerMovement.speed = moveSpeed;
            playerFootSteps.stepDistance = walkStepDistance;
        }
    }

    public void OnCrouchButtonPressed()
    {
        isCrouchButtonPressed = true;
    }

    public void OnSprintButtonPressed()
    {
        isSprintButtonPressed = true;
    }
}
