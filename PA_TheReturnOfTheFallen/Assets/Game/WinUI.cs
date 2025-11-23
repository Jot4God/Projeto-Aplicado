using UnityEngine;
using UnityEngine.SceneManagement;

public class WinUI : MonoBehaviour
{
    [Header("Referências da UI")]
    public GameObject winPanel; // Painel da vitória que aparece ao ganhar

    void Start()
    {
        if (winPanel != null)
            winPanel.SetActive(false); // começa escondido
    }

    // Chamar esta função quando o jogador ganhar
    public void ShowWin()
    {
        if (winPanel != null)
            winPanel.SetActive(true);
    }

    // Recomeçar o jogo
    public void RestartGame()
    {
        SceneManager.LoadScene("TestArea"); // mesma cena do jogo
    }

    // Voltar ao Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
