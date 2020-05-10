using CustomQuest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// The editor showing all information about quest, criterias and rewards in the inspector. Requires the quest component to show any info.
/// </summary>
public class CustomQuestEditor : EditorWindow
{
    #region Field

    private string latestRenamed;

    //TODO: Reorder field, so it makes more sense

    /// <summary>
    /// A toggle bool, used to set dontDespawn on all criterias in a quest at once
    /// </summary>
    private bool dontDespawnToggler;

    /// <summary>
    /// A guistyle, used for to make tekst bold
    /// </summary>
    protected GUIStyle boldStyle;

    /// <summary>
    /// A guistyle, used for big headlines
    /// </summary>
    protected GUIStyle headLineStyle;

    /// <summary>
    /// The currently selected quest
    /// </summary>
    public Quest selectedQuest;

    /// <summary>
    /// The currently selected criteria
    /// </summary>
    public Criteria selectedCriteria;

    /// <summary>
    /// The currently selected reward
    /// </summary>
    public Reward selectedReward;

    /// <summary>
    /// A list of all the quests prefabs
    /// </summary>
    public List<Quest> allQuests = new List<Quest>();

    /// <summary>
    /// A list of all the criteria prefabs
    /// </summary>
    public List<Criteria> allCriterias = new List<Criteria>();

    /// <summary>
    /// A list of all the reward prefabs
    /// </summary>
    public List<Reward> allRewards = new List<Reward>();

    /// <summary>
    /// A list of the nodes shown in quests in scene
    /// </summary>
    private List<QuestNode> nodes = new List<QuestNode>();

    /// <summary>
    /// A bool used to determine when it needs to attach something. Its used when converting scripts.
    /// </summary>
    private bool needToAttach = false;

    /// <summary>
    /// A float used to prevent certain null errors when reloading and creating custom scripts
    /// </summary>
    private float waitForCompile = 1;

    /// <summary>
    /// Enums used to determine which scripts its currently attaching
    /// </summary>
    private enum TypeToAttach { Quest, Reward, Criteria }

    /// <summary>
    /// The type its currently attaching
    /// </summary>
    private TypeToAttach attachType;

    /// <summary>
    /// Enums used to determine which scripts its currently renaming
    /// </summary>
    private enum TypeToRename { Quest, Reward, Criteria }

    /// <summary>
    /// The type its currently renaming
    /// </summary>
    private TypeToRename renameType;

    /// <summary>
    /// Is used to make sure Awake() is called everytime the editor has been compiled. Usefull to avoid crashes if the editor is open when writing new code.
    /// </summary>
    private bool editorCompiled = false;

    /// <summary>
    /// A gameobject used for holding information when converting
    /// </summary>
    private GameObject tmpGameObject;

    /// <summary>
    /// A quests used for holding when converting and moving
    /// </summary>
    private Quest tmpQuest;

    /// <summary>
    /// A criteria used for holdoing when converting and moving a criteria
    /// </summary>
    private Criteria tmpCriteria;

    /// <summary>
    /// A reward used for holding when converting and moving a reward
    /// </summary>
    private Reward tmpReward;

    /// <summary>
    /// Saves the previous name of a quests when renaming it. Used for finding the folder
    /// </summary>
    private string lastName;

    /// <summary>
    /// Used to save processing when renaming a quests.
    /// </summary>
    private string tmpQuestName;

    /// <summary>
    /// A bool used when renaming quest
    /// </summary>
    private bool updateUI;

    /// <summary>
    /// A float used to reduce lag, by only updating the name of a quest every 1 second.
    /// </summary>
    private float updateCounter = 1;

    /// <summary>
    /// A vector 2 used for scrolling the left side of the prefabs
    /// </summary>
    private Vector2 leftScrollview = Vector2.zero;

    /// <summary>
    /// A vector 2 used for scrolling the right side of the prefabs
    /// </summary>
    private Vector2 rightScrollview = Vector2.zero;

    /// <summary>
    /// An int used for controlling the tabs at the top of the editor
    /// </summary>
    private int toolBarInt = 0;

    /// <summary>
    /// An array of strings used by the tab menues at the top of the editor
    /// </summary>
    private string[] toolBarString = new string[] { "Prefabs", "Quests in scene" };

    /// <summary>
    /// An int used for controlling the tabs at the top of the prefabs lists
    /// </summary>
    private int prefabToolBarInt = 0;

    /// <summary>
    /// An array of strings used by the tab menues at the top of the prefabs lists
    /// </summary>
    private string[] prefabToolBarString = new string[] { "Quest", "Criteria", "Reward" };

    /// <summary>
    /// A bool used when making connections between quets in scene.
    /// When true, draws a line between the originEdge and the mouse
    /// </summary>
    private bool makingNodeConnection;

    /// <summary>
    /// Used when making connections. Saves the node the connection came from
    /// </summary>
    private QuestNode originNode;

    /// <summary>
    /// Used when making connections. Saves the edge the connection came from
    /// </summary>
    private QuestEdge originEdge;

    /// <summary>
    /// Bool used to determine if a node should be deleted
    /// </summary>
    [HideInInspector]
    public bool deletingNode;

    /// <summary>
    /// The node to delete if the deletingNode bool is true
    /// </summary>
    [HideInInspector]
    public QuestNode nodeToDelete;

    /// <summary>
    /// Used to prevent crashing when deleting a conenctiong from a list currently being runned through
    /// </summary>
    private bool connectionDeleted = false;

    /// <summary>
    /// Used to save the current event when updating
    /// </summary>
    private Event currentEvent;

    /// <summary>
    /// The reordable list of the quest prefabs
    /// </summary>
    public ReorderableList R_questPrefabList = null;

    /// <summary>
    /// The reordable list of the quest prefabs, used in quest in scene
    /// </summary>
    public ReorderableList R_questInSceneList = null;

    /// <summary>
    /// The reordable list of the criteria prefabs
    /// </summary>
    public ReorderableList R_criteriaPrefabList = null;

    /// <summary>
    /// The reordable list of the reward prefabs
    /// </summary>
    public ReorderableList R_rewardPrefabList = null;

    /// <summary>
    /// A reordable list of the criterias for a quest
    /// </summary>
    public ReorderableList R_CriteriaList = null;

    /// <summary>
    /// A reordable list of the rewards for a quest
    /// </summary>
    public ReorderableList R_RewardList = null;

    /// <summary>
    /// A reordable list of the optional criterias for a quest
    /// </summary>
    public ReorderableList R_OptionalCriteriaList = null;

    /// <summary>
    /// A reordable list of the optional rewards for a quest
    /// </summary>
    public ReorderableList R_OptionalRewardsList = null;

    /*** Test fields, delete if not used ***/

    public GUISkin thisGUISkin; //GUISkin could be used to give more control

    public Texture background; //A background could be added

    #endregion Field

    /*** UNITY METHODS ***/

    /// <summary>
    /// Opens the editor.
    /// </summary>
    [MenuItem("Tools/Custom Quest System")]
    public static void OpenQuestSystem()
    {
        GetWindow(typeof(CustomQuestEditor));
    }

    /// <summary>
    /// Runs when the script is created (Or when called on)
    /// </summary>
    private void Awake()
    {
        CustomQuestSettings.Start(); //Runs the CustomQuestSettings start
        EditorUtility.SetDirty(CustomQuestSettings.SettingsHolder); // TODO: Might not be needed?
        allQuests = CustomQuestSettings.EditorQuests; //Sets the list of prefab quests equal to the CustomQuestSettings Quests. The Custom Quest Setting holds the quests for when closing the system.
        allCriterias = CustomQuestSettings.EditorCriteras;
        allRewards = CustomQuestSettings.EditorRewards;
        thisGUISkin = CustomQuestSettings.RandomDragonGUISkin;
        foreach (Quest q in FindObjectsOfType<Quest>()) //Goes through all quests in scene and checks if they are in the editors nodes, if not, adds them
        {
            bool notExisting = true;
            foreach (QuestNode qn in CustomQuestSettings.QuestNodes)
            {
                if (qn != null)
                {
                    if (q == qn.quest)
                    {
                        notExisting = false;
                    }
                }
            }
            if (notExisting == true)
            {
                //CustomQuestSettings.AddNewNode(q);
                //CreateQuest(q.gameObject);
                //Destroy(q.gameObject);
            }
        }
        // btnTexture = Resources.Load("twitterButton") as Texture; //TODO: This is just to rememeber how to do it
        UpdateQuestList();
        UpdateCriteriaList();
        UpdateRewardList();
        R_questPrefabList = new ReorderableList(allQuests, typeof(Quest), true, false, false, false);
        R_questPrefabList.showDefaultBackground = false;
        R_questPrefabList.headerHeight = 0;
        R_questPrefabList.onSelectCallback += R_SelectQuest;

        R_questInSceneList = new ReorderableList(allQuests, typeof(Quest), true, false, false, false);
        R_questInSceneList.showDefaultBackground = false;
        R_questInSceneList.headerHeight = 0;
        R_questInSceneList.onSelectCallback += R_SelectQuestInScene;

        R_criteriaPrefabList = new ReorderableList(allCriterias, typeof(Criteria), true, false, false, false);
        R_criteriaPrefabList.showDefaultBackground = false;
        R_criteriaPrefabList.headerHeight = 0;
        R_criteriaPrefabList.onSelectCallback += R_SelectCriteria;

        R_rewardPrefabList = new ReorderableList(allRewards, typeof(Reward), true, false, false, false);
        R_rewardPrefabList.showDefaultBackground = false;
        R_rewardPrefabList.headerHeight = 0;
        R_rewardPrefabList.onSelectCallback += R_SelectReward;
    }

    /// <summary>
    /// Contains the drawing of the editor
    /// </summary>
    private void OnGUI()
    {
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
            string[] results = AssetDatabase.FindAssets("RandomDragonGUISkin");
            foreach (string guid in results)
            {
                thisGUISkin = (GUISkin)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GUISkin)); //TODO: Put the skin in settings - Give settings a toggle for normal skin? (too much work)
                GUI.skin = thisGUISkin;
            }
        }

        GUI.color = new Color32(162, 162, 162, 255);
        GUI.DrawTexture(new Rect(0, 0, maxSize.x, maxSize.y), EditorGUIUtility.whiteTexture, ScaleMode.StretchToFill);
        GUI.color = Color.white;
        if (!EditorApplication.isCompiling) //Checks if the editor is currently compiling
        {
            //GUI.color = new Color(1, 1, 0.4f);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Custom Quest", thisGUISkin.GetStyle("CQTitle"), GUILayout.Width(198));
            toolBarInt = GUILayout.Toolbar(toolBarInt, toolBarString, thisGUISkin.GetStyle("Tabs2")); //Show the toolbar
            if (GUILayout.Button(new GUIContent("@", "Click here to Open Settings"), thisGUISkin.GetStyle("Tabs2"), GUILayout.Width(25), GUILayout.Height(18)))
            {
                CreateSettingsPopUp();
            }
            if (GUILayout.Button(new GUIContent("?", "Click here for help"), thisGUISkin.GetStyle("Tabs2"), GUILayout.Width(25), GUILayout.Height(18)))
            {
                Application.OpenURL("http://randomdragongames.com/games/custom-quest-3/?customize_changeset_uuid=2bc1762e-645b-4822-a887-46ed29039740"); //TODO: maybe make this a popup, where you can also just open the readme (In case of no internet)
            }
            EditorGUILayout.EndHorizontal();
            GUI.color = Color.white;
            switch (toolBarInt)
            { //Depending on which toolbar is clicked, runs a different method.
                case 0:
                    OnGUIPrefabs();
                    break;

                case 1:
                    OnGUIQuestsInScene();
                    break;
            }
        }
        else
        { //Shows some text instead of the normal GUI logic.
            EditorGUILayout.LabelField("Assets are loading, please wait...");
            editorCompiled = true; //Is set to true, so we know it has been compiled
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (editorCompiled == true) //Checks if the editor has been compiled and runs awake if it has. Otherwise Awake is not runned when compiling, which will cause errors.
        {
            editorCompiled = false;
            Awake();
        }

        if (updateUI) //Checks if an update to the name has been done.
        {
            updateCounter -= 0.01f; //Reduces the counter
            if (updateCounter <= 0)
            {
                switch (renameType)
                {
                    case TypeToRename.Quest:
                        RenamePrefab(selectedQuest.gameObject);
                        break;

                    case TypeToRename.Reward:
                        RenamePrefab(selectedReward.gameObject);
                        break;

                    case TypeToRename.Criteria:
                        RenamePrefab(selectedCriteria.gameObject);
                        break;
                }
            }
        }

        if (needToAttach) //Checks if anything needs to be attached
        {
            waitForCompile -= 0.01f; //reduces the counter
            if (waitForCompile <= 0)
            {
                if (!EditorApplication.isCompiling) //Checks if the editor is not compiling - To avoid errors.
                {
                    Type tp; // A holder for various types
                    needToAttach = false; // Resets the needToAttach bool
                    waitForCompile = 1; // Resets the counter
                    switch (attachType) //Branches out, depinding on what type its currenly attaching
                    {
                        case TypeToAttach.Quest: //For attaching a quests

                            #region Attach Quest

                            DestroyImmediate(tmpGameObject.GetComponent<Quest>());
                            tp = GetComponentTypeByName(tmpGameObject.name.Replace(" ", ""));
                            tmpGameObject.AddComponent(tp);

                            //PrefabUtility.CreatePrefab("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + tmpGameObject.name + "/" + tmpGameObject.name + ".prefab", tmpGameObject);
                            PrefabUtility.SaveAsPrefabAsset(tmpGameObject, "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + tmpGameObject.name + "/" + tmpGameObject.name + ".prefab");
                            AssetDatabase.MoveAsset("Assets/" + tmpGameObject.name.Replace(" ", "") + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + tmpGameObject.name + "/" + tmpGameObject.name.Replace(" ", "") + ".cs");
                            GameObject newQuest = AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + tmpGameObject.name + "/" + tmpGameObject.name + ".prefab", typeof(GameObject)) as GameObject;
                            UpdateQuestList();
                            Quest newQuestScript = newQuest.GetComponent<Quest>();
                            newQuestScript.EditorStart();
                            if (tmpQuest.handInObjects != null)
                            {
                                foreach (HandInObject h in tmpQuest.handInObjects)
                                {
                                    HandInObject tmpH = Instantiate(h, newQuestScript.gameObject.transform);
                                    newQuestScript.handInObjects.Add(tmpH);
                                }
                            }
                            if (tmpQuest.questIcon != null)
                            {
                                newQuestScript.questIcon = tmpQuest.questIcon;
                            }
                            if (tmpQuest.criterias.Count > 0)
                            {
                                foreach (Criteria criteria in tmpQuest.criterias)
                                {
                                    Criteria tmpC = newQuestScript.gameObject.AddComponent<Criteria>();
                                    CopyAll(criteria, tmpC);
                                    newQuestScript.criterias.Add(tmpC);
                                }
                            }
                            if (tmpQuest.rewards.Count > 0)
                            {
                                foreach (Reward reward in tmpQuest.rewards)
                                {
                                    Reward tmpR = newQuestScript.gameObject.AddComponent<Reward>();
                                    CopyAll(reward, tmpR);
                                    newQuestScript.rewards.Add(tmpR);
                                }
                            }
                            if (tmpQuest.optionalCriterias.Count > 0)
                            {
                                foreach (Criteria criteria in tmpQuest.optionalCriterias)
                                {
                                    Criteria tmpC = newQuestScript.gameObject.AddComponent<Criteria>();
                                    CopyAll(criteria, tmpC);
                                    newQuestScript.optionalCriterias.Add(tmpC);
                                }
                            }
                            if (tmpQuest.optionalRewards.Count > 0)
                            {
                                foreach (Reward reward in tmpQuest.optionalRewards)
                                {
                                    Reward tmpR = newQuestScript.gameObject.AddComponent<Reward>();
                                    CopyAll(reward, tmpR);
                                    newQuestScript.optionalRewards.Add(tmpR);
                                }
                            }
                            tmpGameObject.name = "";
                            selectedQuest = newQuest.GetComponent<Quest>();
                            DestroyImmediate(tmpQuest.gameObject);

                            #endregion Attach Quest

                            break;

                        case TypeToAttach.Criteria: //For attaching a criteria

                            #region Attach Criteria

                            DestroyImmediate(tmpGameObject.GetComponent<Criteria>());
                            tp = GetComponentTypeByName(tmpGameObject.name.Replace(" ", ""));
                            tmpGameObject.AddComponent(tp);
                            GameObject criteriaPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + ".prefab", typeof(GameObject));
                            criteriaPrefab.AddComponent(tp);
                            if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + ".prefab", typeof(GameObject)) != null)
                            {
                                AssetDatabase.CreateFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias", tmpGameObject.name);
                                AssetDatabase.MoveAsset("Assets/" + tmpGameObject.name.Replace(" ", "") + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + "/" + tmpGameObject.name.Replace(" ", "") + ".cs");
                                AssetDatabase.MoveAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + ".prefab", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + "/" + tmpGameObject.name + ".prefab");
                            }
                            else
                            {
                                AssetDatabase.MoveAsset("Assets/" + tmpGameObject.name.Replace(" ", "") + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + tmpGameObject.name + "/" + tmpGameObject.name.Replace(" ", "") + ".cs");
                            }
                            UpdateCriteriaList();
                            Criteria c = (Criteria)criteriaPrefab.GetComponent(tp);
                            c.EditorStart();
                            if (tmpCriteria.criteriaObject != null)
                            {
                                c.criteriaObject = tmpCriteria.criteriaObject;
                            }
                            if (tmpCriteria.spawnZones != null)
                            {
                                c.spawnZones = tmpCriteria.spawnZones;
                            }
                            if (tmpCriteria.rewards != null)
                            {
                                c.rewards = tmpCriteria.rewards;
                            }
                            c.timed = tmpCriteria.timed;
                            c.time = tmpCriteria.time;
                            c.dontDespawnObjectsWhenComplete = tmpCriteria.dontDespawnObjectsWhenComplete;
                            c.giveRewardsOnCompletion = tmpCriteria.giveRewardsOnCompletion;
                            c.Level = tmpCriteria.Level;
                            DestroyImmediate(tmpCriteria.gameObject);
                            tmpGameObject.name = "";

                            #endregion Attach Criteria

                            break;

                        case TypeToAttach.Reward: // For attaching a reward

                            #region Attach Reward

                            DestroyImmediate(tmpGameObject.GetComponent<Reward>());
                            tp = GetComponentTypeByName(tmpGameObject.name.Replace(" ", ""));
                            tmpGameObject.gameObject.AddComponent(tp);
                            GameObject rewardPrefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + ".prefab", typeof(GameObject));
                            rewardPrefab.AddComponent(tp);
                            if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + ".prefab", typeof(GameObject)) != null)
                            {
                                AssetDatabase.CreateFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards", tmpGameObject.name);
                                AssetDatabase.MoveAsset("Assets/" + tmpGameObject.name.Replace(" ", "") + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + "/" + tmpGameObject.name.Replace(" ", "") + ".cs");
                                AssetDatabase.MoveAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + ".prefab", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + "/" + tmpGameObject.name + ".prefab");
                            }
                            else
                            {
                                AssetDatabase.MoveAsset("Assets/" + tmpGameObject.name.Replace(" ", "") + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + tmpGameObject.name + "/" + tmpGameObject.name.Replace(" ", "") + ".cs");
                            }
                            UpdateRewardList();
                            Reward r = (Reward)rewardPrefab.GetComponent(tp);
                            r.EditorStart();
                            if (tmpReward.rewardObject != null)
                            {
                                r.rewardObject = tmpReward.rewardObject;
                            }
                            DestroyImmediate(tmpReward.gameObject);
                            tmpGameObject.name = "";

                            #endregion Attach Reward

                            break;

                        default:
                            break;
                    }

                    DestroyImmediate(tmpGameObject);
                }
            }
        }
        Repaint();
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// Runs when the window is destroyed
    /// </summary>
    private void OnDestroy()
    {
        CustomQuestSettings.EditorQuests = allQuests;
        CustomQuestSettings.EditorCriteras = allCriterias;
        CustomQuestSettings.EditorRewards = allRewards;
#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        if (!EditorApplication.isPlaying)
        {
            EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        }
#endif
    }

    /// <summary>
    /// Runs when a change in the hierarchy is made
    /// </summary>
    public void OnHierarchyChange()
    {
        Awake();
    }

    #region PopUps

    /// <summary>
    /// Creates the window for converting a quest
    /// </summary>
    private void CreateQuestPopUp()
    {
        CustomQuestPopUp window = CustomQuestPopUp.Instance; // Gets the window, or creates a new one if none exists
        window.SetQuestEditor(this); //Sets the editor in the CustomQuestPopUp
        // Sets the new windows position to the mouse positoin
        Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.position = new Rect(mousePos.x - 200, mousePos.y, 400, 200);
    }

    /// <summary>
    /// Creates the window for converting a reward
    /// </summary>
    /// <param name="r">The reward to convert</param>
    private void CreateRewardPopUp(Reward r)
    {
        CustomRewardPopUp window = CustomRewardPopUp.Instance; // Gets the window, or creates a new one if none exists
        window.SetQuestEditor(this, r); //Sets the editor in the CustomRewardPopUp
        // Sets the new windows position to the mouse positoin
        Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.position = new Rect(mousePos.x - 200, mousePos.y, 400, 200);
    }

    /// <summary>
    /// Creates the window for converting a criteria
    /// </summary>
    /// <param name="c">The criteria to convert</param>
    private void CreateCriteriaPopUp(Criteria c)
    {
        CustomCriteriaPopUp window = CustomCriteriaPopUp.Instance; //Gets the window, or creates a new one if none exists
        window.SetQuestEditor(this, c); //Sets the editor in the CustomCriteriaPopUp
        // Sets the new windows position to the mouse positoin
        Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.position = new Rect(mousePos.x - 200, mousePos.y, 400, 200);
    }

    /// <summary>
    /// Creates and targets the settings window
    /// </summary>
    private void CreateSettingsPopUp()
    {
        SettingsPopUp window = SettingsPopUp.Instance; // Gets the window, or creates a new one if none exists
        // Sets the new windows position to the mouse positoin
        Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
        window.position = new Rect(mousePos.x - 200, mousePos.y, 400, 220);
    }

    #endregion PopUps

    /*** PRIVATE METHODS ***/

    private static System.Type GetComponentTypeByName(string name)
    { //Source: https://forum.unity3d.com/threads/update-changes-addcomponent-and-strings.308309/#post-2018213
        if (string.IsNullOrEmpty(name)) return null;

        var ctp = typeof(Component);
        foreach (var assemb in System.AppDomain.CurrentDomain.GetAssemblies())
        {
            foreach (var tp in assemb.GetTypes())
            {
                if (ctp.IsAssignableFrom(tp) && tp.Name == name) return tp;
            }
        }
        return null;
    } //TODO: put it public static class, so anyone can use it?

    private void ClearFocus()
    {
        GUI.SetNextControlName("");
        GUI.FocusControl("");
    }

    private void OnGUIPrefabs()
    {
        #region Font style

        // base.OnInspectorGUI();
        boldStyle = new GUIStyle(EditorStyles.foldout);
        //CopyAll(EditorStyles.foldout, boldStyle); This creates an error :(
        boldStyle.fontStyle = thisGUISkin.GetStyle("Foldout").fontStyle;
        boldStyle.font = thisGUISkin.GetStyle("Foldout").font;
        boldStyle.fontSize = thisGUISkin.GetStyle("Foldout").fontSize;
        boldStyle.padding = thisGUISkin.GetStyle("Foldout").padding;
        boldStyle.normal.textColor = thisGUISkin.GetStyle("Foldout").normal.textColor;
        boldStyle.active.textColor = thisGUISkin.GetStyle("Foldout").active.textColor;
        boldStyle.hover.textColor = thisGUISkin.GetStyle("Foldout").hover.textColor;
        headLineStyle = new GUIStyle();
        headLineStyle.fontStyle = EditorStyles.label.fontStyle;
        headLineStyle.font = EditorStyles.label.font;
        headLineStyle.focused = EditorStyles.label.focused;
        headLineStyle.active = EditorStyles.label.active;
        headLineStyle.normal = EditorStyles.label.normal;
        headLineStyle.fontSize = 30;

        #endregion Font style

        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(6);
        prefabToolBarInt = GUILayout.Toolbar(prefabToolBarInt, prefabToolBarString, thisGUISkin.GetStyle("Tabs1"), GUILayout.Width(196)); //Show the toolbar

        EditorGUILayout.EndHorizontal();
        // GUI.color = new Color32(196, 185, 127, 255);
        //GUI.color = new Color32(35, 120, 161, 255);
        GUI.color = new Color32(135, 135, 135, 255);
        GUILayout.BeginArea(new Rect(5, 40, 196, position.height - 45), thisGUISkin.box);
        GUI.color = Color.white;
        leftScrollview = EditorGUILayout.BeginScrollView(leftScrollview, GUILayout.Width(196), GUILayout.Height(position.height - 20));

        // GUILayout.Label("Quests", GUILayout.Width(120));
        //GUILayout.Space(5);
        GUI.skin = null;
        switch (prefabToolBarInt)
        {
            case 0: //Quest prefabs

                #region Quest List

                if (allQuests.ElementAtOrDefault(0) != null && selectedQuest == null)
                {
                    selectedQuest = allQuests[0];
                }

                GUILayout.BeginHorizontal(/*EditorStyles.helpBox, GUILayout.Width(180)*/);
                if (GUILayout.Button(new GUIContent("+", "Click here to create a new quest"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    CreateNewQuest();
                }
                GUILayout.Space(5);
                if (selectedQuest != null)
                {
                    if (GUILayout.Button(new GUIContent("-", "Click here to delete " + selectedQuest.questName), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        QuestDeleteWindow window = QuestDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                        window.SetQuestEditor(this, selectedQuest); //Sets the editor in the QuestDeleteWindow
                        window.position = ClampToScreen(new Rect(Screen.width / 2, Screen.height / 2, 250, 50), Screen.width, Screen.height); // Sets the new windows position to the mouse positoin
                    }
                }
                else if (GUILayout.Button(new GUIContent("-", "Click here to delete the selected quest"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No quest selected, cannot delete");
                }
                GUILayout.Space(5);
                if (selectedQuest != null)
                {
                    if (GUILayout.Button(new GUIContent("Λ", "Click here to move " + selectedQuest.questName + " one up."), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        int index1 = allQuests.IndexOf(selectedQuest);
                        if (index1 > 0)
                        {
                            Quest tmpQuest = allQuests[index1 - 1];
                            allQuests[index1 - 1] = selectedQuest;
                            allQuests[index1] = tmpQuest;
                            R_questPrefabList.index = index1 - 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("Λ", "Click here to move the selected quest one up"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No quest selected, cannot move");
                }
                GUILayout.Space(5);
                if (selectedQuest != null)
                {
                    if (GUILayout.Button(new GUIContent("V", "Click here to move " + selectedQuest.questName + " one down"), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        int index1 = allQuests.IndexOf(selectedQuest);
                        if (index1 < allQuests.Count - 1)
                        {
                            Quest tmpQuest = allQuests[index1 + 1];
                            allQuests[index1 + 1] = selectedQuest;
                            allQuests[index1] = tmpQuest;
                            R_questPrefabList.index = index1 + 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("V", "Click here to move selected quest one down"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No quest selected, cannot move");
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUI.color = Color.white;
                if (R_questPrefabList != null)
                {
                    for (int i = 0; i < allQuests.Count; i++)
                    {
                        Quest q = allQuests[i];
                        if (q == null || q.gameObject == null)
                        {
                            allQuests.Remove(q);
                            DestroyImmediate(q);
                        }
                    }
                    if (R_questPrefabList.list.Count > 0)
                    {
                        GUI.skin = null;
                        R_questPrefabList.DoLayoutList(); //TODO: Sometimes throws an error here when empthy, no crash, but still //TODO: Check if fixed now? //not fixed
                        GUI.skin = thisGUISkin;
                    }
                }
                else
                {
                    Debug.Log("ReorderAble Quest list is null, restarting application to fix");
                    Awake();
                }

                #endregion Quest List

                break;

            case 1:

                #region Criteria List

                if (allCriterias.ElementAtOrDefault(0) != null && selectedCriteria == null)
                {
                    selectedCriteria = allCriterias[0];
                }

                GUILayout.BeginHorizontal(/*EditorStyles.helpBox, GUILayout.Width(130)*/);
                if (GUILayout.Button(new GUIContent("+", "Click here to create a new criterion"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    CreateNewCriteriaPrefab();
                }
                GUILayout.Space(5);
                if (selectedCriteria != null)
                {
                    if (GUILayout.Button(new GUIContent("-", "Click here to delete " + selectedCriteria.criteriaName), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        CriteriaPrefablDeleteWindow window = CriteriaPrefablDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                        window.SetQuestEditor(this, selectedCriteria); //Sets the editor in the QuestDeleteWindow
                        window.position = ClampToScreen(new Rect(Screen.width / 2, Screen.height / 2, 250, 50), Screen.width, Screen.height); // Sets the new windows position to the mouse position
                    }
                }
                else if (GUILayout.Button(new GUIContent("-", "Click here to delete the selected criterion"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No criterion selected, cannot delete");
                }
                GUILayout.Space(5);
                if (selectedCriteria != null)
                {
                    if (GUILayout.Button(new GUIContent("Λ", "Click here to move " + selectedCriteria.criteriaName + " one up."), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        int index1 = allCriterias.IndexOf(selectedCriteria);
                        if (index1 > 0)
                        {
                            Criteria tmpCriteria = allCriterias[index1 - 1];
                            allCriterias[index1 - 1] = selectedCriteria;
                            allCriterias[index1] = tmpCriteria;
                            R_criteriaPrefabList.index = index1 - 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("Λ", "Click here to move the selected criterion one up"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No criterion selected, cannot move");
                }
                GUILayout.Space(5);
                if (selectedCriteria != null)
                {
                    if (GUILayout.Button(new GUIContent("V", "Click here to move " + selectedCriteria.criteriaName + " one down"), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        int index1 = allCriterias.IndexOf(selectedCriteria);
                        if (index1 < allCriterias.Count - 1)
                        {
                            Criteria tmpCriteria = allCriterias[index1 + 1];
                            allCriterias[index1 + 1] = selectedCriteria;
                            allCriterias[index1] = tmpCriteria;
                            R_criteriaPrefabList.index = index1 + 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("V", "Click here to move selected criterion one down"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No criterion selected, cannot move");
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (R_criteriaPrefabList != null)
                {
                    for (int i = 0; i < allCriterias.Count; i++)
                    {
                        Criteria c = allCriterias[i];
                        if (c == null || c.gameObject == null)
                        {
                            allCriterias.Remove(c);
                            DestroyImmediate(c);
                        }
                    }
                    R_criteriaPrefabList.DoLayoutList();
                }
                else
                {
                    Debug.Log("ReorderAble Criteria list is null, restarting application to fix");
                    Awake();
                }

                #endregion Criteria List

                break;

            case 2:

                #region Reward List

                if (allRewards.ElementAtOrDefault(0) != null && selectedReward == null)
                {
                    selectedReward = allRewards[0];
                }

                GUILayout.BeginHorizontal(/*EditorStyles.helpBox, GUILayout.Width(130)*/);
                if (GUILayout.Button(new GUIContent("+", "Click here to create a new reward"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    CreateNewRewardPrefab();
                }
                GUILayout.Space(5);
                if (selectedReward != null)
                {
                    if (GUILayout.Button(new GUIContent("-", "Click here to delete " + selectedReward.rewardName), GUILayout.Width(38), GUILayout.Height(32)))
                    {
                        RewardPrefabDeleteWindow window = RewardPrefabDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                        window.SetQuestEditor(this, selectedReward); //Sets the editor in the QuestDeleteWindow
                        window.position = ClampToScreen(new Rect(Screen.width / 2, Screen.height / 2, 250, 50), Screen.width, Screen.height); // Sets the new windows position to the mouse position
                    }
                }
                else if (GUILayout.Button(new GUIContent("-", "Click here to delete the selected criteria"), GUILayout.Width(38), GUILayout.Height(32)))
                {
                    Debug.Log("No criteria selected, cannot delete");
                }
                GUILayout.Space(5);
                if (selectedReward != null)
                {
                    if (GUILayout.Button(new GUIContent("Λ", "Click here to move " + selectedReward.rewardName + " one up."), GUILayout.Width(42), GUILayout.Height(32)))
                    {
                        int index1 = allRewards.IndexOf(selectedReward);
                        if (index1 > 0)
                        {
                            Reward tmpReward = allRewards[index1 - 1];
                            allRewards[index1 - 1] = selectedReward;
                            allRewards[index1] = tmpReward;
                            R_rewardPrefabList.index = index1 - 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("Λ", "Click here to move the selected reward one up"),  GUILayout.Width(42), GUILayout.Height(32)))
                {
                    Debug.Log("No reward selected, cannot move");
                }
                GUILayout.Space(5);
                if (selectedReward != null)
                {
                    if (GUILayout.Button(new GUIContent("V", "Click here to move " + selectedReward.rewardName + " one down"), GUILayout.Width(42), GUILayout.Height(32)))
                    {
                        int index1 = allRewards.IndexOf(selectedReward);
                        if (index1 < allRewards.Count - 1)
                        {
                            Reward tmpReward = allRewards[index1 + 1];
                            allRewards[index1 + 1] = selectedReward;
                            allRewards[index1] = tmpReward;
                            R_rewardPrefabList.index = index1 + 1;
                        }
                    }
                }
                else if (GUILayout.Button(new GUIContent("V", "Click here to move selected reward one down"), GUILayout.Width(42), GUILayout.Height(32)))
                {
                    Debug.Log("No reward selected, cannot move");
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = Color.white;
                if (R_rewardPrefabList != null)
                {
                    for (int i = 0; i < allRewards.Count; i++)
                    {
                        Reward r = allRewards[i];
                        if (r == null || r.gameObject == null)
                        {
                            allRewards.Remove(r);
                            DestroyImmediate(r);
                        }
                    }
                    R_rewardPrefabList.DoLayoutList();
                }
                else
                {
                    Debug.Log("ReorderAble Reward list is null, restarting application to fix");
                    Awake();
                }

                #endregion Reward List

                break;
        }
        GUI.skin = thisGUISkin;

        GUILayout.EndArea();
        GUILayout.EndScrollView();

        //GUI.color = new Color(0, 0.4549f, 0.9015f);
        // GUI.color = new Color32(196, 185, 127, 255);
        //GUI.color = new Color32(254, 252, 224, 255);
        GUI.color = new Color32(194, 194, 194, 255);
        GUILayout.BeginArea(new Rect(205, 20, position.width - 209, position.height - 25), thisGUISkin.box);
        GUI.color = Color.white;
        rightScrollview = GUILayout.BeginScrollView(rightScrollview, GUILayout.Width(position.width - 210), GUILayout.Height(position.height - 30));
        switch (prefabToolBarInt)
        {
            case 0:

                #region Selected Quest

                if (selectedQuest)
                {
                    GUI.color = Color.white;
                    EditorGUILayout.BeginHorizontal(); //Quest Name, with Convert / Open button and Select Prefab button
                    //Icon
                    EditorGUILayout.BeginVertical(GUILayout.Width(40));
                    GUILayout.Space(4);
                    GUI.skin = null;
                    selectedQuest.questIcon = (Sprite)EditorGUILayout.ObjectField(selectedQuest.questIcon, typeof(Sprite), true, GUILayout.Height(34), GUILayout.Width(34));
                    GUI.skin = thisGUISkin;
                    EditorGUILayout.EndVertical();
                    EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The icon of this quest"));
                    // EditorGUILayout.LabelField(new GUIContent("Name: ", "The name of this quest prefab"), thisGUISkin.GetStyle("Title"), GUILayout.Height(40), GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    //EditorGUILayout.Space();
                    selectedQuest.questName = EditorGUILayout.TextField(selectedQuest.questName, thisGUISkin.GetStyle("TitleName"), GUILayout.Height(thisGUISkin.GetStyle("TitleName").CalcHeight(new GUIContent(selectedQuest.questName), position.width - 440)), GUILayout.Width(position.width - 440)); //TODO: Change name of script/Folder too

                    EditorGUILayout.EndVertical();
                    if (selectedQuest.questName != tmpQuestName)
                    { //Starts the change of the name
                        tmpQuestName = selectedQuest.questName;
                        lastName = selectedQuest.name;
                        updateUI = true;
                        updateCounter = 1;
                        renameType = TypeToRename.Quest;
                    }

                    //GUILayout.FlexibleSpace();

                    GUI.skin = null;
                    GUI.skin.button.wordWrap = true;
                    // Open / Convert script buttons
                    if (selectedQuest.isCustomScript != true)
                    {
                        if (GUILayout.Button(new GUIContent("Convert Script", "Click here to convert this script"),GUILayout.Height(35), GUILayout.MinWidth(70), GUILayout.MaxWidth(70))) { CreateQuestPopUp(); }
                    }
                    else if (GUILayout.Button(new GUIContent("Open Script", "Click here to open this script"), GUILayout.Height(35), GUILayout.MinWidth(70), GUILayout.MaxWidth(70)))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + selectedQuest.name + "/" + selectedQuest.name + ".cs", typeof(UnityEngine.Object)));
                    }
                    // Select prefab button
                    if (GUILayout.Button(new GUIContent("Select Prefab", "Click here to select the prefab"), GUILayout.Height(35), GUILayout.MinWidth(70), GUILayout.MaxWidth(70)))
                    {
                        UnityEngine.Object foundAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(selectedQuest), typeof(UnityEngine.Object));
                        if (foundAsset != null) { Selection.activeObject = foundAsset; }
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.skin.button.wordWrap = false;
                    GUI.skin = thisGUISkin;

                    EditorGUILayout.BeginHorizontal(); //Quest Description
                    EditorGUILayout.LabelField(new GUIContent("Description: ", "The description of this quests"), thisGUISkin.GetStyle("Label"), GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
                    selectedQuest.description = EditorGUILayout.TextArea(selectedQuest.description, GUILayout.ExpandHeight(true), GUILayout.Width(position.width - 385));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal(); //Quest Tooltip
                    EditorGUILayout.LabelField(new GUIContent("Tooltip: ", "The tooltip of the quest"), thisGUISkin.GetStyle("Label"), GUILayout.MinWidth(150), GUILayout.MaxWidth(150));
                    selectedQuest.toolTip = EditorGUILayout.TextArea(selectedQuest.toolTip, EditorStyles.textArea, GUILayout.ExpandHeight(true), GUILayout.Width(position.width - 385));
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Separator();

                    #region Settings

                    //Quest settings
                    selectedQuest.showSettings = EditorGUILayout.Foldout(selectedQuest.showSettings, new GUIContent("Quest Settings", "Settings for the quests"), true, boldStyle);
                    if (selectedQuest.showSettings)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Start Availability", "If true, will be avaible to the players when it is initiated"), GUILayout.Width(150)); //TODO: Make the bool "Light up" when clicking on the names (like normal inspector)
                        selectedQuest.startAvailability = EditorGUILayout.Toggle(selectedQuest.startAvailability, GUILayout.Width(15));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent("Constant Availability", "If true, will be avaible to all players always. Until completed. Unless repeaterable is true!"), GUILayout.Width(150));
                        selectedQuest.constantAvailability = EditorGUILayout.Toggle(selectedQuest.constantAvailability, GUILayout.Width(15));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Auto Complete", "If true, will complete as soon as all the criterias are done"), GUILayout.Width(150));
                        selectedQuest.autoComplete = EditorGUILayout.Toggle(selectedQuest.autoComplete, GUILayout.MaxWidth(15));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent("Pick Up Able", "If the player is able to pick this quest up at a quest giver"), GUILayout.Width(150));
                        selectedQuest.pickUpAble = EditorGUILayout.Toggle(selectedQuest.pickUpAble, GUILayout.Width(15));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Don't Delete", "If true, this quest will never be deleted."), GUILayout.Width(150));
                        selectedQuest.dontDelete = EditorGUILayout.Toggle(selectedQuest.dontDelete, GUILayout.Width(15));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent("Single Complete", "If true, this quest can only ever be completed once, by one player"), GUILayout.Width(150));
                        selectedQuest.singleComplete = EditorGUILayout.Toggle(selectedQuest.singleComplete, GUILayout.Width(15));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Timed", "If true, this quest will be timed. If the timer runs out, the quest fails"), GUILayout.Width(150));
                        selectedQuest.timed = EditorGUILayout.Toggle(selectedQuest.timed, GUILayout.Width(15));
                        if (selectedQuest.timed)
                        {
                            selectedQuest.time = EditorGUILayout.FloatField(selectedQuest.time);
                            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The time in seconds the player have to complete this quest"));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Repeatable", "If true, this quest can be picket up and completed multiple times by the same player"), GUILayout.Width(150));
                        selectedQuest.repeatable = EditorGUILayout.Toggle(selectedQuest.repeatable, GUILayout.Width(15));
                        if (selectedQuest.repeatable)
                        {
                            selectedQuest.repeatableTime = EditorGUILayout.FloatField(selectedQuest.repeatableTime);
                            EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The time in seconds the player have to complete this quest"));
                        }
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Start Spawn on Discover", "If true, this quest will start spawning it's criteria objects, when it's available"), GUILayout.Width(150)); //TODO: Make these multiple choice? "What to do on discover..."
                        selectedQuest.startSpawningOnDiscover = EditorGUILayout.Toggle(selectedQuest.startSpawningOnDiscover, GUILayout.Width(15));
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent("No Spawn If No Player", "If true, this quest will not spawn it's criteria if no player has the quest"), GUILayout.Width(150));
                        selectedQuest.noSpawnIfNoPlayer = EditorGUILayout.Toggle(selectedQuest.noSpawnIfNoPlayer, GUILayout.Width(15));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField(new GUIContent("Set all criterias despawn", "Sets all criteria dontDepawnOnCompletion to this bool"), GUILayout.Width(150));
                        dontDespawnToggler = EditorGUILayout.Toggle(dontDespawnToggler, GUILayout.Width(15));
                        GUI.skin = null;
                        if (GUILayout.Button(new GUIContent("Set all", "Sets all criteria dontDepawnOnCompletion"), GUILayout.Width(70)))
                        {
                            foreach (Criteria c in selectedQuest.criterias)
                            {
                                c.dontDespawnObjectsWhenComplete = dontDespawnToggler;
                            }
                        }
                        GUI.skin = thisGUISkin;
                        GUILayout.FlexibleSpace();
                        EditorGUILayout.LabelField(new GUIContent("Match criteria levels", "If the quest should match criteria levels with optional criteria levels. So when a criteria level is done, optional criterias levels up aswell"), GUILayout.Width(150));
                        selectedQuest.matchOptionalLevels = EditorGUILayout.Toggle(selectedQuest.matchOptionalLevels, GUILayout.Width(15));
                        EditorGUILayout.EndHorizontal();

                        EditorGUILayout.EndVertical();
                    }

                    #endregion Settings

                    #region Relations

                    //selectedQuest.showRelations = EditorGUILayout.Foldout(selectedQuest.showRelations, new GUIContent("Relations", "Different relations for this quest"), true, boldStyle);
                    //if (selectedQuest.showRelations)
                    //{
                    //    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    //    //Hand in Object
                    //    EditorGUILayout.BeginHorizontal();
                    //    EditorGUILayout.LabelField(new GUIContent("Hand In Objects / Radius", "The object the player has to go to, to hand in the quest"), GUILayout.Width(150));
                    //    if (GUILayout.Button(new GUIContent("Add handInObject", "Click here to add a new handInObject to this quest"), GUILayout.Width(150)))
                    //    {
                    //        if (CustomQuestSettings.SettingsHolder.handInObjectPrefab)
                    //        {
                    //            GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.handInObjectPrefab, selectedQuest.gameObject.transform);
                    //            if (go.GetComponent<HandInObject>())
                    //            {
                    //                selectedQuest.handInObjects.Add(go.GetComponent<HandInObject>());
                    //                go.GetComponent<HandInObject>().handInByCollision = true;
                    //            }
                    //            else
                    //            {
                    //                Debug.LogWarning("No handInObject script was found in the handInObjectPrefab, please assign a prefab with the handInObject scrip attached");
                    //            }
                    //        }
                    //        else
                    //        {
                    //            Debug.LogWarning("handInObjectPrefab is null in settings(CustomQuest's settings), please assign a prefab with the HandInObject script attached. If you have one assisgned, please close and open the editor window");
                    //        }
                    //    }
                    //    GUILayout.FlexibleSpace();
                    //    EditorGUILayout.EndHorizontal();
                    //    for (int i = 0; i < selectedQuest.handInObjects.Count; i++)
                    //    {
                    //        if (selectedQuest.handInObjects[i])
                    //        {
                    //            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
                    //            selectedQuest.handInObjects[i] = (HandInObject)EditorGUILayout.ObjectField(selectedQuest.handInObjects[i], typeof(HandInObject), true, GUILayout.Width(150));
                    //            selectedQuest.handInObjects[i].Radius = EditorGUILayout.FloatField(selectedQuest.handInObjects[i].Radius);
                    //            GUILayout.FlexibleSpace();
                    //            if (GUILayout.Button(new GUIContent("-", "Click here to delete this handInObject")))
                    //            {
                    //                DestroyImmediate(selectedQuest.handInObjects[i].gameObject);
                    //                selectedQuest.handInObjects.RemoveAt(i);
                    //                break;
                    //            }
                    //            EditorGUILayout.EndHorizontal();
                    //        }
                    //        else
                    //        {
                    //            selectedQuest.handInObjects.RemoveAt(i);
                    //        }
                    //    }

                    //    //Add quest giver - Adding before converting the quest, currently crashes unity
                    //    //EditorGUILayout.BeginHorizontal();
                    //    //if (GUILayout.Button(new GUIContent("Add Quest Giver", "Click here to spawn a quest giver, giving this quest")))
                    //    //{
                    //    //    if (CustomQuestSettings.SettingsHolder.questGiverPrefab)
                    //    //    {
                    //    //        GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.questGiverPrefab, selectedQuest.gameObject.transform);
                    //    //        QuestGiver questGiver = go.GetComponent<QuestGiver>();
                    //    //        if (!questGiver)
                    //    //        {
                    //    //            questGiver = go.GetComponentInChildren<QuestGiver>();
                    //    //        }
                    //    //        if (questGiver)
                    //    //        {
                    //    //            questGiver.quest = selectedQuest;
                    //    //            Selection.activeObject = go;
                    //    //        }
                    //    //    }
                    //    //}
                    //    //EditorGUILayout.EndHorizontal();

                    //    EditorGUILayout.EndVertical();
                    //}

                    #endregion Relations

                    #region Criterias

                    headLineStyle.fontSize = 20;
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = new Color(0.8f, 0, 0);
                    EditorGUILayout.LabelField(new GUIContent("Criteria", "A list of criteria for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(26));
                    GUI.contentColor = Color.white;
                    GUI.skin = null;
                    if (GUILayout.Button(new GUIContent("Add Criterion", "Click to add a criterion to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Empty"), false, CreateNewCriteriaCallBack, selectedQuest);
                        foreach (Criteria c in allCriterias)
                        {
                            menu.AddItem(new GUIContent(c.criteriaName), false, AddNewCriteriaCallBack, c);
                        }
                        menu.ShowAsContext();
                    }
                    GUI.skin = thisGUISkin;
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    if (R_CriteriaList == null) { SelectQuest(selectedQuest); }
                    if (R_CriteriaList != null)
                    {
                        GUI.skin = null;
                        R_CriteriaList.DoLayoutList();
                        GUI.skin = thisGUISkin;
                    }

                    EditorGUI.indentLevel += 1;
                    selectedQuest.showThresholds = EditorGUILayout.Foldout(selectedQuest.showThresholds, new GUIContent("Tresholds", "Click here to show the thresholds for the different levels of criteria"), true);
                    EditorGUI.indentLevel -= 1;
                    if (selectedQuest.showThresholds)
                    {
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
                        EditorGUILayout.LabelField(new GUIContent("Criteria Thresholds", "A list over the different thresholds for the different levels of criterias"));
                        int maxLevel = 0;
                        if (Event.current.type == EventType.Layout)
                        {
                            foreach (Criteria c in selectedQuest.criterias)
                            {
                                if (selectedQuest.thresholds.Count <= c.Level)
                                {
                                    selectedQuest.thresholds.Add(0);
                                }
                                if (c.Level > maxLevel)
                                {
                                    maxLevel = c.Level;
                                }
                            }
                            maxLevel += 1;
                            if (maxLevel < selectedQuest.thresholds.Count)
                            {
                                selectedQuest.thresholds.RemoveRange(maxLevel, selectedQuest.thresholds.Count - maxLevel);
                            }
                        }

                        for (int i = 0; i < selectedQuest.thresholds.Count; i++)
                        {
                            EditorGUILayout.BeginHorizontal();
                            EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
                            selectedQuest.thresholds[i] = EditorGUILayout.IntField(selectedQuest.thresholds[i]);
                            EditorGUILayout.EndHorizontal();
                        }
                        EditorGUILayout.EndVertical();
                    }
                    EditorGUILayout.EndVertical();

                    #endregion Criterias

                    #region Rewards

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Rewards
                    EditorGUILayout.BeginHorizontal();
                    GUI.contentColor = new Color(0, 0.8f, 0);
                    EditorGUILayout.LabelField(new GUIContent("Rewards", "A list of rewards for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(26));
                    GUI.contentColor = Color.white;
                    GUI.skin = null;
                    if (GUILayout.Button(new GUIContent("Add Reward", "Click to add a reward to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Empty"), false, CreateNewRewardCallBack, selectedQuest);
                        foreach (Reward r in allRewards)
                        {
                            menu.AddItem(new GUIContent(r.rewardName), false, AddNewRewardCallBack, r);
                        }
                        menu.ShowAsContext();
                    }
                    GUI.skin = thisGUISkin;
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                    if (R_RewardList == null) 
                    {
                        SelectQuest(selectedQuest);
                    }
                    if (R_RewardList != null)
                    {
                        R_RewardList.DoLayoutList();
                    }
                    EditorGUILayout.EndVertical();

                    #endregion Rewards

                    #region Optional Criterias

                    if (!CustomQuestSettings.SettingsHolder.optional)
                    {
                        headLineStyle.fontSize = 20;
                        EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
                        EditorGUILayout.BeginHorizontal();
                        GUI.contentColor = new Color(0.8f, 0, 0);
                        EditorGUILayout.LabelField(new GUIContent("Optional Criteria", "A list of optional criterias for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(thisGUISkin.GetStyle("Title").CalcHeight(new GUIContent("Optional Criterias"), EditorGUIUtility.currentViewWidth - 400)), GUILayout.Width(EditorGUIUtility.currentViewWidth - 400));
                        GUI.contentColor = Color.white;
                        GUI.skin = null;
                        if (GUILayout.Button(new GUIContent("Add Criteria", "Click to add an optional criteria to this quest"),  GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Empty"), false, CreateNewOptionalCriteriaCallBack, selectedQuest);
                            foreach (Criteria c in allCriterias)
                            {
                                menu.AddItem(new GUIContent(c.criteriaName), false, AddNewOptionalCriteriaCallBack, c);
                            }
                            menu.ShowAsContext();
                        }
                        GUI.skin = thisGUISkin;
                        EditorGUILayout.EndHorizontal();
                        GUI.backgroundColor = Color.white;
                        if (R_OptionalCriteriaList == null) { SelectQuest(selectedQuest); }
                        if (R_OptionalCriteriaList != null)
                        {
                            R_OptionalCriteriaList.DoLayoutList();
                        }

                        EditorGUI.indentLevel += 1;
                        selectedQuest.showOptionalThresholds = EditorGUILayout.Foldout(selectedQuest.showOptionalThresholds, new GUIContent("Optional Criteria Tresholds", "Click here to show the thresholds for the different levels of criterias"), true);
                        EditorGUI.indentLevel -= 1;
                        if (selectedQuest.showOptionalThresholds)
                        {
                            EditorGUILayout.BeginVertical(EditorStyles.helpBox); //Criterias
                            EditorGUILayout.LabelField(new GUIContent("Optional Criteria Thresholds", "A list over the different optional thresholds for the different levels of criterias"));
                            int maxLevel = 0;
                            if (Event.current.type == EventType.Layout)
                            {
                                foreach (Criteria c in selectedQuest.optionalCriterias)
                                {
                                    if (selectedQuest.optionalThresholds.Count <= c.Level)
                                    {
                                        selectedQuest.optionalThresholds.Add(0);
                                    }
                                    if (c.Level > maxLevel)
                                    {
                                        maxLevel = c.Level;
                                    }
                                }
                                maxLevel += 1;
                                if (maxLevel < selectedQuest.optionalThresholds.Count)
                                {
                                    selectedQuest.optionalThresholds.RemoveRange(maxLevel, selectedQuest.optionalThresholds.Count - maxLevel);
                                }
                            }

                            for (int i = 0; i < selectedQuest.optionalThresholds.Count; i++)
                            {
                                EditorGUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(i.ToString(), GUILayout.Width(30));
                                selectedQuest.optionalThresholds[i] = EditorGUILayout.IntField(selectedQuest.optionalThresholds[i]);
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
                        EditorGUILayout.LabelField(new GUIContent("Optional Rewards", "A list of optional rewards for this quest. Drag to reorder."), thisGUISkin.GetStyle("Title"), GUILayout.Height(thisGUISkin.GetStyle("Title").CalcHeight(new GUIContent("Optional Rewards"), EditorGUIUtility.currentViewWidth - 430)), GUILayout.Width(EditorGUIUtility.currentViewWidth - 430));
                        selectedQuest.completedOptionalThreshold = EditorGUILayout.IntField(/*new GUIContent("Threshold", "The threshold for getting the optional rewards."),*/ selectedQuest.completedOptionalThreshold, GUILayout.Height(22), GUILayout.Width(20), GUILayout.ExpandWidth(false));
                        EditorGUI.LabelField(GUILayoutUtility.GetLastRect(), new GUIContent("", "The amount of criteria needed to get the optional rewards."));
                        //TODO: Fix this layout, so it does not overlap
                        GUILayout.FlexibleSpace();
                        GUI.skin = null;
                        if (GUILayout.Button(new GUIContent("Add Reward", "Click to add an optional reward to this quest"), GUILayout.Height(22), GUILayout.MinWidth(150), GUILayout.MaxWidth(150)))
                        {
                            GenericMenu menu = new GenericMenu();
                            menu.AddItem(new GUIContent("Empty"), false, CreateNewOptionalRewardCallBack, selectedQuest);
                            foreach (Reward r in allRewards)
                            {
                                menu.AddItem(new GUIContent(r.rewardName), false, AddNewOptionalRewardCallBack, r);
                            }
                            menu.ShowAsContext();
                        }
                        GUI.skin = thisGUISkin;
                        EditorGUILayout.EndHorizontal();
                        GUI.backgroundColor = Color.white;
                        if (R_OptionalRewardsList == null) 
                        {
                            SelectQuest(selectedQuest);
                        }
                        if (R_OptionalRewardsList != null)
                        {
                            R_OptionalRewardsList.DoLayoutList();
                        }
                        EditorGUILayout.EndVertical();
                    }

                    #endregion Optional Rewards
                }

                #endregion Selected Quest

                break;

            case 1:

                #region Selected Criteria

                if (selectedCriteria != null)
                {
                    // GUI.color = new Color(0.75f, 0.10f, 0);
                    GUI.color = new Color32(35, 120, 161, 255);
                    Rect rect = new Rect(0, 0, position.width - 238, 70);
                    float height = rect.height - 4;
                    if (selectedCriteria.ShowSpawns)
                    {
                        foreach (SpawnZone zone in selectedCriteria.spawnZones)
                        {
                            if (zone != null)
                            {
                                height += EditorGUIUtility.singleLineHeight * 4 + 5;
                            }
                            else
                            {
                                selectedCriteria.spawnZones.Remove(zone);
                                DestroyImmediate(zone, true);
                            }
                        }
                    }

                    if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                    {
                        if (selectedCriteria.ShowRewards)
                        {
                            foreach (Reward r in selectedCriteria.rewards)
                            {
                                if (r != null)
                                {
                                    height += EditorGUIUtility.singleLineHeight * 2 + 13;
                                }
                                else
                                {
                                    selectedCriteria.rewards.Remove(r);
                                }
                            }
                        }
                    }
                    if (selectedCriteria.ShowSettings)
                    {
                        height += EditorGUIUtility.singleLineHeight * 2;
                    }

                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width + 20, height), "", MessageType.None);

                    #region Standard info

                    GUI.color = Color.white;
                    rect.y += 10;
                    rect.x += 10;
                    selectedCriteria.criteriaName = EditorGUI.TextField(
                        new Rect(rect.x, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight),
                         selectedCriteria.criteriaName);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 200, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this criteria"));
                    if (selectedCriteria.criteriaName != tmpQuestName)
                    { //Starts the change of the name
                        tmpQuestName = selectedCriteria.criteriaName;
                        lastName = selectedCriteria.name;
                        updateUI = true;
                        updateCounter = 1;
                        renameType = TypeToRename.Criteria;
                    }

                    selectedCriteria.Level = EditorGUI.IntField(new Rect(rect.x + rect.width - 200, rect.y, 30, EditorGUIUtility.singleLineHeight), selectedCriteria.Level);
                    if (selectedCriteria.Level < 0)
                    {
                        selectedCriteria.Level = 0;
                    }
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 200, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "The level of this criteria controlling when this criteria is avalible for completion. All level '0' will have to be completed, before level '1' will activate"));

                    GUI.skin = null;
                    if (selectedCriteria.isCustomScript != true)
                    {
                        if (GUI.Button(new Rect(rect.x + rect.width - 165, rect.y, 70, EditorGUIUtility.singleLineHeight), new GUIContent("Convert", "Click here to convert this script"))) { CreateCriteriaPopUp(selectedCriteria); }
                    }
                    else if (GUI.Button(new Rect(rect.x + rect.width - 165, rect.y, 70, EditorGUIUtility.singleLineHeight), new GUIContent("Open", "Click here to open this script")))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + selectedCriteria.name + "/" + selectedCriteria.name + ".cs", typeof(UnityEngine.Object)));
                    }
                    // Select prefab button
                    if (GUI.Button(new Rect(rect.x + rect.width - 90, rect.y, 60, EditorGUIUtility.singleLineHeight), new GUIContent("Select", "Click here to select the prefab")))
                    {
                        UnityEngine.Object foundAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(selectedCriteria), typeof(UnityEngine.Object));
                        if (foundAsset != null) { Selection.activeObject = foundAsset; }
                    }

                    if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, 20, EditorGUIUtility.singleLineHeight), "-"))
                    {
                        CriteriaPrefablDeleteWindow window = CriteriaPrefablDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                        window.SetQuestEditor(this, selectedCriteria); //Sets the editor in the QuestDeleteWindow
                        window.position = ClampToScreen(new Rect(Screen.width / 2, Screen.height / 2, 250, 50), Screen.width, Screen.height); // Sets the new windows position to the mouse position
                    }
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 40, rect.y, 20, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this criteria"));

                    GUI.skin = thisGUISkin;

                    selectedCriteria.type = (criteriaType)EditorGUI.EnumPopup(
                        new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        selectedCriteria.type);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "Change the criteria type"));

                    selectedCriteria.amount = EditorGUI.IntField(
                       new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                       selectedCriteria.amount);
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "Amount of criteria objects to do, before criteria is completed"));

                    selectedCriteria.criteriaObject = (GameObject)EditorGUI.ObjectField(
                         new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        selectedCriteria.criteriaObject, typeof(GameObject), true);
                    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The criteria object. The goblin to kill. The berries to collect, etc."));

                    #endregion Standard info

                    EditorGUI.indentLevel += 1;
                    //selectedCriteria.ShowSpawns = EditorGUI.Foldout(
                    //    new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                    //    selectedCriteria.ShowSpawns, "Show Spawns", true);
                    //if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                    //{
                    //    selectedCriteria.ShowRewards = EditorGUI.Foldout(
                    //        new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                    //        selectedCriteria.ShowRewards, "Show Rewards", true);
                    //}
                    selectedCriteria.ShowSettings = EditorGUI.Foldout(
                        new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                        selectedCriteria.ShowSettings, "Show Settings", true);
                    EditorGUI.indentLevel -= 1;
                    float zoneHeight = rect.y + 3 + EditorGUIUtility.singleLineHeight * 3; ;
                    switch (selectedCriteria.toolbarInt)
                    {
                        case 1: //Spawns
                            if (selectedCriteria.ShowSpawns)
                            {
                                #region Spawns

                                if (GUI.Button(new Rect(rect.x + rect.width / 3 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click here to add a spawn to this criteria"), thisGUISkin.GetStyle("Plus")))
                                {
                                    if (Resources.FindObjectsOfTypeAll(typeof(CustomQuestEditor)) != null)
                                    {
                                        if (CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab)
                                        {
                                            GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab, selectedCriteria.gameObject.transform);
                                            if (go.GetComponent<SpawnZone>())
                                            {
                                                selectedCriteria.spawnZones.Add(go.GetComponent<SpawnZone>());
                                                go.GetComponent<SpawnZone>().Criteria = selectedCriteria;
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

                                if (selectedCriteria.ShowSpawns)
                                {
                                    foreach (SpawnZone zone in selectedCriteria.spawnZones)
                                    {
                                        if (zone == null)
                                        {
                                            selectedCriteria.spawnZones.Remove(zone);
                                            break;
                                        }
                                        EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 4 + 5), "", MessageType.None);
                                        zone.SpawnName = EditorGUI.TextField(
                                            new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight),
                                            zone.SpawnName);
                                        EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this spawnZone"));
                                        zone.Spawn = EditorGUI.Toggle(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), zone.Spawn);
                                        EditorGUI.LabelField(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click here to enable / disable spawning."));
                                        if (GUI.Button(new Rect(rect.x + rect.width - 30, zoneHeight + 3, 30, EditorGUIUtility.singleLineHeight), new GUIContent("-", "Click here to delete this zone")))
                                        {
                                            selectedCriteria.spawnZones.Remove(zone);
                                            DestroyImmediate(zone.gameObject, true);
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
                            if (selectedCriteria.ShowRewards && CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                            {
                                #region Rewards

                                if (GUI.Button(new Rect(rect.x + rect.width / 3 * 2 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click here to add a spawn to this criteria"), thisGUISkin.GetStyle("Plus")))
                                {
                                    Reward r = selectedCriteria.gameObject.AddComponent<Reward>();
                                    r.editoreRewardType = editorRewardType.Criteria;
                                    selectedCriteria.rewards.Add(r);
                                }
                                foreach (Reward r in selectedCriteria.rewards)
                                {
                                    GUI.color = new Color(0, 0.75f, 0.15f);
                                    EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 2 + 13), "", MessageType.None);
                                    GUI.color = Color.white;
                                    r.rewardName = EditorGUI.TextField(
                                        new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight),
                                         r.rewardName);
                                    EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
                                    if (GUI.Button(new Rect(rect.x + rect.width - 30, zoneHeight + 5, 30, EditorGUIUtility.singleLineHeight),
                                        "-")) { selectedCriteria.rewards.Remove(r); DestroyImmediate(r, true); break; }
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
                            if (selectedCriteria.ShowSettings)
                            {
                                #region Settings

                                EditorGUI.indentLevel += 1;

                                if (selectedCriteria.ShowSettings)
                                {
                                    selectedCriteria.timed = EditorGUI.Toggle(new Rect(rect.x + 40, zoneHeight, 30, EditorGUIUtility.singleLineHeight), selectedCriteria.timed);
                                    EditorGUI.LabelField(new Rect(rect.x, zoneHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("Timed", "If true, this criteria is timed. If time runs out, the criteria will fail. And maybe the entire quest!"));
                                    if (selectedCriteria.timed) { selectedCriteria.time = EditorGUI.FloatField(new Rect(rect.x + 60, zoneHeight, 50, EditorGUIUtility.singleLineHeight), selectedCriteria.time); }
                                    GUILayout.FlexibleSpace();
                                    EditorGUI.LabelField(new Rect(rect.xMax - 170, zoneHeight, rect.width / 2, EditorGUIUtility.singleLineHeight), new GUIContent("Dont Despawn Objects", "If true, this criteria will not remove its spawned objects when its completed (and no player is current doing it)"));
                                    selectedCriteria.dontDespawnObjectsWhenComplete = EditorGUI.Toggle(new Rect(rect.xMax - 35, zoneHeight, 30, EditorGUIUtility.singleLineHeight), selectedCriteria.dontDespawnObjectsWhenComplete);
                                    zoneHeight += EditorGUIUtility.singleLineHeight;
                                    EditorGUI.LabelField(new Rect(rect.x, zoneHeight, 185, EditorGUIUtility.singleLineHeight), new GUIContent("Give Rewards OnCompletion", "If true, will give this criterias rewards when this criteria is completed. Otherwise, it will give them when the quest is completed"));
                                    selectedCriteria.giveRewardsOnCompletion = EditorGUI.Toggle(new Rect(rect.x + 170, zoneHeight, 30, EditorGUIUtility.singleLineHeight), selectedCriteria.giveRewardsOnCompletion);
                                }
                                EditorGUI.indentLevel -= 1;

                                #endregion Settings
                            }
                            break;
                    }
                }

                #endregion Selected Criteria

                break;

            case 2:

                #region Selected Reward

                if (selectedReward != null)
                {
                    Rect rect = new Rect(0, 0, position.width - 238, 60);
                    Reward r = selectedReward;
                    //GUI.color = new Color(0, 0.75f, 0.15f);
                    GUI.color = new Color32(35, 120, 161, 255);
                    EditorGUI.HelpBox(new Rect(rect.x, rect.y, rect.width + 20, rect.height - 5), "", MessageType.None);
                    GUI.color = Color.white;
                    rect.y += 10;
                    rect.x += 10;
                    r.rewardName = EditorGUI.TextField(
                        new Rect(rect.x, rect.y, rect.width - 170, EditorGUIUtility.singleLineHeight),
                         r.rewardName);
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 170, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
                    if (selectedReward.rewardName != tmpQuestName)
                    { //Starts the change of the name
                        tmpQuestName = selectedReward.rewardName;
                        lastName = selectedReward.name;
                        updateUI = true;
                        updateCounter = 1;
                        renameType = TypeToRename.Reward;
                    }

                    GUI.skin = null;
                    if (selectedReward.isCustomScript != true)
                    {
                        if (GUI.Button(new Rect(rect.x + rect.width - 165, rect.y, 70, EditorGUIUtility.singleLineHeight), new GUIContent("Convert", "Click here to convert this script"))) { CreateRewardPopUp(selectedReward); }
                    }
                    else if (GUI.Button(new Rect(rect.x + rect.width - 165, rect.y, 70, EditorGUIUtility.singleLineHeight), new GUIContent("Open", "Click here to open this script")))
                    {
                        AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + selectedReward.name + "/" + selectedReward.name + ".cs", typeof(UnityEngine.Object)));
                    }
                    // Select prefab button
                    if (GUI.Button(new Rect(rect.x + rect.width - 90, rect.y, 60, EditorGUIUtility.singleLineHeight), new GUIContent("Select", "Click here to select the prefab")))
                    {
                        UnityEngine.Object foundAsset = AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(selectedReward), typeof(UnityEngine.Object));
                        if (foundAsset != null) { Selection.activeObject = foundAsset; }
                    }

                    if (GUI.Button(new Rect(rect.x + rect.width - 15, rect.y, 20, EditorGUIUtility.singleLineHeight),
                        ""))
                    {
                        RewardPrefabDeleteWindow window = RewardPrefabDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                        window.SetQuestEditor(this, selectedReward); //Sets the editor in the QuestDeleteWindow
                        window.position = ClampToScreen(new Rect(Screen.width / 2, Screen.height / 2, 250, 50), Screen.width, Screen.height); // Sets the new windows position to the mouse position
                    }
                    EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, rect.y, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this reward"));
                    GUI.skin = thisGUISkin;

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

                #endregion Selected Reward

                break;
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void OnGUIQuestsInScene()
    {
        currentEvent = Event.current;
        if (currentEvent.type == EventType.MouseDown && currentEvent.button == 1)
        {
            makingNodeConnection = false;
        }
        //GUI.color = new Color32(196, 185, 127, 255);
        //GUI.color = new Color32(35, 120, 161, 255);
        GUI.color = new Color32(135, 135, 135, 255);
        GUILayout.BeginArea(new Rect(5, 20, 195, position.height - 25), thisGUISkin.box);
        GUI.color = Color.white;
        leftScrollview = EditorGUILayout.BeginScrollView(leftScrollview, GUILayout.Width(195), GUILayout.Height(position.height - 20));
        if (R_questInSceneList != null)
        {
            if (R_questInSceneList.list.Count > 0 && R_questInSceneList.list != null)
            {
                GUI.skin = null;
                R_questInSceneList.DoLayoutList(); //TODO: Repaint error here?
                GUI.skin = thisGUISkin;
            }
        }
        else
        {
            Debug.Log("ReorderAble Quest in scene list is null, restarting window to fix");
            Awake();
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        //GUI.color = new Color32(196, 185, 127, 255);
        //GUI.color = new Color32(254, 252, 224, 255);
        GUI.color = new Color32(194, 194, 194, 255);
        GUILayout.BeginArea(new Rect(205, 20, position.width - 210, position.height - 25), thisGUISkin.box);
        GUI.color = Color.white;
        rightScrollview = GUILayout.BeginScrollView(rightScrollview, GUILayout.Width(position.width - 210), GUILayout.Height(position.height - 20));

        if (makingNodeConnection == true)
        {
            if (originEdge.left == false)
            { //Right Edges
                if (originEdge.questNode.completeEdge == originEdge)
                { //Draws green line
                    DrawCurves(new Rect(originNode.Rectangle.x + originEdge.rect.x, originNode.Rectangle.y + originEdge.rect.y, originEdge.rect.width, originEdge.rect.height), new Rect(Event.current.mousePosition, new Vector2(0, 0)), new Color(0, 0.5f, 0));
                }
                else if (originEdge.questNode.failEdge == originEdge)
                { //Draws red line
                    DrawCurves(new Rect(originNode.Rectangle.x + originEdge.rect.x, originNode.Rectangle.y + originEdge.rect.y, originEdge.rect.width, originEdge.rect.height), new Rect(Event.current.mousePosition, new Vector2(0, 0)), new Color(0.5f, 0, 0));
                }
                else
                { //Draws black line
                    DrawCurves(new Rect(originNode.Rectangle.x + originEdge.rect.x, originNode.Rectangle.y + originEdge.rect.y, originEdge.rect.width, originEdge.rect.height), new Rect(Event.current.mousePosition, new Vector2(0, 0)), new Color(0, 0, 0));
                }
            }
            else
            { // Left Edges
              //Draws blue line
                DrawCurves(new Rect(Event.current.mousePosition, new Vector2(0, 0)), new Rect(originNode.Rectangle.x + originEdge.rect.x, originNode.Rectangle.y + originEdge.rect.y, originEdge.rect.width, originEdge.rect.height), new Color(0, 0, 0.5f));
            }
        }
        nodes.Clear();
        foreach (Quest q in FindObjectsOfType<Quest>())
        {
            if (q.questNode != null)
            {
                nodes.Add(q.questNode);
            }
            else
            {
                Debug.Log(q + " is missing its questNode");
            }
        }
        BeginWindows();
        int cnt = 0;
        foreach (QuestNode qn in nodes)
        {
            if (qn != null) //TODO: make a check to tell the player if the quest this questnode is displaying, for some reason is null
            {
                if (qn.quest != null)
                {
                    foreach (QuestEdge qe in qn.allEdges)
                    {
                        if (qe.left == false)
                        {
                            foreach (QuestConnection qc in qe.connections)
                            {
                                if (nodes.Contains(qc.leftEdge.questNode))
                                {
                                    if (nodes.Contains(qc.rightEdge.questNode))
                                    {
                                        if (qc.leftEdge.questNode.completeEdge == qc.leftEdge)
                                        { //If it comes from a complete edge, draws a green line
                                            DrawCurves(new Rect(qc.leftEdge.questNode.Rectangle.x + qc.leftEdge.rect.x, qc.leftEdge.questNode.Rectangle.y + qc.leftEdge.rect.y, qc.leftEdge.rect.width, qc.leftEdge.rect.height), new Rect(qc.rightEdge.questNode.Rectangle.x + qc.rightEdge.rect.x, qc.rightEdge.questNode.Rectangle.y + qc.rightEdge.rect.y, qc.rightEdge.rect.width, qc.rightEdge.rect.height), new Color(0, 0.5f, 0));
                                        }
                                        else if (qc.leftEdge.questNode.failEdge == qc.leftEdge)
                                        { //If it comes from a fail edge, draws a red line
                                            DrawCurves(new Rect(qc.leftEdge.questNode.Rectangle.x + qc.leftEdge.rect.x, qc.leftEdge.questNode.Rectangle.y + qc.leftEdge.rect.y, qc.leftEdge.rect.width, qc.leftEdge.rect.height), new Rect(qc.rightEdge.questNode.Rectangle.x + qc.rightEdge.rect.x, qc.rightEdge.questNode.Rectangle.y + qc.rightEdge.rect.y, qc.rightEdge.rect.width, qc.rightEdge.rect.height), new Color(0.5f, 0, 0));
                                        }
                                        else
                                        { //Draws with default color
                                            DrawCurves(new Rect(qc.leftEdge.questNode.Rectangle.x + qc.leftEdge.rect.x, qc.leftEdge.questNode.Rectangle.y + qc.leftEdge.rect.y, qc.leftEdge.rect.width, qc.leftEdge.rect.height), new Rect(qc.rightEdge.questNode.Rectangle.x + qc.rightEdge.rect.x, qc.rightEdge.questNode.Rectangle.y + qc.rightEdge.rect.y, qc.rightEdge.rect.width, qc.rightEdge.rect.height), new Color(0, 0, 0, 0.5f));
                                        }
                                    }
                                    else
                                    {
                                        qc.rightEdge.connections.Remove(qc);
                                        break;
                                    }
                                }
                                else
                                {
                                    qc.leftEdge.connections.Remove(qc);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < qe.connections.Count; i++)
                            {
                                if (qe.connections[i] == null)
                                {
                                    qe.connections.RemoveAt(i);
                                }
                            }
                        }
                    }
                    // qn.Rectangle = GUI.Window(cnt, qn.Rectangle, DoQuestNodeWindow, qn.quest.questName);
                    qn.Rectangle = qn.quest.rectangleNode;
                    GUI.backgroundColor = new Color32(132, 132, 132, 255);
                    GUI.skin = null;
                    qn.Rectangle = ClampToScreen(GUI.Window(cnt, qn.Rectangle, DoQuestNodeWindow, qn.quest.questName), Screen.width - 150, Screen.height - 50);
                    GUI.skin = thisGUISkin;
                    GUI.backgroundColor = Color.white;
                    qn.windowID = cnt;
                    cnt++;
                }
                else
                {
                    object o = EditorUtility.InstanceIDToObject(qn.quest.GetInstanceID()); //Tries to find the quest again
                    qn.quest = (Quest)o;
                    if (qn.quest == null) //Remove if it found none
                    {
                        nodes.Remove(qn);
                    }
                    break;
                }
            }
            else
            {
                nodes.Remove(qn);
                break;
            }
        }
        EndWindows(); //TODO: Fix the error given here when deleting a node //Fixed??
        if (deletingNode == true)
        {
            if (nodeToDelete)
            {
                for (int i = 0; i < nodeToDelete.allEdges.Count; i++)
                {
                    QuestEdge qe = nodeToDelete.allEdges[i];
                    if (qe.left)
                    { //Left
                        for (int j = 0; j < qe.connections.Count; j++)
                        {
                            QuestConnection qc = qe.connections[j];
                            qc.leftEdge.questNode.quest.questsToUnlock.Remove(qc.rightEdge.questNode.quest);
                            qc.leftEdge.connections.Remove(qc);
                        }
                    }
                    else
                    { //Right
                        for (int j = 0; j < qe.connections.Count; j++)
                        {
                            QuestConnection qc = qe.connections[j];
                            qc.rightEdge.questNode.quest.unCompletedQuests.Remove(qc.leftEdge.questNode.quest);
                            qc.rightEdge.connections.Remove(qc);
                        }
                    }
                }
                DestroyImmediate(nodeToDelete.quest.gameObject);
                if (!EditorSceneManager.GetActiveScene().isDirty)
                {
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }
            nodeToDelete = null;
            deletingNode = false;
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void CreateQuest(GameObject go)
    {
        GameObject tmpGo = Instantiate(go, new Vector3(0, 0, 0), Quaternion.identity);
        Quest quest = tmpGo.GetComponent<Quest>();
        quest.prefab = go;
        QuestNode qn = CreateInstance<QuestNode>();
        qn.quest = quest;
        quest.questNode = qn;
        qn.Start();
        if (!EditorSceneManager.GetActiveScene().isDirty)
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
    }

    private void DoQuestNodeWindow(int id)
    {
        if (nodes.ElementAtOrDefault(id) != null)
        {
            QuestNode qn = nodes[id];

            bool buttonClicked = false;

            //GUI.TextArea(new Rect(new Vector2(14, 4), thisGUISkin.label.CalcSize(new GUIContent(qn.quest.questName))), qn.quest.questName);

            if (GUI.Button(new Rect(qn.Rectangle.width - 14, 4, 10, 10), " "/*, thisGUISkin.GetStyle("SmallButton")*/))
            {
                NodeDeleteWindow window = NodeDeleteWindow.Instance; // Gets the window, or creates a new one if none exists
                window.SetQuestEditor(this, qn); //Sets the editor in the NodeDeleteWindow
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                window.position = new Rect(mousePos.x - 200, mousePos.y, 400, 200);// Sets the new windows position to the mouse positoin
            }
            if (GUI.Button(new Rect(qn.Rectangle.width / 2 - 45, qn.Rectangle.height - 30, 90, 25), "Select"))
            {
                object o = EditorUtility.InstanceIDToObject(qn.quest.GetInstanceID());
                Selection.activeObject = (Quest)o;
            }
            if (qn.startEdge)
            {
                EditorGUI.DrawRect(qn.startEdge.rect, new Color(0, 0, 0.5f));
                if (qn.startEdge.rect.Contains(currentEvent.mousePosition))
                {
                    EditorGUI.LabelField(new Rect(currentEvent.mousePosition, new Vector2(500, 500)), "Start Edge");
                }
                if (currentEvent.isMouse && qn.startEdge.rect.Contains(currentEvent.mousePosition))
                { // Mouse Click
                    if (currentEvent.button == 0)
                    { // Left click
                        buttonClicked = true;
                        if (makingNodeConnection != true && currentEvent.type == EventType.MouseDown)
                        {
                            //currentEdge = qn.startEdge;
                            makingNodeConnection = true;
                            originNode = qn;
                            originEdge = qn.startEdge;
                        }
                        else if (originNode != null)
                        {
                            if (originNode.startEdge != qn.startEdge && currentEvent.type == EventType.MouseUp)
                            {  // Click and drag connections logic
                                makingNodeConnection = false;
                                MakeQuestConnection(originEdge, qn.startEdge);
                            }
                        }
                    }
                    else if (currentEvent.button == 1)
                    { // Right Click
                        buttonClicked = true;
                        if (currentEvent.type == EventType.MouseDown)
                        {
                            GenericMenu menu = new GenericMenu();
                            foreach (QuestConnection c in qn.startEdge.connections)
                            {
                                menu.AddItem(new GUIContent("Delete connection to: " + c.leftEdge.questNode.quest.questName), false, ConnectionDeleteCallBack, c); //TODO: somehow make this delete the connections
                                if (connectionDeleted)
                                {
                                    connectionDeleted = false;
                                    break;
                                }
                            }
                            menu.ShowAsContext();
                            currentEvent.Use();
                        }
                    }
                }
            }
            if (qn.completeEdge)
            {
                EditorGUI.DrawRect(qn.completeEdge.rect, new Color(0, 0.5f, 0));
                GUI.color = new Color(0, 0.5f, 0);
                EditorGUI.LabelField(new Rect(qn.completeEdge.rect.x - 90, qn.completeEdge.rect.y - 3, 100, 100), "Complete Edge"); //TODO: Tooltip is not showing properly, looks like a layer problem?
                GUI.color = Color.white;
                if (currentEvent.isMouse && qn.completeEdge.rect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.button == 0)
                    {
                        buttonClicked = true;
                        if (makingNodeConnection != true && currentEvent.type == EventType.MouseDown)
                        {
                            //currentEdge = qn.completeEdge;
                            makingNodeConnection = true;
                            originNode = qn;
                            originEdge = qn.completeEdge;
                        }
                        else if (originNode) //Checks for null, to avoid error
                        {
                            if (originNode.completeEdge != qn.completeEdge && currentEvent.type == EventType.MouseUp)
                            { // Click and drag connections logic
                                makingNodeConnection = false;
                                MakeQuestConnection(qn.completeEdge, originEdge);
                            }
                        }
                    }
                    else if (currentEvent.button == 1)
                    { // Right Click
                        buttonClicked = true;
                        if (currentEvent.type == EventType.MouseDown)
                        {
                            GenericMenu menu = new GenericMenu();
                            foreach (QuestConnection c in qn.completeEdge.connections)
                            {
                                menu.AddItem(new GUIContent("Delete connection to: " + c.rightEdge.questNode.quest.questName), false, ConnectionDeleteCallBack, c);
                                if (connectionDeleted)
                                {
                                    connectionDeleted = false;
                                    break;
                                }
                            }
                            menu.ShowAsContext();
                            currentEvent.Use();
                        }
                    }
                }
            }

            if (qn.failEdge)
            {
                EditorGUI.DrawRect(qn.failEdge.rect, new Color(0.5f, 0, 0));
                GUI.color = new Color(0.5f, 0, 0);
                EditorGUI.LabelField(new Rect(qn.failEdge.rect.x - 55, qn.failEdge.rect.y - 3, 100, 100), "Fail Edge"); //TODO: Tooltip is not showing properly, looks like a layer problem?
                GUI.color = Color.white;
                if (currentEvent.isMouse && qn.failEdge.rect.Contains(currentEvent.mousePosition))
                {
                    if (currentEvent.button == 0)
                    {
                        buttonClicked = true;
                        if (makingNodeConnection != true && currentEvent.type == EventType.MouseDown)
                        {
                            //currentEdge = qn.failEdge;
                            makingNodeConnection = true;
                            originNode = qn;
                            originEdge = qn.failEdge;
                        }
                        else if (originNode)
                        {
                            if (originNode.failEdge != qn.failEdge && currentEvent.type == EventType.MouseUp)
                            { // Click and drag connections logic
                                makingNodeConnection = false;
                                MakeQuestConnection(qn.failEdge, originEdge);
                            }
                        }
                    }
                    else if (currentEvent.button == 1)
                    { // Right Click
                        buttonClicked = true;
                        if (currentEvent.type == EventType.MouseDown)
                        {
                            GenericMenu menu = new GenericMenu();
                            foreach (QuestConnection c in qn.failEdge.connections)
                            {
                                menu.AddItem(new GUIContent("Delete connection to: " + c.rightEdge.questNode.quest.questName), false, ConnectionDeleteCallBack, c);
                                if (connectionDeleted)
                                {
                                    connectionDeleted = false;
                                    break;
                                }
                            }
                            menu.ShowAsContext();
                            currentEvent.Use();
                        }
                    }
                }
            }

            if (buttonClicked == false && new Rect(0, 0, 200, 15).Contains(currentEvent.mousePosition))
            { //Drags window, if no button is clicked, but the mouse is in the top bar of the node
                GUI.DragWindow();
            }
        }
    }

    private void MakeQuestConnection(QuestEdge from, QuestEdge to)
    {
        if (from.left != to.left)
        {
            bool skip = false;

            foreach (QuestConnection qn in to.connections)
            {
                if (qn.leftEdge == from)
                {
                    skip = true;
                }
                if (qn.rightEdge == from)
                {
                    skip = true;
                }
            }
            if (!skip)
            {
                QuestConnection qC = CreateInstance<QuestConnection>();
                qC.leftEdge = from;
                qC.rightEdge = to;
                from.connections.Add(qC);
                to.connections.Add(qC);
                from.questNode.quest.questsToUnlock.Add(to.questNode.quest);
                to.questNode.quest.unCompletedQuests.Add(from.questNode.quest);
                if (!EditorSceneManager.GetActiveScene().isDirty)
                {
                    if (!Application.isPlaying)
                    {
                        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                    }
                }
            }
            else
            {
                Debug.Log("Edge already contains connection");
            }
        }
    }

    private void ConnectionDeleteCallBack(object obj)
    {
        QuestConnection qc = (QuestConnection)obj;
        qc.leftEdge.connections.Remove(qc);
        qc.leftEdge.questNode.quest.questsToUnlock.Remove(qc.rightEdge.questNode.quest);
        qc.rightEdge.connections.Remove(qc);
        qc.rightEdge.questNode.quest.unCompletedQuests.Remove(qc.leftEdge.questNode.quest);
        DestroyImmediate(qc, true);
        AssetDatabase.SaveAssets();
        if (!EditorSceneManager.GetActiveScene().isDirty)
        {
            if (!Application.isPlaying)
            {
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        connectionDeleted = true;
    }

    public static void DrawCurves(Rect startRect, Rect endRect, Color color)
    {
        Vector3 startPos = new Vector3(startRect.x + startRect.width, startRect.y + 3 + startRect.height / 2, 0);
        Vector3 endPos = new Vector3(endRect.x, endRect.y + endRect.height / 2, 0);
        float mnog = Vector3.Distance(startPos, endPos);
        Vector3 startTangent = startPos + Vector3.right * (mnog / 3f);
        Vector3 endTangent = endPos + Vector3.left * (mnog / 3f);
        Handles.BeginGUI();
        Handles.DrawBezier(startPos, endPos, startTangent, endTangent, color, null, 3f);
        Handles.EndGUI();
    }

    private Rect ClampToScreen(Rect r, float screenWidth, float screenHeight)
    {
        r.x = Mathf.Clamp(r.x, 0, screenWidth - r.width);
        r.y = Mathf.Clamp(r.y, 0, screenHeight - r.height);
        return r;
    }

    private List<string> WordWrap(string input, int maxCharacters)
    {
        List<string> lines = new List<string>();

        if (!input.Contains(" ") && !input.Contains("\n"))
        {
            int start = 0;
            while (start < input.Length)
            {
                lines.Add(input.Substring(start, Math.Min(maxCharacters, input.Length - start)));
                start += maxCharacters;
            }
        }
        else
        {
            string[] paragraphs = input.Split('\n');

            foreach (string paragraph in paragraphs)
            {
                string[] words = paragraph.Split(' ');

                string line = "";
                foreach (string word in words)
                {
                    if ((line + word).Length > maxCharacters)
                    {
                        lines.Add(line.Trim());
                        line = "";
                    }

                    line += string.Format("{0} ", word);
                }

                if (line.Length > 0)
                {
                    lines.Add(line.Trim());
                }
            }
        }
        return lines;
    } //TODO: Put somewhere so everyone can use?

    private void RenamePrefab(GameObject go)
    {
        updateUI = false;
        updateCounter = 1;
        switch (renameType)
        {
            case TypeToRename.Quest:
                /*Quest q = go.GetComponent<Quest>();
                q.name = q.questName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(q), q.questName);
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + lastName + "/" + lastName + ".cs", q.questName); //Renames the quest script //TODO: make it also rename the name of the script
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + lastName, q.questName);*/ //TODO: Check for invalid names (Unity kinda does that for you?)
                Quest q = go.GetComponent<Quest>();
                latestRenamed = q.name;
                q.name = q.questName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(q), q.name);
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + lastName + "/" + lastName + ".cs", q.name); //Renames the quest script //TODO: make it also rename the name of the script
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + lastName, q.name);//TODO: Check for invalid names (Unity kinda does that for you?)
                break;

            case TypeToRename.Reward:
                /*Reward r = go.GetComponent<Reward>();
                r.name = r.rewardName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(r), r.rewardName);
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + lastName, r.rewardName);*/
                Reward r = go.GetComponent<Reward>();
                r.name = r.rewardName;
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(r), r.name);
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + lastName, r.name);
                break;

            case TypeToRename.Criteria:
                /*Criteria c = go.GetComponent<Criteria>();
                c.name = c.criteriaName;
                //if (AssetDatabase.IsValidFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName))
                //{
                //    System.IO.File.Move("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + lastName + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + c.criteriaName + ".cs");
                //}
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(c), c.criteriaName);
                //AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + lastName + ".cs", c.criteriaName); //Renames the quest script //TODO: make it also rename the name of the script)
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName, c.criteriaName);*/
                Criteria c = go.GetComponent<Criteria>();
                c.name = c.criteriaName;
                //if (AssetDatabase.IsValidFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName))
                //{
                //    System.IO.File.Move("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + lastName + ".cs", "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + c.criteriaName + ".cs");
                //}
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(c), c.name);
                //AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName + "/" + lastName + ".cs", c.criteriaName); //Renames the quest script //TODO: make it also rename the name of the script)
                AssetDatabase.RenameAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + lastName, c.name);
                break;
        }
    }

    /*** PUBLIC METHODS ***/

    public void CopyAll<T>(T source, T target)
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

    /*** Quest Methods ***/

    private void UpdateQuestList()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests" });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject newQuest = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (newQuest != null)
            {
                if (newQuest.GetComponent<Quest>())
                {
                    if (!allQuests.Contains(newQuest.GetComponent<Quest>()))
                    {
                        allQuests.Add(newQuest.GetComponent<Quest>());
                    }
                }
            }
        }
    }

    private void CreateNewQuest()
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedQuest.gameObject);
        }
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Quest>();
        //PrefabUtility.CreatePrefab("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/tmpQuestName.prefab", gameObject);
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/tmpQuestName.prefab");
        GameObject tmpNewGameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/tmpQuestName.prefab", typeof(GameObject));
        tmpNewGameObject.name = "tmpQuestName";
        tmpNewGameObject.GetComponent<Quest>().questName = tmpNewGameObject.name;
        SelectQuest(tmpNewGameObject.GetComponent<Quest>());
        DestroyImmediate(gameObject);
        ClearFocus();
        UpdateQuestList();
    }

    public void ConvertToCustomQuest(Quest q)
    {
        //Instantiate a new gameObject
        tmpGameObject = new GameObject();

        //Name it the same as the character name
        tmpGameObject.name = q.name;

        //Add the quest script component.
        tmpGameObject.AddComponent<Quest>();

        tmpQuest = Instantiate(q);

        if (!AssetDatabase.IsValidFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + tmpGameObject.name))
        {
            AssetDatabase.CreateFolder("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests", tmpGameObject.name);
        }
        TextAsset templateTextFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Templates/QuestTemplate.txt", typeof(TextAsset));

        string contents = "";
        if (templateTextFile != null)
        {
            contents = templateTextFile.text;
            contents = contents.Replace("QUESTCLASS_NAME_HERE", tmpGameObject.name.Replace(" ", ""));
            contents = contents.Replace("QUEST_NAME_HERE", tmpGameObject.name);
            q.description = q.description.Replace("\r\n", "NEWLINE").Replace("\n", "NEWLINE").Replace("\r", "NEWLINE");
            contents = contents.Replace("DESCRIPTION_HERE", q.description);
            q.toolTip = q.toolTip.Replace("\r\n", "NEWLINE").Replace("\n", "NEWLINE").Replace("\r", "NEWLINE");
            contents = contents.Replace("TOOLTIP_HERE", q.toolTip);
            contents = contents.Replace("START_AVAILABILTY_HERE", q.startAvailability.ToString().ToLower());
            contents = contents.Replace("AUTO_COMPLETE_HERE", q.autoComplete.ToString().ToLower());
            contents = contents.Replace("REPEATABLE_HERE", q.repeatable.ToString().ToLower());
        }
        else
        {
            Debug.LogError("Can't find the QuestTemplate.txt!, Is it the path you wrote at templateTextFile wrong?");
        }

        //   Debug.Log(Application.dataPath);
        using (StreamWriter sw = new StreamWriter(string.Format(Application.dataPath + "/{0}.cs",
            new object[] { tmpGameObject.name.Replace(" ", "") })))
        {
            sw.Write(contents);
        }
        AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(q));

        AssetDatabase.Refresh();

        needToAttach = true;

        attachType = TypeToAttach.Quest;
    }

    public void DeleteQuest(Quest q)
    {
        allQuests.Remove(q);
        if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + q.name + ".prefab", typeof(GameObject)))
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + q.name + ".prefab");
        }
        else
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + q.name);
        }
        ClearFocus();
    }

    private void SelectQuest(Quest q)
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedQuest.gameObject);
        }
        ClearFocus();
        selectedQuest = q;
        tmpQuestName = selectedQuest.questName;
        R_CriteriaList = CreateCriteriaList(selectedQuest.criterias);
        R_RewardList = CreateRewardList(selectedQuest.rewards);
        R_OptionalCriteriaList = CreateCriteriaList(selectedQuest.optionalCriterias);
        R_OptionalRewardsList = CreateRewardList(selectedQuest.optionalRewards);
    }

    private void R_SelectQuest(ReorderableList questList)
    {
        SelectQuest(allQuests[questList.index]);
    }

    private void R_SelectQuestInScene(ReorderableList questList)
    {
        CreateQuest(allQuests[questList.index].gameObject);
    }

    /*** Criteria Methods ***/

    private void UpdateCriteriaList()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias" });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject newCriteria = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (newCriteria != null)
            {
                if (newCriteria.GetComponent<Criteria>())
                {
                    if (!allCriterias.Contains(newCriteria.GetComponent<Criteria>()))
                    {
                        allCriterias.Add(newCriteria.GetComponent<Criteria>());
                    }
                }
            }
        }
    }

    private void CreateNewCriteriaPrefab()
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedCriteria.gameObject);
        }
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Criteria>();
        //PrefabUtility.CreatePrefab("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/tmpCriteriaName.prefab", gameObject);
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/tmpCriteriaName.prefab");
        GameObject tmpNewGameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/tmpCriteriaName.prefab", typeof(GameObject));
        tmpNewGameObject.name = "tmpCriteriaName";
        tmpNewGameObject.GetComponent<Criteria>().criteriaName = tmpNewGameObject.name;
        SelectCriteriaPrefab(tmpNewGameObject.GetComponent<Criteria>());
        DestroyImmediate(gameObject);
        ClearFocus();
        UpdateCriteriaList();
    }

    public void DeleteCriteriaPrefab(Criteria c)
    {
        allCriterias.Remove(c);
        if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + c.name + ".prefab", typeof(GameObject)))
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + c.name + ".prefab");
        }
        else
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Criterias/" + c.name);
        }
        ClearFocus();
    }

    private void SelectCriteriaPrefab(Criteria c)
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedCriteria.gameObject);
        }
        ClearFocus();
        selectedCriteria = c;
        tmpQuestName = selectedCriteria.criteriaName;
    }

    private void R_SelectCriteria(ReorderableList criteraList)
    {
        SelectCriteriaPrefab(allCriterias[criteraList.index]);
    }

    private void CreateNewCriteriaCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Criteria c = q.gameObject.AddComponent<Criteria>();
        q.criterias.Add(c);
    }

    private void AddNewCriteriaCallBack(object obj)
    {
        Criteria c = (Criteria)obj;
        Criteria tmpCriteria = selectedQuest.gameObject.AddComponent<Criteria>();
        selectedQuest.criterias.Add(tmpCriteria);
        CopyAll(c, tmpCriteria);
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
        Criteria c = (Criteria)obj;
        Criteria tmpCriteria = selectedQuest.gameObject.AddComponent<Criteria>();
        selectedQuest.optionalCriterias.Add(tmpCriteria);
        CopyAll(c, tmpCriteria);
        tmpCriteria.editorType = editorCriteriaType.Optional;
    }

    public void ConvertToCustomCriteria(Criteria c)
    {
        //Instantiate a new gameObject
        tmpGameObject = new GameObject();

        //Name it the same as the character name
        tmpGameObject.name = c.criteriaName;

        //Add the quest script component.
        tmpGameObject.AddComponent<Criteria>();

        TextAsset templateTextFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Templates/CriteriaTemplate.txt", typeof(TextAsset));
        string contents = "";

        if (templateTextFile != null)
        {
            contents = templateTextFile.text;
            contents = contents.Replace("CRITERIACLASS_NAME_HERE", tmpGameObject.name.Replace(" ", ""));
            contents = contents.Replace("CRITERIA_NAME_HERE", tmpGameObject.name);
            contents = contents.Replace("TYPE_HERE", c.type.ToString());
            contents = contents.Replace("CRITERIA_AMOUNT_HERE", c.amount.ToString());
        }
        else
        {
            Debug.LogError("Can't find the CriteriaTemplate.txt!, Is it the path you wrote at templateTextFile wrong?");
        }

        //Debug.Log(Application.dataPath);

        using (StreamWriter sw = new StreamWriter(string.Format(Application.dataPath + "/{0}.cs",
            new object[] { tmpGameObject.name.Replace(" ", "") })))
        {
            sw.Write(contents);
        }

        AssetDatabase.Refresh();

        needToAttach = true;

        attachType = TypeToAttach.Criteria;

        tmpCriteria = Instantiate(c);

        DestroyImmediate(c, true);
        ClearFocus();
    }

    private void DeleteCriteria(Criteria c)
    {
        selectedQuest.criterias.Remove(c);
        if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + selectedQuest.name + "/" + c.criteriaName + ".cs", typeof(MonoScript)))
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + selectedQuest.name + "/" + c.criteriaName + ".cs");
        }
        DestroyImmediate(c, true);
        ClearFocus();
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
            // GUI.color = new Color(0.75f, 0.10f, 0);
            GUI.color = new Color32(35, 120, 161, 255);
            float height = rect.height - 4;
            if (c.ShowSpawns)
            {
                foreach (SpawnZone zone in c.spawnZones)
                {
                    if (zone != null)
                    {
                        //height += EditorGUIUtility.singleLineHeight * 4 + 5;
                    }
                    else
                    {
                        c.spawnZones.Remove(zone);
                        DestroyImmediate(zone, true);
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
            if (c.ShowSettings)
            {
                //height += EditorGUIUtility.singleLineHeight * 2;
            }

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
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, EditorGUIUtility.singleLineHeight * 1.25f, EditorGUIUtility.singleLineHeight), "-"))
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
            GUI.skin = thisGUISkin;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 20, rect.y, EditorGUIUtility.singleLineHeight * 1.25f, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this criteria"));

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
            //c.ShowSpawns = EditorGUI.Foldout(
            //    new Rect(rect.x, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
            //    c.ShowSpawns, "Show Spawns", true);
            //if (CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
            //{
            //    c.ShowRewards = EditorGUI.Foldout(
            //        new Rect(rect.x + rect.width / 3, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
            //        c.ShowRewards, "Show Rewards", true);
            //}
            c.ShowSettings = EditorGUI.Foldout(
                new Rect(rect.x + rect.width / 3 * 2, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, rect.width / 3 - 30, EditorGUIUtility.singleLineHeight),
                c.ShowSettings, "Show Settings", true);
            EditorGUI.indentLevel -= 1;
            float zoneHeight = rect.y + 3 + EditorGUIUtility.singleLineHeight * 3; ;
            switch (c.toolbarInt)
            {
                case 1: //Spawns
                    if (c.ShowSpawns)
                    {
                        #region Spawns

                        //if (GUI.Button(new Rect(rect.x + rect.width / 3 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, 30, EditorGUIUtility.singleLineHeight), new GUIContent("+", "Click here to add a spawn to this criteria")))
                        //{
                        //    if (Resources.FindObjectsOfTypeAll(typeof(CustomQuestEditor)) != null)
                        //    {
                        //        if (CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab)
                        //        {
                        //            GameObject go = Instantiate(CustomQuestSettings.SettingsHolder.criteriaSpawnPrefab, c.gameObject.transform);
                        //            if (go.GetComponent<SpawnZone>())
                        //            {
                        //                c.spawnZones.Add(go.GetComponent<SpawnZone>());
                        //                go.GetComponent<SpawnZone>().Criteria = c;
                        //                go.GetComponent<SpawnZone>().spawnAreaObject = go;
                        //                go.GetComponent<SpawnZone>().SpawnName = "SpawnZone";
                        //            }
                        //            else
                        //            {
                        //                Debug.LogWarning("No spawnZone script was found in the spawnZonePrefab, please assign a prefab with the spawnZone scrip attached");
                        //            }
                        //        }
                        //        else
                        //        {
                        //            Debug.LogWarning("spawnZonePrefab is null in customQuestEditor, please assign a prefab with the spawnZone script attached. If you have one assisgned, please close and open the editor window");
                        //        }
                        //    }
                        //}

                        //if (c.ShowSpawns)
                        //{
                        //    foreach (SpawnZone zone in c.spawnZones)
                        //    {
                        //        if (zone == null)
                        //        {
                        //            c.spawnZones.Remove(zone);
                        //            break;
                        //        }
                        //        EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 4 + 5), "", MessageType.None);
                        //        zone.SpawnName = EditorGUI.TextField(
                        //            new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight),
                        //            zone.SpawnName);
                        //        EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 3, rect.width - 80, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this spawnZone"));
                        //        zone.Spawn = EditorGUI.Toggle(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), zone.Spawn);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width - 70, zoneHeight + 3, 15, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click here to enable / disable spawning."));
                        //        if (GUI.Button(new Rect(rect.x + rect.width - 30, zoneHeight + 3, 30, EditorGUIUtility.singleLineHeight), new GUIContent("-", "Click here to delete this zone")))
                        //        {
                        //            c.spawnZones.Remove(zone);
                        //            DestroyImmediate(zone.gameObject, true);
                        //            break;
                        //        }
                        //        zoneHeight += 5;
                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //            "      Object / Radius");
                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to spawn around, and the radius in which to spawn"));
                        //        zone.spawnAreaObject = (GameObject)EditorGUI.ObjectField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //            zone.spawnAreaObject, typeof(GameObject), true);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to spawn around"));
                        //        zone.spawnRadius = EditorGUI.FloatField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //            zone.spawnRadius);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The objects radius"));

                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //            "      Amount / Rate");
                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of objects to spawn each time, and how often to spawn that amount"));
                        //        zone.spawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //           zone.spawnAmount);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of object to spawn each time"));
                        //        zone.spawnRate = EditorGUI.FloatField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //          zone.spawnRate);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 2, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "How often to spawn objects, in seconds."));

                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //          "      Initial / Max");
                        //        EditorGUI.LabelField(new Rect(rect.x, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The initial amount of objects to spawn, when this criteria starts spawning, and the max amount of objects there can be spawned at once"));
                        //        zone.initialSpawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //          zone.initialSpawnAmount);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The initial amount of objects to spawn, when this criteria starts spawning."));
                        //        zone.maxSpawnAmount = EditorGUI.IntField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //          zone.maxSpawnAmount);
                        //        EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + EditorGUIUtility.singleLineHeight * 3, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The max amount of objects there can be spawned at once"));
                        //        zoneHeight += EditorGUIUtility.singleLineHeight * 4;
                        //    }
                        //}

                        #endregion Spawns
                    }
                    break;

                case 2: //Rewards
                    if (c.ShowRewards && CustomQuestSettings.SettingsHolder.criteriaSpecificRewards == true)
                    {
                        #region Rewards

                        //if (GUI.Button(new Rect(rect.x + rect.width / 3 * 2 - 30, rect.y + 3 + EditorGUIUtility.singleLineHeight * 2, 30, EditorGUIUtility.singleLineHeight), new GUIContent("+", "Click here to add a spawn to this criteria")))
                        //{
                        //    Reward r = c.gameObject.AddComponent<Reward>();
                        //    r.editoreRewardType = editorRewardType.Criteria;
                        //    c.rewards.Add(r);
                        //}
                        //foreach (Reward r in c.rewards)
                        //{
                        //    GUI.color = new Color(0, 0.75f, 0.15f);
                        //    EditorGUI.HelpBox(new Rect(rect.x, zoneHeight, rect.width + 5, EditorGUIUtility.singleLineHeight * 2 + 13), "", MessageType.None);
                        //    GUI.color = Color.white;
                        //    r.rewardName = EditorGUI.TextField(
                        //        new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight),
                        //         r.rewardName);
                        //    EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 5, rect.width - 35, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
                        //    if (GUI.Button(new Rect(rect.x + rect.width - 30, zoneHeight + 5, 30, EditorGUIUtility.singleLineHeight),
                        //        "-")) { c.rewards.Remove(r); DestroyImmediate(r, true); break; }
                        //    EditorGUI.LabelField(new Rect(rect.x + rect.width - 30, zoneHeight + 5, 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this reward"));
                        //    r.type = (rewardType)EditorGUI.EnumPopup(
                        //        new Rect(rect.x + 5, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3 - 5, EditorGUIUtility.singleLineHeight),
                        //        r.type);
                        //    EditorGUI.LabelField(new Rect(rect.x + 5, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3 - 5, EditorGUIUtility.singleLineHeight), new GUIContent("", "Change the reward type"));
                        //    r.amount = EditorGUI.IntField(
                        //       new Rect(rect.x + rect.width / 3, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //       r.amount);
                        //    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The amount of the reward to give (100 gold, 1 sword...)"));
                        //    r.rewardObject = (GameObject)EditorGUI.ObjectField(
                        //         new Rect(rect.x + rect.width / 3 * 2, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight),
                        //        r.rewardObject, typeof(GameObject), true);
                        //    EditorGUI.LabelField(new Rect(rect.x + rect.width / 3 * 2, zoneHeight + 5 + 3 + EditorGUIUtility.singleLineHeight, rect.width / 3, EditorGUIUtility.singleLineHeight), new GUIContent("", "The object to reward, if any"));
                        //    zoneHeight += EditorGUIUtility.singleLineHeight * 2 + 13;
                        //}

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
            }
            else
            {
                criteriaList.Remove(criteriaList[index]);
                return 0;
            }
            return height;
        };

        return R_list;
    }

    /*** Reward Methods ***/

    private void UpdateRewardList()
    {
        string[] guids = AssetDatabase.FindAssets("t:GameObject", new string[] { "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards" });
        foreach (string guid in guids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            GameObject newReward = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            if (newReward != null)
            {
                if (newReward.GetComponent<Reward>())
                {
                    if (!allRewards.Contains(newReward.GetComponent<Reward>()))
                    {
                        allRewards.Add(newReward.GetComponent<Reward>());
                    }
                }
            }
        }
    }

    private void CreateNewRewardPrefab()
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedReward.gameObject);
        }
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<Reward>();
        //PrefabUtility.CreatePrefab("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/tmpRewardName.prefab", gameObject);
        PrefabUtility.SaveAsPrefabAsset(gameObject, "Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/tmpRewardName.prefab");
        GameObject tmpNewGameObject = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/tmpRewardName.prefab", typeof(GameObject));
        tmpNewGameObject.name = "tmpRewardName";
        tmpNewGameObject.GetComponent<Reward>().rewardName = tmpNewGameObject.name;
        SelectRewardPrefab(tmpNewGameObject.GetComponent<Reward>());
        DestroyImmediate(gameObject);
        ClearFocus();
        UpdateRewardList();
    }

    public void DeleteRewardPrefab(Reward r)
    {
        allRewards.Remove(r);
        if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + r.name + ".prefab", typeof(GameObject)))
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + r.name + ".prefab");
        }
        else
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Rewards/" + r.name);
        }
        ClearFocus();
    }

    private void SelectRewardPrefab(Reward r)
    {
        if (updateUI == true)
        {
            RenamePrefab(selectedReward.gameObject);
        }
        ClearFocus();
        selectedReward = r;
        tmpQuestName = selectedReward.rewardName;
    }

    private void R_SelectReward(ReorderableList rewardList)
    {
        SelectRewardPrefab(allRewards[rewardList.index]);
    }

    private void CreateNewRewardCallBack(object obj)
    {
        Quest q = (Quest)obj;
        Reward r = q.gameObject.AddComponent<Reward>();
        q.rewards.Add(r);
    }

    private void AddNewRewardCallBack(object obj)
    {
        Reward r = (Reward)obj;
        Reward tmpReward = selectedQuest.gameObject.AddComponent<Reward>();
        selectedQuest.rewards.Add(tmpReward);
        CopyAll(r, tmpReward);
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
        Reward r = (Reward)obj;
        Reward tmpReward = selectedQuest.gameObject.AddComponent<Reward>();
        selectedQuest.optionalRewards.Add(tmpReward);
        CopyAll(r, tmpReward);
        tmpReward.editoreRewardType = editorRewardType.Optional;
    }

    private void CreateNewReward(Quest q)
    {
        Reward r = q.gameObject.AddComponent<Reward>();
        q.rewards.Add(r);
    }

    public void ConvertToCustomReward(Reward r, Quest q)
    {
        //Instantiate a new gameObject
        tmpGameObject = new GameObject();

        //Name it the same as the character name
        tmpGameObject.name = r.rewardName;

        //Add the quest script component.
        tmpGameObject.AddComponent<Reward>();

        TextAsset templateTextFile = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Templates/RewardTemplate.txt", typeof(TextAsset));
        string contents = "";

        if (templateTextFile != null)
        {
            contents = templateTextFile.text;
            contents = contents.Replace("REWARDCLASS_NAME_HERE", tmpGameObject.name.Replace(" ", ""));
            contents = contents.Replace("REWARD_NAME_HERE", tmpGameObject.name);
            contents = contents.Replace("TYPE_HERE", r.type.ToString());
            contents = contents.Replace("AMOUNT_HERE", r.amount.ToString());
        }
        else
        {
            Debug.LogError("Can't find the RewardTemplate.txt!, Is it the path you wrote at templateTextFile wrong?");
        }

        // Debug.Log(Application.dataPath);

        using (StreamWriter sw = new StreamWriter(string.Format(Application.dataPath + "/{0}.cs",
            new object[] { tmpGameObject.name.Replace(" ", "") })))
        {
            sw.Write(contents);
        }

        AssetDatabase.Refresh();

        needToAttach = true;

        attachType = TypeToAttach.Reward;

        tmpQuest = q;

        tmpReward = Instantiate(r);

        q.rewards.Remove(r);
        DestroyImmediate(r, true);
        ClearFocus();
    }

    private void DeleteReward(Reward r)
    {
        selectedQuest.rewards.Remove(r);
        if (AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + selectedQuest.name + "/" + r.rewardName + ".cs", typeof(MonoScript)))
        {
            AssetDatabase.DeleteAsset("Assets/From Asset Store/CustomQuest/Assets/Prefabs/Quests/" + selectedQuest.name + "/" + r.rewardName + ".cs");
        }
        DestroyImmediate(r, true);
        ClearFocus();
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
            //GUI.color = new Color(0, 0.75f, 0.15f);
            GUI.color = new Color32(35, 120, 161, 255);
            EditorGUI.HelpBox(new Rect(rect.x - 20, rect.y - 3, rect.width + 25, rect.height - 5), "", MessageType.None);
            GUI.color = Color.white;
            rect.y += 2;
            r.rewardName = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight),
                 r.rewardName);
            EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight), new GUIContent("", "The name of this reward"));
            GUI.skin = null;
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, EditorGUIUtility.singleLineHeight * 1.25f, EditorGUIUtility.singleLineHeight), "-")) { rewardsList.Remove(r); DestroyImmediate(r, true); }
            GUI.skin = thisGUISkin;
            EditorGUI.LabelField(new Rect(rect.x + rect.width - 20, rect.y, EditorGUIUtility.singleLineHeight * 1.25f, EditorGUIUtility.singleLineHeight), new GUIContent("", "Click to delete this reward"));
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
}