using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSwitch : MonoBehaviour
{
    [SerializeField] private int selectedGun = 0;
    [SerializeField] private int previousSelectedGun;

    // Start is called before the first frame update
    void Start()
    {
        HandleWeapon();
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        if (previousSelectedGun != selectedGun)
        {
            Invoke("HandleWeapon", .1f);
            previousSelectedGun = selectedGun;
        }
    }

    public void HandleInput()
    { 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedGun = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2) && transform.childCount >= 2)
        {
            selectedGun = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3) && transform.childCount >= 3)
        {
            selectedGun = 2;
        }
    }

    public void HandleWeapon()
    {
        int i = 0;

        foreach (Transform weapon in transform)
        {
            if (i == selectedGun)
            {
                weapon.gameObject.SetActive(true);
            }
            else
            {
                weapon.gameObject.SetActive(false);
            }

            i++;
        }
    }
}
