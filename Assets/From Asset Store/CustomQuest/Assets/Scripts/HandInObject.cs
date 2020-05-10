using System.Collections.Generic;
using UnityEngine;
using CustomQuest;

/// <summary>
/// Script used by handinobject, which is an object a player can hand its quest in at, if needed.
/// </summary>
public class HandInObject : MonoBehaviour
{
    #region Fields

    /// <summary>
    /// The radius of the sphereCollider
    /// </summary>
    public float radius = 3;

    /// <summary>
    /// The sphere collider of this handInObject
    /// </summary>
    public SphereCollider sphere;

    /// <summary>
    /// A list of quests, this is the handInObject of.
    /// </summary>
    public List<Quest> quests = new List<Quest>();

    /// <summary>
    /// If true, will run the attached quest's collision method, when gameobject collides with this object. Otherwise, another method will be needed to hand in the quest
    /// </summary>
    public bool handInByCollision = true;

    #endregion Fields

    #region Properties

    /// <summary>
    /// Gets the radius of this handinobject, or sets it and the attached sqherecolliders radius
    /// </summary>
    [SerializeField]
    public float Radius { get { return radius; } set { radius = value; if (sphere) { sphere.radius = radius; } } }

    #endregion Properties

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        sphere = gameObject.AddComponent<SphereCollider>();
        sphere.radius = Radius;
        sphere.isTrigger = true;
    }

    /// <summary>
    /// When a object collides with this object, it runs some checks to check if the its a player, and if it has completed all the needed criterias. If all that is good, it completed the quest.
    /// </summary>
    /// <param name="coll">The other collider</param>
    public void OnTriggerEnter(Collider coll)
    {
        if (handInByCollision)
        {
            if (coll.GetComponentInParent<CQPlayerObject>())
            {
                CQPlayerObject CQplayer = coll.GetComponentInParent<CQPlayerObject>();
                CQExamplePlayer player = coll.GetComponentInParent<CQExamplePlayer>();
                foreach (Quest q in quests)
                {
                    if (q != null)
                    {
                        if (q.handInObjects.Contains(this))
                        {
                            if (q.unCompletedCriterias.ContainsKey(CQplayer) != true || q.completedCriterias.ContainsKey(CQplayer) != true)
                            {
                                q.AddPlayerToCriterias();
                            }
                            if (q.activeCriterias.ContainsKey(CQplayer))
                            {
                                if (q.activeCriterias[CQplayer].Count <= 0)
                                {
                                    q.OnCompletion(CQplayer, player);
                                }
                                else
                                {
                                    for (int i = q.activeCriterias[CQplayer].Count - 1; i >= 0; i--)
                                    {
                                        Criteria c = q.activeCriterias[CQplayer][i];
                                        if (c.type == criteriaType.Deliver)
                                        {
                                            for (int j = player.items.Count - 1; j >= 0; j--)
                                            {
                                                if (player.items.Count > j)
                                                {
                                                    if (player.items[j])
                                                    {
                                                        if (player.items[j].GetComponent<QuestObject>())
                                                        {
                                                            if (player.items[j].GetComponent<QuestObject>().criteria == c)
                                                            {
                                                                if ((player.items[j]))
                                                                {
                                                                    c.Remove(player.items[j].gameObject);
                                                                }
                                                                player.items.RemoveAt(j);
                                                                c.Progress(CQplayer, player);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    if (q.activeCriterias[CQplayer].Count <= 0)
                                    {
                                        q.OnCompletion(CQplayer, player);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        quests.Remove(q);
                        break;
                    }
                }
            }
        }
    }
}