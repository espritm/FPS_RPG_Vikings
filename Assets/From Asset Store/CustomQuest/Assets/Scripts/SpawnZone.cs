using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A spawn zone for a quest object.
/// Contains information about spawning
/// </summary>
public class SpawnZone : MonoBehaviour
{
    #region Field

#if UNITY_EDITOR //Contains fields used within the Editor

    /// <summary>
    /// Bool used to show / hide spawn information in the Editor
    /// </summary>
    [HideInInspector]
    public bool toggleSpawn = false;

#endif

    [SerializeField]
    /// <summary>
    /// The name of this spawnZone
    /// </summary>
    private string spawnName = "SpawnZone";

    /// <summary>
    /// A spawn timer, goes up with deltaTime, and is compared to the criterias spawnrate
    /// </summary>
    public float spawnRateTimer;

    [SerializeField]
    /// <summary>
    /// If the quest should spawn objects or not.
    /// </summary>
    private bool spawn;

    /// <summary>
    /// The object the quest will spawn the Criteria Objects around.
    /// </summary>
    public GameObject spawnAreaObject;

    /// <summary>
    /// The radius around the spawn object, where the objects will spawn inside
    /// </summary>
    public float spawnRadius;

    /// <summary>
    /// The amount of objects you wan't the quest to spawn.
    /// </summary>
    public int spawnAmount;

    /// <summary>
    /// The rate of how often the objects should spawn.
    /// </summary>
    public float spawnRate;

    /// <summary>
    /// How many objects the quest should spawn from the beginning.
    /// </summary>
    public int initialSpawnAmount;

    /// <summary>
    /// The max amount of spawns at once.
    /// </summary>
    public int maxSpawnAmount;

    [SerializeField]
    /// <summary>
    /// The criteria this spawnZone is spawning for
    /// </summary>
    private Criteria criteria;

    /// <summary>
    /// List of the spawnedObjects.
    /// </summary>
    public List<GameObject> spawnedObjects = new List<GameObject>();

    #endregion Field

    #region Properties

    public bool Spawn
    {
        get { return spawn; }
        set
        {
//#if !UNITY_EDITOR
//            if (spawn != value && value == true) { Criteria.quest.SpawnQuestObjects(Criteria.criteriaObject, initialSpawnAmount, this); }
//#endif
            spawn = value;
        }
    }

    /// <summary>
    /// The criteria this spawnzone is the spawnzone for
    /// </summary>
    public Criteria Criteria { get { if (criteria == null) { Criteria = GetComponentInParent<Criteria>(); } return criteria; } set { criteria = value; } }

    /// <summary>
    /// The name of this spawnZone
    /// </summary>
    public string SpawnName
    {
        get { return spawnName; }
        set
        {
            spawnName = value;
            if (gameObject.name != value)
            {
                gameObject.name = value;
            }
        }
    }

    #endregion Properties

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    private void Start()
    {
        if (Criteria == null)
        {
            Criteria = GetComponentInParent<Criteria>();
        }

        if (Spawn == true)
        {
            if (Criteria.Quest)
            {
                Criteria.Quest.SpawnQuestObjects(Criteria.criteriaObject, initialSpawnAmount, this);
            }
            else
            {
                Debug.Log(Criteria.Quest);
            }
        }
    }

    /// <summary>
    /// Despawns the QuestObjects
    /// </summary>
    /// <param name="questObjects">The objects to despawn</param>
    public void DespawnQuestObjects()
    {
        List<GameObject> tmpQuestObject = new List<GameObject>(spawnedObjects);
        foreach (GameObject questObject in tmpQuestObject)
        {
            spawnedObjects.Remove(questObject);
            Destroy(questObject);
        }
    }
}