using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A holder for settings for the custom quest system
/// </summary>
public class SettingsHolder : ScriptableObject
{
    public List<Quest> prefabQuests = new List<Quest>();

    public List<Criteria> prefabCriteria = new List<Criteria>();

    public List<Reward> prefabReward = new List<Reward>();

    public List<QuestNode> questNodes = new List<QuestNode>();

    public bool showQuestName;

    public bool showDescription;

    public bool showCriterias;

    public bool showRewards;

    public GameObject handInObjectPrefab;

    public GameObject criteriaSpawnPrefab;

    public GameObject questGiverPrefab;

    public bool criteriaSpecificRewards;

    public bool optionalCriteriaSpecificRewards;

    public bool optional;

    public GUISkin randomDragonGUISkin;

    /// <summary>
    /// Destroys an scriptableObject immediately, warning: will destroy assets without warning.
    /// </summary>
    /// <param name="o">The object to destroy</param>
    public void DestroyObject(ScriptableObject o)
    {
        DestroyImmediate(o, true);
    }
}