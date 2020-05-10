using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor.SceneManagement;

#endif

/// <summary>
/// The node for displaying a quest in scene. Contains Edges and a rectangle for pos and size.
/// </summary>
public class QuestNode : ScriptableObject
{
    #region Fields

    /// <summary>
    /// The rectangle of this questNode
    /// </summary>
    private Rect rectangle = new Rect(100, 100, 130, 100);

    /// <summary>
    /// The ID of this questNode
    /// </summary>
    public int windowID;

    /// <summary>
    /// The quest this quetsNode is attached to
    /// </summary>
    public Quest quest;

    /// <summary>
    /// All the edges this quetsnode has
    /// </summary>
    public List<QuestEdge> allEdges = new List<QuestEdge>();

    /// <summary>
    /// The start edge for this questNode
    /// </summary>
    public QuestEdge startEdge;

    /// <summary>
    /// The completede edge for this questNode
    /// </summary>
    public QuestEdge completeEdge;

    /// <summary>
    /// The fail edge for this questNode
    /// </summary>
    public QuestEdge failEdge;

    /// <summary>
    /// The rectangle of this questNode
    /// </summary>
    public Rect Rectangle
    {
        get { return rectangle; }
        set
        {
            rectangle = value;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                if (rectangle != quest.rectangleNode)
                {
                    quest.rectangleNode = rectangle;
                    if (!EditorSceneManager.GetActiveScene().isDirty)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }
#endif
        }
    }

    #endregion Fields

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public void Start()
    {
        if (startEdge == null)
        {
            startEdge = CreateInstance<QuestEdge>();
            startEdge.rect = new Rect(5, 4, 10, 10);
            startEdge.questNode = this;
            startEdge.left = true;
            allEdges.Add(startEdge);
        }

        if (completeEdge == null)
        {
            completeEdge = CreateInstance<QuestEdge>();
            completeEdge.rect = new Rect(Rectangle.width - 14, 20, 10, 10);
            completeEdge.questNode = this;
            completeEdge.left = false;
            allEdges.Add(completeEdge);
        }

        if (failEdge == null)
        {
            failEdge = CreateInstance<QuestEdge>();
            failEdge.rect = new Rect(Rectangle.width - 14, 40, 10, 10);
            failEdge.questNode = this;
            failEdge.left = false;
            allEdges.Add(failEdge);
        }
    }
}