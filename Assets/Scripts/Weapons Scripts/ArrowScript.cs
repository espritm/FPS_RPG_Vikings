using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private WeaponManager weaponManager;

    private Rigidbody myRigidBody;

    public float speed = 30f;

    private float lifeTimer;
    public float deactivate_Timer = 60f;

    public Vector3 rotationToApply = Vector3.zero;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody>();
        weaponManager = FindObjectOfType<WeaponManager>();
    }

    void Start()
    {
        //Invoke this function in a given time
        Invoke("DeactivateGameObject", deactivate_Timer);
    }

    public void Launch(Camera mainCamera)
    {
        //Move toward the crosshair direction thanks to mainCamera !
        myRigidBody.velocity = mainCamera.transform.forward * speed;

        //LookAt !! make the object looking at the right direction.
        //For instance, arrow is oriented allong the Z axis. Thanks to LookAt, the arrow won't be oriented along Z axis but along the axis between the Player and the Crosshair !!
        transform.LookAt(transform.position + myRigidBody.velocity + rotationToApply);
    }

    void DeactivateGameObject()
    {
        if (gameObject.activeInHierarchy)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider target)
    {
        //After we tuch an enemy deactivate game object
        if (target.gameObject.layer == LayerMask.NameToLayer(Tags.ENEMY_TAG))
        {
            target.GetComponent<HealthScript>().ApplyDamage(weaponManager.GetCurrentSelectedWeapon().GetDamage());

            StickArrow(target);
        }
    }

    void StickArrow(Collider target)
    {
        //Make the arrow children of the target, then freeze it in the targer.
        Transform parent = FindArrowStickTargetInChildren(target.transform);

        if (parent == null)
        {
            //Should not happen, it means prefab has not been configured correctly. Need to tag the Hips gameobject in the prefabs (gameobject parent of the animated skeleton of the 3D object) with "Arrow Stick Target" tag.
            Invoke("DeactivateGameObject", 3f);
        }
        else
        { 
            transform.parent = parent.transform;

            //Freeze the arrow at the current position in the enemy body (or in a wall or a tree etc...)
            myRigidBody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    Transform FindArrowStickTargetInChildren(Transform transform)
    {
        for (int i = 0; i < transform?.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child?.tag == Tags.ARROW_STICK_TARGET)
                return child;
            else
            {
                Transform res = FindArrowStickTargetInChildren(child);
                if (res != null)
                    return res;
            }
        }

        return null;
    }
}
