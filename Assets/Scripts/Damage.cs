using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] private int damage;

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger");
        switch (other.gameObject.tag)
        {
            case "Player":
                other.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
                Destroy(gameObject);
                break;
            case "Environment":
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
