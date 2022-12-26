using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSystem : MonoBehaviour
{
    // Switch Gun
    private int selectedGun = 0;
    private int previousSelectedGun;

    public bool allowPistol;
    public bool allowSMG;
    public bool allowRifle;
    public bool allowShotgun;
    public bool allowLauncher;

    void Start()
    {
        HandleWeapon();
    }

    void Update()
    {
        HandleSwitchWeapon();

        if (previousSelectedGun != selectedGun)
        {
            Invoke("HandleWeapon", .1f);
            previousSelectedGun = selectedGun;
        }
    }

    private void HandleSwitchWeapon()
    {
        if (allowPistol)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                selectedGun = 0;
            }
        }

        if (allowSMG)
        {
            if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
            {
                selectedGun = 1;
            }
        }

        if (allowRifle)
        {
            if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
            {
                selectedGun = 2;
            }
        }

        if (allowShotgun)
        {
            if (Input.GetKeyDown(KeyCode.Alpha4) && transform.childCount >= 4)
            {
                selectedGun = 3;
            }
        }

        if (allowLauncher)
        {
            if (Input.GetKeyDown(KeyCode.Alpha5) && transform.childCount >= 5)
            {
                selectedGun = 4;
            }
        }
    }

    private void HandleWeapon()
    {
        int i = 0;

        foreach (Transform weapon in transform)
        {
            if (i == selectedGun)
            {
                weapon.gameObject.SetActive(true);
                //weapon.transform.GetChild(0).gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
                //weapon.transform.GetChild(0).gameObject.SetActive(false);
            }

            i++;
        }
    }
}
