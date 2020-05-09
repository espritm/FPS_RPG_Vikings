using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HealthScript : MonoBehaviour
{
    private EnemyAnimator enemyAnimator;
    private NavMeshAgent navMeshAgent;
    private EnemyController enemyController;

    public float health = 100f;

    public float timeBeforeDeadEnemyDisappear = 20f;

    public bool isPlayer/*, isBoar, isCannibal, isSkeleton*/;
    public EnemyType enemyType;

    private bool isDead;

    private EnemyAudio enemyAudio;
    private PlayerAudio playerAudio;
    private PlayerAnimator playerAnimator;
    private PlayerStats playerStats;

    void Awake()
    {
        if (enemyType == EnemyType.NONE)
        {
            //If player
            playerStats = GetComponent<PlayerStats>();
            playerAudio = GetComponentInChildren<PlayerAudio>();
            playerAnimator = GetComponent<PlayerAnimator>();
        }
        else
        {
            //if enemy
            enemyAnimator = GetComponent<EnemyAnimator>();
            enemyController = GetComponent<EnemyController>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            enemyAudio = GetComponentInChildren<EnemyAudio>();
        }
    }


    public void ApplyDamage(float damage)
    {
        if (isDead)
            return;

        health -= damage;

        if (enemyType == EnemyType.NONE)
        {
            //If player is hitten, Update UI
            playerStats.DisplayHealthStats(health);

            playerAudio.PlayInjuredSound();
        }
        else
        {
            enemyAudio.PlayInjuredSound();

            //If player hits the enemy, the enemy must run and attack to the player
            if (enemyController.EnemyState == EnemyState.PATROL)
            {
                enemyController.chaseDistance = 5000f;
            }
        }

        if (health <= 0f)
        {
            PlayerDied();

            isDead = true;
        }
    }

    private void PlayerDied()
    {
        //If player died
        if (enemyType == EnemyType.NONE)
        {
            //Player Dead sound + Game Over sound
            StartCoroutine("PlayerGameOverSound");

            //Animate camera to die
            playerAnimator.StartDeadAnimation();

            //Make the enemies ignore the died player.
            List<GameObject> enemies = LayerHelper.FindGameObjectInLayer(Tags.ENEMY_TAG);

            for (int i = 0; i < enemies.Count; i++)
            {
                EnemyController enemyCtrl = enemies[i].GetComponent<EnemyController>();
                if (enemyCtrl != null)
                    enemyCtrl.enabled = false;
            }

            //Call enemy manager to stop spawning enemies
            EnemyManager.instance.StopSpawning();

            GetComponent<PlayerMovement>().enabled = false;
            GetComponent<PlayerAttack>().enabled = false;
            GetComponent<WeaponManager>().GetCurrentSelectedWeapon().gameObject.SetActive(false);
        }

        else //if enemy died
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.velocity = Vector3.zero;
            enemyController.enabled = false;

            enemyAnimator.Dead();

            //StartCoroutine
            StartCoroutine(EnemyDeadSound());

            //Enemy Manager spawn more enemies
            EnemyManager.instance.EnemyDied(enemyType);
        }

        if (tag == Tags.PLAYER_TAG)
            Invoke("RestartGame", 9f);
        else
            Invoke("TurnOffGameObject", timeBeforeDeadEnemyDisappear);
    }

    void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }

    void TurnOffGameObject()
    {
        gameObject.SetActive(false);
    }

    IEnumerator EnemyDeadSound()
    {
        yield return new WaitForSeconds(0.3f);
        enemyAudio.PlayDeadSound();
    }

    IEnumerator PlayerGameOverSound()
    {
        yield return new WaitForSeconds(0.3f);
        playerAudio.PlayGameOverSound();
    }
}
