using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private WeaponHandler[] weapons;

    [SerializeField]
    private ShieldHandler[] shields;

    private PlayerAttack playerAttackController;

    [SerializeField]
    private Button fireButton;

    [SerializeField]
    private Button aimButton;

    [SerializeField]
    private Button aimShieldButton;
        
    private int currentWeaponIndex;
    private int currentShieldIndex;


    void Awake()
    {
        playerAttackController = GetComponent<PlayerAttack>();
    }

    void Start()
    {
        currentWeaponIndex = 0;
        weapons[currentWeaponIndex].gameObject.SetActive(true);
        shields[currentShieldIndex].gameObject.SetActive(true);
        ActivateDeactivateShield();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TurnOnSelectedWeapon(0);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TurnOnSelectedWeapon(1);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TurnOnSelectedWeapon(2);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            TurnOnSelectedWeapon(3);
        }

        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            TurnOnSelectedWeapon(4);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            TurnOnSelectedWeapon(5);
        }
    }

    void TurnOnSelectedWeapon(int weaponIndex)
    {
        if (weaponIndex >= weapons.Length || weapons[currentWeaponIndex].gameObject == null)
            return;

        //Do nothing if the new weapon selected weapon and the current weapon are the same, do nothing
        if (currentWeaponIndex == weaponIndex)
            return;

        //Unaim current weapon if needed
        playerAttackController.ForceUnaimWeapon();

        //Turn of the current weapon
        weapons[currentWeaponIndex].gameObject.SetActive(false);

        //Turn on the selected weapon
        weapons[weaponIndex].gameObject.SetActive(true);

        //Update Fire button icon and AimAndFireButton
        fireButton.image.sprite = weapons[weaponIndex].weaponFireSprite;

        //Store the current selected weapon index
        currentWeaponIndex = weaponIndex;

        ActivateDeactivateShield();
    }

    void ActivateDeactivateShield()
    {
        //Temp : disable shield if not axe
        if (currentWeaponIndex == 0)
        {
            shields[currentShieldIndex].gameObject.SetActive(true);
            aimButton.gameObject.SetActive(false);
            aimShieldButton.gameObject.SetActive(true);
        }
        else
        {
            shields[currentShieldIndex].gameObject.SetActive(false);
            aimButton.gameObject.SetActive(true);
            aimShieldButton.gameObject.SetActive(false);
        }
    }

    public WeaponHandler GetCurrentSelectedWeapon()
    {
        return weapons[currentWeaponIndex];
    }

    public ShieldHandler GetCurrentSelectedShield()
    {
        return shields[currentShieldIndex];
    }


    public void OnInventary1ButtonClicked()
    {
        TurnOnSelectedWeapon(0);
    }
    public void OnInventary2ButtonClicked()
    {
        TurnOnSelectedWeapon(1);
    }
    public void OnInventary3ButtonClicked()
    {
        TurnOnSelectedWeapon(2);
    }
    public void OnInventary4ButtonClicked()
    {
        TurnOnSelectedWeapon(3);
    }
    public void OnInventary5ButtonClicked()
    {
        TurnOnSelectedWeapon(4);
    }
    public void OnInventary6ButtonClicked()
    {
        TurnOnSelectedWeapon(5);
    }
}
