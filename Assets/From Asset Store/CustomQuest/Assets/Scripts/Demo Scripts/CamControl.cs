using UnityEngine;

/// <summary>
/// Controls an object to follow a target, with a distance behind and above. Made to make a camera follow a player
/// </summary>
public class CamControl : MonoBehaviour
{
    /// <summary>
    /// The target the camera should follow
    /// </summary>
    public Transform target;

    /// <summary>
    /// The distance behind the target the camera should be
    /// </summary>
    public float distanceBehind;

    /// <summary>
    /// The distance above the target the camera should be
    /// </summary>
    public float distanceTop;

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        transform.position = new Vector3(target.position.x, target.position.y + distanceTop, target.position.z - distanceBehind);
    }
}