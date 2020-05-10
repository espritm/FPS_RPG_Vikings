using UnityEngine;

/// <summary>
/// The quest compass script. Used for controlling the direction of the compass arrows. Gets an origin point, from the center of the camera.
/// </summary>
public class QuestCompass : MonoBehaviour
{
    /// <summary>
    ///  Where we are looking from, or where the player is
    /// </summary>
    public GameObject origin;

    /// <summary>
    /// Where the compas is pointing from
    /// </summary>
    public Vector3 targetLocation;

    /// <summary>
    /// Point from player, or middle of screen
    /// </summary>
    public bool player;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    public virtual void Start()
    {
        if (player)
        {
            if (QuestHandler.Instance.SelectedPlayer)
            {
                origin = QuestHandler.Instance.SelectedPlayer.gameObject;
            }
        }
        else
        {
            origin = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    /// <summary>
    /// Is run every frame
    /// </summary>
    public virtual void Update()
    {
        if (player)
        {
            targetLocation = origin.transform.position;
        }
        else
        {
            Ray ray = origin.GetComponent<Camera>().ViewportPointToRay(new Vector3(.5f, .5f, 0));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                targetLocation = hit.point;
            }
        }
        //Debug.DrawLine(origin.transform.position, targetLocation, Color.red);
    }
}