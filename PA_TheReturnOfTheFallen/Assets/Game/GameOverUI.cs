using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour
{
    // Recomeçar o jogo
    public void RestartGame()
    {
        SceneManager.LoadScene("TestArea"); 
    }

    // Voltar ao Main Menu
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu"); 
    }
}
