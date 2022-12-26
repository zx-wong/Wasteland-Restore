using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage;

    private void Start()
    {
        Invoke("DestroyObject", 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject, .1f);
        }
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
