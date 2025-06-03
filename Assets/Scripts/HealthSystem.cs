using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public string hitAnimationTrigger = "Impact";
    public string deathAnimationTrigger = "Die";
    public float dieDelay = 5f;

    public Image healthFillImage;

    private Animator animator;
    public bool isDead { get; private set; } = false;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateHealthBar();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Current health: {currentHealth}");

        if (animator && !string.IsNullOrEmpty(hitAnimationTrigger))
        {
            animator.SetTrigger(hitAnimationTrigger);
        }

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            StartCoroutine(Die());
        }
    }

    IEnumerator Die()
    {
        isDead = true;

        Debug.LogWarning($"{gameObject.name} is dying...");

        if (animator && !string.IsNullOrEmpty(deathAnimationTrigger))
        {
            animator.SetTrigger(deathAnimationTrigger);
        }

        // Optionally disable collider or navagent here
        Collider col = GetComponent<Collider>();
        if (col) col.enabled = false;

        // Optional: disable Rigidbody physics
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb) rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(dieDelay);

        Destroy(gameObject);
    }

    void UpdateHealthBar()
    {
        if (healthFillImage != null)
        {
            healthFillImage.fillAmount = Mathf.Clamp01((float)currentHealth / maxHealth);
        }
    }
}
