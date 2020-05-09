using System.Collections;
using UnityEngine;

public enum EnemyType
{
    NONE,
    BOAR,
    CANNIBAL,
    SKELETON,
    ZOMBIE
}

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager instance;

    [SerializeField]
    private GameObject boarPrefab, cannibalPrefab, skeletonPrefab, zombiePrefab;

    public Transform[] boarSpawnPoints, cannibalSpawnPoints, skeletonSpawnPoints, zombieSpawnPoints;

    private int boarEnemyCountToSpawn, cannibalEnemyCountToSpawn, skeletonEnemyCountToSpawn, zombieEnemyCountToSpawn;

    [SerializeField]
    private int boarCount, cannibalCount, skeletonCount, zombieCount;

    public float waitBeforeSpawnEnemies = 10f;

    private void MakeInstance()
    {
        if (instance == null)
            instance = this;
    }

    private void Awake()
    {
        MakeInstance();
    }

    // Start is called before the first frame update
    void Start()
    {
        boarEnemyCountToSpawn = boarCount;
        cannibalEnemyCountToSpawn = cannibalCount;
        skeletonEnemyCountToSpawn = skeletonCount;
        zombieEnemyCountToSpawn = zombieCount;

        SpawnEnemies();

        StartCoroutine("CheckToSpawnEnemies");
    }

    private void SpawnEnemies()
    {
        SpawnBoars();
        SpawnCannibals();
        SpawnSkeletons();
        SpawnZombies();
    }

    private void SpawnZombies()
    {
        for (int i = 0; i < zombieEnemyCountToSpawn; i++)
            Instantiate(zombiePrefab, zombieSpawnPoints[Random.Range(0, zombieSpawnPoints.Length - 1)].position, Quaternion.identity);

        zombieEnemyCountToSpawn = 0;
    }

    void SpawnBoars()
    {
        for (int i = 0; i < boarEnemyCountToSpawn; i++)
            Instantiate(boarPrefab, boarSpawnPoints[Random.Range(0, boarSpawnPoints.Length - 1)].position, Quaternion.identity);

        boarEnemyCountToSpawn = 0;
    }

    void SpawnCannibals()
    {
        for (int i = 0; i < cannibalEnemyCountToSpawn; i++)
            Instantiate(cannibalPrefab, cannibalSpawnPoints[Random.Range(0, cannibalSpawnPoints.Length - 1)].position, Quaternion.identity);

        cannibalEnemyCountToSpawn = 0;
    }

    void SpawnSkeletons()
    {
        for (int i = 0; i < skeletonEnemyCountToSpawn; i++)
            Instantiate(skeletonPrefab, skeletonSpawnPoints[Random.Range(0, skeletonSpawnPoints.Length - 1)].position, Quaternion.identity);

        skeletonEnemyCountToSpawn = 0;
    }

    IEnumerator CheckToSpawnEnemies()
    {
        yield return new WaitForSeconds(waitBeforeSpawnEnemies);

        SpawnEnemies();

        StartCoroutine("CheckToSpawnEnemies");
    }

    public void EnemyDied(EnemyType type)
    {
        if (type == EnemyType.BOAR)
        {
            //if boar
            if (boarEnemyCountToSpawn < boarCount)
                boarEnemyCountToSpawn++;
        }
        else if (type == EnemyType.CANNIBAL)
        {
            //if cannibal
            if (cannibalEnemyCountToSpawn < cannibalCount)
                cannibalEnemyCountToSpawn++;
        }
        else if (type == EnemyType.SKELETON)
        {
            //if skeleton
            if (skeletonEnemyCountToSpawn < skeletonCount)
                skeletonEnemyCountToSpawn++;
        }
        else if (type == EnemyType.ZOMBIE)
        {
            //if skeleton
            if (zombieEnemyCountToSpawn < zombieCount)
                zombieEnemyCountToSpawn++;
        }
    }

    public void StopSpawning()
    {
        StopCoroutine("CheckToSpawnEnemies");
    }
}
