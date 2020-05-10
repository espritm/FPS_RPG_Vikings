using CustomQuest;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A criteria for a quest
/// Contains amount, type, name, individual player progressing, spawnzones, rewards and many other options
/// </summary>
[Serializable]
public class Criteria : MonoBehaviour
{
    #region Unity Editor

#if UNITY_EDITOR //Contains fields used within the Editor

    [HideInInspector]
    public bool isCustomScript = false;

    [HideInInspector]
    private bool showSpawns = false;

    [HideInInspector]
    private bool showRewards = false;

    [HideInInspector]
    private bool showSettings = false;

    [HideInInspector]
    public int toolbarInt;

    
    public editorCriteriaType editorType = editorCriteriaType.Standard;

    /*** Editor Properties ***/

    public bool ShowSpawns { get { return showSpawns; } set { showSpawns = value; if (value == true) { toolbarInt = 1; showRewards = false; showSettings = false; } } }

    public bool ShowRewards { get { return showRewards; } set { showRewards = value; if (value == true) { toolbarInt = 2; showSpawns = false; showSettings = false; } } }

    public bool ShowSettings { get { return showSettings; } set { showSettings = value; if (value == true) { toolbarInt = 3; showSpawns = false; showRewards = false; } } }

#endif

    #endregion Unity Editor

    #region Field

    /// <summary>
    /// The amount of objects the player have to kill to complete the quest.
    /// </summary>
    public int amount;

    /// <summary>
    /// Name of the Criteria
    /// </summary>
    public string criteriaName;

    /// <summary>
    /// Type of the Criteria
    /// </summary>
    public criteriaType type;

    /// <summary>
    /// The Criteria Object.
    /// </summary>
    public GameObject criteriaObject;

    /// <summary>
    /// Dictionary over the players progression.
    /// </summary>
    public Dictionary<CQPlayerObject, int> playerProgression = new Dictionary<CQPlayerObject, int>();

    /// <summary>
    /// A list of spawnZones for this criteria
    /// </summary>
    public List<SpawnZone> spawnZones = new List<SpawnZone>();

    /// <summary>
    /// An int controlling when this criteria is avalible for completion. All level '0' will have to be completed, before level '1' will activate, and so on.
    /// </summary>
    [SerializeField]
    private int level;

    /// <summary>
    /// The rewards this criteria contains
    /// </summary>
    public List<Reward> rewards = new List<Reward>();

    /// <summary>
    /// If true, this criteria will give its reward as soon as the criteria is completed. Otherwise, the reward is given when the quest is completed
    /// </summary>
    public bool giveRewardsOnCompletion;

    /// <summary>
    /// A reference to the Quest script.
    /// </summary>
    [SerializeField]
    private Quest quest;

    /// <summary>
    /// If this criteria is timed
    /// </summary>
    public bool timed;

    /// <summary>
    /// Dont despawns all the spawned objects for this criteria when its completed
    /// </summary>
    public bool dontDespawnObjectsWhenComplete;

    /// <summary>
    /// If its timed, how long (in seconds)
    /// </summary>
    public float time;

    /// <summary>
    /// A dictonary of the different players induvidual remaining timers for this criteria
    /// </summary>
    private Dictionary<CQPlayerObject, float> remainingTimer = new Dictionary<CQPlayerObject, float>();

    #endregion Field

    #region Properties

    /// <summary>
    /// A reference to the Quest script.
    /// </summary>
    public Quest Quest { get { if (quest == null) { quest = GetComponent<Quest>(); } return quest; } set { quest = value; } }

    /// <summary>
    /// An int controlling when this criteria is avalible for completion. All level '0' will have to be completed, before level '1' will activate, and so on. It have a hard limit of 100.
    /// </summary>
    public int Level { get { return level; } set { if (value > 100) { value = 100; } level = value; } }

    #endregion Properties

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    public virtual void Start()
    {
        //Adds the players to the dictionary
        foreach (CQPlayerObject player in QuestHandler.Instance.players)
        {
            if (!playerProgression.ContainsKey(player))
            {
                playerProgression.Add(player, 0);
            }
        }
        Quest = GetComponent<Quest>();
    }

    /// <summary>
    /// Used to set values when converting to a custom criteria
    /// </summary>
    public virtual void EditorStart()
    {

    }

    /// <summary>
    /// Runs once every frame
    /// </summary>
    public virtual void Update()
    {
        if (timed)
        {
            List<CQPlayerObject> players = new List<CQPlayerObject>(remainingTimer.Keys);
            foreach (CQPlayerObject p in players)
            {
                remainingTimer[p] -= Time.deltaTime;
                if (remainingTimer[p] <= 0)
                {
                    Fail(p);
                }
            }
        }
    }

    /// <summary>
    /// Starts the time and spawn on this criteria
    /// </summary>
    public virtual void StartCriteria(CQPlayerObject player)
    {
        if (timed)
        {
            if (!remainingTimer.ContainsKey(player))
            {
                remainingTimer.Add(player, time);
            }
            else
            {
                remainingTimer[player] = time;
            }
        }
        if (quest.startSpawningOnDiscover)
        {
            foreach (SpawnZone zone in spawnZones)
            {
                if (zone != null)
                {
                    zone.Spawn = true;
                    quest.SpawnQuestObjects(criteriaObject, zone.initialSpawnAmount, zone);
                }
            }
        }
    }

    /// <summary>
    /// Updates the player progress.
    /// </summary>
    /// <param name="player"></param>
    public virtual void Progress(CQPlayerObject player, CQExamplePlayer unit)
    {
        if (playerProgression.ContainsKey(player))
        {
            playerProgression[player] += 1;
            if (playerProgression[player] <= amount)
            {
                EventInfoHolder tmpE = new EventInfoHolder();
                tmpE.player = player;
                tmpE.unit = unit;
                tmpE.criteria = this;
                QuestHandler.TriggerEvent("CriterionProgress", tmpE); //Sends out the CriteriaProgress event

            }
            if (playerProgression[player] >= amount)
            {
                Quest.CriteriaCompleted(unit, player, this);
            }
        }
    }

    /// <summary>
    /// Removes an object from this criterias spawnedObjects lists, and destroys it
    /// </summary>
    /// <param name="obj">The object to remove and destroy</param>
    public virtual void Remove(GameObject obj)
    {
        foreach (SpawnZone zone in spawnZones)
        {
            if (zone.spawnedObjects.Contains(obj))
            {
                zone.spawnedObjects.Remove(obj);
            }
        }
        Destroy(obj);
    }

    /// <summary>
    /// Is run when this criteria is completed
    /// </summary>
    /// <param name="player">The player who completed the criteria</param>
    public virtual void Complete(CQPlayerObject player, CQExamplePlayer unit)
    {
        EventInfoHolder tmpE = new EventInfoHolder();
        tmpE.criteria = this;
        tmpE.player = player;
        tmpE.unit = unit;
        QuestHandler.TriggerEvent("CriteriaCompleted", tmpE); //Sends out the CriteriaCompleted event 
        if (!dontDespawnObjectsWhenComplete)
        {
            bool despawn = true;
            foreach (CQPlayerObject p in quest.playersUnCompleted)
            {
                if (quest.activeCriterias[p].Contains(this))
                {
                    despawn = false;
                }
            }
            if (despawn == true)
            {
                foreach (SpawnZone zone in spawnZones)
                {
                    zone.DespawnQuestObjects();
                    zone.Spawn = false;
                }
            }
        }
    }

    /// <summary>
    /// Is run when this criteria fails
    /// </summary>
    public virtual void Fail(CQPlayerObject player)
    {
        EventInfoHolder tmpE = new EventInfoHolder();
        tmpE.criteria = this;
        tmpE.player = player;
        QuestHandler.TriggerEvent("CriteriaFailed", tmpE); //Sends out the CriteriaFailed event 
        quest.CriteriaFailed(player, this);
        remainingTimer.Remove(player);
    }
}