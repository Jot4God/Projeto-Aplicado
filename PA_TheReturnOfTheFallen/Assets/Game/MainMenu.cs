using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Função chamada pelo botão "Start Game"
    public void StartGame()
    {
        SceneManager.LoadScene("TestArea"); // mesma cena que o jogo
    }

    // Função chamada pelo botão "Quit Game" (opcional)
    public void QuitGame()
    {
        Debug.Log("Sair do jogo");
        Application.Quit(); // fecha o jogo
    }
}
