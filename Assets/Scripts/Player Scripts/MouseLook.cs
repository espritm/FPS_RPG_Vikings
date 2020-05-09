using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseLook : MonoBehaviour
{
    [SerializeField]
    private Transform playerRoot, lookRoot;

    [SerializeField]
    private FixedTouchField touchToRotateCameraPanel;

    [SerializeField]
    private bool invert;

    [SerializeField]
    private bool canUnlock = true;

    [SerializeField]
    private float sensivity = 5f;

    [SerializeField]
    private float mobileSensivity = 0.5f;

    [SerializeField]
    private int smoothSteps = 10;

    [SerializeField]
    private float smoothWeight = 0.4f;

    [SerializeField]
    private float rollAngle = 10f;

    [SerializeField]
    private float rollSpeed = 3f;

    [SerializeField]
    private Vector2 defaultLookLimits = new Vector2(-70f, 80f);

    private Vector2 lookAngles;
    private Vector2 currentMouseLook;
    private Vector2 smoothMove;
    private float currentRollAngle;
    private int lastLookFrame;

    private bool bUseMobileGUI;

    // Start is called before the first frame update
    void Start()
    {
        bUseMobileGUI = PlatformHelper.ShouldUseMobileGUI();

        if (bUseMobileGUI)
        {
            sensivity = mobileSensivity;
        }
        else
        {
            //Lock the mouse cursor to the center of the screen and hide it.
            //Dont do it on mobile device because it blocks the virtual joystick !!.....
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // Update is called once per frame
    void Update()
    {
        LockAndUnlockCursor();

        //If mobile app : no mouse so no lock state. Else we are on computer, there is a mouse and we need a lock state.
        //If not locked, user is not playing but is in the menu. So we should not moove the player's camera
        if (bUseMobileGUI)
            LookAround();
        else
            if (Cursor.lockState == CursorLockMode.Locked)
                LookAround();
    }

    void LockAndUnlockCursor()
    {
        //For instance, if we press Escape, display a Menu so we need to see the cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void LookAround()
    {
        //When we moove mouse horizontaly, this is X axis. And verticaly is Y axis.
        //So if user moove the mouse horizontaly, that means in the world space we want to rotate the camera around Y axis.
        //And if user moove the mouse verticaly, that means in the world space we want to rotate the camera around X axis.
        if (bUseMobileGUI)
            currentMouseLook = new Vector2(touchToRotateCameraPanel.TouchDist.y, touchToRotateCameraPanel.TouchDist.x);
        else
            currentMouseLook = new Vector2(Input.GetAxis(MouseAxis.MOUSEY), Input.GetAxis(MouseAxis.MOUSEX));

        //Invert is only for mouse's Y axis.
        lookAngles.x += currentMouseLook.x * sensivity * (invert ? 1f : -1f);
        lookAngles.y += currentMouseLook.y * sensivity;

        //Maths.Camp(A, b, c) means retourne A si b < A < c, sinon si A < b retourne b, sinon si c < A retourne c.
        lookAngles.x = Mathf.Clamp(lookAngles.x, defaultLookLimits.x, defaultLookLimits.y);

        //Mathf.Lerp(A, B, t) means go from value A to value B in a t time.
        currentRollAngle = Mathf.Lerp(currentRollAngle, Input.GetAxisRaw(MouseAxis.MOUSEX) * rollAngle, Time.deltaTime * rollSpeed);

        //Apply changement to gameobjects
        lookRoot.localRotation = Quaternion.Euler(lookAngles.x, 0f, currentRollAngle);
        playerRoot.localRotation = Quaternion.Euler(0f, lookAngles.y, 0f);
    }
}
