using UnityEditor;
using UnityEngine;

/// <summary>
/// A confirmation window for when deleting a quest in scene (A quest node)
/// </summary>
public class NodeDeleteWindow : EditorWindow
{
    public static NodeDeleteWindow Instance { get { return GetWindow<NodeDeleteWindow>(); } }

    private CustomQuestEditor questEditor;
    private QuestNode nodeToDelete;

    /// <summary>
    /// The GUI code for this editor window
    /// </summary>
    private void OnGUI()
    {
        GUI.skin = questEditor.thisGUISkin;
        GUI.color = new Color32(105, 105, 105, 255);
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
        GUI.color = new Color32(194, 194, 194, 255);
        GUILayout.BeginArea(new Rect(5, 5, position.width - 10, position.height - 10), GUI.skin.box);
        GUI.color = Color.white;
        GUI.skin = null;
        EditorGUILayout.LabelField("Do you wish to delete " + nodeToDelete.quest.questName + "?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("No")) this.Close();
        if (GUILayout.Button("Yes") || Event.current.keyCode == (KeyCode.Return))
        {
            questEditor.deletingNode = true;
            questEditor.nodeToDelete = nodeToDelete;
            this.Close();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the quest editor and the quest node about to be deleted
    /// </summary>
    /// <param name="editor">The editor who spawned this window</param>
    /// <param name="qn">The quest node being deleted</param>
    public void SetQuestEditor(CustomQuestEditor editor, QuestNode qn)
    {
        questEditor = editor;
        nodeToDelete = qn;
    }
}