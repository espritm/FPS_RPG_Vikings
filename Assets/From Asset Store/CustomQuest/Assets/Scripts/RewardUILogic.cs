using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A UI logic holder for a reward. Used for displaying a list of quest.
/// </summary>
public class RewardUILogic : MonoBehaviour
{
    /// <summary>
    /// The reward of this rewardUILogic object
    /// </summary>
    [Tooltip("The reward of this rewardUILogic object")]
    public Reward reward;

    /// <summary>
    /// The text which should be showing the name of the reward
    /// </summary>
    [Tooltip("The text which should be showing the name of the reward")]
    public Text rewardName;

    /// <summary>
    /// The text which should be showing the type of the reward
    /// </summary>
    [Tooltip("The text which should be showing the type of the reward")]
    public Text rewardType;

    /// <summary>
    /// The text which should be showing the amount of the reward
    /// </summary>
    [Tooltip("The text which should be showing the amount of the reward")]
    public Text rewardAmount;

    /// <summary>
    /// The recttransform of this rewardUILogic object
    /// </summary>
    [Tooltip("The recttransform of this rewardUILogic object")]
    public RectTransform rectTransform;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
}