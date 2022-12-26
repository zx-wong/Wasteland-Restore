using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Melee : MonoBehaviour
{
    [SerializeField] private int damage;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger");
        if (other.gameObject.tag == "Player")
        {
            other.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
        }
    }
}
