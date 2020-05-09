using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAttack : MonoBehaviour
{
    private WeaponManager weaponManager;
    private PlayerSprintAndCrouch playerSprintAndCrouch;

    private bool isAttackButtonDown;
    private bool isAimButtonPressed;

    private float nextTimeToFire;

    private Animator zoomCameraAnim;
    private bool zoomed;

    private GameObject crosshair;

    private bool isAiming;
    private bool isAimingShield;

    private bool bUseMobileGUI;

    void Awake()
    {
        weaponManager = GetComponent<WeaponManager>();
        playerSprintAndCrouch = GetComponent<PlayerSprintAndCrouch>();
        zoomCameraAnim = transform.Find(Tags.LOOK_ROOT).transform.Find(Tags.ZOOM_CAMERA).GetComponent<Animator>();

        crosshair = GameObject.FindWithTag(Tags.CROSSHAIR);
    }

    // Start is called before the first frame update
    void Start()
    {
        bUseMobileGUI = PlatformHelper.ShouldUseMobileGUI();
    }

    // Update is called once per frame
    void Update()
    {
        WeaponShoot();
        ZoomInAndOut();
    }

    void WeaponShoot()
    {
        if (isAimingShield)
            return;

        //If we have assault riffle, user can hold attack button to shoot. Else, user must click once per shoot
        if (weaponManager.GetCurrentSelectedWeapon().fireType == WeaponFireType.MULTIPLE)
        {
            //If we press and hold left mouse button or attack UI Button AND if Time is great than the nextTimeToFire
            if (((!bUseMobileGUI && Input.GetMouseButton(0)) || isAttackButtonDown) && Time.time > nextTimeToFire)
            {
                //Reset timer for firerate
                nextTimeToFire = Time.time + 1f / weaponManager.GetCurrentSelectedWeapon().GetFirerate();

                //Fire !
                weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
            }
        }
        else
        {
            //If we press once the left mouse button
            if (((!bUseMobileGUI && Input.GetMouseButtonDown(0)) || isAttackButtonDown))
            {
                //Force release attack button
                isAttackButtonDown = false;

                //If Arrow, verify user is aiming before shouting.
                //Else, shoot.

                if (weaponManager.GetCurrentSelectedWeapon().bulletType == WeaponBulletType.ARROW)
                {
                    if (isAiming)
                        weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                }
                else
                {
                    //Handle Axe or bullet shoot
                    weaponManager.GetCurrentSelectedWeapon().ShootAnimation();
                }
            }
        }
    }
    
    void ZoomInAndOut()
    {
        //If current weapon can aim
        if (weaponManager.GetCurrentSelectedWeapon().weaponAim == WeaponAim.AIM)
        {
            //If we press and hold right mouse button or if user has pressed the Aim UI Button
            if (((!bUseMobileGUI && Input.GetMouseButtonDown(1)) || (isAimButtonPressed && crosshair.activeSelf) && !isAimingShield))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_IN_ANIM);

                crosshair.SetActive(false);

                //Force player to walk
                playerSprintAndCrouch.ForceWalk();

                isAimButtonPressed = false;
            }

            //If we release right mouse button
            if ((!bUseMobileGUI && Input.GetMouseButtonUp(1)) || (isAimButtonPressed && !crosshair.activeSelf))
            {
                zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);

                crosshair.SetActive(true);

                if (!isAimingShield)
                    playerSprintAndCrouch.ReleaseForceWalk();

                isAimButtonPressed = false;
            }
        }

        //If current weapon if Bow or spear
        if (weaponManager.GetCurrentSelectedWeapon().weaponAim == WeaponAim.SELF_AIM)
        {
            if (((!bUseMobileGUI && Input.GetMouseButtonDown(1)) || (isAimButtonPressed && !isAiming) && !isAimingShield))
            {
                weaponManager.GetCurrentSelectedWeapon().Aim(true);
                isAiming = true;
                playerSprintAndCrouch.ForceWalk();
                isAimButtonPressed = false;
            }

            if ((!bUseMobileGUI && Input.GetMouseButtonUp(1)) || (isAimButtonPressed && isAiming))
            {
                weaponManager.GetCurrentSelectedWeapon().Aim(false);
                isAiming = false;
                if (!isAimingShield)
                    playerSprintAndCrouch.ReleaseForceWalk();
                isAimButtonPressed = false;
            }
        }
    }

    public void OnFireButtonDown()
    {
        isAttackButtonDown = true;
    }

    public void OnFireButtonUp()
    {
        isAttackButtonDown = false;
    }

    public void OnAimButtonPressed()
    {
        isAimButtonPressed = true;
    }

    public void ForceUnaimWeapon()
    {
        //if current weapon was aiming, unaim.
        if (!crosshair.activeSelf)
        {
            zoomCameraAnim.Play(AnimationTags.ZOOM_OUT_ANIM);

            crosshair.SetActive(true);
        }

        //If current weapon is Self_Aim type
        if (isAiming)
        {
            weaponManager.GetCurrentSelectedWeapon().Aim(false);
            isAiming = false;
        }
    }

    public void OnAimAndFireButtonDown()
    {
        isAttackButtonDown = true;
        isAimButtonPressed = true;
    }

    public void OnAimAndFireButtonUp()
    {
        isAttackButtonDown = false;
        isAimButtonPressed = true;
    }

    public void OnAimShieldButtonDown()
    {
        isAimingShield = true;

        //Stop aiming weapon..not working
        if (isAiming)
            isAimButtonPressed = true;

        weaponManager.GetCurrentSelectedShield()?.AimShieldAnimation();

        playerSprintAndCrouch.ForceWalk();
    }

    public void OnAimShieldButtonUp()
    {
        isAimingShield = false;

        weaponManager.GetCurrentSelectedShield()?.StopAimShieldAnimation();

        playerSprintAndCrouch.ReleaseForceWalk();
    }
}
