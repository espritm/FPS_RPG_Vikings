using UnityEditor;
using UnityEngine;

/// <summary>
/// An editor window used to display a confirmation pop up, when converting a reward.
/// </summary>
public class CustomRewardPopUp : EditorWindow
{
    public static CustomRewardPopUp Instance { get { return GetWindow<CustomRewardPopUp>(); } }

    private CustomQuestEditor questEditor;
    private Reward rewardToConvert;

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
        EditorGUILayout.LabelField("To Open a Reward Script, you must convert it to a custom script. It may take a minute.", EditorStyles.wordWrappedLabel);
        GUILayout.Space(10);
        GUI.skin = null;
        if (GUILayout.Button("Close")) this.Close();
        if (GUILayout.Button("Convert") || Event.current.keyCode == (KeyCode.Return))
        {
            bool invalid = false;
            string rewardName = questEditor.selectedQuest.name;
            foreach (char c in System.IO.Path.GetInvalidFileNameChars())
            {
                if (rewardName.Contains(c.ToString()))
                {
                    invalid = true;
                }
            }

            if (rewardName == "" || rewardName == "tmpRewardName" || rewardName == "as")
            {
                invalid = true;
            }

            string[] assetPaths = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in assetPaths)
            {
                if (assetPath.Contains(".cs"))
                {
                    if (AssetDatabase.LoadMainAssetAtPath(assetPath).name == rewardName)
                    {
                        invalid = true;
                    }
                }
            }

            if (invalid == false)
            {
                questEditor.ConvertToCustomReward(rewardToConvert, questEditor.selectedQuest); this.Close();
            }
            else
            {
                EditorUtility.DisplayDialog("The Reward must a valid name", "The current reward name is not valid, it must differ from other scripts, and not contain invalid characters. (Or be named tmpRewardName)", "Okay");
            }
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Sets the quest editor and the reward about to be converted
    /// </summary>
    /// <param name="editor">The editor who spawned this window</param>
    /// <param name="r">The reward being converted</param>
    public void SetQuestEditor(CustomQuestEditor editor, Reward r)
    {
        questEditor = editor;
        rewardToConvert = r;
    }
}