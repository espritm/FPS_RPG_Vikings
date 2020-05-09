using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAnimationController : MonoBehaviour
{
    [SerializeField]
    bool IsMoving;

    Animator animationController;

    private void Awake()
    {
        animationController = GetComponent<Animator>();
    }

    private void Start()
    {
        SetShipMoving(IsMoving);
    }

    public void SetShipMoving(bool isMoving)
    {
        IsMoving = isMoving;

        animationController.SetBool(AnimationTags.IS_MOVING, IsMoving);
    }
}
