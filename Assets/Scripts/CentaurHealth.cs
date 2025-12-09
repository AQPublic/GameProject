using UnityEngine;

public class CentaurHealth : MonoBehaviour
{
    public int maxHealth = 200;
    public int currentHealth;

    public Animator animator;
    public CentaurAI ai;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;

        ai.PlayHitSound();

        if (currentHealth <= 0)
        {
            ai.Die();
        }
    }
}
