using UnityEngine;

[CreateAssetMenu(fileName = "NewPlayerStats", menuName = "Stats/Player Stats")]
public class PlayerStats : ScriptableObject
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Health")]
    public int maxHealth = 20;

    [Header("Combat")]
    public int attackDamage = 1;
    [Tooltip("O delay em segundos desde o início do ataque até a hitbox ser ativada.")]
    public float attackHitboxDelay = 0.1f;
    [Tooltip("Por quanto tempo a hitbox permanecerá ativa em segundos.")]
    public float attackHitboxActiveTime = 0.15f;
    [Tooltip("A duração total da animação de ataque. Deve ser maior que a soma do delay e do tempo ativo.")]
    public float attackAnimationDuration = 0.36f;
}