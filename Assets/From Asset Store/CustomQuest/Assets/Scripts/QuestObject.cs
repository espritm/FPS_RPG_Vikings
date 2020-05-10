using UnityEngine;

/// <summary>
/// Component for a questObject. Holds a reference to the criteria its a part of.
/// Used on all quest objects, enemies, gather objects etc.
/// </summary>
public class QuestObject : MonoBehaviour
{
    /// <summary>
    /// A reference to the criteria.
    /// </summary>
    public Criteria criteria;
}