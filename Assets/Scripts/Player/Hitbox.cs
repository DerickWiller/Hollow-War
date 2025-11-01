using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public int damage; // O PlayerController continua nos dizendo quanto dano causar

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Verifica se colidiu com o Hurtbox de um inimigo
        if (other.CompareTag("Enemy"))
        {
            // O script de vida do inimigo está no objeto pai do Hurtbox
            EnemyHealth enemyHealth = other.GetComponentInParent<EnemyHealth>();
            
            if (enemyHealth != null)
            {
                // 1. Calcula a direção do knockback
                // A direção é um vetor que aponta do centro da hitbox (próximo ao player)
                // para o centro do Hurtbox do inimigo.
                Vector2 knockbackDirection = (other.transform.position - transform.position).normalized;

                // 2. Chama a função TakeDamage do inimigo, passando o dano E a direção do knockback
                enemyHealth.TakeDamage(damage, knockbackDirection);
            }
        }
    }
}