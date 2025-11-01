using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private MonsterStats stats;

    [Header("Hitbox References")]
    [SerializeField] private GameObject hitboxN;
    [SerializeField] private GameObject hitboxS;
    [SerializeField] private GameObject hitboxE;
    [SerializeField] private GameObject hitboxW;
    
    // Componentes e Variáveis
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector2 lastDirection = Vector2.down; // Começa olhando para Sul
    private Color originalColor;
    private bool canAttack = true;

    private enum State { Idle, Chasing, Attacking, Feedback }
    private State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        
        // Configura o raio da zona de detecção
        transform.Find("DetectionZone").GetComponent<CircleCollider2D>().radius = stats.detectionRadius;
    }

    void Start()
    {
        // Encontra o player pela tag do seu Hurtbox
        GameObject playerHurtbox = GameObject.FindGameObjectWithTag("Player");
        if(playerHurtbox != null)
        {
            player = playerHurtbox.transform.parent; // Pega o objeto pai (o Player em si)
        }
        currentState = State.Idle;
    }

    void Update()
    {
        if (player == null || currentState == State.Attacking || currentState == State.Feedback)
        {
            animator.SetBool("isMoving", false);
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (currentState == State.Chasing)
        {
            if (distanceToPlayer <= stats.attackRadius && canAttack)
            {
                StartCoroutine(Attack());
            }
            else
            {
                ChasePlayer();
            }
        }
    }

    public void OnPlayerEnterDetectionZone()
    {
        if (currentState == State.Idle)
        {
            currentState = State.Chasing;
        }
    }

    public void OnPlayerExitDetectionZone()
    {
        if (currentState == State.Chasing)
        {
            currentState = State.Idle;
            rb.linearVelocity = Vector2.zero;
            animator.SetBool("isMoving", false);
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.moveSpeed;
        UpdateAnimation(direction);
    }

    private IEnumerator Attack()
    {
        currentState = State.Attacking;
        canAttack = false;
        rb.linearVelocity = Vector2.zero;
        
        Vector2 direction = (player.position - transform.position).normalized;
        UpdateAnimation(direction);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(stats.attackHitboxDelay);

        GameObject hitboxToActivate = GetHitboxForDirection(direction);
        hitboxToActivate.GetComponent<EnemyHitbox>().damage = stats.attackDamage;
        hitboxToActivate.SetActive(true);

        yield return new WaitForSeconds(stats.attackHitboxActiveTime);

        hitboxToActivate.SetActive(false);
        currentState = State.Chasing;

        yield return new WaitForSeconds(stats.attackCooldown);
        canAttack = true;
    }

    private void UpdateAnimation(Vector2 direction)
    {
        animator.SetBool("isMoving", direction.magnitude > 0);
        lastDirection = direction;
        // Para uma Blend Tree cardinal, é melhor passar valores "puros"
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            animator.SetFloat("moveX", direction.x > 0 ? 1 : -1);
            animator.SetFloat("moveY", 0);
        }
        else
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", direction.y > 0 ? 1 : -1);
        }
    }
    
    public void TriggerFeedback(Vector2 knockbackDirection)
    {
        StartCoroutine(FeedbackCoroutine(knockbackDirection));
    }

    private IEnumerator FeedbackCoroutine(Vector2 knockbackDirection)
    {
        currentState = State.Feedback;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * stats.knockbackForce, ForceMode2D.Impulse);

        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(stats.flashDuration);
        spriteRenderer.color = originalColor;

        yield return new WaitForSeconds(stats.knockbackDuration - stats.flashDuration);

        rb.linearVelocity = Vector2.zero; // Para o knockback abruptamente
        currentState = State.Chasing;
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            return direction.x > 0 ? hitboxE : hitboxW;
        }
        else
        {
            return direction.y > 0 ? hitboxN : hitboxS;
        }
    }
}