using CustomQuest;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

/// <summary>
/// Used for displaying a UI quest list. Is not a list for holding quests.
/// </summary>
public class QuestUI : MonoBehaviour
{
    #region Field

    /// <summary>
    /// The current total height of the questlist
    /// </summary>
    [SerializeField]
    private float totalHeight;

    /// <summary>
    /// A holder for the questHandler
    /// </summary>
    private QuestHandler questHandler;

    /// <summary>
    /// A list of quests in the questList
    /// </summary>
    public List<Quest> canvasQuests = new List<Quest>();

    /// <summary>
    /// A list of the quest uis currently used for showing the quests
    /// </summary>
    public List<QuestUILogic> questUis = new List<QuestUILogic>();

    /// <summary>
    /// The questHolder, the UI prefabs are initialized under this gameobject
    /// </summary>
    public GameObject questHolder;

    /// <summary>
    /// The template for quests UI
    /// </summary>
    public GameObject questTemplate;

    /// <summary>
    /// The template for criterias UI
    /// </summary>
    public GameObject criteriaTemplate;

    /// <summary>
    /// The template for reward UI
    /// </summary>
    public GameObject rewardTemplate;

    /// <summary>
    /// The template for the 'optional' ui element
    /// </summary>
    public GameObject optionalTemplate;

    /// <summary>
    /// The messages prefab, used for instantiating new on screen messages
    /// </summary>
    public GameObject messagePrefab;

    /// <summary>
    /// The holder for messages, determines where the messages are added
    /// </summary>
    public OnScreenMsgHandler messageHolder;

    /// <summary>
    /// The ui text element, showing the selected players current resource amount
    /// </summary>
    public Text resourceAmount;

    /// <summary>
    /// The ui text element, showing the selected players current list of items
    /// </summary>
    public Text itemList;

    /// <summary>
    /// The prefab of a compassArrow
    /// </summary>
    public GameObject compassArrow;

    /// <summary>
    /// A list of current aktive compassArrows
    /// </summary>
    public List<GameObject> compassArrows;

    /// <summary>
    /// The gameobject holding the Quest Compass script
    /// </summary>
    public GameObject compass;

    /// <summary>
    /// Image used for displaying icons on the minimap
    /// </summary>
    private Image worldIcon;

    /// <summary>
    /// Prefab for the quest pop up, used by the quest giver
    /// </summary>
    public QuestPopUp questPopUpPrefab;

    /// <summary>
    /// A dictionary of the active questPopUps by player
    /// </summary>
    public Dictionary<CQPlayerObject, List<Quest>> activeQuestPopUpQuests = new Dictionary<CQPlayerObject, List<Quest>>();

    /// <summary>
    /// The current questWheel part selected
    /// </summary>
    public int questWheelSelection;

    /// <summary>
    /// Used to ditermin if the quest wheel is visible and active, or not
    /// </summary>
    public bool questWheelAktive = false;

    /// <summary>
    /// The quest wheel to toggle and use
    /// </summary>
    public GameObject questWheel;

    /// <summary>
    /// A holder which all the UI parts of the quest wheel is under.
    /// </summary>
    public GameObject questWheelActions;

    /// <summary>
    /// To be assigned to the text in the middle of the quest wheel. Will change depending on which quest is hovered
    /// </summary>
    public Text middleText;

    /// <summary>
    /// Contains all the UIPolygon backgrounds for the quest wheel circle
    /// </summary>
    public List<UIPolygon> questWheelBackgrounds = new List<UIPolygon>();

    /// <summary>
    /// Contains all the images for the quest wheel circle
    /// </summary>
    public List<Image> questWheelImages = new List<Image>();

    /// <summary>
    /// Contains the color of a not highlighted item for the quest wheel circle
    /// </summary>
    private Color notHighlightColor = new Color(1, 1, 1, 0.7f);

    /// <summary>
    /// Contrains the color of a highlighted item for the quest wheel circle
    /// </summary>
    private Color highlightColor = new Color(1, 1, 1, 0.9f);

    /// <summary>
    /// Used to set, disable and enable glow on UI elements
    /// </summary>
    private Image worldIconGlow;

    /// <summary>
    /// The text for displaying the quest name
    /// </summary>
    public Text questNameText;

    /// <summary>
    /// The text for displaying the desciption name
    /// </summary>
    public Text descriptionText;

    /// <summary>
    /// The text for displaying the criterias
    /// </summary>
    public Text criteriaText;

    /// <summary>
    /// The text for displaying the rewards
    /// </summary>
    public Text rewardsText;

    /// <summary>
    /// A string used to hold string content, later used to set Text.
    /// </summary>
    private string contentHolder = "";

    /// <summary>
    /// A reference to the graphic raycaster of this scene. Used to find the graphic raycaster component.
    /// </summary>
    public GraphicRaycaster gr;

    #endregion Field

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    private void OnEnable()
    {
        QuestHandler.StartListening("QuestCompleted", QuestCompleted);
        QuestHandler.StartListening("CriterionProgress", CriteraProgress);
        QuestHandler.StartListening("GiveReward", GiveReward);
        QuestHandler.StartListening("ResetQuestInList", ResetQuestEvent);
        QuestHandler.StartListening("UpdateQuestTracker", UpdateQuestTracker);
        QuestHandler.StartListening("StartQuestPopUp", StartQuestPopUp);
    }

    /// <summary>
    /// This function is called when the behaviour becomes disabled() or inactive.
    /// </summary>
    private void OnDisable()
    {
        QuestHandler.StopListening("QuestCompleted", QuestCompleted);
        QuestHandler.StopListening("CriterionProgress", CriteraProgress);
        QuestHandler.StopListening("GiveReward", GiveReward);
        QuestHandler.StopListening("ResetQuestInList", ResetQuestEvent);
        QuestHandler.StopListening("UpdateQuestTracker", UpdateQuestTracker);
    }

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        questHandler = QuestHandler.Instance;

        if (GetComponent<Canvas>() == null)
        {
            Canvas c = gameObject.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        if (GetComponent<StandaloneInputModule>() == null)
        {
            gameObject.AddComponent<StandaloneInputModule>();
        }

        if (GetComponent<EventSystem>() == null)
        {
            gameObject.AddComponent<EventSystem>();
        }

        if (messagePrefab == null)
        {
            messagePrefab = (GameObject)Resources.Load("OnScreenMsgPrefab", typeof(GameObject));
        }

        if (criteriaTemplate == null)
        {
            criteriaTemplate = (GameObject)Resources.Load("CriteriaUITemplate", typeof(GameObject));
        }

        if (rewardTemplate == null)
        {
            rewardTemplate = (GameObject)Resources.Load("RewardUITemplate", typeof(GameObject));
        }

        if (questTemplate == null)
        {
            questTemplate = (GameObject)Resources.Load("QuestUITemplate", typeof(GameObject));
        }

        if (optionalTemplate == null)
        {
            optionalTemplate = (GameObject)Resources.Load("OptionalUITemplate", typeof(GameObject));
        }

        if (compass == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.name == "Compass")
                {
                    compass = rt.gameObject;
                    break;
                }
            }
        }

        if (questHolder == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.name == "QuestHolder")
                {
                    questHolder = rt.gameObject;
                    break;
                }
            }
        }
        if (questHolder != null)
        {
            totalHeight = -questHolder.GetComponent<RectTransform>().rect.height;
        }

        if (resourceAmount == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.name == "PlayerResourcesAmount")
                {
                    resourceAmount = rt.gameObject.GetComponent<Text>();
                    break;
                }
            }
        }

        if (itemList == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.name == "Items")
                {
                    itemList = rt.gameObject.GetComponent<Text>();
                    break;
                }
            }
        }

        if (gr == null)
        {
            gr = GetComponent<GraphicRaycaster>();
            if (gr == null)
            {
                gr = gameObject.AddComponent<GraphicRaycaster>();
            }
        }

        if (questWheel == null)
        {
            foreach (RectTransform rt in GetComponentsInChildren<RectTransform>())
            {
                if (rt.name == "QuestWheel")
                {
                    questWheel = rt.gameObject;
                    break;
                }
            }
        }

        if (questWheel != null)
        {
            if (questWheelActions == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Actions")
                    {
                        questWheelActions = rt.gameObject;
                        break;
                    }
                }
            }
            if (middleText == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "InfoText")
                    {
                        middleText = rt.GetComponent<Text>();
                        break;
                    }
                }
            }
            if (questNameText == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Quest Name")
                    {
                        questNameText = rt.GetComponent<Text>();
                    }
                }
            }
            if (descriptionText == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Description")
                    {
                        descriptionText = rt.GetComponent<Text>();
                        break;
                    }
                }
            }
            if (criteriaText == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Criterias")
                    {
                        criteriaText = rt.GetComponent<Text>();
                        break;
                    }
                }
            }
            if (rewardsText == null)
            {
                foreach (RectTransform rt in questWheel.GetComponentsInChildren<RectTransform>())
                {
                    if (rt.name == "Rewards")
                    {
                        rewardsText = rt.GetComponent<Text>();
                        break;
                    }
                }
            }
        }
        if (questWheelActions != null)
        {
            /** Adding Hud Wheel elements to list for easy acces **/
            foreach (UIPolygon p in questWheelActions.GetComponentsInChildren<UIPolygon>())
            {
                questWheelBackgrounds.Add(p);
            }
            foreach (Image i in questWheelActions.GetComponentsInChildren<Image>())
            {
                questWheelImages.Add(i);
            }
        }
        questWheelAktive = false;
        if (questWheel != null)
        {
            questWheel.SetActive(false);
        }
        UpdateSelectionCircle(0);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Q))
        {
            if (questWheel != null)
            {
                if (questWheelAktive)
                {
                    questWheelAktive = false;
                    questWheel.SetActive(false);
                }
                else
                {
                    questWheelAktive = true;
                    questWheel.SetActive(true);
                    UpdateSelectionCircle(0);
                    int counter = 0;
                    foreach (Image i in questWheelImages)
                    {
                        try
                        {
                            i.sprite = QuestHandler.Instance.availableQuests[QuestHandler.Instance.SelectedPlayer][counter].questIcon;
                            i.color = new Color(255, 255, 255, 255);
                        }
                        catch
                        {
                            i.sprite = null;
                            i.color = new Color(255, 255, 255, 0);
                        }
                        counter++;
                    }
                }
            }
        }

        List<RaycastResult> UiRayCastResults = RayCastUi();
        if (UiRayCastResults != null)
        {
            UIMouseHover(UiRayCastResults);
        }

        if (Input.GetMouseButtonUp(0))
        {//LeftClick
            List<GameObject> RayCastInGameResults = RayCastInGame();
            if (RayCastInGameResults != null)
            {
                MouseClick(RayCastInGameResults);
            }
        }

        CanvasUpdate();
    }

    /// <summary>
    /// Is run everytime a component is added, or reset button pressed. Used to set needed variables
    /// </summary>
    private void Reset()
    {
        if (questPopUpPrefab == null)
        {
            questPopUpPrefab = (QuestPopUp)Resources.Load("QuestPopUp", typeof(QuestPopUp));
        }

        if (compassArrow == null)
        {
            compassArrow = (GameObject)Resources.Load("CompassArrow", typeof(GameObject)); //TODO: prefab needs to be in resources folder, and forces it to be part of the build. Might not be wanted for user
        }
    }

    /// <summary>
    /// Updates the canvas UI logic
    /// </summary>
    private void CanvasUpdate()
    {
        if (QuestHandler.Instance.SelectedPlayer)
        {
            if (resourceAmount != null)
            {
                resourceAmount.text = QuestHandler.Instance.SelectedPlayer.GetComponent<CQExamplePlayer>().resources.ToString();
            }
            if (itemList != null)
            {
                itemList.text = "Items: \n";
                foreach (Item item in QuestHandler.Instance.SelectedPlayer.GetComponent<CQExamplePlayer>().items)
                {
                    if (item != null)
                    {
                        itemList.text += item.name.ToString() + "\n";
                    }
                    else
                    {
                        QuestHandler.Instance.SelectedPlayer.GetComponent<CQExamplePlayer>().items.Remove(item);
                        break;
                    }
                }
            }
        }

        foreach (QuestUILogic qUI in questUis) //Updates the questUis
        {
            if (qUI.quest != null) //Checks if the quest of the questUI is null
            { //Updates the text of the questUI to match the quest
                if (questHandler.availableQuests[questHandler.SelectedPlayer].Contains(qUI.quest))
                {
                    qUI.questName.text = qUI.quest.questName;
                    qUI.description.text = qUI.quest.description;
                    qUI.questIcon.sprite = qUI.quest.questIcon; //Updates the icon to match the quest
                    foreach (CriteriaUILogic cUI in qUI.criteriaUis) //Updates the criteriaUis of the questUIs
                    {
                        if (cUI == null || cUI.criteria == null) //Checks if the criteriaUi or the criteria of the criteriaUI is null
                        {//Deletes the criteriaUi
                            qUI.criteriaUis.Remove(cUI);
                            Destroy(cUI.gameObject);
                        }
                        if (cUI.amountDone.text == cUI.totalAmount.text) // if a criteria has been completed.
                        {
                            cUI.amountDone.gameObject.SetActive(false); // disable extra text and the  "/"
                            cUI.totalAmount.text = "Completed"; // change totalamount to say completed //  TODO:: test if this works with several quests.
                            cUI.totalAmount.color = Color.green; // change the colour of the word
                            RectTransform rt = cUI.totalAmount.GetComponent<RectTransform>();
                            rt.sizeDelta = new Vector2(100, 16); // make room for the word completed
                            rt.localPosition = new Vector3(rt.localPosition.x + 15, rt.localPosition.y, rt.localPosition.z); // move it over so "completed" looks a little better

                            cUI.criteriaName.text = cUI.criteria.criteriaName;
                            cUI.criteriaType.text = cUI.criteria.type.ToString();
                            cUI.completed = true; // criteria has been completed, yaay!
                        }
                        else if (cUI.completed != true)
                        {//Updates the text of the criteriaUIs to match the criteria
                            cUI.amountDone.text = cUI.criteria.playerProgression[questHandler.SelectedPlayer].ToString();
                            cUI.totalAmount.text = cUI.criteria.amount.ToString();
                            cUI.criteriaName.text = cUI.criteria.criteriaName;
                            cUI.criteriaType.text = cUI.criteria.type.ToString();
                        }
                    }

                    foreach (RewardUILogic rUI in qUI.rewardUis) //Updates the rewardUis of the rewardUIs
                    {
                        if (rUI == null || rUI.reward == null) //Checks if the rewardUI or the reward of the rewardUI is null
                        {//Deletes the rewardUI
                            qUI.rewardUis.Remove(rUI);
                            Destroy(rUI.gameObject);
                            break;
                        }
                        else
                        {//Updates the text of the rewardUis to match the reward
                            rUI.rewardAmount.text = rUI.reward.amount.ToString();
                            rUI.rewardName.text = rUI.reward.rewardName;
                            rUI.rewardType.text = rUI.reward.type.ToString();
                        }
                    }
                }
                else
                {
                    ResetQuest(qUI.quest);
                    break;
                }
            }
            else
            {
                ResetQuest(qUI.quest);
                break;
            }
        }

        // Adds the selected players quests to the canvas quests, and spawns the needed questUis
        foreach (Quest q in questHandler.availableQuests[questHandler.SelectedPlayer])
        {// Goes through every available quest, for the selectedPlayer
            if (!canvasQuests.Contains(q)) // Checks if canvasQuests constains the current quest
            {
                canvasQuests.Add(q); // Adds the quest to the canvas quest, so it does not get in here again
                float criteriaRewardHeight = 0;
                QuestUILogic qUI = null;
                if (questTemplate != null)
                {
                    GameObject tmpQuestHolder = Instantiate(questTemplate) as GameObject; // Instantiates the questTemplate
                    tmpQuestHolder.transform.SetParent(questHolder.transform); // Sets its parent to be the questHolder
                    tmpQuestHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, totalHeight); //Sets the position of the quest
                    qUI = tmpQuestHolder.GetComponent<QuestUILogic>(); //Gets the QuestUILogic script from the object
                    questUis.Add(qUI); //Adds the script to the script list
                    qUI.quest = q; //Adds the quest to the script
                    qUI.Start(); //Runs the start method of the script(Needed in the line below)
                    totalHeight -= qUI.rectTransform.rect.height; //Updates the totalHeight, with the height of the new quest (So, the next quest will be spawned below this one)

                    qUI.questName.text = qUI.quest.questName; //Updates the ui text name to match the script
                    qUI.description.text = qUI.quest.description; //Updates the description to match the script
                    qUI.questIcon.sprite = qUI.quest.questIcon; //Updates the icon to match the quest
                    criteriaRewardHeight = -qUI.GetComponent<RectTransform>().rect.height; //Adds the height of the questUI to criteriaHeight, so that the first criteria will be spawn below it
                    if (CustomQuestSettings.ShowQuestName == false) //Checks if the ShowQuestName setting is false
                    {
                        qUI.questName.enabled = false; //Deaktivates the component
                        qUI.questIcon.enabled = false; //Deaktivates the icon
                        totalHeight += qUI.questName.GetComponent<RectTransform>().rect.height; //Adds the height of the name, so next next ui object will be spawned correctly
                        criteriaRewardHeight += qUI.questName.GetComponent<RectTransform>().rect.height; //Adds the height of the name, so next next ui object will be spawned correctly
                    }

                    if (CustomQuestSettings.ShowDescription == false) //Checks if the ShowDescription setting is false
                    {
                        qUI.description.enabled = false;  //Deaktivates the component
                        totalHeight += qUI.description.GetComponent<RectTransform>().rect.height; //Adds the height of the Description, so next next ui object will be spawned correctly
                        criteriaRewardHeight += qUI.description.GetComponent<RectTransform>().rect.height; //Adds the height of the Description, so next next ui object will be spawned correctly
                    }

                    List<Criteria> allCriterias = new List<Criteria>(qUI.quest.activeCriterias[questHandler.SelectedPlayer]);
                    allCriterias.AddRange(qUI.quest.activeOptionalCriterias[questHandler.SelectedPlayer]);
                    foreach (Criteria c in qUI.quest.activeCriterias[questHandler.SelectedPlayer]) //Goes through every criteria in this quest
                    {
                        if (!qUI.criterias.Contains(c)) //Checks if the quest UI scripts criteria list contains this criteria
                        {
                            qUI.criterias.Add(c); //Adds the criteria the list, so it does not get in here again
                            if (criteriaTemplate != null)
                            {
                                GameObject tmpCriteriaHolder = Instantiate(criteriaTemplate) as GameObject; //Instatiates the criteriaTemplate
                                tmpCriteriaHolder.transform.SetParent(qUI.transform); //Sets its parent to be the Quest
                                tmpCriteriaHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, criteriaRewardHeight); //Sets the position of the quest

                                CriteriaUILogic cUI = tmpCriteriaHolder.GetComponent<CriteriaUILogic>(); //Gets the CriteriaUILogic Script from the object
                                qUI.criteriaUis.Add(cUI); //Adds the criteriaUI script to the quest, so it can be updated later
                                cUI.criteria = c; //Adds the criteria to the script
                                cUI.Start(); //Runs the start methods of the script
                                if (CustomQuestSettings.ShowCriterias == false) //Checks if the ShowCriterias setting is false
                                {
                                    cUI.gameObject.SetActive(false);  //Deaktivates the gameobject
                                }
                                else
                                {
                                    cUI.gameObject.SetActive(true);
                                    criteriaRewardHeight -= cUI.rectTransform.rect.height; //Updates the criteria, with the height of this criteriaUI, so the next will be under it
                                    totalHeight -= cUI.rectTransform.rect.height; //Updates the totalHeight, with the height of this criteriaUI, so the next quest will be under it
                                }
                            }
                        }
                    }
                    foreach (Reward r in qUI.quest.rewards) //Goes through every reward in this quest //TODO: Correct comments
                    {
                        if (!qUI.rewards.Contains(r)) //Checks if the quest UI scripts reward list contains this reward
                        {
                            qUI.rewards.Add(r); //Adds the reward the list, so it does not get in here again
                            if (rewardTemplate != null)
                            {
                                GameObject tmpRewardHolder = Instantiate(rewardTemplate) as GameObject; //Instatiates the rewardTemplate
                                tmpRewardHolder.transform.SetParent(qUI.transform); //Sets its parent to be the Quest
                                tmpRewardHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, criteriaRewardHeight); //Sets the position of the quest

                                RewardUILogic rUI = tmpRewardHolder.GetComponent<RewardUILogic>(); //Gets the RewardUILogic Script from the object
                                qUI.rewardUis.Add(rUI); //Adds the rewardUI script to the quest, so it can be updated later
                                rUI.reward = r; //Adds the reward to the script
                                rUI.Start(); //Runs the start methods of the script
                                if (CustomQuestSettings.ShowRewards == false) //Checks if the ShowRewards setting is false
                                {
                                    rUI.gameObject.SetActive(false);  //Deaktivates the gameobject
                                }
                                else
                                {
                                    rUI.gameObject.SetActive(true);
                                    criteriaRewardHeight -= rUI.rectTransform.rect.height; //Updates the reward, with the height of this rewardUI, so the next will be under it
                                    totalHeight -= rUI.rectTransform.rect.height; //Updates the totalHeight, with the height of this rewardUI, so the next quest will be under it
                                }
                            }
                        }
                    }

                    if (qUI.quest.activeOptionalCriterias[questHandler.SelectedPlayer].Count > 0 || qUI.quest.optionalRewards.Count > 0)
                    {
                        GameObject optionalHolder = Instantiate(optionalTemplate) as GameObject;
                        optionalHolder.transform.SetParent(qUI.transform);
                        optionalHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, criteriaRewardHeight);
                        criteriaRewardHeight -= 15; //Adding the height of the optional template
                        totalHeight -= 15;
                    }

                    foreach (Criteria c in qUI.quest.activeOptionalCriterias[questHandler.SelectedPlayer]) //Goes through every criteria in this quest
                    {
                        if (!qUI.criterias.Contains(c)) //Checks if the quest UI scripts criteria list contains this criteria
                        {
                            qUI.criterias.Add(c); //Adds the criteria the list, so it does not get in here again
                            GameObject tmpCriteriaHolder = Instantiate(criteriaTemplate) as GameObject; //Instatiates the criteriaTemplate
                            tmpCriteriaHolder.transform.SetParent(qUI.transform); //Sets its parent to be the Quest
                            tmpCriteriaHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, criteriaRewardHeight); //Sets the position of the quest

                            CriteriaUILogic cUI = tmpCriteriaHolder.GetComponent<CriteriaUILogic>(); //Gets the CriteriaUILogic Script from the object
                            qUI.criteriaUis.Add(cUI); //Adds the criteriaUI script to the quest, so it can be updated later
                            cUI.criteria = c; //Adds the criteria to the script
                            cUI.Start(); //Runs the start methods of the script
                            if (CustomQuestSettings.ShowCriterias == false) //Checks if the ShowCriterias setting is false
                            {
                                cUI.gameObject.SetActive(false);  //Deaktivates the gameobject
                            }
                            else
                            {
                                cUI.gameObject.SetActive(true);
                                criteriaRewardHeight -= cUI.rectTransform.rect.height; //Updates the criteria, with the height of this criteriaUI, so the next will be under it
                                totalHeight -= cUI.rectTransform.rect.height; //Updates the totalHeight, with the height of this criteriaUI, so the next quest will be under it
                            }
                        }
                    }

                    foreach (Reward r in qUI.quest.optionalRewards) //Goes through every reward in this quest //TODO: Correct comments
                    {
                        if (!qUI.rewards.Contains(r)) //Checks if the quest UI scripts reward list contains this reward
                        {
                            qUI.rewards.Add(r); //Adds the reward the list, so it does not get in here again
                            GameObject tmpRewardHolder = Instantiate(rewardTemplate) as GameObject; //Instatiates the rewardTemplate
                            tmpRewardHolder.transform.SetParent(qUI.transform); //Sets its parent to be the Quest
                            tmpRewardHolder.GetComponent<RectTransform>().localPosition = new Vector3(0, criteriaRewardHeight); //Sets the position of the quest

                            RewardUILogic rUI = tmpRewardHolder.GetComponent<RewardUILogic>(); //Gets the RewardUILogic Script from the object
                            qUI.rewardUis.Add(rUI); //Adds the rewardUI script to the quest, so it can be updated later
                            rUI.reward = r; //Adds the reward to the script
                            rUI.Start(); //Runs the start methods of the script
                            if (CustomQuestSettings.ShowRewards == false) //Checks if the ShowRewards setting is false
                            {
                                rUI.gameObject.SetActive(false);  //Deaktivates the gameobject
                            }
                            else
                            {
                                rUI.gameObject.SetActive(true);
                                criteriaRewardHeight -= rUI.rectTransform.rect.height; //Updates the reward, with the height of this rewardUI, so the next will be under it
                                totalHeight -= rUI.rectTransform.rect.height; //Updates the totalHeight, with the height of this rewardUI, so the next quest will be under it
                            }
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resets a quest, by deleting it from the UI list. And then update will add it again
    /// </summary>
    /// <param name="q">The quest to reset</param>
    public void ResetQuest(Quest q)
    {
        for (int i = 0; i < questUis.Count; i++)
        {
            QuestUILogic qUI = questUis[i];
            foreach (CriteriaUILogic cUI in qUI.criteriaUis)
            {
                totalHeight += cUI.rectTransform.rect.height;
            }
            foreach (RewardUILogic rUI in qUI.rewardUis)
            {
                totalHeight += rUI.rectTransform.rect.height;
            }
            if (qUI.quest.activeOptionalCriterias[questHandler.SelectedPlayer].Count > 0 || qUI.quest.optionalRewards.Count > 0)
            {
                totalHeight += 15;
            }

            totalHeight += qUI.rectTransform.rect.height;

            if (CustomQuestSettings.ShowQuestName == false) //Checks if the ShowQuestName setting is false
            {
                totalHeight -= qUI.questName.GetComponent<RectTransform>().rect.height; //removes the height of the name, so next next ui object will be spawned correctly
            }
            if (CustomQuestSettings.ShowDescription == false) //Checks if the ShowDescription setting is false
            {
                totalHeight -= qUI.description.GetComponent<RectTransform>().rect.height; //removes the height of the Description, so next next ui object will be spawned correctly
            }
            canvasQuests.Remove(qUI.quest);
            Destroy(qUI.gameObject);
        }
        questUis.Clear();
    }

    /// <summary>
    /// Adds an OnScreenMsg to be displayed for the player
    /// </summary>
    /// <param name="lifeTime">The lifetime of the msg</param>
    /// <param name="msg">The actualt msg</param>
    /// <param name="size">The size of the msg</param>
    /// <param name="color">The color of the text</param>
    public void AddOnSreenMsg(float lifeTime, string msg, int size, Color color)
    {
        if (messageHolder == null)
        {
            messageHolder = GetComponentInChildren<OnScreenMsgHandler>();
        }
        if (messageHolder != null)
        {
            messageHolder.AddMsg(lifeTime, msg, size, color);
        }
        //else
        //{
        //    Debug.LogWarning("Could not find a OnScreenMsgHandler, unable to add an OnScreenMsg");
        //}
    }

    /// <summary>
    /// Updates the quest tracker, pointing arrows on all the active quests
    /// </summary>
    public void UpdateQuestTracker()
    {
        foreach (GameObject ar in compassArrows)
        {
            Destroy(ar);
        }
        compassArrows.Clear();

        foreach (Quest q in QuestHandler.Instance.availableQuests[QuestHandler.Instance.SelectedPlayer])
        {
            if (q.unCompletedCriterias.ContainsKey(QuestHandler.Instance.SelectedPlayer) != true)
            {
                q.AddPlayerToCriterias();
            }
            if (q.unCompletedCriterias[QuestHandler.Instance.SelectedPlayer].Count <= 0)
            {
                if (q.handInObjects.Count > 0)
                {
                    if (q.handInObjects[0]) //TODO: Do arrow to nearest handINObject
                    {
                        foreach (HandInObject h in q.handInObjects)
                        {
                            if (compassArrow != null && compass != null)
                            {
                                var arrow = Instantiate(compassArrow, compassArrow.transform.localPosition, compassArrow.transform.rotation, compass.transform);
                                arrow.transform.localPosition = compassArrow.transform.localPosition;
                                arrow.GetComponent<QuestCompassArrow>().target = h.transform;
                                compassArrows.Add(arrow);
                            }
                        }
                        foreach (Criteria c in q.completedCriterias[QuestHandler.Instance.SelectedPlayer])
                        {
                            foreach (SpawnZone zone in c.spawnZones)
                            {
                                foreach (Image img in zone.spawnAreaObject.GetComponentsInChildren<Image>())
                                {
                                    if (img.name == "Icon" && worldIcon != null)
                                    {
                                        worldIcon = img;
                                        worldIcon.enabled = false;
                                    }
                                }
                            }
                        }
                    }
                }
                break;
            }
            foreach (Criteria c in q.unCompletedCriterias[QuestHandler.Instance.SelectedPlayer])
            {
                foreach (SpawnZone zone in c.spawnZones)
                {
                    if (zone != null)
                    {
                        if (zone.spawnAreaObject)
                        {
                            if (compassArrow != null && compass != null)
                            {
                                var arrow = Instantiate(compassArrow, compassArrow.transform.localPosition, compassArrow.transform.rotation, compass.transform);
                                arrow.transform.localPosition = compassArrow.transform.localPosition;
                                arrow.GetComponent<QuestCompassArrow>().target = zone.spawnAreaObject.transform;
                                compassArrows.Add(arrow);
                                foreach (Image img in zone.spawnAreaObject.GetComponentsInChildren<Image>())
                                {
                                    if (img.name == "Icon" && worldIcon != null)
                                    {
                                        worldIcon = img;
                                        worldIcon.enabled = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("Criteria does not have a spawnAreaObject, so no arrow will point to it");
                        }
                    }
                    else
                    {
                        c.spawnZones.Remove(zone);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Opens a quest pop up
    /// </summary>
    /// <param name="questGiver">The questgiver giving the quest</param>
    /// <param name="player">The player recieving the quest</param>
    public void StartQuestPopUp(QuestGiver questGiver, CQPlayerObject player, Quest quest)
    {
        if (!activeQuestPopUpQuests.ContainsKey(player))
        {
            activeQuestPopUpQuests.Add(player, new List<Quest>());
        }
        if (!activeQuestPopUpQuests[player].Contains(quest))
        {
            if (!quest.playersUnCompleted.Contains(player))
            {
                if (!quest.playersCompleted.Contains(player) || (quest.repeatable && quest.remainingRepeatableTime[player] <= 0))
                {
                    QuestPopUp questPopUp = Instantiate(questPopUpPrefab) as QuestPopUp;
                    questPopUp.transform.SetParent(this.gameObject.transform, false);
                    questPopUp.questGiver = questGiver;
                    questPopUp.player = player;
                    questPopUp.quest = quest;
                    activeQuestPopUpQuests[player].Add(quest);
                    questPopUp.SetStartValues(this);
                }
            }
        }
    }

    /// <summary>
    /// Updates the quest wheel selection
    /// </summary>
    /// <param name="highlightedItem">The new highlighted item</param>
    public void UpdateSelectionCircle(int highlightedItem)
    {
        int counter = 0;
        foreach (UIPolygon p in questWheelBackgrounds)
        {
            counter++;
            if (counter == highlightedItem)
            {
                p.color = highlightColor;
            }
            else
            {
                p.color = notHighlightColor;
                if (worldIconGlow != null)
                {
                    worldIconGlow.enabled = false;
                }
            }
        }

        questWheelSelection = highlightedItem;
        try
        {
            Quest highlightedQuest = QuestHandler.Instance.availableQuests[QuestHandler.Instance.selectedPlayer][highlightedItem - 1];
            middleText.text = highlightedQuest.questName;
            questNameText.text = highlightedQuest.questName;
            descriptionText.text = highlightedQuest.description;
            contentHolder = "Criteria";
            contentHolder += "\n";
            foreach (Criteria c in highlightedQuest.criterias)
            {
                contentHolder += c.criteriaName + "  " + c.playerProgression[QuestHandler.Instance.selectedPlayer] + " / " + c.amount + "\n";
            }
            if (highlightedQuest.unCompletedCriterias[QuestHandler.Instance.selectedPlayer].Count <= 0)
            {
                foreach (HandInObject h in highlightedQuest.handInObjects)
                {
                    foreach (Image img in h.GetComponentsInChildren<Image>())
                    {
                        if (img.name == "Glow")
                        {
                            worldIconGlow = img;
                            worldIconGlow.enabled = true;
                        }
                    }
                }
            }
            foreach (Criteria c in highlightedQuest.unCompletedCriterias[QuestHandler.Instance.selectedPlayer])
            {
                foreach (SpawnZone zone in c.spawnZones)
                {
                    foreach (Image img in zone.spawnAreaObject.GetComponentsInChildren<Image>())
                    {
                        if (img.name == "Glow")
                        {
                            worldIconGlow = img;
                            worldIconGlow.enabled = true;
                        }
                    }
                }
            }
            criteriaText.text = contentHolder;
            contentHolder = "Reward\n";
            foreach (Reward r in highlightedQuest.rewards)
            {
                contentHolder += r.amount + "  " + r.rewardName + "\n";
            }
            rewardsText.text = contentHolder;
        }
        catch (System.Exception)
        {
            if (middleText != null)
            {
                middleText.text = "";
            }
            if (worldIconGlow != null)
            {
                worldIconGlow.enabled = false;
            }
        }
    }

    /// <summary>
    /// Controls the hover over UI
    /// </summary>
    /// <param name="results"></param>
    private void UIMouseHover(List<RaycastResult> results)
    {
        bool foundValidHover = false;
        foreach (RaycastResult r in results)
        {
            if (foundValidHover == true) { break; }
            if (r.gameObject.tag == "UINameHolder")
            {
                switch (r.gameObject.name)
                {
                    case "Middle":
                        foundValidHover = true;
                        break;

                    case "Action1":
                        UpdateSelectionCircle(1);
                        foundValidHover = true;
                        break;

                    case "Action2":
                        UpdateSelectionCircle(2);
                        foundValidHover = true;
                        break;

                    case "Action3":
                        UpdateSelectionCircle(3);
                        foundValidHover = true;
                        break;

                    case "Action4":
                        UpdateSelectionCircle(4);
                        foundValidHover = true;
                        break;

                    case "Action5":
                        UpdateSelectionCircle(5);
                        foundValidHover = true;
                        break;

                    case "Action6":
                        UpdateSelectionCircle(6);
                        foundValidHover = true;
                        break;

                    case "Action7":
                        UpdateSelectionCircle(7);
                        foundValidHover = true;
                        break;

                    case "Action8":
                        UpdateSelectionCircle(8);
                        foundValidHover = true;
                        break;

                    default:
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Mouse Click Logic
    /// </summary>
    /// <param name="RayHits">The hits from a raycast (Things clicked on)</param>
    private void MouseClick(List<GameObject> RayHits)
    {
        QuestGiver qG = GetComponent<QuestGiver>();
        foreach (GameObject go in RayHits)
        {
            if (go.GetComponent<QuestGiver>())
            {
                qG = go.GetComponent<QuestGiver>();
                break;
            }
            else if (go.GetComponentInParent<QuestGiver>())
            {
                qG = go.GetComponentInParent<QuestGiver>();
                break;
            }
            else if (go.GetComponentInChildren<QuestGiver>())
            {
                qG = go.GetComponentInChildren<QuestGiver>();
                break;
            }
        }

        if (qG != null)
        {
            //Debug.Log(qG);
            if (Vector3.Distance(QuestHandler.Instance.SelectedPlayer.transform.position, qG.transform.position) <= qG.radius || qG.radius == 0) //TODO: From unit distance?
            {
                if (Vector3.Distance(QuestHandler.Instance.SelectedPlayer.transform.position, qG.transform.position) <= qG.declineDistance || qG.declineDistance == 0)
                {
                    foreach (Quest quest in qG.quests)
                    {
                        qG.StartQuestPopUp(QuestHandler.Instance.selectedPlayer, quest);
                    }
                }
            }
        }
    }

    /// <summary>
    /// An ingame raycast from the mouseposition
    /// </summary>
    /// <returns>A list of everything hit</returns>
    private List<GameObject> RayCastInGame()
    {
        List<GameObject> allResults = new List<GameObject>();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        hits = Physics.RaycastAll(ray, 10000);

        if (hits.Length >= 0)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                RaycastHit hit = hits[i];
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Quest Object"))
                {
                    allResults.Add(hit.collider.gameObject);
                }
            }
            return allResults;
        }
        return null;
    }

    /// <summary>
    /// A UI raycast
    /// </summary>
    /// <returns>A list of everything UI hit</returns>
    private List<RaycastResult> RayCastUi()
    {
        if (gr != null)
        {
            //Create the PointerEventData with null for the EventSystem
            PointerEventData ped = new PointerEventData(null);
            //Set the mouse position
            ped.position = Input.mousePosition;
            //Create list to receive all results
            List<RaycastResult> allResults = new List<RaycastResult>();
            List<RaycastResult> sortedResults = new List<RaycastResult>();
            //Raycast it
            gr.Raycast(ped, allResults);
            if (allResults.Count != 0)
            {
                foreach (RaycastResult r in allResults)
                {
                    if (r.gameObject.layer == LayerMask.NameToLayer("UI"))
                    {
                        sortedResults.Add(r);
                    }
                }
                if (sortedResults.Count != 0)
                {
                    return sortedResults;
                }
                return allResults;
            }
        }
        return null;
    }

    /*** Event Methods ***/

    private void QuestCompleted(EventInfoHolder info)
    {
        if (info.quest)
        {
            AddOnSreenMsg(5f, info.quest.questName + " Completed!", 20, Color.white); //Adds an on screen telling the player, he has completed this quest
        }
    }

    private void CriteraProgress(EventInfoHolder info)
    {
        if (info.criteria && info.player)
        {
            AddOnSreenMsg(5f, info.criteria.criteriaName + " " + info.criteria.playerProgression[info.player] + "/" + info.criteria.amount, 15, Color.white);
        }
    }

    private void GiveReward(EventInfoHolder info)
    {
        if (info.reward)
        {
            AddOnSreenMsg(5f, info.reward.amount + " " + info.reward.rewardName + " gained!", 15, Color.white);
        }
    }

    private void ResetQuestEvent(EventInfoHolder info)
    {
        if (info.quest)
        {
            ResetQuest(info.quest);
        }
    }

    private void UpdateQuestTracker(EventInfoHolder info)
    {
        UpdateQuestTracker();
    }

    private void StartQuestPopUp(EventInfoHolder info)
    {
        StartQuestPopUp(info.questGiver, info.player, info.quest);
    }
}