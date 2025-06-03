using UnityEngine;

public class EnemyAttackHitbox : MonoBehaviour
{
    [Header("Hit Settings")]
    public float hitRange = 1.2f;
    public float hitRadius = 0.5f;
    public LayerMask targetMask; // Assign Player layer here

    [Header("Damage Settings")]
    public int damageAmount = 10;

    // This is called from an animation event
    public void PerformHit()
    {
        Vector3 origin = transform.position + Vector3.up * 1f; // chest height
        Vector3 direction = transform.forward;

        Collider[] hits = Physics.OverlapSphere(origin + direction * hitRange * 0.5f, hitRadius, targetMask);

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                hit.GetComponent<HealthSystem>()?.TakeDamage(damageAmount);
                Debug.Log($"{gameObject.name} hit {hit.name}!");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(origin + transform.forward * hitRange * 0.5f, hitRadius);
    }
}
