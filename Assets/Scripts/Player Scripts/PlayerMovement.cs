using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private CharacterController characterController;

    [SerializeField]
    Joystick leftJoystick;

    private Vector3 moveDirection;
    
    public float speed = 5f;
    private float gravity = 20f;

    public float jumpForce = 10f;
    private float verticalVelocity;

    private float currentWalkVelocity;

    private bool isJumpButtonPressed;

    void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MoveThePlayer();
    }

    void MoveThePlayer()
    {
        //When A or D is pressed, Input.GetAxis("Horizontal") will return a positive or negative value every update frame.
        float x = Input.GetAxis(TagHolder.HORIZONTAL) + leftJoystick.Horizontal;

        //When W or S is pressed, Input.GetAxis("Vertical") will return a positive or negative value every update frame.
        float z = Input.GetAxis(TagHolder.VERTICAL) + leftJoystick.Vertical;

        moveDirection = new Vector3(x, 0f, z);

        //Normqlize vector to hqve 1 or 0 values. Avoid to walk slowly if user use joystick
        moveDirection.Normalize();

        //Transform from local space to world space
        moveDirection = transform.TransformDirection(moveDirection);

        //This function is called on every Update. Update is called on every Frame Update of the game. Time.deltaTime is the time between each frame.
        moveDirection *= speed * Time.deltaTime;

        currentWalkVelocity = Mathf.Max(Mathf.Abs(moveDirection.x), Mathf.Abs(moveDirection.z));

        ApplyGravity();

        //Use unity Character Controller Component built-in function to move our character
        characterController.Move(moveDirection);
    }

    void ApplyGravity()
    {
        //If we don't do this, character will never go back to ground (we would never add negative value to Y axis)
        verticalVelocity -= gravity * Time.deltaTime;
        
        JumpIfNeeded();

        moveDirection.y = verticalVelocity * Time.deltaTime;
    }

    void JumpIfNeeded()
    {
        if (characterController.isGrounded && (Input.GetKeyDown(KeyCode.Space) || isJumpButtonPressed))
        {
            isJumpButtonPressed = false;
            verticalVelocity = jumpForce;
        }
    }

    public float GetCurrentWalkVelocity()
    {
        return currentWalkVelocity;
    }

    public void OnJumpButtonClicked()
    {
        isJumpButtonPressed = true;
    }
}
