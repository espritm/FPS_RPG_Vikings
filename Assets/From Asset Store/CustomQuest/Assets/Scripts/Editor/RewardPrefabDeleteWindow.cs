using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window used to display a confirmation pop up, when deleting a criteria.
/// </summary>
public class RewardPrefabDeleteWindow : EditorWindow
{
    public static RewardPrefabDeleteWindow Instance { get { return GetWindow<RewardPrefabDeleteWindow>(); } }

    private CustomQuestEditor questEditor;
    private Reward rewardToDelete;

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
        EditorGUILayout.LabelField("Do you wish to delete " + rewardToDelete.rewardName + "?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("No")) this.Close();
        if (GUILayout.Button("Yes") || Event.current.keyCode == (KeyCode.Return))
        {
            questEditor.DeleteRewardPrefab(rewardToDelete);
            questEditor.R_rewardPrefabList.index = 0;
            Close();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the editor controlling this window, and the reward about to be deleted
    /// </summary>
    /// <param name="editor">The editor which spawned this window</param>
    /// <param name="r">The reward about to be deleted</param>
    public void SetQuestEditor(CustomQuestEditor editor, Reward r)
    {
        questEditor = editor;
        rewardToDelete = r;
    }
}