using UnityEngine;

/// <summary>
/// A class for controlling when the sword is lethal, and for dealing dmg
/// </summary>
public class Sword : MonoBehaviour
{
    private CQExamplePlayer player;

    /// <summary>
    /// Use this for initialization
    /// </summary>
    private void Start()
    {
        if (player == null)
        {
            player = GetComponentInParent<CQExamplePlayer>();
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        if (player.attacking)
        {
            GetComponent<Collider>().enabled = true;
        }
        else
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}