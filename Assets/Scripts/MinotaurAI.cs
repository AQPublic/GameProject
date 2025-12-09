using UnityEngine;
using UnityEngine.AI;

public class MinotaurAI : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public NavMeshAgent agent;
    public Animator animator;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip stepSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip idleGrunt;

    [Header("AI Settings")]
    public float detectionRange = 15f;
    public float attackRange = 3f;
    public float timeBetweenAttacks = 2f;

    private bool alreadyAttacked = false;
    private bool isDead = false;
    private bool isPlayingStepLoop = false;
    private float distanceToPlayer;

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
            AttackPlayer();
        else if (distanceToPlayer <= detectionRange)
            ChasePlayer();
        else
            IdleState();

        // Update animator Speed for movement blend
        animator.SetFloat("Speed", agent.velocity.magnitude);
        HandleMovementSounds();
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
    }

    private void IdleState()
    {
        agent.SetDestination(transform.position);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        if (!alreadyAttacked)
        {
            transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

            // Play attack animation using AttackIndex
            int randomAttack = Random.Range(0, 3);
            animator.SetInteger("AttackIndex", randomAttack);

            PlayAttackSound();

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
        animator.SetInteger("AttackIndex", -1);
    }

    // ========== AUDIO ==========
    private void HandleMovementSounds()
    {
        bool isMoving = agent.velocity.magnitude > 0.2f;

        if (isMoving && !isPlayingStepLoop)
        {
            isPlayingStepLoop = true;
            audioSource.loop = true;
            audioSource.clip = stepSound;
            audioSource.Play();
        }
        else if (!isMoving && isPlayingStepLoop)
        {
            isPlayingStepLoop = false;
            audioSource.Stop();
        }
    }

    public void PlayHitSound()
    {
        if (hitSound)
            audioSource.PlayOneShot(hitSound);
    }

    public void PlayAttackSound()
    {
        if (attackSound)
            audioSource.PlayOneShot(attackSound);
    }

    public void PlayIdleGrunt()
    {
        if (idleGrunt)
            audioSource.PlayOneShot(idleGrunt);
    }

    public void PlayDeathSound()
    {
        if (deathSound)
            audioSource.PlayOneShot(deathSound);
    }

    // Called by MinotaurHealth
    public void Die()
    {
        if (isDead) return;
        isDead = true;

        agent.isStopped = true;
        animator.SetTrigger("Die");

        PlayDeathSound();

        Destroy(gameObject, 4f);
    }
}
