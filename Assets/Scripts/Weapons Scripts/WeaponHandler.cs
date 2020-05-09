using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponAim
{
    NONE, 
    SELF_AIM,
    AIM
}

public enum WeaponFireType
{
    SINGLE,
    MULTIPLE
}

public enum WeaponBulletType
{
    BULLET,
    ARROW,
    NONE
}

public class WeaponHandler : MonoBehaviour
{
    private Animator anim;
    private WeaponManager weaponManager;

    public WeaponAim weaponAim;

    [SerializeField]
    private GameObject muzzleFlash;

    [SerializeField]
    private AudioSource shootSound, reloadSound1, reloadSound2;

    public WeaponFireType fireType;

    public WeaponBulletType bulletType;

    [SerializeField]
    private GameObject bulletPrefab;

    [SerializeField]
    private Transform bulletStartPosition;

    public GameObject attackPoint;

    public Sprite weaponFireSprite;
    public Sprite weaponAimAndFireSprite;

    public float firerate = 15f;
    public float damage = 15f;
    public float radius = 1f;

    private Camera mainCam;

    void Awake()
    {
        anim = GetComponent<Animator>();

        mainCam = Camera.main;

        weaponManager = GetComponentInParent<WeaponManager>();
    }

    public void ShootAnimation()
    {
        anim.SetTrigger(AnimationTags.SHOOT_TRIGGER);
    }

    public void Aim(bool canAim)
    {
        anim.SetBool(AnimationTags.AIM_PARAMETER, canAim);
    }

    void TurnOnMuzzleFlash()
    {
        muzzleFlash.SetActive(true);
    }

    void TurnOffMuzzleFlash()
    {
        muzzleFlash.SetActive(false);
    }

    void PlayShootSound()
    {
        shootSound.Play();
    }

    void PlayReload1Sound()
    {
        reloadSound1.Play();
    }
    void PlayReload2Sound()
    {
        reloadSound2.Play();
    }

    //Called from Animation
    void TurnOnAttackPoint()
    {
        attackPoint.SetActive(true);
    }

    //Called from Animation
    void TurnOffAttackPoint()
    {
        if (attackPoint.activeInHierarchy)
            attackPoint.SetActive(false);
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetFirerate()
    {
        return firerate;
    }

    public float GetRadius()
    {
        return radius;
    }

    public void ThrowBullet()
    {
        GameObject arrow = Instantiate(bulletPrefab);

        arrow.transform.position = bulletStartPosition.position;
        arrow.transform.rotation = bulletStartPosition.rotation;

        arrow.GetComponent<ArrowScript>().Launch(mainCam);
    }

    public void ThrowRaycast()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit))
        {
            //if (hit.transform.tag == Tags.ENEMY_TAG)
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer(Tags.ENEMY_TAG))
            {
                hit.transform.GetComponent<HealthScript>().ApplyDamage(weaponManager.GetCurrentSelectedWeapon().GetDamage());
            }
        }
    }
}
