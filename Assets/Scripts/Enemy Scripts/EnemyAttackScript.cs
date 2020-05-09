using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackScript : MonoBehaviour
{
    private LayerMask layerMask;
    private EnemyController enemyControler;

    private void Awake()
    {
        enemyControler = GetComponentInParent<EnemyController>();

        layerMask = LayerMask.GetMask(LayerTags.PLAYER, LayerTags.SHIELD);
    }

    void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, enemyControler.radius, layerMask);

        if (hits.Length > 0)
        {
            if (hits[0].gameObject.layer == 12) //Shield
            {
                //User blocked with shield ! Play Shield Sound
                hits[0].gameObject.GetComponentInParent<PlayerShieldSound>().PlayBlockedSound();
            }
            else
            {
                hits[0].gameObject.GetComponent<HealthScript>().ApplyDamage(enemyControler.damage);
            }

            gameObject.SetActive(false);
        }
    }
}
