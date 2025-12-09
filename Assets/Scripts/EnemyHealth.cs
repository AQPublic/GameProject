using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Animator animator;

    void Start()
    {
        currentHealth = maxHealth;

        // Auto-assign animator if not set
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        // Play hit animation
        animator.SetTrigger("Hit");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        animator.SetTrigger("Die");

        // Disable AI movement
        GetComponent<EnemyAI>().enabled = false;

        // Disable NavMeshAgent so it stops sliding
        var agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent) agent.enabled = false;

        // Optional: destroy after animation
        Destroy(gameObject, 3f);
    }
}
