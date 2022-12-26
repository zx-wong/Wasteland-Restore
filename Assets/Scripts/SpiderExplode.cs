using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderExplode : MonoBehaviour
{
    [SerializeField] private int damage;

    [SerializeField] private GameObject explodeParticle;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
            gameObject.GetComponentInParent<SpiderController>().HandleHealth(20);
            Instantiate(explodeParticle, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
