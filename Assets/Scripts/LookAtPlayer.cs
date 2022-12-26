using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    [SerializeField] private Transform playerCamera;

    void LateUpdate()
    {
        transform.LookAt(playerCamera);
    }
}
