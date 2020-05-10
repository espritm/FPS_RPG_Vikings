using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using CustomQuest;

/// <summary>
/// The settings window
/// </summary>
public class SettingsPopUp : EditorWindow
{

    public static SettingsPopUp Instance { get { return GetWindow<SettingsPopUp>(); } }

    private CustomQuestEditor questEditor;

    /// <summary>
    /// The GUI code for this editor window
    /// </summary>
    private void OnGUI()
    {
        if (CustomQuestSettings.RandomDragonGUISkin)
        {
            GUI.color = new Color32(105, 105, 105, 255);
            GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
            GUI.skin = CustomQuestSettings.RandomDragonGUISkin;
            GUI.color = new Color32(194, 194, 194, 255);
            GUILayout.BeginArea(new Rect(5, 5, position.width - 10, position.height - 10), GUI.skin.box);
            GUI.color = Color.white;
            //leftScrollview = EditorGUILayout.BeginScrollView(leftScrollview, GUILayout.Width(195), GUILayout.Height(position.height - 20));

            EditorGUILayout.LabelField("Change Settings in here");
            CustomQuestSettings.ShowQuestName = EditorGUILayout.Toggle("Show Quest Name", CustomQuestSettings.ShowQuestName/*, GUI.skin.GetStyle("Toggle")*/); //TODO: Use or not use style
            CustomQuestSettings.ShowDescription = EditorGUILayout.Toggle("Show Description", CustomQuestSettings.ShowDescription);
            CustomQuestSettings.ShowCriterias = EditorGUILayout.Toggle("Show Criterias", CustomQuestSettings.ShowCriterias);
            CustomQuestSettings.ShowRewards = EditorGUILayout.Toggle("Show Rewards", CustomQuestSettings.ShowRewards);
            CustomQuestSettings.CriteriaSpecificRewards = EditorGUILayout.Toggle("Enable criteria specific rewards", CustomQuestSettings.CriteriaSpecificRewards);
            CustomQuestSettings.HideOptional = EditorGUILayout.Toggle(new GUIContent("Hide Optional", "Click here to toggle whether to hide or show the optional criteras and rewards in quests"), CustomQuestSettings.HideOptional);
            CustomQuestSettings.SettingsHolder.questGiverPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Quest Giver Prefab", "The Quest Giver Prefab"), CustomQuestSettings.SettingsHolder.questGiverPrefab, typeof(GameObject), false);
            CustomQuestSettings.SettingsHolder.handInObjectPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("handInObjectPrefab", "The handInObjectPrefab"), CustomQuestSettings.SettingsHolder.handInObjectPrefab, typeof(GameObject), false);
            CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("criteriaSpawnPrefab", "The criteriaSpawnPrefab"), CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab, typeof(GameObject), false);
            CustomQuestSettings.RandomDragonGUISkin = (GUISkin)EditorGUILayout.ObjectField(new GUIContent("GUISkin", "The GUISkin CustomQuest will use. RandomDragonGUISkin is recommended"), CustomQuestSettings.RandomDragonGUISkin, typeof(GUISkin), false);
            GUILayout.EndArea();
        }
        else
        {
            EditorGUILayout.LabelField("Change Settings in here");
            CustomQuestSettings.ShowQuestName = EditorGUILayout.Toggle(new GUIContent("Show Quest Name", "Whether to show quest name in the questList UI"), CustomQuestSettings.ShowQuestName);
            CustomQuestSettings.ShowDescription = EditorGUILayout.Toggle("Show Description", CustomQuestSettings.ShowDescription);
            CustomQuestSettings.ShowCriterias = EditorGUILayout.Toggle("Show Criterias", CustomQuestSettings.ShowCriterias);
            CustomQuestSettings.ShowRewards = EditorGUILayout.Toggle("Show Rewards", CustomQuestSettings.ShowRewards);
            CustomQuestSettings.CriteriaSpecificRewards = EditorGUILayout.Toggle("Enable criteria specific rewards", CustomQuestSettings.CriteriaSpecificRewards);
            CustomQuestSettings.HideOptional = EditorGUILayout.Toggle(new GUIContent("Hide Optional Criterias", "Click here to toggle whether to hide or show the optional criteras in quests"), CustomQuestSettings.HideOptional);
            CustomQuestSettings.HideOptional = EditorGUILayout.Toggle(new GUIContent("Hide Optional Rewards", "Click here to toggle whether to hide or show the optional rewards in quests"), CustomQuestSettings.HideOptional);
            CustomQuestSettings.SettingsHolder.questGiverPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Quest Giver Prefab", "The Quest Giver Prefab"), CustomQuestSettings.SettingsHolder.questGiverPrefab, typeof(GameObject), false);
            CustomQuestSettings.SettingsHolder.handInObjectPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("handInObjectPrefab", "The handInObjectPrefab"), CustomQuestSettings.SettingsHolder.handInObjectPrefab, typeof(GameObject), false);
            CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab = (GameObject)EditorGUILayout.ObjectField(new GUIContent("criteriaSpawnPrefab", "The criteriaSpawnPrefab"), CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab, typeof(GameObject), false);
            CustomQuestSettings.RandomDragonGUISkin = (GUISkin)EditorGUILayout.ObjectField(new GUIContent("GUISkin", "The GUISkin CustomQuest will use. RandomDragonGUISkin is recommended"), CustomQuestSettings.RandomDragonGUISkin, typeof(GUISkin), false);
        }
    }
}
