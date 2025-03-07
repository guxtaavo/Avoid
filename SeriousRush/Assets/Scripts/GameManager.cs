using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GameObject gameOverPanel; // Refer�ncia ao painel de Game Over
    public TMP_Text ScoreText;
    private int score;
    private float timer;
    public static GameManager Instance { get; private set; }
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

    private void UpdateScore()
    {
        int scorePerSeconds = 10;
        timer += Time.deltaTime;
        score = (int)(timer * scorePerSeconds);
        ScoreText.text = string.Format("{0:00000}", score);
    }

    private void Update()
    {
        UpdateScore();
    }
}
