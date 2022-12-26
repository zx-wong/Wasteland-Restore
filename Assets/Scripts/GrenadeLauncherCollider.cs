using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeLauncherCollider : MonoBehaviour
{
    private SphereCollider explodeCollider;

    private void Start()
    {
        explodeCollider = GetComponent<SphereCollider>();

        explodeCollider.enabled = false;
    }

    private void Update()
    {
        
    }
}
