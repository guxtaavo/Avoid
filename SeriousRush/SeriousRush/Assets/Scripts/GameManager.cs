using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Referência ao painel de Game Over

    public void GameOver()
    {
        Debug.Log("GameOver chamado");
        gameOverPanel.SetActive(true);
        //Time.timeScale = 0f; // Pausa o jogo
    }

    public void RestartGame()
    {
        Debug.Log("RestartGame foi chamado!");
        gameOverPanel.SetActive(false);
        Time.timeScale = 1f;
        SceneManager.LoadScene("SampleScene");
    }
}
