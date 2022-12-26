using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPickUp : MonoBehaviour
{
    [SerializeField] private GunSystem gunSystem;
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Transform gunHolder;

    [SerializeField] private BoxCollider gunCollider;

    private new Rigidbody rigidbody;

    private float pickUpRange = 1.5f;

    [SerializeField] private bool equipped;
    [SerializeField] private bool slotFull;

    private void Start()
    {
        gunSystem = GetComponent<GunSystem>();
        gunCollider = GetComponent<BoxCollider>();
        rigidbody = GetComponent<Rigidbody>();

        if (!equipped)
        {
            gunSystem.enabled = false;
            rigidbody.isKinematic = false;
            gunCollider.isTrigger = false;
        }
        else
        {
            gunSystem.enabled = true;
            rigidbody.isKinematic = true;
            gunCollider.isTrigger = true;
        }
    }

    private void Update()
    {
        Vector3 distance = player.position - transform.position;

        if (!equipped && distance.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && !slotFull)
        {
            PickUp();
        }
    }

    public void PickUp()
    {
        equipped = true;
        slotFull = true;

        transform.SetParent(gunHolder, true);
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rigidbody.isKinematic = true;
        gunCollider.isTrigger = true;

        gunSystem.enabled = true;
    }
}
