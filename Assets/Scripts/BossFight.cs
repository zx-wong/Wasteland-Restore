using UnityEngine;
using UnityEngine.UI;

public class BossFight : MonoBehaviour
{
    [SerializeField] GargoyleController gargoyleController;

    [SerializeField] GameObject bossFightCollider;

    [SerializeField] Slider bossHealthSlider;

    private void Start()
    {
        gargoyleController = GetComponent<GargoyleController>();

        bossHealthSlider.maxValue = gargoyleController.maxHealth;

        bossHealthSlider.gameObject.SetActive(false);
        bossFightCollider.SetActive(false);
    }

    private void Update()
    {
        bossHealthSlider.value = gargoyleController.health;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            bossHealthSlider.gameObject.SetActive(true);
            bossFightCollider.SetActive(true);
        }
    }
}
