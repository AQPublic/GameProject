using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum State { Idle, Chase, Attack }
    public State currentState = State.Idle;

    [Header("References")]
    public NavMeshAgent agent;
    public Animator animator;
    public Transform player;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip attackSound;
    public AudioClip stepSound;
    public AudioClip hitSound;
    public AudioClip deathSound;
    public AudioClip idleGrunt;

    [Header("Stats")]
    public float maxHealth = 100;
    public float currentHealth;
    public float sightRange = 12f;
    public float attackRange = 2.2f;
    public float timeBetweenAttacks = 1.5f;
    public float attackDamage = 15f;

    private float attackCooldown;
    private bool isDead = false;
    private bool stepLoopPlaying = false;

    void Start()
    {
        if (agent == null) agent = GetComponent<NavMeshAgent>();
        if (animator == null) animator = GetComponent<Animator>();
        if (player == null) player = GameObject.FindWithTag("Player").transform;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();

        currentHealth = maxHealth;
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                animator.SetFloat("Speed", 0f);
                HandleStepAudio(false);

                if (distance < sightRange)
                    currentState = State.Chase;
                break;

            case State.Chase:
                if (distance > sightRange)
                {
                    currentState = State.Idle;
                    break;
                }

                if (distance <= attackRange)
                {
                    currentState = State.Attack;
                    agent.ResetPath();
                    break;
                }

                agent.isStopped = false;
                agent.SetDestination(player.position);

                animator.SetFloat("Speed", agent.velocity.magnitude);
                HandleStepAudio(agent.velocity.magnitude > 0.2f);
                break;

            case State.Attack:
                agent.isStopped = true;
                animator.SetFloat("Speed", 0f);
                HandleStepAudio(false);

                if (distance > attackRange + 0.5f)
                {
                    currentState = State.Chase;
                    break;
                }

                transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));

                if (attackCooldown <= 0f)
                {
                    PlayRandomAttack();
                    attackCooldown = timeBetweenAttacks;
                }
                else
                {
                    attackCooldown -= Time.deltaTime;
                }
                break;
        }
    }

    // -------------------------
    // ATTACK LOGIC
    // -------------------------
    void PlayRandomAttack()
    {
        int index = Random.Range(1, 4); // 1, 2, or 3

        switch (index)
        {
            case 1: animator.SetTrigger("Attack01"); break;
            case 2: animator.SetTrigger("Attack02"); break;
            case 3: animator.SetTrigger("Attack03"); break;
        }

        PlayAttackSound();
    }

    // -------------------------
    // HEALTH SYSTEM
    // -------------------------
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;

        animator.SetTrigger("Hit");
        PlayHitSound();

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        agent.isStopped = true;

        animator.SetTrigger("Die");
        PlayDeathSound();

        Destroy(gameObject, 4f);
    }

    // -------------------------
    // AUDIO
    // -------------------------
    void HandleStepAudio(bool walking)
    {
        if (walking && !stepLoopPlaying)
        {
            stepLoopPlaying = true;
            audioSource.loop = true;
            audioSource.clip = stepSound;
            audioSource.Play();
        }
        else if (!walking && stepLoopPlaying)
        {
            stepLoopPlaying = false;
            audioSource.Stop();
        }
    }

    void PlayAttackSound()
    {
        if (attackSound)
            audioSource.PlayOneShot(attackSound);
    }

    void PlayHitSound()
    {
        if (hitSound)
            audioSource.PlayOneShot(hitSound);
    }

    void PlayDeathSound()
    {
        if (deathSound)
            audioSource.PlayOneShot(deathSound);
    }
}
