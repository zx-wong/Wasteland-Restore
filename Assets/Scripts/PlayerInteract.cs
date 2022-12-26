using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GunSystem gunSystem;
    [SerializeField] private WeaponSystem weaponSystem;

    [SerializeField] private GameObject weaponHolder;
    private Transform activeWeapon;

    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip itemClip;

    [SerializeField] private TMP_Text notifyText;

    void Start()
    {
        playerHealth = GetComponentInChildren<PlayerHealth>();
        gunSystem = GetComponentInChildren<GunSystem>();
        weaponSystem = GetComponentInChildren<WeaponSystem>();
        source = GetComponent<AudioSource>();

        notifyText.gameObject.SetActive(false);
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.tag == "Gun")
        {
            source.PlayOneShot(itemClip);
            notifyText.gameObject.SetActive(true);
            //Debug.Log(hit.gameObject.name);
            switch (hit.gameObject.name)
            {
                case "SMG":
                    weaponSystem.allowSMG = true;
                    Destroy(hit.gameObject);
                    break;
                case "Rifle":
                    weaponSystem.allowRifle = true;
                    Destroy(hit.gameObject);
                    break;
                case "Shotgun":
                    weaponSystem.allowShotgun = true;
                    Destroy(hit.gameObject);
                    break;
                case "Launcher":
                    weaponSystem.allowLauncher = true;
                    Destroy(hit.gameObject);
                    break;
                default:
                    break;
            }

            notifyText.text = "Picked up " + hit.gameObject.name;
        }

        if (hit.gameObject.tag == "Interactable")
        {
            source.PlayOneShot(itemClip);
            notifyText.gameObject.SetActive(true);

            if (hit.gameObject.name == "HealthKit" || hit.gameObject.name == "HealthKit(Clone)")
            {
                if (!playerHealth.healthFull)
                {
                    playerHealth.RestoreHealth(10);
                    Destroy(hit.gameObject);
                }
            }

            if (hit.gameObject.name == "ArmorKit" || hit.gameObject.name == "ArmorKit(Clone)")
            {
                if (!playerHealth.armorFull)
                {
                    playerHealth.RestoreArmor(10);
                    Destroy(hit.gameObject);
                }
            }

            if (hit.gameObject.name == "AmmoBox" || hit.gameObject.name == "AmmoBox(Clone)")
            {
                for (int i = 0; i < weaponHolder.transform.childCount; i++)
                {
                    if (weaponHolder.transform.GetChild(i).gameObject.activeSelf == true)
                    {
                        activeWeapon = weaponHolder.transform.GetChild(i);
                    }
                }

                activeWeapon.GetComponent<GunSystem>().IncreaseAmmo(Random.Range(10, 15));
                Destroy(hit.gameObject);
            }

            if (hit.gameObject.name == "AmmoCrate" || hit.gameObject.name == "AmmoCrate(Clone)")
            {
                for (int i = 0; i < weaponHolder.transform.childCount; i++)
                {
                    if (weaponHolder.transform.GetChild(i).gameObject.activeSelf == true)
                    {
                        activeWeapon = weaponHolder.transform.GetChild(i);
                    }
                }

                activeWeapon.GetComponent<GunSystem>().IncreaseAmmo(Random.Range(10, 15));
            }

            notifyText.text = "Picked up " + hit.gameObject.name;
        }
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(3f);
        notifyText.gameObject.SetActive(false);
    }
}
