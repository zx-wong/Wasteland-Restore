using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupSystem : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Camera playerCamera;

    [SerializeField] GameObject equiped;
    [SerializeField] GameObject unequiped;

    private int range = 5;

    private RaycastHit hitInfo;

    [SerializeField] private LayerMask interactMask;

    private void Awake()
    {
        playerInput = new PlayerInput();
        playerInput.Enable();

        playerCamera = GetComponentInChildren<Camera>();

        playerInput.Player.Interact.performed += i => PickUp();
    }

    private void Update()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * range, Color.yellow, .1f);
    }

    public void PickUp()
    {
        //Debug.Log("E");

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, range, interactMask))
        {
            if (hitInfo.collider.tag == "Gun")
            {
                //EquipGun(hitInfo.collider.name);
            }
        }
    }

    public void EquipGun(string gunName)
    {
        foreach (Transform weapon in unequiped.transform)
        {
            if (weapon.name == gunName)
            {
                //Debug.Log("Move");
                weapon.gameObject.transform.SetParent(equiped.transform);
            }
        }
    }
}
