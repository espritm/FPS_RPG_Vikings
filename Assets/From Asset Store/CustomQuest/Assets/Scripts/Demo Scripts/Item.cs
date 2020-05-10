using UnityEngine;

/// <summary>
/// A sample class for an Item. When it collides with a player, the players pickUpItem method is run.
/// </summary>
public class Item : MonoBehaviour
{
    /// <summary>
    /// Is run when another trigger enters this gameobjects trigger
    /// </summary>
    /// <param name="other">The other trigger colliding</param>
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<CQExamplePlayer>()) //TODO: Should this be in CQPlayerObject instead?
        {
            other.GetComponent<CQExamplePlayer>().pickUpItem(this);
        }
    }
}