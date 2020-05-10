using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// An edge on a quest
/// </summary>
public class QuestEdge : ScriptableObject
{
    #region Fields

    /// <summary>
    /// The quest node this edge is on
    /// </summary>
    public QuestNode questNode;
    
    /// <summary>
    /// The connections this edge has
    /// </summary>
    public List<QuestConnection> connections = new List<QuestConnection>();

    /// <summary>
    /// If its on the left side of the node, or not. (Used for drawing lines correctly)
    /// </summary>
    public bool left;

    /// <summary>
    /// The rectangle of this edge
    /// </summary>
    public Rect rect;

    #endregion Fields
}