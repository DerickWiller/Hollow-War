using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // 1. O Padrão Singleton: Permite que outros scripts o encontrem facilmente.
    public static GameManager Instance;

    [Header("Configurações de Vidas")]
    public int vidasAtuais = 3;

    // Arrays e Sprites para a UI
    public Image[] coracoesUI;
    public Sprite coracaoCheioSprite;
    public Sprite coracaoVazioSprite;

    [Header("Configurações de Game Over")]
    public GameObject painelGameOver;
    public string nomeCenaRespawn = "Overworld";


    void Awake()
    {
        // O Singleton garante que apenas uma instância exista
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(false);
        }
        AtualizarUI();
    }

    // Função principal chamada pelo player quando ele perde a barra de vida
    public void PersonagemMorreu()
    {
        vidasAtuais--;

        // 1. Atualiza a UI de corações
        AtualizarUI();

        // 2. Verifica se o jogo acabou
        if (vidasAtuais <= 0)
        {
            GameOver();
        }
        else
        {
            // Se ainda há vidas, recarrega a cena para "respawnar"
            RespawnPlayer();
        }
    }

    void RespawnPlayer()
    {
        // Recarrega a cena. O player será criado novamente com vida total.
        SceneManager.LoadScene(nomeCenaRespawn);
        SceneManager.LoadScene("Pradaria", LoadSceneMode.Additive);
    }

    void AtualizarUI()
    {
        // Percorre os corações na UI
        for (int i = 0; i < coracoesUI.Length; i++)
        {
            if (i < vidasAtuais)
            {
                // Coração Cheio (mostra que há vida)
                coracoesUI[i].sprite = coracaoCheioSprite;
                coracoesUI[i].enabled = true; // Garante que a imagem esteja ativa
            }
            else
            {
                // Coração Vazio (mostra que a vida foi perdida)
                coracoesUI[i].sprite = coracaoVazioSprite;
                coracoesUI[i].enabled = true;
            }
        }
    }

    void GameOver()
    {
        if (painelGameOver != null)
        {
            painelGameOver.SetActive(true);
            Time.timeScale = 0f; // Pausa o jogo
        }
    }
}