using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldHandler : MonoBehaviour
{
    private Animator anim;

    private PlayerShieldSound playerShieldSound;

    private BoxCollider blockZone;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerShieldSound = GetComponent<PlayerShieldSound>();
        blockZone = GetComponentInChildren<BoxCollider>();
    }

    public void AimShieldAnimation()
    {
        anim.SetBool(AnimationTags.AIM_PARAMETER, true);
    }

    public void StopAimShieldAnimation()
    {
        anim.SetBool(AnimationTags.AIM_PARAMETER, false);
    }
    
    public void ActivateBlockZone()
    {
        blockZone.enabled = true;
    }

    public void DeactivateBlockZone()
    {
        blockZone.enabled = false;
    }
}
