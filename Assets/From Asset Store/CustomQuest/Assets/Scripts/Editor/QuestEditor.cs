using CustomQuest;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// Contains the editor logic for displaying all the info about a quest. It also contains info about Criteria and Reward components attached to this quest.
/// If you make new fields in the quest, which you want to acces through the inspector, this is where you would write a way to display it.
/// </summary>
[CustomEditor(typeof(Quest), true)]
public class QuestEditor : Editor
{
    #region Field

    private Quest questScript;

    protected GUIStyle foldOutStyle;

    protected GUIStyle headLineStyle;

    public ReorderableList R_CriteriaList = null;

    public ReorderableList R_RewardList = null;

    public ReorderableList R_OptionalCriteriaList = null;

    public ReorderableList R_OptionalRewardsList = null;

    private bool dontDespawnObjectsOnLevelChangeToggler; //#Worst name ever TODO: fix?

    private Criteria criteriaToAddReward;

    private GUISkin thisGUISkin;

    #endregion Field

    private void OnEnable()
    {
        questScript = (Quest)target;
        CustomQuestSettings.Start();
        thisGUISkin = CustomQuestSettings.RandomDragonGUISkin;
        foreach (Criteria c in questScript.GetComponents<Criteria>())
        {
            if (!questScript.criterias.Contains(c) && c.editorType == editorCriteriaType.Standard)
            {
                questScript.criterias.Add(c);
            }
            c.hideFlags = HideFlags.HideInInspector;
            foreach (Reward r in c.rewards)
            {
                r.hideFlags = HideFlags.HideInInspector;
            }
        }
        foreach (Reward r in questScript.GetComponents<Reward>())
        {
            if (!questScript.rewards.Contains(r) && r.editoreRewardType == editorRewardType.Standard)
            {
                questScript.rewards.Add(r);
            }
            r.hideFlags = HideFlags.HideInInspector;
        }

        R_CriteriaList = CreateCriteriaList(questScript.criterias);
        R_RewardList = CreateRewardList(questScript.rewards);
        R_OptionalCriteriaList = CreateCriteriaList(questScript.optionalCriterias);
        R_OptionalRewardsList = CreateRewardList(questScript.optionalRewards);
    }

    /// <summary>
    /// Runs the GUI logic of the quest
    /// </summary>
    public override void OnInspectorGUI()
    {
        #region FindSkin

        bool findSkin = false;
        if (thisGUISkin != null)
        {
            if (thisGUISkin.name != "RandomDragonGUISkin")
            {
                findSkin = true;
            }
            else
            {
                GUI.skin = thisGUISkin;
            }
        }
        else
        {
            findSkin = true;
        }

        if (findSkin)
        {
            thisGUISkin = CustomQuestSettings.RandomDragonGUISkin;
        }

        #endregion FindSkin

        #region Font style

        // base.OnInspectorGUI();
        foldOutStyle = new GUIStyle(EditorStyles.foldout);
        //CopyAll(EditorStyles.foldout, boldStyle); This creates an error :(
        foldOutStyle.fontStyle = thisGUISkin.GetStyle("Foldout").fontStyle;
        foldOutStyle.font = thisGUISkin.GetStyle("Foldout").font;
        foldOutStyle.fontSize = thisGUISkin.GetStyle("Foldout").fontSize;
        foldOutStyle.padding = thisGUISkin.GetStyle("Foldout").padding;
        foldOutStyle.normal.textColor = thisGUISkin.GetStyle("Foldout").normal.textColor;
        foldOutStyle.active.textColor = thisGUISkin.GetStyle("Foldout").active.textColor;
        foldOutStyle.hover.textColor = thisGUISkin.GetStyle("Foldout").hover.textColor;
        headLineStyle = new GUIStyle();
        headLineStyle.fontStyle = EditorStyles.label.fontStyle;
        headLineStyle.font = EditorStyles.label.font;
        headLineStyle.focused = EditorStyles.label.focused;
        headLineStyle.active = EditorStyles.label.active;
        headLineStyle.normal = EditorStyles.label.normal;
        headLineStyle.fontSize = 30;

        #endregion Font style

        questScript = (Quest)target;

        questScript.showDefault = EditorGUILayout.Foldout(questScript.showDefault, "Default Inspector", true, foldOutStyle); //Draws the default inspector, should that be needed
        if (questScript.showDefault)
        {
            DrawDefaultInspector();
        }

        //GUI.color = new Color(0.0f, 0.10f, 0.75f, 0.3f);
        //GUI.color = new Color32(254, 252, 224, 255);
        EditorGUILayout.BeginVertical(/*thisGUISkin.box*/);
        GUI.color = Color.white;

        #region Name / Icon / Select prefab button

        GUI.color = new Color(0.0f, 0.10f, 0.75f, 0.5f);
        GUI.color = Color.white;
        EditorGUILayout.BeginHorizontal();
        GUI.skin = null;
        questScript.questIcon = (Sprite)EditorGUILayout.ObjectField(questScript.questIcon, typeof(Sprite), true, GUILayout.MinHeight(38), GUILayout.Width(38));
        EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The icon of this quest"));
        GUI.skin = thisGUISkin;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        headLineStyle.wordWrap = true;
        questScript.questName = EditorGUILayout.TextArea(questScript.questName, thisGUISkin.GetStyle("TitleName"), GUILayout.Height(thisGUISkin.GetStyle("TitleName").CalcHeight(new GUIContent(questScript.questName), EditorGUIUtility.currentViewWidth - 170)), GUILayout.Width(EditorGUIUtility.currentViewWidth - 170));
        headLineStyle.wordWrap = false;
        EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The name of this quest"));
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();

//        if ((PrefabUtility.GetPrefabObject(questScript.gameObject) == null))
        if(PrefabUtility.GetPrefabInstanceHandle(questScript.gameObject) == null)
        {
            GUI.skin = null;
            GUI.skin.button.wordWrap = true;
            if (GUILayout.Button(new GUIContent("Select Prefab", "Click here to select the prefab"), GUILayout.Width(70), GUILayout.MinHeight(34)))
            {
                if (questScript.prefab != null)
                {
                    Object foundAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(questScript.prefab), typeof(Object));
                    if (foundAsset != null) { Selection.activeObject = foundAsset; }
                }
                else
                {
                    Debug.Log("Null");
                }
            }
            GUI.skin.button.wordWrap = false;
            GUI.skin = thisGUISkin;
        }
        EditorGUILayout.EndHorizontal();


        #endregion Name / Icon / Select prefab button

        #region Description / Tooltip

        EditorGUI.indentLevel += 1;
        questScript.showDescription = EditorGUILayout.Foldout(questScript.showDescription, new GUIContent("Description", "The longer description of this quests"), true, foldOutStyle);
        EditorGUI.indentLevel -= 1;
        if (questScript.showDescription)
        {
            EditorGUILayout.BeginHorizontal();
            questScript.description = EditorGUILayout.TextArea(questScript.description, GUILayout.ExpandHeight(true), GUILayout.Width(EditorGUIUtility.currentViewWidth - 45));
            EditorGUILayout.EndHorizontal();
        }

        EditorGUI.indentLevel += 1; //Tooltip
        questScript.showTooltip = EditorGUILayout.Foldout(questScript.showTooltip, new GUIContent("Tooltip", "Small description of this quest, used for hover tooltip"), true, foldOutStyle);
        EditorGUI.indentLevel -= 1;
        if (questScript.showTooltip)
        {
            questScript.toolTip = EditorGUILayout.TextArea(questScript.toolTip, GUILayout.ExpandHeight(true), GUILayout.Width(EditorGUIUtility.currentViewWidth - 45));
        }

        #endregion Description / Tooltip

        #region Settings

        EditorGUI.indentLevel += 1; //Quest settings
        questScript.showSettings = EditorGUILayout.Foldout(questScript.showSettings, new GUIContent("Quest Setings", "Settings for the quests"), true, foldOutStyle);
        EditorGUI.indentLevel -= 1;
        if (questScript.showSettings)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Start Availability", "If true, will be avaible to the players when it is initiated"), GUILayout.Width(150)); //TODO: Make the bool "Light up" when clicking on the names (like normal inspector)
            questScript.startAvailability = EditorGUILayout.Toggle(questScript.startAvailability, GUILayout.Width(15));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Constant Availability", "If true, will be avaible to all players always. Until completed. Unless repeaterable is true!"), GUILayout.Width(150));
            questScript.constantAvailability = EditorGUILayout.Toggle(questScript.constantAvailability, GUILayout.Width(15));

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Auto Complete", "If true, will complete as soon as all the criterias are done"), GUILayout.Width(150));
            questScript.autoComplete = EditorGUILayout.Toggle(questScript.autoComplete, GUILayout.MaxWidth(15));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Pick Up Able", "If the player is able to pick this quest up at a quest giver"), GUILayout.Width(150));
            questScript.pickUpAble = EditorGUILayout.Toggle(questScript.pickUpAble, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Don't Delete", "If true, this quest will never be deleted."), GUILayout.Width(150));
            questScript.dontDelete = EditorGUILayout.Toggle(questScript.dontDelete, GUILayout.Width(15));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Single Complete", "If true, this quest can only ever be completed once, by one player"), GUILayout.Width(150));
            questScript.singleComplete = EditorGUILayout.Toggle(questScript.singleComplete, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Timed", "If true, this quest will be timed. If the timer runs out, the quest fails"), GUILayout.Width(150));
            questScript.timed = EditorGUILayout.Toggle(questScript.timed, GUILayout.Width(15));
            if (questScript.timed)
            {
                questScript.time = EditorGUILayout.FloatField(questScript.time);
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The time in seconds the player have to complete this quest"));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Repeatable", "If true, this quest can be picket up and completed multiple times by the same player"), GUILayout.Width(150));
            questScript.repeatable = EditorGUILayout.Toggle(questScript.repeatable, GUILayout.Width(15));
            if (questScript.repeatable)
            {
                questScript.repeatableTime = EditorGUILayout.FloatField(questScript.repeatableTime);
                EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The time in seconds the player have to complete this quest"));
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Start Spawn on Discover", "If true, this quest will start spawning it's criteria objects, when it's available"), GUILayout.Width(150)); //TODO: Make these multiple choice? "What to do on discover..."
            questScript.startSpawningOnDiscover = EditorGUILayout.Toggle(questScript.startSpawningOnDiscover, GUILayout.Width(15));
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("No Spawn If No Player", "If true, this quest will not spawn it's criteria if no player has the quest"), GUILayout.Width(150));
            questScript.noSpawnIfNoPlayer = EditorGUILayout.Toggle(questScript.noSpawnIfNoPlayer, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Set all criterias despawn", "Sets all criteria dontDepawnOnCompletion to this bool"), GUILayout.Width(150));
            dontDespawnObjectsOnLevelChangeToggler = EditorGUILayout.Toggle(dontDespawnObjectsOnLevelChangeToggler, GUILayout.Width(15));
            GUI.skin = null;
            GUI.skin.button.wordWrap = true;
            if (GUILayout.Button(new GUIContent("Set all", "Sets all criteria dontDepawnOnCompletion"), GUILayout.Width(70)))
            {
                foreach (Criteria c in questScript.criterias)
                {
                    c.dontDespawnObjectsWhenComplete = dontDespawnObjectsOnLevelChangeToggler;
                }
            }
            GUI.skin = thisGUISkin;
            GUI.skin.button.wordWrap = false;
            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Match criteria levels", "If the quest should match criteria levels with optional criteria levels. So when a criteria level is done, optional criterias levels up aswell"), GUILayout.Width(150));
            questScript.matchOptionalLevels = EditorGUILayout.Toggle(questScript.matchOptionalLevels, GUILayout.Width(15));
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        #endregion Settings

        #region References
        GUI.skin = null;
        EditorGUI.indentLevel += 1; //Quest relations
        questScript.showRelations = EditorGUILayout.Foldout(questScript.showRelations, new GUIContent("Relations", "Different relations for this quest"), true, foldOutStyle);
        EditorGUI.indentLevel -= 1;
        if (questScript.showRelations)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal(); //Add quest giver
            if (GUILayout.Button(new GUIContent("Add Quest Giver", "Click here to spawn a quest giver, giving this quest")))
            {
                GenericMenu menu = new GenericMenu();
                if (CustomQuestSettings.SettingsHolder.questGiverPrefab)
                {
                    menu.AddItem(new GUIContent("New Quest Giver"), false, CreateNewQuestGiverCallBack, questScript);
                }
                int counter = 0;
                foreach (QuestGiver qG in Resources.FindObjectsOfTypeAll(typeof(QuestGiver)))
                {
                    if (qG.gameObject.activeInHierarchy)
                    {
                        counter++;
                        menu.AddItem(new GUIContent(counter + " " + qG.name), false, AddNewQuestGiverCallBack, qG);
                    }
                }
                foreach (HandInObject hO in Resources.FindObjectsOfTypeAll(typeof(HandInObject)))
                {
                    if (hO.gameObject.activeInHierarchy)
                    {
                        if (!hO.GetComponent<QuestGiver>() || !hO.GetComponentInChildren<QuestGiver>())
                        {
                            counter++;
                            menu.AddItem(new GUIContent(counter + " " + hO.name), false, AddQuestGiverToHandInCallBack, hO);
                        }
                    }
                }
                menu.ShowAsContext();
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < questScript.questGivers.Count; i++)
            {
                if (questScript.questGivers[i])
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    questScript.questGivers[i] = (QuestGiver)EditorGUILayout.ObjectField(questScript.questGivers[i], typeof(QuestGiver), true, GUILayout.Width(150));
                    questScript.questGivers[i].radius = EditorGUILayout.FloatField(questScript.questGivers[i].radius);
                    questScript.questGivers[i].declineDistance = EditorGUILayout.FloatField(questScript.questGivers[i].declineDistance);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("-", "Click here to remove this quest from the Quest Giver"), GUILayout.Width(20)))
                    {
                        questScript.questGivers[i].quests.Remove(questScript);
                        questScript.questGivers.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    questScript.questGivers.RemoveAt(i);
                }
            }

            EditorGUILayout.BeginHorizontal(); //Hand in Object
            if (GUILayout.Button(new GUIContent("Add handInObject", "Click here to add a new handInObject to this quest")/*, GUILayout.Width(150)*/))
            {
                GenericMenu menu = new GenericMenu();
                if (CustomQuestSettings.SettingsHolder.handInObjectPrefab)
                {
                    menu.AddItem(new GUIContent("New HandInObject"), false, CreateNewHandInObjectCallBack, questScript);
                }
                int counter = 0;
                foreach (HandInObject hO in Resources.FindObjectsOfTypeAll(typeof(HandInObject)))
                {
                    if (hO.gameObject.activeInHierarchy)
                    {
                        counter++;
                        menu.AddItem(new GUIContent(counter + " " + hO.name), false, AddNewHandInObjectCallBack, hO);
                    }
                }
                foreach (QuestGiver qG in Resources.FindObjectsOfTypeAll(typeof(QuestGiver)))
                {
                    if (qG.gameObject.activeInHierarchy)
                    {
                        if (!qG.GetComponent<HandInObject>() || !qG.GetComponentInChildren<HandInObject>())
                        {
                            counter++;
                            menu.AddItem(new GUIContent(counter + " " + qG.name), false, AddHandInObjectToQuestGiverCallBack, qG);
                        }
                    }
                }
                menu.ShowAsContext();
            }
            //GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
            for (int i = 0; i < questScript.handInObjects.Count; i++)
            {
                if (questScript.handInObjects[i])
                {
                    EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    questScript.handInObjects[i] = (HandInObject)EditorGUILayout.ObjectField(questScript.handInObjects[i], typeof(HandInObject), true, GUILayout.Width(150));
                    questScript.handInObjects[i].Radius = EditorGUILayout.FloatField(questScript.handInObjects[i].Radius);
                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button(new GUIContent("-", "Click here to delete this from the handInObject"), GUILayout.Width(20)))
                    {
                        questScript.handInObjects[i].quests.Remove(questScript);
                        questScript.handInObjects.RemoveAt(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                else
                {
                    questScript.handInObjects.RemoveAt(i);
                }
            }
            EditorGUILayout.EndVertical();
        }

        GUI.skin = thisGUISkin;
        #endregion References

        #region Criterias

        headLineStyle.fontSize = 20;
        EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Criterias", "A list of criterias for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(26));
        GUI.skin = null;
        if (GUILayout.Button(new GUIContent("Add Criteria", "Click to add a criteria to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Empty"), false, CreateNewCriteriaCallBack, questScript);
            foreach (Criteria c in CustomQuestSettings.EditorCriteras)
            {
                menu.AddItem(new GUIContent(c.criteriaName), false, AddNewCriteriaCallBack, c);
            }
            menu.ShowAsContext();
        }
        GUI.skin = thisGUISkin;
        EditorGUILayout.EndHorizontal();
        if (R_CriteriaList == null) { OnEnable(); }
        GUI.skin = null;
        R_CriteriaList.DoLayoutList();
        GUI.skin = thisGUISkin;

        EditorGUI.indentLevel += 1;
        questScript.showThresholds = EditorGUILayout.Foldout(questScript.showThresholds, new GUIContent("Tresholds", "Click here to show the thresholds for the different levels of criterias"), true, foldOutStyle);
        EditorGUI.indentLevel -= 1;
        if (questScript.showThresholds)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
            EditorGUILayout.LabelField(new GUIContent("Criteria Thresholds", "A list over the different thresholds for the different levels of criterias"));
            int maxLevel = 0;
            if (Event.current.type == EventType.Layout)
            {
                foreach (Criteria c in questScript.criterias)
                {
                    if (questScript.thresholds.Count <= c.Level)
                    {
                        questScript.thresholds.Add(0);
                    }
                    if (c.Level > maxLevel)
                    {
                        maxLevel = c.Level;
                    }
                }
                maxLevel += 1;
                if (maxLevel < questScript.thresholds.Count)
                {
                    questScript.thresholds.RemoveRange(maxLevel, questScript.thresholds.Count - maxLevel);
                }
            }

            for (int i = 0; i < questScript.thresholds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
                questScript.thresholds[i] = EditorGUILayout.IntField(questScript.thresholds[i]);
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();

        #endregion Criterias

        #region Rewards

        EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Rewards
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(new GUIContent("Rewards", "A list of rewards for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(26));
        GUI.skin = null;
        if (GUILayout.Button(new GUIContent("Add Reward", "Click to add a reward to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Empty"), false, CreateNewRewardCallBack, questScript);
            foreach (Reward r in CustomQuestSettings.EditorRewards)
            {
                menu.AddItem(new GUIContent(r.rewardName), false, AddNewRewardCallBack, r);
            }
            menu.ShowAsContext();
        }
        GUI.skin = thisGUISkin;
        EditorGUILayout.EndHorizontal();
        if (R_RewardList == null) { OnEnable(); }
        R_RewardList.DoLayoutList();
        EditorGUILayout.EndVertical();

        #endregion Rewards

        #region Optional Criterias

        if (!CustomQuestSettings.SettingsHolder.optional)
        {
            headLineStyle.fontSize = 20;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Optional Criterias", "A list of optional criterias for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(thisGUISkin.GetStyle("Title").CalcHeight(new GUIContent("Optional Criterias"), EditorGUIUtility.currentViewWidth - 210)), GUILayout.Width(EditorGUIUtility.currentViewWidth - 210));
            GUI.skin = null;
            if (GUILayout.Button(new GUIContent("Add Criteria", "Click to add an optional criteria to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Empty"), false, CreateNewOptionalCriteriaCallBack, questScript);
                foreach (Criteria c in CustomQuestSettings.EditorCriteras)
                {
                    menu.AddItem(new GUIContent(c.criteriaName), false, AddNewOptionalCriteriaCallBack, c);
                }
                menu.ShowAsContext();
            }
            GUI.skin = thisGUISkin;
            EditorGUILayout.EndHorizontal();
            if (R_OptionalCriteriaList == null) { OnEnable(); }
            R_OptionalCriteriaList.DoLayoutList();

            EditorGUI.indentLevel += 1;
            questScript.showOptionalThresholds = EditorGUILayout.Foldout(questScript.showOptionalThresholds, new GUIContent("Optional Tresholds", "Click here to show the thresholds for the different levels of criterias"), true, foldOutStyle);
            EditorGUI.indentLevel -= 1;
            if (questScript.showOptionalThresholds)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
                EditorGUILayout.LabelField(new GUIContent("Optional Criteria Thresholds", "A list over the different optional thresholds for the different levels of criterias"));
                int maxLevel = 0;
                if (Event.current.type == EventType.Layout)
                {
                    foreach (Criteria c in questScript.optionalCriterias)
                    {
                        if (questScript.optionalThresholds.Count <= c.Level)
                        {
                            questScript.optionalThresholds.Add(0);
                        }
                        if (c.Level > maxLevel)
                        {
                            maxLevel = c.Level;
                        }
                    }
                    maxLevel += 1;
                    if (maxLevel < questScript.optionalThresholds.Count)
                    {
                        questScript.optionalThresholds.RemoveRange(maxLevel, questScript.optionalThresholds.Count - maxLevel);
                    }
                }

                for (int i = 0; i < questScript.optionalThresholds.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
                    questScript.optionalThresholds[i] = EditorGUILayout.IntField(questScript.optionalThresholds[i]);
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndVertical();
        }

        #endregion Optional Criterias

        #region Optional Rewards

        if (!CustomQuestSettings.HideOptional)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Rewards
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("Optional Rewards", "A list of optional rewards for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(thisGUISkin.GetStyle("Title").CalcHeight(new GUIContent("Optional Rewards"), EditorGUIUtility.currentViewWidth - 240)), GUILayout.Width(EditorGUIUtility.currentViewWidth - 240));
            questScript.completedOptionalThreshold = EditorGUILayout.IntField(/*new GUIContent("", "The amount of criteria needed to get the optional rewards."),*/ questScript.completedOptionalThreshold, GUILayout.Height(22), GUILayout.Width(20), GUILayout.ExpandWidth(false));
            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The amount of criteria needed to get the optional rewards."));
            //TODO: Fix this layout, so it does not overlap
            GUILayout.FlexibleSpace();
            GUI.skin = null;
            if (GUILayout.Button(new GUIContent("Add Reward", "Click to add an optional reward to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
            {
                GenericMenu menu = new GenericMenu();
                menu.AddItem(new GUIContent("Empty"), false, CreateNewOptionalRewardCallBack, questScript);
                foreach (Reward r in CustomQuestSettings.EditorRewards)
                {
                    menu.AddItem(new GUIContent(r.rewardName), false, AddNewOptionalRewardCallBack, r);
                }
                menu.ShowAsContext();
            }
            GUI.skin = thisGUISkin;
            EditorGUILayout.EndHorizontal();
            if (R_OptionalRewardsList == null) { OnEnable(); }
            R_OptionalRewardsList.DoLayoutList();
            EditorGUILayout.EndVertical();
        }

        #endregion Optional Rewards

        GUILayout.Space(5);

        EditorGUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();

        if (GUI.changed) //Makes sure that changes made to a quest in a scene, is saved
        {
            if (!Application.isPlaying)
            {
                if (!EditorSceneManager.GetActiveScene().isDirty)
                {
                    EditorUtility.SetDirty(questScript);
#if UNITY_EDITOR
                    EditorSceneManager.MarkSceneDirty(questScript.gameObject.scene);
#endif
                }
            }
        }
    }

    /// <summary>
    /// Creates a new criteria and adds it to the quest
    /// </summary>
    /// <param name="q">The quest to add the new criteria to</param>
    private void CreateNewCriteria(Quest q)
    {
        Criteria c = q.gameObject.AddComponent<Criteria>();
        q.criterias.Add(c);
        //Sets the thresholds to avoid errors later
        int maxLevel = 0;
        foreach (Criteria cri in questScript.criterias)
        {
            if (questScript.thresholds.Count <= cri.Level)
            {
                questScript.thresholds.Add(0);
            }
            if (cri.Level > maxLevel)
            {
                maxLevel = cri.Level;
            }
        }
        maxLevel += 1;
        if (maxLevel < questScript.thresholds.Count)
        {
            questScript.thresholds.RemoveRange(maxLevel, questScript.thresholds.Count - maxLevel);
        }
    }

    /// <summary>
    /// Creates a new reward and adds it to the quest
    /// </summary>
    /// <param name="q">The quest to add the new reward to</param>
    private void CreateNewReward(Quest q)
    {
        Reward r = q.gameObject.AddComponent<Reward>();
        q.rewards.Add(r);
    }

    /// <summary>
    /// Creates a reordeable list for criterias and defines its settings and layout
    /// </summary>
    private ReorderableList CreateCriteriaList(List<Criteria> criteriaList)
    {
        ReorderableList R_list = new ReorderableList(criteriaList, typeof(Criteria), true, false, false, false);
        R_list.showDefaultBackground = false;
        R_list.headerHeight = 0;
        R_list.elementHeight = EditorGUIUtility.singleLineHeight * 4;
        R_list.footerHeight = -10;
        R_list.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            //if (isFocused)
            //{
            //    GUI.color = Color.blue;
            //}
            //else
            //{
            //    GUI.color = Color.red;
            //}
        };

        //Defining how criterias are displayed
        R_list.drawElementCallback =
    (Rect rect, int index, bool isActive, bool isFocused) =>
    {
        if (criteriaList.ElementAtOrDefault(index) != null)
        {
            Criteria c = criteriaList[index];
            GUI.color = new Color32(35, 120, 161, 255);
            float height = rect.height - 4;
            if (c.ShowSpawns)
            {
                foreach (SpawnZone zone in c.spawnZones)
                {
                    if (zone != null)
                    {
                        // height += EditorGUIUtility.singleLineHeight * 4 + 5;
                    }
                    else
                    {
                        c.spawnZones.Remove(zone);
                        DestroyImmediate(zone);
                    }
                }
            }

            if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
            {
                if (c.ShowRewards)
                {
                    foreach (Reward r in c.rewards)
                    {
                        if (r != null)
                        {
                            //height += EditorGUIUtility.singleLineHeight * 2 + 13;
                        }
                        else
                        {
                            c.rewards.Remove(r);
                        }
                    }
                }
            }
            //if (c.ShowSettings)
            //{
            //    height += EditorGUIUtility.singleLineHeight * 2;
            //}
            EditorGUI.HelpBox(new Rect(rect.x - 20, rect.y - 3, rect.width + 25, height), "", MessageType.None);

            #region Standard info

            GUI.color = Color.white;
            rect.y += 2;
            c.criteriaName = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight),
                 c.criteriaName);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 60, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this criteria"));

            c.Level = EditorGUI.IntField(new Rect(rect.x + rect.width - 60, rect.y, 30, EditorGUIUtility.singleLineHeight), c.Level);
            if (c.Level < 0)
            {
                c.Level = 0;
            }
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 60, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "The level of this criteria controlling when this criteria is avalible for completion. All level '0' will have to be completed, before level '1' will activate"));
            GUI.skin = null;
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight), "-"))
            {
                criteriaList.Remove(c);
                for (int i = 0; i < c.spawnZones.Count; i++)
                {
                    if (c.spawnZones[i])
                    {
                        if (c.spawnZones[i].gameObject)
                        {
                            DestroyImmediate(c.spawnZones[i].gameObject, true);
                        }
                        else
                        {
                            c.spawnZones.RemoveAt(i);
                            DestroyImmediate(c.spawnZones[i], true);
                        }
                    }
                    else
                    {
                        c.spawnZones.RemoveAt(i);
                    }
                }
                DestroyImmediate(c, true);
            }
            //GUI.skin = thisGUISkin;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this criteria"));

            c.type = (criteriaType)EditorGUI.EnumPopup(
                new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                c.type);
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "Change the criteria type"));

            c.amount = EditorGUI.IntField(
               new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
               c.amount);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "Amount of criteria objects to do, before criteria is completed"));

            c.criteriaObject = (GameObject)EditorGUI.ObjectField(
                 new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                c.criteriaObject, typeof(GameObject), true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The criteria object. The goblin to kill. The berries to collect, etc."));

            #endregion Standard info

            EditorGUI.indentLevel += 1;
            c.ShowSpawns = EditorGUI.Foldout(
                new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                c.ShowSpawns, "Show Spawns", true, foldOutStyle);
            if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
            {
                c.ShowRewards = EditorGUI.Foldout(
                    new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                    c.ShowRewards, "Show Rewards", true, foldOutStyle);
            }
            c.ShowSettings = EditorGUI.Foldout(
                new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                c.ShowSettings, "Show Settings", true, foldOutStyle);
            EditorGUI.indentLevel -= 1;
            float zoneHeight = rect.y + 3 + EditorGUIUtility.singleLineHeight * 3; ;
            switch (c.toolbarInt)
            {
                case 1: //Spawns
                    if (c.ShowSpawns)
                    {
                        #region Spawns

                        if (GUI.Button(new Rect(rect.x + rect.width / 3 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight + 5, EditorGUIUtility.singleLineHeight), new GUIContent("+", "Click here to add a spawn to this criteria")))
                        {
                            if (Resources.FindObjectsOfTypeAll(typeof(CustomQuestEditor)) != null)
                            {
                                if (CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab)
                                {
                                    GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab, c.gameObject.transform);
                                    if (go.GetComponent<SpawnZone>())
                                    {
                                        c.spawnZones.Add(go.GetComponent<SpawnZone>());
                                        go.GetComponent<SpawnZone>().Criteria = c;
                                        go.GetComponent<SpawnZone>().spawnAreaObject = go;
                                        go.GetComponent<SpawnZone>().SpawnName = "SpawnZone";
                                    }
                                    else
                                    {
                                        Debug.LogWarning("No spawnZone script was found in the spawnZonePrefab, please assign a prefab with the spawnZone scrip attached");
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning("spawnZonePrefab is null in customQuestEditor, please assign a prefab with the spawnZone script attached. If you have one assisgned, please close and open the editor window");
                                }
                            }
                        }

                        if (c.ShowSpawns)
                        {
                            foreach (SpawnZone zone in c.spawnZones)
                            {
                                if (zone == null)
                                {
                                    c.spawnZones.Remove(zone);
                                    break;
                                }
                                EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 4 + 5), "", MessageType.None);
                                zone.SpawnName = EditorGUI.TextField(
                                    new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight),
                                    zone.SpawnName);
                                EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this spawnZone"));
                                zone.Spawn = EditorGUI.Toggle(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), zone.Spawn);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click here to enable / disable spawning."));
                                if (GUI.Button(new Rect(rect.x + rect.width - 20, zoneHeight + 3, 20, EditorGUIUtility.singleLineHeight), new GUIContent("-", "Click here to delete this zone")))
                                {
                                    c.spawnZones.Remove(zone);
                                    DestroyImmediate(zone.gameObject);
                                    break;
                                }
                                zoneHeight += 5;
                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                    "      Object / Radius");
                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to spawn around, and the radius in which to spawn"));
                                zone.spawnAreaObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                    zone.spawnAreaObject, typeof(GameObject), true);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to spawn around"));
                                zone.spawnRadius = EditorGUI.FloatField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                    zone.spawnRadius);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The objects radius"));

                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                    "      Amount / Rate");
                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of objects to spawn each time, and how often to spawn that amount"));
                                zone.spawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                   zone.spawnAmount);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of object to spawn each time"));
                                zone.spawnRate = EditorGUI.FloatField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                  zone.spawnRate);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "How often to spawn objects, in seconds."));

                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                  "      Initial / Max");
                                EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The initial amount of objects to spawn, when this criteria starts spawning, and the max amount of objects there can be spawned at once"));
                                zone.initialSpawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                  zone.initialSpawnAmount);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The initial amount of objects to spawn, when this criteria starts spawning."));
                                zone.maxSpawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                  zone.maxSpawnAmount);
                                EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The max amount of objects there can be spawned at once"));
                                zoneHeight += EditorGUIUtility.singleLineHeight * 4;
                            }
                        }

                        #endregion Spawns
                    }
                    break;

                case 2: //Rewards
                    if (c.ShowRewards && CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                    {
                        #region Rewards

                        if (GUI.Button(new Rect(rect.x + rect.width / 3 * 2 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, EditorGUIUtility.singleLineHeight + 5, EditorGUIUtility.singleLineHeight), new GUIContent("+", "Click here to add a spawn to this criteria")))
                        {
                            criteriaToAddReward = c;
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Empty"), false, CreateNewCriteriaRewardCallBack, questScript);
                            foreach (Reward r in CustomQuestSettings.EditorRewards)
                            {
                                menu.AddItem(new GUIContent(r.rewardName), false, AddNewCriteriaRewardCallBack, r);
                            }
                            menu.ShowAsContext();
                        }
                        foreach (Reward r in c.rewards)
                        {
                            //GUI.color = new Color(0, 0.75f, 0.15f);
                            EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 2 + 13), "", MessageType.None);
                            GUI.color = Color.white;
                            r.rewardName = EditorGUI.TextField(
                                new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight),
                                 r.rewardName);
                            EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
                            if (GUI.Button(new Rect(rect.x + rect.width - 20, zoneHeight + 5, 20, EditorGUIUtility.singleLineHeight),
                                "-")) { c.rewards.Remove(r); DestroyImmediate(r); break; }
                            EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, zoneHeight + 5, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this reward"));
                            r.type = (rewardType)EditorGUI.EnumPopup(
                                new Rect(rect.x + 5, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3 - 5, EditorGUIUtility.singleLineHeight),
                                r.type);
                            EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3 - 5, EditorGUIUtility.singleLineHeight), new GUIContent("", "Change the reward type"));
                            r.amount = EditorGUI.IntField(
                               new Rect(rect.x + rect.width / 3, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                               r.amount);
                            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of the reward to give (100 gold, 1 sword...)"));
                            r.rewardObject = (GameObject)EditorGUI.ObjectField(
                                 new Rect(rect.x + rect.width / 3 * 2, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                                r.rewardObject, typeof(GameObject), true);
                            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to reward, if any"));
                            zoneHeight += EditorGUIUtility.singleLineHeight * 2 + 13;
                        }

                        #endregion Rewards
                    }
                    break;

                case 3: //Settings
                    if (c.ShowSettings)
                    {
                        #region Settings

                        EditorGUI.indentLevel += 1;

                        if (c.ShowSettings)
                        {
                            c.timed = EditorGUI.Toggle(new Rect(rect.x + 40, zoneHeight, 30, EditorGUIUtility.singleLineHeight), c.timed);
                            EditorGUI.LabelField(new Rect(rect.x, zoneHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("Timed", "If true, this criteria is timed. If time runs out, the criteria will fail. And maybe the entire quest!"));
                            if (c.timed) { c.time = EditorGUI.FloatField(new Rect(rect.x + 60, zoneHeight, 50, EditorGUIUtility.singleLineHeight), c.time); }
                            GUILayout.FlexibleSpace();
                            EditorGUI.LabelField(new Rect(rect.xMax - 170, zoneHeight, rect.width / 2, EditorGUIUtility.singleLineHeight), new GUIContent("Dont Despawn Objects", "If true, this criteria will not remove its spawned objects when its completed (and no player is current doing it)"));
                            c.dontDespawnObjectsWhenComplete = EditorGUI.Toggle(new Rect(rect.xMax - 35, zoneHeight, 30, EditorGUIUtility.singleLineHeight), c.dontDespawnObjectsWhenComplete);
                            zoneHeight += EditorGUIUtility.singleLineHeight;
                            EditorGUI.LabelField(new Rect(rect.x, zoneHeight, 185, EditorGUIUtility.singleLineHeight), new GUIContent("Give Rewards OnCompletion", "If true, will give this criterias rewards when this criteria is completed. Otherwise, it will give them when the quest is completed"));
                            c.giveRewardsOnCompletion = EditorGUI.Toggle(new Rect(rect.x + 170, zoneHeight, 30, EditorGUIUtility.singleLineHeight), c.giveRewardsOnCompletion);
                        }
                        EditorGUI.indentLevel -= 1;

                        #endregion Settings
                    }
                    break;
            }
        }
    };

        R_list.elementHeightCallback = (index) =>
            {
                Repaint();
                float height = EditorGUIUtility.singleLineHeight * 4;

                if (criteriaList[index])
                {
                    if (criteriaList[index].ShowSpawns)
                    {
                        foreach (SpawnZone zone in criteriaList[index].spawnZones)
                        {
                            if (zone)
                            {
                                height += EditorGUIUtility.singleLineHeight * 4 + 5;
                            }
                        }
                    }
                }
                else
                {
                    criteriaList.Remove(criteriaList[index]);
                }
                if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                {
                    if (criteriaList[index].ShowRewards)
                    {
                        foreach (Reward r in criteriaList[index].rewards)
                        {
                            height += EditorGUIUtility.singleLineHeight * 2 + 13;
                        }
                    }
                }
                if (criteriaList[index].ShowSettings)
                {
                    height += EditorGUIUtility.singleLineHeight * 2;
                }
                return height;
            };

        return R_list;
    }

    /// <summary>
    /// Creates a reordeable list for rewards and defines its settings and layout
    /// </summary>
    private ReorderableList CreateRewardList(List<Reward> rewardsList)
    {
        ReorderableList R_List = new ReorderableList(rewardsList, typeof(Reward), true, false, false, false);
        R_List.showDefaultBackground = false;
        R_List.headerHeight = 0;
        R_List.elementHeight = EditorGUIUtility.singleLineHeight * 3;
        R_List.footerHeight = -10;
        R_List.drawElementBackgroundCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
        };

        R_List.drawElementCallback = //Defining how criterias are displayed
    (Rect rect, int index, bool isActive, bool isFocused) =>
    {
        if (rewardsList.ElementAtOrDefault(index) != null)
        {
            Reward r = rewardsList[index];
            GUI.color = new Color32(35, 120, 161, 255);
            EditorGUI.HelpBox(new Rect(rect.x - 20, rect.y - 3, rect.width + 25, rect.height - 5), "", MessageType.None);
            GUI.color = Color.white;
            rect.y += 2;
            r.rewardName = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
                 r.rewardName);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
            GUI.skin = null;
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight),
                "-")) { rewardsList.Remove(r); DestroyImmediate(r, true); }
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this reward"));
            r.type = (rewardType)EditorGUI.EnumPopup(
                new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                r.type);
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "Change the reward type"));
            r.amount = EditorGUI.IntField(
               new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
               r.amount);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of the reward to give (100 gold, 1 sword...)"));
            r.rewardObject = (GameObject)EditorGUI.ObjectField(
                 new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                r.rewardObject, typeof(GameObject), true);
            EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to reward, if any"));
        }
    };

        R_List.elementHeightCallback = (index) =>
        {
            Repaint();
            float height = EditorGUIUtility.singleLineHeight * 3;
            return height;
        };
        return R_List;
    }

    public void CopyAll<T>(T source, T target) //TODO: Make static and accesable from anywhere
    {
        var type = typeof(T);

        foreach (var sourceProperty in type.GetProperties())
        {
            var targetProperty = type.GetProperty(sourceProperty.Name);
            if (targetProperty.GetSetMethod(true) != null)
            {
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
        }
        foreach (var sourceField in type.GetFields())
        {
            var targetField = type.GetField(sourceField.Name);
            targetField.SetValue(target, sourceField.GetValue(source));
        }
    }

    /*** CallsBacks ***/ //TODO: Add descriptions to all these

    private void CreateNewCriteriaCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Criteria c = q.gameObject.AddComponent<Criteria>();
        q.criterias.Add(c);
    }

    private void AddNewCriteriaCallBack(object obj)
    {
        string name = questScript.name.ToString();
        Criteria c = (Criteria)obj;
        Criteria tmpCriteria = questScript.gameObject.AddComponent<Criteria>();
        questScript.criterias.Add(tmpCriteria);
        CopyAll(c, tmpCriteria);
        tmpCriteria.rewards = new List<Reward>(c.rewards);
        tmpCriteria.spawnZones = new List<SpawnZone>(c.spawnZones);
        questScript.name = name;
    }

    private void CreateNewOptionalCriteriaCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Criteria c = q.gameObject.AddComponent<Criteria>();
        c.editorType = editorCriteriaType.Optional;
        q.optionalCriterias.Add(c);
    }

    private void AddNewOptionalCriteriaCallBack(object obj)
    {
        string name = questScript.name.ToString();
        Criteria c = (Criteria)obj;
        Criteria tmpCriteria = questScript.gameObject.AddComponent<Criteria>();
        CopyAll(c, tmpCriteria);
        tmpCriteria.rewards = new List<Reward>(c.rewards);
        tmpCriteria.spawnZones = new List<SpawnZone>(c.spawnZones);
        tmpCriteria.editorType = editorCriteriaType.Optional;
        questScript.optionalCriterias.Add(tmpCriteria);
        questScript.name = name;
    }

    private void CreateNewRewardCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Reward r = q.gameObject.AddComponent<Reward>();
        q.rewards.Add(r);
    }

    private void AddNewRewardCallBack(object obj)
    {
        string name = questScript.name.ToString();
        Reward r = (Reward)obj;
        Reward tmpReward = questScript.gameObject.AddComponent<Reward>();
        questScript.rewards.Add(tmpReward);
        CopyAll(r, tmpReward);
        questScript.name = name;
    }

    private void CreateNewCriteriaRewardCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Reward r = q.gameObject.AddComponent<Reward>();
        r.editoreRewardType = editorRewardType.Criteria;
        criteriaToAddReward.rewards.Add(r);
    }

    private void AddNewCriteriaRewardCallBack(object obj)
    {
        string name = questScript.name.ToString();
        Reward r = (Reward)obj;
        Reward tmpReward = questScript.gameObject.AddComponent<Reward>();
        CopyAll(r, tmpReward);
        tmpReward.editoreRewardType = editorRewardType.Criteria;
        criteriaToAddReward.rewards.Add(tmpReward);
        questScript.name = name;
    }

    private void CreateNewOptionalRewardCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Reward r = q.gameObject.AddComponent<Reward>();
        r.editoreRewardType = editorRewardType.Optional;
        q.optionalRewards.Add(r);
    }

    private void AddNewOptionalRewardCallBack(object obj)
    {
        string name = questScript.name.ToString();
        Reward r = (Reward)obj;
        Reward tmpReward = questScript.gameObject.AddComponent<Reward>();
        CopyAll(r, tmpReward);
        tmpReward.editoreRewardType = editorRewardType.Optional;
        questScript.optionalRewards.Add(tmpReward);
        questScript.name = name;
    }

    private void CreateNewQuestGiverCallBack(object obj)
    {
        if (CustomQuestSettings.SettingsHolder.questGiverPrefab)
        {
            Quest q = (Quest)obj;
            GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.questGiverPrefab);
            QuestGiver questGiver = go.GetComponent<QuestGiver>();
            if (!questGiver)
            {
                questGiver = go.GetComponentInChildren<QuestGiver>();
            }
            if (questGiver)
            {
                questGiver.quests.Add(q);
                q.questGivers.Add(questGiver);
            }
            else
            {
                Debug.LogWarning("No questGiver script was found in the questGiverPrefab, please assign a prefab with the questGiver scrip attached");
            }
        }
        else
        {
            Debug.LogWarning("questGiverPrefab is null in settings(CustomQuest's settings), please assign a prefab with the QuestGiver script attached. If you have one assisgned, please close and open the editor window");
        }
    }

    private void AddNewQuestGiverCallBack(object obj)
    {
        QuestGiver qG = (QuestGiver)obj;
        qG.quests.Add(questScript);
        questScript.questGivers.Add(qG);
    }

    private void AddQuestGiverToHandInCallBack(object obj)
    {
        HandInObject hO = (HandInObject)obj;
        QuestGiver qG = hO.GetComponent<QuestGiver>();
        if (!qG)
        {
            qG = hO.GetComponentInChildren<QuestGiver>();
            if (!qG)
            {
                qG = hO.gameObject.AddComponent<QuestGiver>();
                GameObject gO = Instantiate(CustomQuestSettings.SettingsHolder.questGiverPrefab.GetComponent<QuestGiver>().questSymbol);
                gO.transform.parent = qG.transform;
                qG.questSymbol = gO;

            }
        }
        qG.quests.Add(questScript);
        questScript.questGivers.Add(qG);
    }

    private void CreateNewHandInObjectCallBack(object obj)
    {
        if (CustomQuestSettings.SettingsHolder.handInObjectPrefab)
        {
            Quest q = (Quest)obj;
            GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.handInObjectPrefab);
            HandInObject handInObject = go.GetComponent<HandInObject>();
            if (!handInObject)
            {
                handInObject = go.GetComponentInChildren<HandInObject>();
            }
            if (handInObject)
            {
                handInObject.quests.Add(q);
                q.handInObjects.Add(handInObject);
            }
            else
            {
                Debug.LogWarning("No handInObject script was found in the handInObjectPrefab, please assign a prefab with the handInObject scrip attached");
            }
        }
        else
        {
            Debug.LogWarning("handInObjectPrefab is null in settings(CustomQuest's settings), please assign a prefab with the HandInObject script attached. If you have one assisgned, please close and open the editor window");
        }
    }

    private void AddNewHandInObjectCallBack(object obj)
    {
        HandInObject hO = (HandInObject)obj;
        hO.quests.Add(questScript);
        questScript.handInObjects.Add(hO);
    }

    private void AddHandInObjectToQuestGiverCallBack(object obj)
    {
        QuestGiver qG = (QuestGiver)obj;
        HandInObject hO = qG.GetComponent<HandInObject>();
        if (!hO)
        {
            hO = qG.GetComponentInChildren<HandInObject>();
            if (!hO)
            {
                hO = qG.gameObject.AddComponent<HandInObject>();
            }
        }
        hO.quests.Add(questScript);
        questScript.handInObjects.Add(hO);
    }
}