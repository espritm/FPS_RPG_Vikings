using UnityEngine;

/// <summary>
/// An example enemy class. Will take damage when colliding with a player, and when its health is 0, will process the criteria its a part of (If its part of a criteria)
/// </summary>
public class Enemy : MonoBehaviour
{
    #region Field

    /// <summary>
    /// The health of this enemy
    /// </summary>
    public int health = 100;

    /// <summary>
    /// The last player who did dmg to this enemy.
    /// </summary>
    private CQPlayerObject player;

    /// <summary>
    /// The last unit who did dmg to this enemy
    /// </summary>
    private CQExamplePlayer unit;

    /// <summary>
    /// The quest object component attached to this enemy
    /// </summary>
    private QuestObject questObject;

    #endregion Field

    /// <summary>
    ///  Use this for initialization
    /// </summary>
    private void Start()
    {
        questObject = GetComponent<QuestObject>();
    }

    /// <summary>
    /// Runs when this objects trigger colliders with another
    /// </summary>
    /// <param name="other">The other object colliding with this one</param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Sword>())
        {
            health -= other.GetComponentInParent<CQExamplePlayer>().Damage;
            player = other.GetComponentInParent<CQPlayerObject>();
            unit = other.GetComponentInParent<CQExamplePlayer>();

            if (health <= 0)
            { // Checks if this object is out of health, kills it if that is true
                health = 0;
                if (questObject == null) { questObject = GetComponent<QuestObject>(); }
                if (questObject != null)
                {
                    if (questObject.criteria)
                    {
                        questObject.criteria.Remove(this.gameObject);
                        if (QuestHandler.Instance.availableQuests[player].Contains(questObject.criteria.Quest))
                        {
                            questObject.criteria.Progress(player, unit); // Send quest progress result, with the killer
                        }
                    }
                    Destroy(this.gameObject);
                }
            }
        }
    }
}