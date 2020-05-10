using CustomQuest;
using UnityEngine;

/// <summary>
/// A reward given by a quest or a criteria.
/// </summary>
public class Reward : MonoBehaviour
{
    #region Field

#if UNITY_EDITOR

    /// <summary>
    /// Used to tell the editor if this script has been converted
    /// </summary>
    [HideInInspector]
    public bool isCustomScript = false;

    /// <summary>
    /// Used to tell editor which type of reward this is
    /// </summary>
    public editorRewardType editoreRewardType = editorRewardType.Standard;

#endif

    /// <summary>
    /// Name of the Reward.
    /// </summary>
    public string rewardName;

    /// <summary>
    /// Type of the Reward.
    /// </summary>
    public rewardType type;

    /// <summary>
    /// The reward Object.
    /// </summary>
    public GameObject rewardObject;

    /// <summary>
    /// Amount of rewards.
    /// </summary>
    public int amount;

    #endregion Field

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public virtual void Start()
    {
    }

    /// <summary>
    /// Used when converting a script, so the fields are set correctly (It's supposed to be empthy, see the .txt files for explanation)
    /// </summary>
    public virtual void EditorStart()
    {
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public virtual void Update()
    {
    }
}