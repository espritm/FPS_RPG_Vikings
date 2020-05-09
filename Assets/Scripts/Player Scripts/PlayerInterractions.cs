using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterractions : MonoBehaviour
{
    public GameObject interractionPanel;
    private Text tapToInteractText;
    private Text objectToInteractNameText;

    public float distanceRequiredForInterraction = 3f;

    public LayerMask layerMaskForInterraction;

    private Camera mainCam;

    private string sActionToInteract;

    private void Awake()
    {
        mainCam = Camera.main;

        if (PlatformHelper.ShouldUseMobileGUI())
            sActionToInteract = "Tap"; 
        else
            sActionToInteract = "E"; 

        //Find UI Text from the panel, and deactivate it by default
        tapToInteractText = interractionPanel.transform.GetChild(0).GetComponent<Text>();
        objectToInteractNameText = interractionPanel.transform.GetChild(1).GetComponent<Text>();
        interractionPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        ActivateInterractionPanel();
    }

    private void ActivateInterractionPanel()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCam.transform.position, mainCam.transform.forward, out hit, distanceRequiredForInterraction, layerMaskForInterraction))
        {
            //Update UI Text with interactive object targeted by user
            InteractiveObject target = hit.transform.GetComponent<InteractiveObject>();

            //Update Tap To Interact Text
            if (target.interactAction == InteractAction.TALK)
                tapToInteractText.text = sActionToInteract + " " + Translation.Get(TranslationKeys.ToTalk);
            else if (target.interactAction == InteractAction.LOOT)
                tapToInteractText.text = sActionToInteract + " " + Translation.Get(TranslationKeys.ToLoot);
            else if (target.interactAction == InteractAction.OPEN)
                tapToInteractText.text = sActionToInteract + " " + Translation.Get(TranslationKeys.ToOpen);

            //Update Object Display Name
            objectToInteractNameText.text = target.displayName;

            //User is looking at an interactibe object. Display UI
            if (!interractionPanel.activeInHierarchy)
                interractionPanel.SetActive(true);
        }
        else
        {
            //Deactivate UI if user is not looking at interactive object
            if (interractionPanel.activeInHierarchy)
                interractionPanel.SetActive(false);
        }
    }
}
