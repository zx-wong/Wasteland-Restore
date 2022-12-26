using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Image crosshair;

    RaycastHit hitInfo;

    private void Start()
    {
        playerCamera = GetComponentInParent<Camera>();
        crosshair = GetComponent<Image>();
    }

    private void Update()
    {
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hitInfo, Mathf.Infinity))
        {
            if (hitInfo.collider.tag == "Enemy")
            {
                crosshair.color = new Color(1, 0, 0);
            }
            else
            {
                crosshair.color = new Color(1, 1, 1);
            }
        }
    }
}
