using System.Collections.Generic;
using UnityEngine;

public class OnScreenMsg : MonoBehaviour
{
    #region Field

    [SerializeField, Header("info")]
    private float lifeTime;

    [SerializeField]
    private string msg;

    [SerializeField]
    private int size;

    [SerializeField]
    private Color color;

    [SerializeField]
    private Vector2 msgPosition;

    private GUIStyle myGuiStyle = new GUIStyle();

    #endregion Field

    #region Properties

    public float LifeTime { get { return lifeTime; } set { lifeTime = value; } }

    public string Msg { get { return msg; } set { msg = value; } }

    public int Size { get { return size; } set { size = value; } }

    public Color Color { get { return color; } set { color = value; } }

    public Vector2 MsgPosition { get { return msgPosition; } set { msgPosition = value; } }

    #endregion Properties

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        LifeTimeCheck();
    }

    /// <summary>
    /// The OnGui logic
    /// </summary>
    private void OnGUI()
    {//TODO: Remake it to canvas logic (So its behind the top bar)
        myGuiStyle.fontSize = size;
        myGuiStyle.normal.textColor = color;
        GUI.depth = 20;
        GUI.Label(new Rect(MsgPosition.x, MsgPosition.y, 200f, 200f), msg, myGuiStyle);
    }

    /*** Private Methods ***/

    /// <summary>
    /// Checks the lifeTime of the OnScreenMsgs and removes it if its under 0.
    /// </summary>
    private void LifeTimeCheck()
    {
        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0)
        {
            List<OnScreenMsg> msgsHolder = GetComponentInParent<OnScreenMsgHandler>().Msgs;
            msgsHolder.Remove(this);
            Destroy(this.gameObject);
        }
    }
}