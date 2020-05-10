using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The UI logic holder for a criteria. Used by UI logic.
/// </summary>
public class CriteriaUILogic : MonoBehaviour
{
    #region Field

    [Tooltip("The criteria this criteria UI is showing, should be set when spawned")]
    public Criteria criteria;

    [Tooltip("The text who should be showing the name of the criteria")]
    public Text criteriaName;

    [Tooltip("The text who should be showing the amount done of the criteria")]
    public Text amountDone;

    [Tooltip("The slash between the amountdone and the amount")]
    public Text slash;

    [Tooltip("The text who should be showing the totalamount of the criteria")]
    public Text totalAmount;

    [Tooltip("The text who should be showing the type of the criteria")]
    public Text criteriaType;

    [Tooltip("The recttransform of this criteriaUILogic object")]
    public RectTransform rectTransform;

    public bool completed;

    #endregion Field

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        completed = false;
    }
}