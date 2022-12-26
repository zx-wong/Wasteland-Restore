using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] private ParticleSystem fireballParticle;

    [SerializeField] private int damage;
    [SerializeField] private float firerate;
    [SerializeField] private bool fireballCooldown;
    [SerializeField] public bool reset;

    private List<ParticleCollisionEvent> particleCollisionEvents;

    void Start()
    {
        fireballParticle = GetComponent<ParticleSystem>();

        particleCollisionEvents = new List<ParticleCollisionEvent>();

        reset = false;
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(fireballParticle, other, particleCollisionEvents);

        for (int i = 0; i < particleCollisionEvents.Count; i++)
        {
            var collider = particleCollisionEvents[i].colliderComponent;

            if (collider.CompareTag("Player"))
            {
                var playerHealth = GetComponent<PlayerHealth>();
                playerHealth.TakeDamage(damage);
            }
        }
    }

    public void FireballAttack()
    {
        if (fireballCooldown)
        {
            return;
        }

        fireballParticle.Emit(1);
        fireballCooldown = true;
        reset = true;
        StartCoroutine(ResetCooldown());
    }

    IEnumerator ResetCooldown()
    {
        yield return new WaitForSeconds(firerate);
        fireballCooldown = false;
        reset = false;
    }
}
