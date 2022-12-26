using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamCollider : MonoBehaviour
{
    private ParticleSystem particle;

    void Start()
    {
        particle = GetComponent<ParticleSystem>();
    }

    void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);

        if (other.gameObject.tag == "Player")
        {
            other.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(1);
        }
    }
}

