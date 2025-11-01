using UnityEngine;

[CreateAssetMenu(fileName = "NewMonsterStats", menuName = "Stats/Monster Stats")]
public class MonsterStats : ScriptableObject
{
    [Header("Base Stats")]
    public int maxHealth = 3;
    public float moveSpeed = 2f;
    public int attackDamage = 1;

    [Header("AI Behavior")]
    [Tooltip("O raio da zona onde o inimigo detecta o player.")]
    public float detectionRadius = 10f;
    [Tooltip("A distância que o inimigo tenta manter do player para atacar.")]
    public float attackRadius = 1.5f;
    [Tooltip("Tempo de espera entre os ataques em segundos.")]
    public float attackCooldown = 2f;

    [Header("Combat Timings")]
    [Tooltip("O delay em segundos desde o início do ataque até a hitbox ser ativada.")]
    public float attackHitboxDelay = 0.2f;
    [Tooltip("Por quanto tempo a hitbox permanecerá ativa em segundos.")]
    public float attackHitboxActiveTime = 0.3f;

    [Header("Feedback on Hit")]
    public float knockbackForce = 5f;
    public float knockbackDuration = 0.2f;
    public float flashDuration = 0.1f;
}