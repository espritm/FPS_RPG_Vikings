using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A pop up for when picking up a quest, when the game is running
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class QuestPopUp : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// Rect transform, used for storing the rectTransform this component is attached to
    /// </summary>
    private RectTransform rect;

    /// <summary>
    /// The quest this quest pop up is giving
    /// </summary>
    public Quest quest;

    /// <summary>
    /// The player currently recieving this quest
    /// </summary>
    public CQPlayerObject player;

    /// <summary>
    /// The giver of this quest
    /// </summary>
    public QuestGiver questGiver;

    /// <summary>
    /// The text to contains the title
    /// </summary>
    public Text title;

    /// <summary>
    /// The text to contain the description
    /// </summary>
    public Text description;

    /// <summary>
    /// The text to contain the criterias
    /// </summary>
    public Text criterias;

    /// <summary>
    /// The text to contains the rewards
    /// </summary>
    public Text rewards;

    /// <summary>
    /// The image to contain the icon
    /// </summary>
    public Image icon;

    /// <summary>
    /// The questUI this popup is a part of
    /// </summary>
    public QuestUI questUI;

    #endregion Fields

    /// <summary>
    /// Start is called once, when the object is instatiated
    /// </summary>
    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    /// <summary>
    ///  Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (player)
        {
            if (questGiver.declineDistance > 0)
            {
                if (Vector3.Distance(player.transform.position, questGiver.gameObject.transform.position) >= questGiver.declineDistance)
                {
                    DeclineQuest();
                }
            }
        }
    }

    /// <summary>
    /// OnDestroy is called, just before this object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        questUI.activeQuestPopUpQuests[player].Remove(quest);
    }

    /// <summary>
    /// Sets this QuestPopUp's start values
    /// </summary>
    public void SetStartValues(QuestUI questUI)
    {
        this.questUI = questUI;
        if (questGiver != null)
        {
            title.text = quest.questName;
            description.text = quest.description;
            criterias.text = "";
            //foreach (Criteria c in quest.criterias)
            //{
            //    criterias.text += c.type.ToString() + " " + c.amount.ToString();
            //    if (c.criteriaObject != null)
            //    {
            //        criterias.text = " " + c.criteriaObject.name;
            //    }
            //    criterias.text += "\n";
            //    description.rectTransform.position = new Vector3(description.rectTransform.position.x, description.rectTransform.position.y - 15, description.rectTransform.position.z);
            //}
            //rewards.text = "";
            //rewards.rectTransform.position = new Vector3(description.rectTransform.position.x, description.rectTransform.position.y - 15, description.rectTransform.position.z);
            //foreach (Reward r in quest.rewards)
            //{
            //    rewards.text += r.type.ToString() + " " + r.amount.ToString();
            //    if (r.rewardObject != null)
            //    {
            //        rewards.text += " " + r.rewardObject.name;
            //    }
            //    rewards.text += "\n";
            //}
            icon.sprite = quest.questIcon;
        }
        else
        {
            Debug.LogWarning("No questgiver in " + this + "Start values could not be set");
        }
    }

    /// <summary>
    /// The player accepted this questPopUps quest
    /// </summary>
    public void AcceptQuest()
    {
        QuestHandler.Instance.QuestsDiscovered(quest, player);
        Destroy(this.gameObject);
    }

    /// <summary>
    /// The player declined this questPopUps quest
    /// </summary>
    public void DeclineQuest()
    {
        Destroy(this.gameObject);
    }

    /// <summary>
    /// Drags the window with the mouse
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrag(UnityEngine.EventSystems.BaseEventData eventData)
    {
        var pointerData = eventData as UnityEngine.EventSystems.PointerEventData;
        if (pointerData == null) { return; }

        var currentPosition = rect.position;
        currentPosition.x += pointerData.delta.x;
        currentPosition.y += pointerData.delta.y;
        rect.position = currentPosition;
    }
}