using UnityEngine;

public class MinotaurHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;
    public Animator animator;
    public MinotaurAI ai;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        animator.SetTrigger("Hit");
        ai.PlayHitSound();

        if (currentHealth <= 0)
        {
            ai.Die();
        }
    }
}
