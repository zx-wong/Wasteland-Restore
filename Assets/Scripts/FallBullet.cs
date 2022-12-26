using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBullet : MonoBehaviour
{
    [SerializeField] private int damage;

    new Rigidbody rigidbody;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            //Debug.Log(collision.gameObject.name);
            collision.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Environment")
        {
            Destroy(gameObject);
        }
    }
}
