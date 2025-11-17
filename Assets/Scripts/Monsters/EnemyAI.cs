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
    
    // Componentes
    private Transform currentTarget; 
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    
    // Variáveis de Estado
    private Vector2 lastDirection = Vector2.down; 
    private Color originalColor;
    private bool canAttack = true;

    // State Machine
    private enum State { Idle, Chasing, CombatIdle, Attacking, Feedback }
    private State currentState;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
        
        // Tenta configurar o raio da DetectionZone automaticamente se ela existir
        var zone = transform.Find("DetectionZone");
        if(zone != null) 
        {
            var col = zone.GetComponent<CircleCollider2D>();
            if(col != null) col.radius = stats.detectionRadius;
        }
    }

    void Start()
    {
        currentState = State.Idle;
    }

    void Update()
    {
        // 1. Bloqueio de Ações: Se estiver atacando ou sofrendo knockback, não processa movimento nem decisão.
        if (currentState == State.Attacking || currentState == State.Feedback) return;

        // 2. Sem alvo -> Estado Idle
        if (currentTarget == null)
        {
            if (currentState != State.Idle) EnterIdleState();
            return;
        }

        // 3. Lógica de Combate e Movimento
        float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);

        // Verifica se está dentro do raio de ataque
        if (distanceToTarget <= stats.attackRadius)
        {
            if (canAttack)
            {
                // Está perto e pode atacar -> INICIA ATAQUE
                StartCoroutine(AttackSequence());
            }
            else
            {
                // Está perto mas em Cooldown -> COMBAT IDLE (Encara o player mas fica parado)
                EnterCombatIdleState();
            }
        }
        else
        {
            // Está longe -> PERSEGUE
            ChaseTarget();
        }
    }

    // --- MÉTODOS DE ESTADO ---

    private void EnterIdleState()
    {
        currentState = State.Idle;
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }

    private void EnterCombatIdleState()
    {
        // Se já estiver nesse estado, não faz nada para economizar processamento
        if (currentState == State.CombatIdle) return;

        currentState = State.CombatIdle;
        rb.linearVelocity = Vector2.zero; // Garante parada total
        animator.SetBool("isMoving", false);
        
        // Opcional: Virar para o player mesmo parado
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        UpdateAnimationFacing(direction); 
    }

    private void ChaseTarget()
    {
        currentState = State.Chasing;
        
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        rb.linearVelocity = direction * stats.moveSpeed;
        
        animator.SetBool("isMoving", true);
        UpdateAnimationFacing(direction);
    }

    // --- SEQUÊNCIA DE ATAQUE ---

    private IEnumerator AttackSequence()
    {
        currentState = State.Attacking;
        canAttack = false;
        
        // 1. Trava Movimento e Animação
        rb.linearVelocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        // 2. Define direção e inicia animação
        Vector2 direction = (currentTarget.position - transform.position).normalized;
        UpdateAnimationFacing(direction);
        animator.SetTrigger("Attack");

        // 3. Windup (Delay antes do dano)
        yield return new WaitForSeconds(stats.attackHitboxDelay);

        // 4. Ativa Hitbox
        GameObject hitboxToActivate = GetHitboxForDirection(direction);
        var hitboxScript = hitboxToActivate.GetComponent<EnemyHitbox>();
        if(hitboxScript) hitboxScript.damage = stats.attackDamage;
        
        hitboxToActivate.SetActive(true);

        // 5. Duração da Hitbox Ativa
        yield return new WaitForSeconds(stats.attackHitboxActiveTime);
        hitboxToActivate.SetActive(false);

        // 6. RECOVERY (Novo): Tempo parado respirando após o ataque
        // O estado continua 'Attacking', impedindo o Update de mover o personagem.
        yield return new WaitForSeconds(stats.attackRecovery);

        // 7. Libera a IA para perseguir ou decidir o próximo passo
        currentState = State.Chasing; 

        // 8. Cooldown (Tempo até poder atacar DE NOVO)
        yield return new WaitForSeconds(stats.attackCooldown);
        canAttack = true;
    }

    // --- MÉTODOS AUXILIARES E INTERFACE PÚBLICA ---

    public void SetTarget(Transform target) { currentTarget = target; }
    public void ClearTarget() { currentTarget = null; }

    private void UpdateAnimationFacing(Vector2 direction)
    {
        lastDirection = direction;
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
        
        // Aplica força
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockbackDirection * stats.knockbackForce, ForceMode2D.Impulse);

        // Flash Branco
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(stats.flashDuration);
        spriteRenderer.color = originalColor;

        // Espera o resto do knockback
        yield return new WaitForSeconds(stats.knockbackDuration - stats.flashDuration);

        rb.linearVelocity = Vector2.zero; 
        
        // Retorna ao combate
        if(currentTarget != null)
            currentState = State.Chasing;
        else
            currentState = State.Idle;
    }

    private GameObject GetHitboxForDirection(Vector2 direction)
    {
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            return direction.x > 0 ? hitboxE : hitboxW;
        else
            return direction.y > 0 ? hitboxN : hitboxS;
    }

    // DEBUG VISUAL ATUALIZADO
    void OnDrawGizmosSelected()
    {
        if(stats != null)
        {
            // Vermelho = Alcance de Ataque (Onde ele para de andar e bate)
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, stats.attackRadius);
        }
    }
}