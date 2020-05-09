using UnityEngine;
using UnityEngine.AI;

//The enemy is going to patrol to search the player and attack him. We list differents states here.
public enum EnemyState
{
    PATROL,
    CHASE,
    ATTACK
}

public class EnemyController : MonoBehaviour
{
    private EnemyAnimator enemyAnimator;
    private NavMeshAgent navMeshAgent;
    private BoxCollider boxCollider;

    public EnemyState EnemyState { get; set; }
    private EnemyState enemyState;

    public float walkSpeed = 0.5f;
    public float RunSpeed = 4f;

    public float chaseDistance = 7f;
    private float currentChaseDistance;
    public float attackDistance = 1.8f;
    public float chaseAfterAttackDistance = 2f;

    public float patrolRadiusMin = 20f, patrolRadiusMax = 60f;
    public float patrolForThisTime = 15f;
    private float patrolTimer;

    public float waitBeforeAttack = 2f; //Attack speed
    private float attackTimer;

    private Transform target;

    public GameObject attackPoint;
    public float damage = 2f;
    public float radius = 1f;

    private EnemyAudio enemyAudio;

    void Awake()
    {
        enemyAnimator = GetComponent<EnemyAnimator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        boxCollider = GetComponent<BoxCollider>();

        target = GameObject.FindWithTag(Tags.PLAYER_TAG).transform;

        enemyAudio = GetComponentInChildren<EnemyAudio>();
    }

    void Start()
    {
        enemyState = EnemyState.PATROL;

        patrolTimer = patrolForThisTime;

        //When the enemy first gets to the player, attack right away
        attackTimer = waitBeforeAttack;

        //Memorise the value of chase distance so that we can put it back
        currentChaseDistance = chaseDistance;
    }

    void Update()
    {
        if (enemyState == EnemyState.PATROL)
        {
            Patrol();
        }

        if (enemyState == EnemyState.CHASE)
        {
            Chase();
        }

        if (enemyState == EnemyState.ATTACK)
        {
            Attack();
        }
    }

    private void Attack()
    {
        //Stop the enemy
        navMeshAgent.velocity = Vector3.zero;
        navMeshAgent.isStopped = true;

        attackTimer += Time.deltaTime;

        if (attackTimer > waitBeforeAttack)
        {
            enemyAnimator.Attack();

            attackTimer = 0;

            //Play attack sound
            enemyAudio.PlayAttackSound();
        }

        //Make the enemy looking at the player at all time while attacking
        transform.LookAt(target.position);

        if (Vector3.Distance(transform.position, target.position) > attackDistance + chaseAfterAttackDistance)
        {
            enemyState = EnemyState.CHASE;
        }
    }

    private void Chase()
    {
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = RunSpeed;

        //Run toward the player. Use the AI 
        navMeshAgent.SetDestination(target.position);

        //If enemy is mooving
        if (navMeshAgent.velocity.sqrMagnitude > 0)
            enemyAnimator.SetRun(true);
        else
            enemyAnimator.SetRun(false);
            
        //Verify if the enemy can attack. To attack, the enemy should be near the player
        if(Vector3.Distance(transform.position, target.position) <= attackDistance)
        {
            enemyAnimator.SetRun(false);
            enemyAnimator.SetWalk(false);
            enemyState = EnemyState.ATTACK;

            //Reset the chase distance if needed
            if (chaseDistance != currentChaseDistance)
                chaseDistance = currentChaseDistance;
        }
        else if (Vector3.Distance(transform.position, target.position) > chaseDistance) 
        {
            //if player run away from the enemy

            //Patrol again
            enemyAnimator.SetRun(false);
            enemyState = EnemyState.PATROL;
            patrolTimer = patrolForThisTime;

            //Reset the chase distance if needed
            if (chaseDistance != currentChaseDistance)
                chaseDistance = currentChaseDistance;
        }
    }

    private void Patrol()
    {
        //Tell nav agent that he can move
        navMeshAgent.isStopped = false;
        navMeshAgent.speed = walkSpeed;

        patrolTimer += Time.deltaTime;

        if (patrolTimer > patrolForThisTime)
        {
            SetNewRandomDestination();

            patrolTimer = 0;
        }

        //If enemy is mooving
        if (navMeshAgent.velocity.sqrMagnitude > 0)
            enemyAnimator.SetWalk(true);
        else
            enemyAnimator.SetWalk(false);

        //Verify if the enemy should attack the player
        if (Vector3.Distance(transform.position, target.position) <= chaseDistance)
        {
            enemyAnimator.SetWalk(false);

            enemyState = EnemyState.CHASE;

            //Play spotted audio
            enemyAudio.PlayScreamSound();
        }
    }

    private void SetNewRandomDestination()
    {
        //Calculate random values for the new destinations
        float randRadius = Random.Range(patrolRadiusMin, patrolRadiusMax);
        Vector3 randDir = Random.insideUnitSphere * randRadius;
        randDir += transform.position;

        //Use the AI to get a new destination with the random values generated below.
        //The AI verify if the new destination if "walkable" before returning the new destination. (see in unity window > ai > Navigation and select "terrain") 
        NavMeshHit navHit;
        NavMesh.SamplePosition(randDir, out navHit, randRadius, -1); //-1 means "include all Layers"

        //Navigate to the new walkable destination
        navMeshAgent.SetDestination(navHit.position);
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

    void DeactivateBoxCollider()
    {
        boxCollider.enabled = false;
    }
}
