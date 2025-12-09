using UnityEngine;
using UnityEngine.AI;

public class CentaurAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip stepSound;   // galloping loop
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip idleGrunt;

    [Header("AI Settings")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float timeBetweenAttacks = 2f;

    private bool alreadyAttacked = false;
    private bool isMovingSoundPlaying = false;
    private bool isDead = false;

    private void Start()
    {
        if (player == null)
        {
            Debug.LogError("Player reference missing on Centaur!", this);
        }
    }

    private void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        // Movement Sound
        HandleMovementSound();

        // Movement + Attack logic
        if (distance <= attackRange)
        {
            AttackPlayer();
        }
        else if (distance <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Idle();
        }
    }

    // -------------------------------
    // MOVEMENT & ANIMATION
    // -------------------------------
    private void ChasePlayer()
    {
        if (isDead) return;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void Idle()
    {
        if (isDead) return;

        if (!agent.isStopped)
            agent.isStopped = true;
        animator.SetFloat("Speed", 0f);

        if (idleGrunt != null && Random.value < 0.002f)
        {
            audioSource.PlayOneShot(idleGrunt);
        }
    }

    // -------------------------------
    // ATTACK LOGIC
    // -------------------------------
    private void AttackPlayer()
    {
        if (alreadyAttacked || isDead) return;

        agent.isStopped = true;
        animator.SetFloat("Speed", 0f);

        // Pick random attack (0,1,2)
        int attackIndex = Random.Range(0, 3);
        animator.SetInteger("AttackIndex", attackIndex);
        animator.SetTrigger("Attack");

        PlayAttackSound();

        alreadyAttacked = true;
        Invoke(nameof(ResetAttack), timeBetweenAttacks);
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    // -------------------------------
    // AUDIO HANDLING
    // -------------------------------
    private void HandleMovementSound()
    {
        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;

        if (isMoving && !isMovingSoundPlaying)
        {
            if (stepSound != null)
            {
                audioSource.clip = stepSound;
                audioSource.loop = true;
                audioSource.Play();
                isMovingSoundPlaying = true;
            }
        }
        else if (!isMoving && isMovingSoundPlaying)
        {
            audioSource.Stop();
            isMovingSoundPlaying = false;
        }
    }

    private void PlayAttackSound()
    {
        if (attackSound != null)
            audioSource.PlayOneShot(attackSound);
    }

    public void PlayHitSound()
    {
        if (hitSound != null)
            audioSource.PlayOneShot(hitSound);
    }

    public void PlayDeathSound()
    {
        if (deathSound != null)
            audioSource.PlayOneShot(deathSound);
    }

    // -------------------------------
    // DAMAGE & DEATH
    // -------------------------------
    public void TakeDamage(int amount)
    {
        if (isDead) return;

        PlayHitSound();
        animator.SetTrigger("Hit");
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        agent.isStopped = true;
        animator.SetTrigger("Die");
        PlayDeathSound();

        Destroy(gameObject, 5f);
    }
}
