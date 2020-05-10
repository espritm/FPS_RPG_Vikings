using UnityEditor;
using UnityEngine;


/// <summary>
/// An editor window used to display a confirmation pop up, when converting a criteria.
/// </summary>
public class CustomCriteriaPopUp : EditorWindow
{
    public static CustomCriteriaPopUp Instance { get { return GetWindow<CustomCriteriaPopUp>(); } }

    private CustomQuestEditor questEditor;
    private Criteria criteriaToConvert;

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
        EditorGUILayout.LabelField("To Open the Criteria Script, you must first convert it to a custom script. It may take a minute.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        GUI.skin = null;
        if (GUILayout.Button("Close")) this.Close();
        if (GUILayout.Button("Convert") || Event.current.keyCode == (KeyCode.Return))
        {
            bool invalid = false;
            string criteriaName = questEditor.selectedQuest.name;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (criteriaName.Contains(c.ToString()))
                {
                    invalid = true;
                }
            }

            if (criteriaName == "" || criteriaName == "tmpCriteriaName" || criteriaName == "as")
            {
                invalid = true;
            }

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in assetPaths)
            {
                if (assetPath.Contains(".cs"))
                {
                    if (AssetDatabase.LoadMainAssetAtPath(assetPath).name == criteriaName)
                    {
                        invalid = true;
                    }
                }
            }

            if (invalid == false)
            {
                questEditor.ConvertToCustomCriteria(criteriaToConvert);
                this.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("The criteria must a valid name", "The current criteria name is not valid, it must differ from other scripts, and not contain invalid characters. (Or be named tmpCriteriaName)", "Okay");
            }
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the quest editor and the criteria about to be converted
    /// </summary>
    /// <param name="editor">The editor who spawned this window</param>
    /// <param name="c">The criteria being converted</param>
    public void SetQuestEditor(CustomQuestEditor editor, Criteria c)
    {
        questEditor = editor;
        criteriaToConvert = c;
    }
}