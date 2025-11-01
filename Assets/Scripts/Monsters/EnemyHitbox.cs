using UnityEngine;

public class EnemyHitbox : MonoBehaviour
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // A hitbox do inimigo colidiu com o Hurtbox do Player, que tem a tag "Player"
        if (other.CompareTag("Player"))
        {
            // O script Health.cs está no objeto pai do Hurtbox
            Health playerHealth = other.GetComponentInParent<Health>();
            
            if (playerHealth != null)
            {
                // Linha descomentada e funcionando!
                playerHealth.TakeDamage(damage);
            }
        }
    }
}