using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

namespace CustomQuest
{
    /// <summary>
    /// A static class, used for controlling the different settings in the custom quest extension.
    /// Also used for this purpose, is the SettingsHolder
    /// </summary>
    public static class CustomQuestSettings
    {
        private static List<Quest> editorQuests = new List<Quest>();

        /// <summary>
        /// A list controlling the order of the quests in the editor
        /// </summary>
        public static List<Quest> EditorQuests { get { return editorQuests; } set { editorQuests = value; SettingsHolder.prefabQuests = editorQuests; } }

        private static List<Criteria> editorCriterias = new List<Criteria>();

        /// <summary>
        /// A list controlling the order of the criterias in the editor
        /// </summary>
        public static List<Criteria> EditorCriteras { get { return editorCriterias; } set { editorCriterias = value; SettingsHolder.prefabCriteria = editorCriterias; } }

        private static List<Reward> editorRewards = new List<Reward>();

        /// <summary>
        /// A list controlling the order of the rewards in the editor
        /// </summary>
        public static List<Reward> EditorRewards { get { return editorRewards; } set { editorRewards = value; SettingsHolder.prefabReward = editorRewards; } }

        private static List<QuestNode> questNodes = new List<QuestNode>();

        /// <summary>
        /// A list holding the questNotes for "Quests in Scene", their position and connections
        /// </summary>
        public static List<QuestNode> QuestNodes { get { return questNodes; } set { questNodes = value; } }

        private static bool showQuestName;

        /// <summary>
        /// A bool to either show or hide the quest name in the questUI
        /// </summary>
        public static bool ShowQuestName
        {
            get { return showQuestName; }
            set
            {
                showQuestName = value;
                SettingsHolder.showQuestName = showQuestName;
            }
        }

        private static bool showDescription;

        /// <summary>
        /// A bool to either show or hide the description of the quest in the questUI
        /// </summary>
        public static bool ShowDescription
        {
            get { return showDescription; }
            set
            {
                showDescription = value;
                SettingsHolder.showDescription = showDescription;
            }
        }

        private static bool showCriterias;

        /// <summary>
        /// A bool to either show or hide the criterias of the quest in the questUI
        /// </summary>
        public static bool ShowCriterias
        {
            get { return showCriterias; }
            set
            {
                showCriterias = value;
                SettingsHolder.showCriterias = showCriterias;
            }
        }

        private static bool showRewards;

        /// <summary>
        /// A bool to either show or hide the rewards of the quest in the questUI
        /// </summary>
        public static bool ShowRewards
        {
            get { return showRewards; }
            set
            {
                showRewards = value;
                SettingsHolder.showRewards = showRewards;
            }
        }

        private static SettingsHolder settingsHolder;

        /// <summary>
        /// The holder for all the settings, including nodes, edges and connections.
        /// </summary>
        public static SettingsHolder SettingsHolder
        {
            get
            {
                if (settingsHolder == null)
                {
#if UNITY_EDITOR
                    settingsHolder = (SettingsHolder)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/QuestSettings.asset", typeof(SettingsHolder));
#endif
                    // settingsHolder = SettingsHolder.Instance;
                    if (settingsHolder == null)
                    {
                        CreateHolder();
                    }
                }
                return settingsHolder;
            }
            set
            {
                if (settingsHolder == null)
                {
#if UNITY_EDITOR

                    settingsHolder = (SettingsHolder)AssetDatabase.LoadAssetAtPath("Assets/From Asset Store/CustomQuest/Assets/Prefabs/QuestSettings.asset", typeof(SettingsHolder));
#endif
                    // settingsHolder = SettingsHolder.Instance;
                    if (settingsHolder == null)
                    {
                        CreateHolder();
                    }
                }
                settingsHolder = value;
            }
        }

        private static QuestHandler questHandler;

        /// <summary>
        /// The quest handler
        /// </summary>
        public static QuestHandler QuestHandler { get { if (questHandler == null) { questHandler = GameObject.FindObjectsOfType<QuestHandler>()[0]; } return questHandler; } set { questHandler = value; } }

        private static GUISkin randomDragonGUISkin;

        /// <summary>
        /// The gui skin used for the editors - Recommended to be the RandomDragonGUISkin
        /// </summary>
        public static GUISkin RandomDragonGUISkin { get { return randomDragonGUISkin; } set { randomDragonGUISkin = value; SettingsHolder.randomDragonGUISkin = randomDragonGUISkin; } }

        private static bool criteriaSpecificRewards;

        /// <summary>
        /// A bool controlling if the criteria specific rewards are enabled or not
        /// </summary>
        public static bool CriteriaSpecificRewards { get { return criteriaSpecificRewards; } set { criteriaSpecificRewards = value; SettingsHolder.criteriaSpecificRewards = criteriaSpecificRewards; } }

        private static bool optionalCriteriaSpecificRewards;

        /// <summary>
        /// A bool controlling if the optional criteria specific rewards are enabled or not
        /// </summary>
        public static bool OptionalCriteriaSpecificRewards { get { return optionalCriteriaSpecificRewards; } set { optionalCriteriaSpecificRewards = value; SettingsHolder.optionalCriteriaSpecificRewards = optionalCriteriaSpecificRewards; } }

        private static bool optional;

        /// <summary>
        /// A bool controlling whether to hide or show the optional criterias in quests
        /// </summary>
        public static bool HideOptional { get { return optional; } set { optional = value; SettingsHolder.optional = optional; } }

        /*** METHODS ***/

        /// <summary>
        /// The start method for the custom quest settings
        /// </summary>
        public static void Start()
        {
            editorQuests = SettingsHolder.prefabQuests;
            editorCriterias = SettingsHolder.prefabCriteria;
            editorRewards = SettingsHolder.prefabReward;
            questNodes = SettingsHolder.questNodes;
            showQuestName = SettingsHolder.showQuestName;
            showDescription = SettingsHolder.showDescription;
            showCriterias = SettingsHolder.showCriterias;
            showRewards = SettingsHolder.showRewards;
#if UNITY_EDITOR

            randomDragonGUISkin = SettingsHolder.randomDragonGUISkin;
            if (SettingsHolder.randomDragonGUISkin == null)
            {
                string[] results = AssetDatabase.FindAssets("RandomDragonGUISkin");
                foreach (string guid in results)
                {
                    RandomDragonGUISkin = (GUISkin)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GUISkin)); //TODO: Give settings a toggle for normal skin? (too much work)
                }
            }
#endif

            criteriaSpecificRewards = SettingsHolder.criteriaSpecificRewards;
            optionalCriteriaSpecificRewards = SettingsHolder.optionalCriteriaSpecificRewards;
            optional = SettingsHolder.optional;
        }

        /// <summary>
        /// Creates a settings holder
        /// </summary>
        private static void CreateHolder()
        {
            settingsHolder = ScriptableObject.CreateInstance<SettingsHolder>();
#if UNITY_EDITOR

            AssetDatabase.CreateAsset(settingsHolder, "Assets/From Asset Store/CustomQuest/Assets/Prefabs/QuestSettings.asset");
            //settingsHolder.hideFlags = HideFlags.HideInHierarchy; //TOOD: Might use in final product, to hide where the data is
            AssetDatabase.SaveAssets();
#endif
        }
    }
}