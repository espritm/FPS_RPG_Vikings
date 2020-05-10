using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window used to display a confirmation pop up, when deleting a criteria.
/// </summary>
public class CriteriaPrefablDeleteWindow : EditorWindow
{
    public static CriteriaPrefablDeleteWindow Instance { get { return GetWindow<CriteriaPrefablDeleteWindow>(); } }

    private CustomQuestEditor questEditor;
    private Criteria criteraToDelete;

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
        EditorGUILayout.LabelField("Do you wish to delete " + criteraToDelete.criteriaName + "?", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        if (GUILayout.Button("No")) this.Close();
       
        if (GUILayout.Button("Yes") || Event.current.keyCode == (KeyCode.Return))
        {
            questEditor.DeleteCriteriaPrefab(criteraToDelete);
            questEditor.R_criteriaPrefabList.index = 0;
            Close();
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the editor controlling this window, and the criteria about to be deleted
    /// </summary>
    /// <param name="editor">The editor which spawned this window</param>
    /// <param name="c">The criteria about to be deleted</param>
    public void SetQuestEditor(CustomQuestEditor editor, Criteria c)
    {
        questEditor = editor;
        criteraToDelete = c;
    }
}