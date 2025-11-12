using UnityEngine;
using UnityEngine.SceneManagement;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    public Transform healthBar; // barra verde
    public GameObject healthBarObject; // objeto pai das barras


    private Vector3 healthBarScale;
    private float healthPercent;
    void Start()
    {
        currentHealth = maxHealth;
        healthBarScale = healthBar.localScale;
        healthPercent = healthBarScale.x / currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        UpdateHealthbar();
        Debug.Log(gameObject.name + " tomou " + damage + " de dano. Vida restante: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name + " morreu.");
        // Por enquanto, vamos apenas desativar o objeto.
        // Mais tarde, você pode adicionar animações de morte, efeitos, etc.

        gameObject.SetActive(false);


        // 2. Notifica o Game Manager que o personagem perdeu uma vida
        if (GameManager.Instance != null)
        {
            GameManager.Instance.PersonagemMorreu();
            
        }
        else
        {
            // Caso de erro: se não encontrar o Game Manager, volte ao comportamento anterior
            SceneManager.LoadScene("Overworld");
            SceneManager.LoadScene("Pradaria", LoadSceneMode.Additive);
        }

        
        
    }


    void UpdateHealthbar()
    {
        healthBarScale.x = healthPercent*currentHealth;
        healthBar.localScale = healthBarScale;
    }
}