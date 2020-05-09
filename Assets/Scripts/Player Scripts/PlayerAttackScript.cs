using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackScript : MonoBehaviour
{
    private WeaponHandler weaponHandler;
    public LayerMask layerMask; //enemy

    private void Awake()
    {
        weaponHandler = GetComponentInParent<WeaponHandler>();
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, weaponHandler.GetRadius(), layerMask);

        if (hits.Length > 0)
        {
            hits[0].gameObject.GetComponent<HealthScript>().ApplyDamage(weaponHandler.GetDamage());

            gameObject.SetActive(false);
        }
    }
}
