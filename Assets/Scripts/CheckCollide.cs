using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckCollide : MonoBehaviour
{
    public bool playerCollide;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerCollide = true;
        }
        else
        {
            playerCollide = false;
        }
    }
}
