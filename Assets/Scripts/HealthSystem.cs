using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthSystem : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public string hitAnimationTrigger = "Impact";
    public string deathAnimationTrigger = "Die";
    public float dieDelay = 3.5f;

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

        // Use enemy script if available
        if (TryGetComponent<SimpleEnemyAI>(out var enemyAI))
        {
            enemyAI.Die();
            yield break; // Stop coroutine; enemyAI handles destruction
        }

        // Fallback: handle self-destruction
        if (animator && !string.IsNullOrEmpty(deathAnimationTrigger))
            animator.SetTrigger(deathAnimationTrigger);

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
