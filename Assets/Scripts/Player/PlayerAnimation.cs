using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // --- Componentes ---
    private Animator animator;

    // --- Parâmetros do Animator (use strings para evitar erros de digitação) ---
    private readonly int hashMoveX = Animator.StringToHash("moveX");
    private readonly int hashMoveY = Animator.StringToHash("moveY");
    private readonly int hashIsMoving = Animator.StringToHash("isMoving");
    private readonly int hashAttack = Animator.StringToHash("Attack");


    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // É uma boa prática se inscrever nos eventos em OnEnable
    void OnEnable()
    {
        PlayerController.OnMove += HandleMove;
        PlayerController.OnAttack += HandleAttack;
    }

    // E se desinscrever em OnDisable para evitar memory leaks
    void OnDisable()
    {
        PlayerController.OnMove -= HandleMove;
        PlayerController.OnAttack -= HandleAttack;
    }

    // Este método é chamado pelo evento OnMove
    private void HandleMove(Vector2 direction)
    {
        // Se o player está se movendo, atualiza a direção na Blend Tree
        if (direction.magnitude > 0)
        {
            animator.SetBool(hashIsMoving, true);
            animator.SetFloat(hashMoveX, direction.x);
            animator.SetFloat(hashMoveY, direction.y);
        }
        else
        {
            // Se parou, desativa o booleano de movimento
            animator.SetBool(hashIsMoving, false);
        }
    }

    // Este método é chamado pelo evento OnAttack
    private void HandleAttack(Vector2 attackDirection)
    {
        // Antes de atacar, garante que a Blend Tree está apontando para a direção certa
        animator.SetFloat(hashMoveX, attackDirection.x);
        animator.SetFloat(hashMoveY, attackDirection.y);
        
        // Dispara o gatilho de ataque
        animator.SetTrigger(hashAttack);
    }
}