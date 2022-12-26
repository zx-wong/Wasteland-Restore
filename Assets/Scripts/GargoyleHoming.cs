using UnityEngine;

public class GargoyleHoming : MonoBehaviour
{
    [SerializeField] private int health;
    [SerializeField] private int damage;

    public void HandleHealth(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Trigger");
        switch (other.gameObject.tag)
        {
            case "Player":
                other.transform.gameObject.GetComponentInChildren<PlayerHealth>().TakeDamage(damage);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
