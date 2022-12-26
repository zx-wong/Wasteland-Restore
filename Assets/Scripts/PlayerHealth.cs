using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float maxArmor = 50f;
    [SerializeField] public float health;
    [SerializeField] public float armor;

    private bool gotArmor;
    public bool healthFull;
    public bool armorFull;

    private float lerpTimer;
    private float chipSpeed = 2f;

    [Header("UI")]
    [SerializeField] private Image frontHealthBar;
    [SerializeField] private Image backHealthBar;
    [SerializeField] private Text healthText;

    [SerializeField] private GameObject armorBar;
    [SerializeField] private Image frontArmorBar;
    [SerializeField] private Image backArmorBar;
    [SerializeField] private Text armorText;

    [SerializeField] private RawImage blood;

    private void Start()
    {
        health = maxHealth;
        //armor = maxArmor;

        blood.gameObject.SetActive(true);
    }

    private void Update()
    {
        health = Mathf.Clamp(health, 0, maxHealth);
        armor = Mathf.Clamp(armor, 0, maxArmor);
        
        UpdateHealthUI();
        UpdateArmorUI();
        UpdateText();
        UpdateBlood();

        if (health <= 0)
        {
            Invoke("LoseScene", .3f);
            Cursor.lockState = CursorLockMode.None;
        }

        if (health == maxHealth)
        {
            healthFull = true;
        }
        else
        {
            healthFull = false;
        }

        if (armor == maxArmor)
        {
            armorFull = true;
        }
        else
        {
            armorFull = false;
        }

        if (armor > 0)
        {
            armorBar.SetActive(true);
            gotArmor = true;
        }
        else
        {
            armorBar.SetActive(false);
            gotArmor = false;
        }
    }

    public void LoseScene()
    {
        SceneManager.LoadScene("LoseScene");
    }

    public void WinScene()
    {
        SceneManager.LoadScene("WinScene");
    }

    public void UpdateHealthUI()
    {
        //Debug.Log(health);
        float fillFront = frontHealthBar.fillAmount;
        float fillBack = backHealthBar.fillAmount;

        float fraction = health / maxHealth;

        if (fillBack > fraction)
        {
            frontHealthBar.fillAmount = fraction;
            lerpTimer += Time.deltaTime;

            if (!gotArmor)
            {
                backHealthBar.color = Color.gray;
            }

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backHealthBar.fillAmount = Mathf.Lerp(fillBack, fraction, percentComplete);
        }

        if (fillFront < fraction)
        {
            backHealthBar.fillAmount = fraction;
            lerpTimer += Time.deltaTime;

            if (!gotArmor)
            {
                backHealthBar.color = Color.green;
            }

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontHealthBar.fillAmount = Mathf.Lerp(fillFront, fraction, percentComplete);
        }
    }

    public void UpdateArmorUI()
    {
        //Debug.Log(health);
        float fillFront = frontArmorBar.fillAmount;
        float fillBack = backArmorBar.fillAmount;

        float fraction = armor / maxArmor;

        if (fillBack > fraction)
        {
            frontArmorBar.fillAmount = fraction;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            backArmorBar.fillAmount = Mathf.Lerp(fillBack, fraction, percentComplete);
        }

        if (fillFront < fraction)
        {
            backArmorBar.fillAmount = fraction;
            lerpTimer += Time.deltaTime;

            float percentComplete = lerpTimer / chipSpeed;
            percentComplete = percentComplete * percentComplete;
            frontArmorBar.fillAmount = Mathf.Lerp(fillFront, fraction, percentComplete);
        }
    }

    public void UpdateText()
    {
        healthText.text = health.ToString();
        armorText.text = armor.ToString();
    }

    public void UpdateBlood()
    {
        if (health > 50)
        {
            blood.color = new Color(1f, 1f, 1f, 0.0f);
        }
        else if (health > 20 && health < 50)
        {
            blood.color = new Color(1f, 1f, 1f, 0.20f);
        }
        else if (health < 20)
        {
            blood.color = new Color(1f, 1f, 1f, 0.2f + ((0.8f / 20f) * (20f - health)));
        }
    }

    public void TakeDamage(float damage)
    {
        if (gotArmor)
        {
            armor -= damage;
            lerpTimer = 0f;
        }
        else
        {
            health -= damage;
            lerpTimer = 0f;
        }
    }
    
    public void RestoreHealth(float restoreHealth)
    {
        health += restoreHealth;
        lerpTimer = 0f;
    }

    public void RestoreArmor(float restoreArmor)
    {
        armor += restoreArmor;
        lerpTimer = 0f;
    }
}
