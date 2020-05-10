using CustomQuest;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The quest script.
/// This is the big one.
/// Contains all information, lists and funktionality about a quest.
/// </summary>
public class Quest : MonoBehaviour
{
#if UNITY_EDITOR //Contains fields used within the Editor

    [HideInInspector]
    public bool isCustomScript = false;

    [HideInInspector]
    public Rect rectangleNode = new Rect(100, 100, 130, 100);

    public QuestNode questNode;

    [HideInInspector]
    public bool showDescription = true;

    [HideInInspector]
    public bool showTooltip = false;

    [HideInInspector]
    public bool showDefault = false;

    [HideInInspector]
    public bool showSettings = false;

    [HideInInspector]
    public bool showRelations = false;

    [HideInInspector]
    public bool showThresholds = false;

    [HideInInspector]
    public bool showOptionalThresholds = false;

    [HideInInspector]
    public GameObject prefab;

#endif

    #region Field

    /// <summary>
    /// The icon of the quest
    /// </summary>
    public Sprite questIcon;

    /// <summary>
    /// The name of the Quest
    /// </summary>
    public string questName;

    /// <summary>
    /// The description of the Quest
    /// </summary>
    public string description;

    /// <summary>
    /// The tooltip of the Quest
    /// </summary>
    public string toolTip;

    /// <summary>
    /// Sets whether the quest should be Available from start or not.
    /// </summary>
    public bool startAvailability;

    /// <summary>
    /// Sets whether the quest always should be available to all players. Works best with repeaterable.
    /// </summary>
    public bool constantAvailability;

    /// <summary>
    /// A bool whether the quest should auto complete when all criterias are done, or not
    /// </summary>
    public bool autoComplete;

    /// <summary>
    /// Whether the quest is completed or not
    /// </summary>
    public bool questCompleted;

    /// <summary>
    /// If the quest can be picked up and completed by the same player, multiple times
    /// </summary>
    public bool repeatable;

    /// <summary>
    /// The time before this quest is available again after completion in seconds, if repeatable is true
    /// </summary>
    public float repeatableTime;

    /// <summary>
    /// A dictonary of the remainig time on the induvidual players
    /// </summary>
    public Dictionary<CQPlayerObject, float> remainingRepeatableTime = new Dictionary<CQPlayerObject, float>();

    /// <summary>
    /// If the quest should ever be deleted
    /// </summary> //Is there to make sure a quest always stays around, even if all the players in the scene have completed it. What if a new player joins the room @ runtime?
    public bool dontDelete;

    /// <summary>
    /// If the quest can only be completed by one player, before its gone
    /// </summary>
    public bool singleComplete;

    /// <summary>
    /// If the quest should start spawning its criterias when its discovered
    /// </summary>
    public bool startSpawningOnDiscover;

    /// <summary>
    /// If the quest should stop spawning its criterias when no player has the quest
    /// </summary>
    public bool noSpawnIfNoPlayer;

    /// <summary>
    /// If this quest is timed, and then fails if its time is up
    /// </summary>
    public bool timed;

    /// <summary>
    /// If the quest should match criteria levels with optional criteria levels. So when a criteria level is done, optional criterias levels up aswell
    /// </summary>
    public bool matchOptionalLevels;

    /// <summary>
    /// If the player is able to pick this quest up at a quest giver
    /// </summary>
    public bool pickUpAble;

    /// <summary>
    /// The time before this quest fails
    /// </summary>
    public float time;

    /// <summary>
    /// A List of the Quest's Criterias
    /// </summary>
    public List<Criteria> criterias = new List<Criteria>();

    /// <summary>
    /// A list of the Quest's Rewards
    /// </summary>
    public List<Reward> rewards = new List<Reward>();

    /// <summary>
    /// The different thresholds for the different levels of quests
    /// </summary>
    public List<int> thresholds = new List<int>();

    /// <summary>
    /// The different thresholds for the different levels of optional quests
    /// </summary>
    public List<int> optionalThresholds = new List<int>();

    /// <summary>
    /// A list of this quests optional criterias
    /// </summary>
    public List<Criteria> optionalCriterias = new List<Criteria>();

    /// <summary>
    /// A list of this quests optional rewards
    /// </summary>
    public List<Reward> optionalRewards = new List<Reward>();

    /// <summary>
    /// Dictonary of each players active optionalCriterias for this quest
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> activeOptionalCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// Dictonary of each players completed optionalCriterias for this quest
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> completedOptionalCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// Dictonary of each players failed optionalCriterias for this quest
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> failedOptionalCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// The threshold of when to give the optional rewards. (3 = one player must complete 3 optional criterias to get the bonus reward)
    /// </summary>
    public int completedOptionalThreshold;

    /// <summary>
    /// Quests that has to be completed before this quest activates.
    /// </summary>
   // [HideInInspector]
    public List<Quest> unCompletedQuests = new List<Quest>();

    /// <summary>
    /// List to put the Quests you want to be able to use as chain quests.
    /// </summary>
  //  [HideInInspector]
    public List<Quest> questsToUnlock = new List<Quest>();

    /// <summary>
    /// Dictionary of the not yet completed criterias.
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> unCompletedCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// Dictionary of the completed criterias.
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> completedCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// A list of the active criterias on this quest
    /// </summary>
    public Dictionary<CQPlayerObject, List<Criteria>> activeCriterias = new Dictionary<CQPlayerObject, List<Criteria>>();

    /// <summary>
    /// A list of handInObjects this quest can hand in its quest to
    /// </summary>
    public List<HandInObject> handInObjects = new List<HandInObject>();

    /// <summary>
    /// A list of questGivers this quets has
    /// </summary>
    public List<QuestGiver> questGivers = new List<QuestGiver>();

    /// <summary>
    /// A dictonary of the remainig time on the induvidual players
    /// </summary>
    public Dictionary<CQPlayerObject, float> remainingTime = new Dictionary<CQPlayerObject, float>();

    /// <summary>
    /// A list of player currently on the quest
    /// </summary>
    public List<CQPlayerObject> playersUnCompleted = new List<CQPlayerObject>();

    /// <summary>
    /// A list of the players who has already done the quest
    /// </summary>
    public List<CQPlayerObject> playersCompleted = new List<CQPlayerObject>();

    private Image worldIcon;

    //private Image worldIconGlow;

    #endregion Field

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    public virtual void Start()
    {
        if (startAvailability)
        {
            foreach (CQPlayerObject p in QuestHandler.Instance.players)
            {
                if (!QuestHandler.Instance.availableQuests[p].Contains(this))
                {
                    QuestHandler.Instance.QuestsDiscovered(this, p);
                }
            }
        }

        AddPlayerToCriterias(); //Adds the current players, to their dictonaries.

        questCompleted = false; //A quest starts out not completed by default.
    }

    /// <summary>
    /// Adds the correct criterias to activeCriterias and other start related things
    /// </summary>
    public virtual void StartCriterias()
    {
        foreach (CQPlayerObject player in playersUnCompleted)
        {
            foreach (Criteria criteria in criterias)
            {
                if (criteria.Level == 0)
                {
                    activeCriterias[player].Add(criteria);
                    criteria.StartCriteria(player);
                }
                for (int i = criteria.spawnZones.Count - 1; i >= 0; i--)
                {
                    SpawnZone spawnZone = criteria.spawnZones[i];
                    if (spawnZone != null)
                    {
                        if (spawnZone.Spawn == true)
                        {
                            if (spawnZone.spawnAreaObject)
                            {
                                if (spawnZone.spawnAreaObject.GetComponentInChildren<Canvas>() == null)
                                {
                                    Debug.LogWarning("The spawnZone prefab dont have a canvas. This is needed, or the minimap icon funktionality won't work");
                                }
                                else
                                {
                                    foreach (Image img in spawnZone.spawnAreaObject.GetComponentsInChildren<Image>())
                                    {
                                        if (img.name == "Icon")
                                        {
                                            worldIcon = img;
                                        }
                                    }
                                    if (questIcon)
                                    {
                                        worldIcon.sprite = questIcon;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        criteria.spawnZones.Remove(spawnZone);
                    }
                }
            }
            foreach (Criteria criteria in optionalCriterias)
            {
                if (criteria.Level == 0)
                {
                    activeOptionalCriterias[player].Add(criteria);
                    criteria.StartCriteria(player);
                }
                foreach (SpawnZone spawnZone in criteria.spawnZones)
                {
                    if (spawnZone.Spawn == true)
                    {
                        if (spawnZone.spawnAreaObject)
                        {
                            if (spawnZone.spawnAreaObject.GetComponentInChildren<Canvas>() == null)
                            {
                                Debug.LogWarning("The spawnZone prefab dont have a canvas. This is needed, or the minimap icon funktionality won work");
                            }
                            foreach (Image img in spawnZone.spawnAreaObject.GetComponentsInChildren<Image>())
                            {
                                if (img.name == "Icon")
                                {
                                    worldIcon = img;
                                }
                            }
                            if (questIcon)
                            {
                                worldIcon.sprite = questIcon;
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// A start function used in the editor
    /// </summary>
    public virtual void EditorStart()
    {
    }

    /// <summary>
    ///  Update is called once per frame
    /// </summary>
    public virtual void Update()
    {
        if (constantAvailability)
        {
            foreach (CQPlayerObject p in QuestHandler.Instance.players)
            {
                if (repeatable)
                {
                    if (remainingRepeatableTime.ContainsKey(p))
                    {
                        if (remainingRepeatableTime[p] > 0)
                        {
                            break;
                        }
                    }
                }
                if (!QuestHandler.Instance.availableQuests[p].Contains(this))
                {
                    QuestHandler.Instance.QuestsDiscovered(this, p);
                }
            }
        }

        //Counts down the timer
        if (timed)
        {
            for (int i = playersUnCompleted.Count - 1; i >= 0; i--)
            {
                CQPlayerObject p = playersUnCompleted[i];
                remainingTime[p] -= Time.deltaTime;
                if (remainingTime[p] <= 0)
                {
                    OnFail(p);
                }
            }
        }

        //Counts down the repeaterable timer
        if (repeatable)
        {
            foreach (CQPlayerObject p in playersCompleted)
            {
                if (remainingRepeatableTime.ContainsKey(p))
                {
                    remainingRepeatableTime[p] -= Time.deltaTime;
                    if (remainingRepeatableTime[p] <= 0)
                    {
                        if (!QuestHandler.Instance.availableQuests[p].Contains(this))
                        {
                            if (startAvailability)
                            {
                                QuestHandler.Instance.QuestsDiscovered(this, p);
                            }
                        }
                        break;
                    }
                }
            }
        }

        //Count the spawn timer up, if the timer than exceeds the spawnrate it spawns the Criteria Objects and resets.
        foreach (Criteria criteria in criterias)
        {
            foreach (SpawnZone zone in criteria.spawnZones)
            {
                if (zone != null)
                {
                    zone.spawnRateTimer += Time.deltaTime;
                    if (zone.spawnRateTimer >= zone.spawnRate)
                    {
                        if (zone.Spawn == true)
                        {
                            SpawnQuestObjects(criteria.criteriaObject, zone.spawnAmount, zone);
                        }
                        zone.spawnRateTimer = 0;
                    }
                }
                else
                {
                    criteria.spawnZones.Remove(zone);
                }
            }
        }

        //Does the same as the above, but for optional criterias
        foreach (Criteria criteria in optionalCriterias)
        {
            foreach (SpawnZone zone in criteria.spawnZones)
            {
                if (zone != null)
                {
                    zone.spawnRateTimer += Time.deltaTime;
                    if (zone.spawnRateTimer >= zone.spawnRate)
                    {
                        if (zone.Spawn == true)
                        {
                            SpawnQuestObjects(criteria.criteriaObject, zone.spawnAmount, zone);
                        }
                        zone.spawnRateTimer = 0;
                    }
                }
                else
                {
                    criteria.spawnZones.Remove(zone);
                }
            }
        }
    }

    /// <summary>
    /// Run if Quest is completed
    /// </summary>
    /// <param name="player">The player completing the quest</param>
    /// <param name="unit">The unit completing the quest</param>
    public virtual void OnCompletion(CQPlayerObject player, CQExamplePlayer unit)
    {
        if (playersUnCompleted.Contains(player))
        {
            EventInfoHolder tmpE = new EventInfoHolder();
            tmpE.player = player;
            tmpE.unit = unit;
            tmpE.quest = this;
            QuestHandler.TriggerEvent("QuestCompleted", tmpE); //Sends out the QuestCompleted event

            List<Criteria> allCriterias = new List<Criteria>(criterias);
            allCriterias.AddRange(optionalCriterias);
            foreach (Criteria c in allCriterias)
            {
                if (c.type == criteriaType.Deliver || c.type == criteriaType.Gather)
                {
                    for (int i = unit.items.Count - 1; i >= 0; i--)
                    {
                        if (unit.items[i] != null)
                        {
                            if (unit.items[i].GetComponent<QuestObject>())
                            {
                                if (unit.items[i].GetComponent<QuestObject>().criteria == c)
                                {
                                    c.Remove(unit.items[i].gameObject);
                                    unit.items.RemoveAt(i);
                                }
                            }
                        }
                    }
                }
            }

            //Gives rewards
            GiveReward(unit, player, rewards); //Standard rewards
            if (completedOptionalThreshold <= completedOptionalCriterias[player].Count) //If the player has completed enought optional objectives, gives the player the 'optional' rewards
            {
                GiveReward(unit, player, optionalRewards); //optional rewards
            }
            if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards)
            {
                foreach (Criteria c in completedCriterias[player])
                {
                    if (!c.giveRewardsOnCompletion)
                    {
                        GiveReward(unit, player, c.rewards); //Individual criteria rewards
                    }
                }
            }
            if (CustomQuestSettings.SettingsHolder.optionalCriteriaSpecificRewards)
            {
                foreach (Criteria c in completedOptionalCriterias[player])
                {
                    if (!c.giveRewardsOnCompletion)
                    {
                        GiveReward(unit, player, c.rewards); //Individual optional criteria rewards
                    }
                }
            }

            QuestHandler.Instance.availableQuests[player].Remove(this);
            foreach (Quest quest in questsToUnlock)
            {
                if (quest != null)
                {
                    quest.UnlockQuest(this, player);
                }
            }
            //  QuestHandler.Instance.UpdateQuestTracker();
            if (!playersCompleted.Contains(player))
            {
                playersCompleted.Add(player);
            }
            playersUnCompleted.Remove(player);

            if (noSpawnIfNoPlayer == true)
            {
                if (playersUnCompleted.Count <= 0)
                {
                    List<Criteria> tmpCriteria = new List<Criteria>(criterias);
                    tmpCriteria.AddRange(optionalCriterias);
                    foreach (Criteria criteria in tmpCriteria)
                    {
                        foreach (SpawnZone zone in criteria.spawnZones)
                        {
                            zone.DespawnQuestObjects();
                        }
                    }
                    StopSpawning();
                }
            }

            if (repeatable == false)
            {
                if (dontDelete == false)
                {
                    bool delete = true;
                    if (!singleComplete)
                    {
                        foreach (CQPlayerObject p in QuestHandler.Instance.players)
                        {
                            if (!playersCompleted.Contains(p))
                            {
                                delete = false;
                            }
                        }
                    }
                    if (delete == true)
                    {
                        QuestHandler.Instance.allQuests.Remove(this);
                        List<Criteria> tmpCriteria = new List<Criteria>(criterias);
                        tmpCriteria.AddRange(optionalCriterias);
                        foreach (Criteria criteria in tmpCriteria)
                        {
                            foreach (SpawnZone zone in criteria.spawnZones)
                            {
                                zone.DespawnQuestObjects();
                            }
                        }
                        Destroy(this.gameObject);
                    }
                }
            }
            else
            {
                //Debug.Log(repeatableTime);
                //if (repeatableTime > 0)
                //{
                List<Criteria> tmpCriteria = new List<Criteria>(criterias);
                tmpCriteria.AddRange(optionalCriterias);
                foreach (Criteria criteria in tmpCriteria)
                {
                    if (!criteria.dontDespawnObjectsWhenComplete)
                    {
                        foreach (SpawnZone zone in criteria.spawnZones)
                        {
                            zone.DespawnQuestObjects();
                        }
                    }
                }
                if (!remainingRepeatableTime.ContainsKey(player))
                {
                    remainingRepeatableTime.Add(player, repeatableTime);
                }
                else
                {
                    remainingRepeatableTime[player] = repeatableTime;
                }
                tmpE.quest = this;
                QuestHandler.TriggerEvent("ResetQuestInList", tmpE); //Sends out the ResetQuestInList event
                //}
                tmpE.player = player;
                tmpE.quest = this;
                QuestHandler.TriggerEvent("UpdateQuestTracker", tmpE); //Sends out the UpdateQuestTracker event
            }
        }
    }

    /// <summary>
    /// Run if Quest failed
    /// </summary>
    /// <param name="player">The player failing the quest</param>
    public virtual void OnFail(CQPlayerObject player)
    {
        EventInfoHolder tmpE = new EventInfoHolder();
        tmpE.quest = this;
        tmpE.player = player;
        QuestHandler.TriggerEvent("QuestFailed", tmpE); //Sends out the QuestFailed event

        QuestHandler.Instance.availableQuests[player].Remove(this);
        playersUnCompleted.Remove(player);
        ResetCriterias(player);

        if (noSpawnIfNoPlayer == true)
        {
            if (playersUnCompleted.Count <= 0)
            {
                List<Criteria> tmpCriteria = new List<Criteria>(criterias);
                foreach (Criteria criteria in tmpCriteria)
                {
                    foreach (SpawnZone zone in criteria.spawnZones)
                    {
                        zone.DespawnQuestObjects();
                    }
                }
                StopSpawning();
            }
        }

        //TODO: Delete, if all players has failed it, and they are not allowed to try again. (OneTry == true) (List of players who failed?)

        tmpE.player = player;
        tmpE.quest = this;
        QuestHandler.TriggerEvent("UpdateQuestTracker", tmpE); //Sends out the UpdateQuestTracker event
    }

    /// <summary>
    /// Adds a criteria to the quest
    /// </summary>
    /// <param name="critera">The criteria to add</param>
    public virtual void CreateCriteria(Criteria critera)
    {
        Criteria tmpC = gameObject.AddComponent<Criteria>();
        tmpC = critera;
        tmpC.Quest = GetComponent<Quest>();
        criterias.Add(tmpC);
    }

    /// <summary>
    /// Deletes a specific criteria
    /// </summary>
    /// <param name="criteria">The criteria to be deleted</param>
    public virtual void DeleteCriteria(Criteria criteria)
    {
        criterias.Remove(criteria);
        Destroy(criteria);
    }

    /// <summary>
    /// Adds a reward to the quest
    /// </summary>
    /// <param name="reward">The reward to be added</param>
    public virtual void CreateReward(Reward reward)
    {
        Reward TmpR = this.gameObject.AddComponent<Reward>();
        TmpR = reward;
        rewards.Add(TmpR);
    }

    /// <summary>
    /// Deletes a specifik reward script
    /// </summary>
    /// <param name="reward">The reward to be deleted</param>
    public virtual void DeleteReward(Reward reward)
    {
        rewards.Remove(reward);
        Destroy(reward);
    }

    /// <summary>
    /// Spawns Quest Objects
    /// </summary>
    /// <param name="questObject">The object to spawn</param>
    /// <param name="objectAmount">The amount to spawn</param>
    /// <param name="criteria">The criteria spawning them</param>
    public virtual void SpawnQuestObjects(GameObject questObject, int objectAmount, SpawnZone zone)
    {
        if (zone.spawnAreaObject)
        {
            for (int i = 0; i < objectAmount; i++)
            {
                if (zone.spawnedObjects.Count < zone.maxSpawnAmount)
                {
                    GameObject tmpObject;

                    Vector3 spawnArea = zone.spawnAreaObject.transform.position + Random.insideUnitSphere * zone.spawnRadius;
                    spawnArea.y = zone.spawnAreaObject.transform.position.y;
                    tmpObject = Instantiate(questObject, spawnArea, zone.spawnAreaObject.transform.rotation);
                    if (tmpObject.GetComponent<QuestObject>())
                    {
                        tmpObject.GetComponent<QuestObject>().criteria = zone.Criteria;
                    }
                    else
                    {
                        tmpObject.AddComponent<QuestObject>();
                        tmpObject.GetComponent<QuestObject>().criteria = zone.Criteria;
                    }
                    zone.spawnedObjects.Add(tmpObject);

                    EventInfoHolder tmpE = new EventInfoHolder();
                    tmpE.quest = this;
                    tmpE.gameObject = questObject;
                    tmpE.criteria = zone.Criteria;
                    tmpE.spawnZone = zone;
                    QuestHandler.TriggerEvent("QuestObjectSpawned", tmpE); //Sends out the QuestObjectSpawned event
                }
            }
        }
        else
        {
            Debug.Log("No spawnAreaObject on: " + zone + ". Cannot spawn questObjects");
        }
    }

    /// <summary>
    /// Completes a Criteria.
    /// </summary>
    /// <param name="player">The player who completed it</param>
    /// <param name="criteria">The criteria to complete</param>
    /// <param name="unit">The unit who completed the criteria</param>
    public virtual void CriteriaCompleted(CQExamplePlayer unit, CQPlayerObject player, Criteria criteria)
    {
        if (unCompletedCriterias.ContainsKey(player) != true || completedCriterias.ContainsKey(player) != true || activeCriterias.ContainsKey(player) != true || completedOptionalCriterias.ContainsKey(player) != true)
        {
            AddPlayerToCriterias();
        }
        if (unCompletedCriterias[player].Contains(criteria)) //TODO: Some check if its in activeCriterias aswell?
        {
            ProcessCriteria(unit, player, criteria);
        }
        else if (activeOptionalCriterias[player].Contains(criteria))
        {
            ProcessOptionalCriteria(unit, player, criteria);
        }
        else if (completedCriterias[player].Contains(criteria) || completedOptionalCriterias[player].Contains(criteria))
        {
            //Criteria has already been completed by that player
        }
        else
        {
            Debug.LogWarning(criteria + " not found in any lists in " + this + ". " + criteria + " could not be completed");
        }

        EventInfoHolder tmpE = new EventInfoHolder();
        tmpE.player = player;
        tmpE.quest = this;
        QuestHandler.TriggerEvent("UpdateQuestTracker", tmpE); //Sends out the UpdateQuestTracker event
    }

    /// <summary>
    /// Processes a criteria
    /// </summary>
    /// <param name="unit">The unit who completed the criteria</param>
    /// <param name="player">The player who completed the criteria</param>
    /// <param name="criteria">The criteria which is completed</param>
    public virtual void ProcessCriteria(CQExamplePlayer unit, CQPlayerObject player, Criteria criteria)
    {
        unCompletedCriterias[player].Remove(criteria);
        activeCriterias[player].Remove(criteria);
        completedCriterias[player].Add(criteria);
        if (activeCriterias[player].Count <= 0 && unCompletedCriterias[player].Count > 0)
        {
            int counter = 1;
            while (activeCriterias[player].Count <= 0)
            { //Adds the new level of criterias
                foreach (Criteria c in unCompletedCriterias[player])
                {
                    if (c.Level == criteria.Level + counter)
                    {
                        activeCriterias[player].Add(c);
                        c.StartCriteria(player);
                    }
                }
                if (matchOptionalLevels)
                {
                    activeOptionalCriterias[player].Clear();
                    foreach (Criteria c in optionalCriterias)
                    {
                        if (c.Level == criteria.Level + counter)
                        {
                            activeOptionalCriterias[player].Add(c);
                            c.StartCriteria(player);
                        }
                    }
                }
                counter++;
                if (counter > 100)
                {
                    Debug.Log("infinite loop prevented");
                    break;
                }
            }
        }
        else if (thresholds.Count > 0) //Checks for any threshold level - Prevents errors.
        {
            if (thresholds[criteria.Level] > 0) //Ignores thresholds if its value is 0 or lower
            { //Threshold logic
                int thresholdCounter = 0;
                foreach (Criteria c in completedCriterias[player])
                {
                    if (c.Level == criteria.Level)
                    {
                        thresholdCounter += 1;
                    }
                }
                if (thresholds[criteria.Level] <= thresholdCounter)
                { //Threshold reached
                    for (int i = 0; i < activeCriterias[player].Count; i++)
                    { //Removing the rest of the activeCriterias and uncompleted criterias
                        unCompletedCriterias[player].Remove(activeCriterias[player][i]);
                        activeCriterias[player].Remove(activeCriterias[player][i]);
                    }
                    int counter = 0;
                    while (activeCriterias[player].Count <= 0)
                    { //Adds the new level of criterias
                        foreach (Criteria c in unCompletedCriterias[player])
                        {
                            if (c.Level == criteria.Level + counter)
                            {
                                activeCriterias[player].Add(c);
                                c.StartCriteria(player);
                            }
                        }
                        if (matchOptionalLevels)
                        {
                            activeOptionalCriterias[player].Clear();
                            foreach (Criteria c in optionalCriterias)
                            {
                                if (c.Level == criteria.Level + counter)
                                {
                                    activeOptionalCriterias[player].Add(c);
                                    c.StartCriteria(player);
                                }
                            }
                        }
                        counter++;
                        if (counter > 100)
                        {
                            Debug.Log("infinite loop prevented");
                            break;
                        }
                    }
                }
            }
        }
        if (activeCriterias[player].Count > 0)
        {
            EventInfoHolder tmpE = new EventInfoHolder();
            tmpE.quest = this;
            QuestHandler.TriggerEvent("ResetQuestInList", tmpE); //Sends out the ResetQuestInList event
        }
        if (autoComplete == true && activeCriterias[player].Count <= 0) { OnCompletion(player, unit); }
        if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards && criteria.giveRewardsOnCompletion) { GiveReward(unit, player, criteria.rewards); }
        criteria.Complete(player, unit);
    }

    /// <summary>
    /// Processes an optional criteria
    /// </summary>
    /// <param name="unit">The unit who completed the criteria</param>
    /// <param name="player">The player who completed the criteria</param>
    /// <param name="criteria">The criteria which is completed</param>
    public virtual void ProcessOptionalCriteria(CQExamplePlayer unit, CQPlayerObject player, Criteria criteria)
    {
        activeOptionalCriterias[player].Remove(criteria);
        completedOptionalCriterias[player].Add(criteria);
        if (CustomQuestSettings.SettingsHolder.optionalCriteriaSpecificRewards && criteria.giveRewardsOnCompletion) { GiveReward(unit, player, criteria.rewards); }
        else if (optionalThresholds[criteria.Level] > 0) //Ignores thresholds if its value is 0 or lower
        { //Threshold logic
            int thresholdCounter = 0;
            foreach (Criteria c in completedOptionalCriterias[player])
            {
                if (c.Level == criteria.Level)
                {
                    thresholdCounter += 1;
                }
            }
            if (optionalThresholds[criteria.Level] <= thresholdCounter)
            { //Threshold reached
                for (int i = 0; i < activeOptionalCriterias[player].Count; i++)
                { //Removing the rest of the activeCriterias and uncompleted criterias
                    completedOptionalCriterias[player].Remove(activeOptionalCriterias[player][i]);
                    activeOptionalCriterias[player].Remove(activeOptionalCriterias[player][i]);
                }
                int counter = 0;
                while (activeOptionalCriterias[player].Count <= 0)
                { //Adds the new level of criterias
                    if (!matchOptionalLevels)
                    {
                        foreach (Criteria c in optionalCriterias)
                        {
                            if (c.Level == criteria.Level + counter)
                            {
                                activeOptionalCriterias[player].Add(c);
                                c.StartCriteria(player);
                            }
                        }
                    }
                    counter++;
                }
            }
        }

        criteria.Complete(player, unit);
    }

    /// <summary>
    /// Run this to fail a criteria
    /// </summary>
    /// <param name="critera">The criteria to fail</param>
    /// <param name="player">The player who failed this criteria</param>
    public virtual void CriteriaFailed(CQPlayerObject player, Criteria criteria)
    {
        if (unCompletedCriterias.ContainsKey(player) != true || completedCriterias.ContainsKey(player) != true || activeCriterias.ContainsKey(player) != true || completedOptionalCriterias.ContainsKey(player) != true)
        {
            AddPlayerToCriterias();
        }
        if (unCompletedCriterias[player].Contains(criteria) && activeCriterias[player].Contains(criteria))
        {
            activeCriterias[player].Remove(criteria);
            unCompletedCriterias[player].Remove(criteria);
            if (thresholds[criteria.Level] > activeCriterias[player].Count || thresholds[criteria.Level] == 0)
            {
                OnFail(player);
            }
            EventInfoHolder tmpE = new EventInfoHolder();
            tmpE.quest = this;
            QuestHandler.TriggerEvent("ResetQuestInList", tmpE); //Sends out the ResetQuestInList event
        }
        else if (activeOptionalCriterias[player].Contains(criteria))
        {
            failedOptionalCriterias[player].Add(criteria);
            activeOptionalCriterias[player].Remove(criteria);
        }
    }

    /// <summary>
    /// Give's the Reward when the player completes the quest.
    /// </summary>
    /// <param name="unit">The unit who completed the quest</param>
    /// <param name="player">The player who completed the quest</param>
    public virtual void GiveReward(CQExamplePlayer unit, CQPlayerObject player, List<Reward> rewards)
    {
        foreach (Reward reward in rewards)
        {
            EventInfoHolder tmpE = new EventInfoHolder();
            tmpE.player = player;
            tmpE.unit = unit;
            tmpE.quest = this;
            tmpE.reward = reward;
            QuestHandler.TriggerEvent("GiveReward", tmpE); //Sends out the QuestCompleted event
        }
    }

    /// <summary>
    /// Unlocks the quest if no quests are left uncompleted.
    /// </summary>
    /// <param name="quest">The quest this quest removes from its unCompletedQuests</param>
    /// <param name="player">The player unlocking the quest</param>
    public virtual void UnlockQuest(Quest quest, CQPlayerObject player)
    {
        unCompletedQuests.Remove(quest);
        if (unCompletedQuests.Count <= 0)
        {
            pickUpAble = true;
            if (startAvailability == true)
            {
                QuestHandler.Instance.QuestsDiscovered(this, player);
            }
            if (Vector3.Distance(player.transform.position, questGivers[0].transform.position) < questGivers[0].radius)
            {
                questGivers[0].StartQuestPopUp(player, this);
            }
        }
    }

    /// <summary>
    /// Starts all the criterias spawn
    /// </summary>
    /// <param name="player">The player starting the spawn</param>
    public virtual void StartSpawning(CQPlayerObject player)
    {
        List<Criteria> tmpCriteria = new List<Criteria>(criterias);
        tmpCriteria.AddRange(optionalCriterias);
        foreach (Criteria c in tmpCriteria)
        {
            if (c.Level == 0)
            {
                c.StartCriteria(player);
            }
        }
    }

    /// <summary>
    /// Stops all the criterias spawn
    /// </summary>
    public virtual void StopSpawning()
    {
        List<Criteria> tmpCriteria = new List<Criteria>(criterias);
        tmpCriteria.AddRange(optionalCriterias);
        foreach (Criteria c in tmpCriteria)
        {
            foreach (SpawnZone zone in c.spawnZones)
            {
                zone.Spawn = false;
            }
        }
    }

    /// <summary>
    /// Resest the criterias for this quest
    /// </summary>
    /// <param name="player">The player to reset the criterias for</param>
    public virtual void ResetCriterias(CQPlayerObject player)
    {
        if (unCompletedCriterias.ContainsKey(player) != true || completedCriterias.ContainsKey(player) != true || completedOptionalCriterias.ContainsKey(player) != true)
        {
            AddPlayerToCriterias();
        }
        unCompletedCriterias[player].Clear();
        foreach (Criteria c in criterias)
        {
            c.playerProgression[player] = 0;
            unCompletedCriterias[player].Add(c);
        }
        foreach (Criteria c in optionalCriterias)
        {
            c.playerProgression[player] = 0;
            unCompletedCriterias[player].Add(c);
        }
        completedCriterias[player].Clear();
        completedOptionalCriterias[player].Clear();
    }

    /// <summary>
    /// Adds players to the Criteria dictionaries, if they are not already there.
    /// </summary>
    public virtual void AddPlayerToCriterias()
    {
        foreach (CQPlayerObject player in playersUnCompleted)
        {
            if (unCompletedCriterias.ContainsKey(player) != true)
            {
                unCompletedCriterias.Add(player, new List<Criteria>(criterias)); // If the player does not exist in the dictonary, he has not done any criterias. Therefor, we add them all here.
            }
            if (completedCriterias.ContainsKey(player) != true)
            {
                completedCriterias.Add(player, new List<Criteria>());
            }
            if (activeCriterias.ContainsKey(player) != true)
            {
                activeCriterias.Add(player, new List<Criteria>());
            }
            if (activeOptionalCriterias.ContainsKey(player) != true)
            {
                activeOptionalCriterias.Add(player, new List<Criteria>());
            }
            if (completedOptionalCriterias.ContainsKey(player) != true)
            {
                completedOptionalCriterias.Add(player, new List<Criteria>());
            }
        }
    }
}