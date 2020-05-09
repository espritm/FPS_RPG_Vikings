using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator playerDeadAnimator;

    private void Awake()
    {
        playerDeadAnimator = GetComponent<Animator>();
        playerDeadAnimator.enabled = false;
    }

    public void StartDeadAnimation()
    {
        //https://answers.unity.com/questions/16180/how-can-i-animate-a-death-sequence-for-the-fps-con.html
        //Make the Player gamepobject child of a parent empty game object in order to be able to allow the animation to be played using local transform instead of world transform.
        GameObject gO = new GameObject();  //Create an empty gameObject.
        gO.transform.position = transform.position; //Set its position and rotation
        gO.transform.rotation = transform.rotation; //  to match the camera's so 
                                                    //there won't be a jump from the position to the animation position.
        
        transform.parent = gO.transform;

        playerDeadAnimator.enabled = true;
        playerDeadAnimator.SetTrigger(AnimationTags.DEAD_TRIGGER);
    }
}
