using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window used to display a confirmation pop up, when deleting a quest.
/// </summary>
public class QuestDeleteWindow : EditorWindow
{
    public static QuestDeleteWindow Instance { get { return GetWindow<QuestDeleteWindow>(); } }

    private CustomQuestEditor questEditor;
    private Quest questToDelete;

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
        EditorGUILayout.LabelField("Do you wish to delete " + questToDelete.questName + "?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("No")) this.Close();
        if (GUILayout.Button("Yes") || Event.current.keyCode == (KeyCode.Return))
        {
            questEditor.DeleteQuest(questToDelete);
            questEditor.R_questPrefabList.index = 0;
            Close();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the quest editor and the quest about to be converted
    /// </summary>
    /// <param name="editor">The editor who spawned this window</param>
    /// <param name="q">The quest being converted</param>
    public void SetQuestEditor(CustomQuestEditor editor, Quest q)
    {
        questEditor = editor;
        questToDelete = q;
    }
}