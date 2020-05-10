using UnityEngine;

/// <summary>
/// A quest compass arrow, pointing at its target
/// </summary>
public class QuestCompassArrow : QuestCompass
{
    /// <summary>
    /// Where is the quest? - Where is the compass pointing? // gameobject or the like
    /// </summary>
    public Transform target;

    public override void Start()
    {
        player = transform.parent.GetComponent<QuestCompass>().player; //Sets the arrows player setting, to the same as the compassholder itself
        base.Start();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    public override void Update()
    {
        base.Update();

        // target is the affiliated quests current spawn area or hand inn
        if (target != null)
        {
            // get the vector between what we need to look at and where we are looking from
            var heading = target.transform.position - targetLocation;
            // how far?
            var distance = heading.magnitude;
            // normalize the vector, to get a direction.
            var direction = heading / distance;
            // make the object look the direction
            transform.localRotation = Quaternion.LookRotation(Vector3.up, -direction);
            if (player)
            {
                transform.localRotation = Quaternion.Inverse(origin.transform.rotation) * transform.localRotation;
            }
        }
    }
}