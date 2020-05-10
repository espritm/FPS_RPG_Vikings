using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI logic holder for a quests. Used for displaying a list of quest.
/// </summary>
public class QuestUILogic : MonoBehaviour
{
    #region Field

    /// <summary>
    /// The Quest Script this QuestUILogic is the logic off
    /// </summary>
    [Tooltip("The Quest Script this QuestUILogic is the logic off")]
    public Quest quest;

    /// <summary>
    /// The text element which should show the name of the quest
    /// </summary>
    [Tooltip("The text element which should show the name of the quest")]
    public Text questName;

    /// <summary>
    /// The text element which should show the description of the quest
    /// </summary>
    [Tooltip("The text element which should show the description of the quest")]
    public Text description;

    /// <summary>
    /// The icon of the quest this element is showing
    /// </summary>
    [Tooltip("The icon of the quest this element is showing")]
    public Image questIcon;

    /// <summary>
    /// A list of the CriteriasUIs this questsUIlogic has
    /// </summary>
    [Tooltip("A list of the CriteriasUIs this questsUIlogic has")]
    public List<CriteriaUILogic> criteriaUis = new List<CriteriaUILogic>();

    /// <summary>
    /// A list of criterias from the quest, this ui is showing
    /// </summary>
    [Tooltip("A list of criterias from the quest, this ui is showing")]
    public List<Criteria> criterias = new List<Criteria>();

    /// <summary>
    /// A list of rewardUIs this questUIlogic has
    /// </summary>
    [Tooltip("A list of rewardUIs this questUIlogic has")]
    public List<RewardUILogic> rewardUis = new List<RewardUILogic>();

    /// <summary>
    /// A list of rewards from the quest, this ui is showing
    /// </summary>
    [Tooltip("A list of rewards from the quest, this ui is showing")]
    public List<Reward> rewards = new List<Reward>();

    /// <summary>
    /// The rect transform of this object
    /// </summary>
    [Tooltip("The rect transform of this object")]
    public RectTransform rectTransform;

    #endregion Field

    /// <summary>
    /// The method used for initialization
    /// </summary>
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}