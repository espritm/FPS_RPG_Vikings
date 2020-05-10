using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A handler used for handling on screen messages
/// </summary>
public class OnScreenMsgHandler : MonoBehaviour
{
    #region Field

    [SerializeField]
    private List<OnScreenMsg> msgs = new List<OnScreenMsg>();

    /// <summary>
    /// The maximum amount of messages
    /// </summary>
    [SerializeField, Tooltip("The maximum amount of messages")]
    private int maxMessages = 8;

    private QuestUI questUI;

    #endregion Field

    #region Properties

    public List<OnScreenMsg> Msgs { get { return msgs; } set { msgs = value; } }

    #endregion Properties

    /// <summary>
    /// Runs on initialization
    /// </summary>
    private void Start()
    {
        if (questUI == null)
        {
            GetQuestList();
        }
    }

    /// <summary>
    /// Tries to find a questList
    /// </summary>
    private void GetQuestList()
    {
        questUI = GetComponentInParent<QuestUI>();
        if (questUI == null)
        {
            GetComponent<QuestUI>();
            if (questUI == null)
            {
                GetComponentInChildren<QuestUI>();
                if (questUI == null)
                {
                    Debug.LogWarning("OnScreenMsgHandler cannot find a QuestUI. Therefor it is unable to funktion. Make sure this component is the child or parent of a questList");
                }
            }
        }
    }

    /*** Public Methods ***/

    /// <summary>
    /// Adds an On Screen Msg
    /// </summary>
    /// <param name="lifeTime">The time the msg should be displayed</param>
    /// <param name="msg">The msg to be shown</param>
    /// <param name="size">The size of text</param>
    /// <param name="color">The color of the text</param>
    public void AddMsg(float lifeTime, string msg, int size, Color color)
    {
        if (questUI == null)
        {
            GetQuestList();
        }
        if (questUI != null)
        {
            GameObject newUnit = (GameObject)Instantiate(questUI.messagePrefab, this.transform.position, this.transform.rotation);
            newUnit.transform.SetParent(this.transform, false);
            OnScreenMsg onScreenMsg = newUnit.GetComponent<OnScreenMsg>();
            if (onScreenMsg != null)
            {
                onScreenMsg.LifeTime = lifeTime;
                onScreenMsg.Msg = msg;
                onScreenMsg.Size = size;
                onScreenMsg.Color = color;
                onScreenMsg.MsgPosition = new Vector2(Screen.width / 2 - 50, 100);

                Vector2 newPos = new Vector2(Screen.width / 2 - 50, 100);
                if (Msgs.Count >= 1)
                {
                    foreach (OnScreenMsg g in Msgs)
                    {
                        newPos.y -= g.Size;
                        g.MsgPosition = newPos;
                    }
                }
                Msgs.Insert(0, onScreenMsg);
                if (Msgs.Count > maxMessages)
                {
                    GameObject toRemove = Msgs[maxMessages].gameObject;
                    Msgs.RemoveAt(maxMessages);
                    Destroy(toRemove);
                }
            }
            else
            {
                Debug.LogWarning("The onScreenPrefab does not contain a OnScreenMsg component. On Screen Messages will not funktion");
            }
        }
    }
}