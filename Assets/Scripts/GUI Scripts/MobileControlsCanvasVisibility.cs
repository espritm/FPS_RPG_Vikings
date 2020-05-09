using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControlsCanvasVisibility : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlatformHelper.ShouldUseMobileGUI())
            transform.gameObject.SetActive(true);
        else
            transform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
