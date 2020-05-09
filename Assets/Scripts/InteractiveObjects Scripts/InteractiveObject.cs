using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractAction
{
    TALK,
    OPEN,
    LOOT
}

public class InteractiveObject : MonoBehaviour
{
    public InteractAction interactAction = InteractAction.TALK;

    public string displayName;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
