/// <summary>
/// The custom quest namespace. Contains a series of enums, used in the different classes:
/// "criteriaType", "rewardType", "editorRewardType", "editorCriteriaType"
/// </summary>
namespace CustomQuest
{
    /// <summary>
    /// Different types of criterias
    /// If you make a new type, add it here
    /// </summary>
    public enum criteriaType { Kill, Gather, Deliver }

    /// <summary>
    /// Different types of rewards
    /// If you make a new type, add it here
    /// </summary>
    public enum rewardType { Resource, Item }

    /// <summary>
    /// Different type of editor reward types. Used to determin, where in the editor a reward should be visible, and which list it should be added to
    /// </summary>
    public enum editorRewardType { Standard, Criteria, Optional }

    /// <summary>
    /// Different type of editor criteria types. Used to determin, where in the editor a criteria should be visible, and which list it should be added to
    /// </summary>
    public enum editorCriteriaType { Standard, Criteria, Optional }
}