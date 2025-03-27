using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentaPlayer : MonoBehaviour
{
    public float velocidadeDoJogador; // Velocidade de movimento do jogador
    public Rigidbody body; // Referência ao Rigidbody da bola
    private float movimentoX; // Controle do movimento no eixo X

    private float minYPosition = -2f; // Posição mínima no eixo Y para encerrar o jogo
    private bool isGameOver = false; // Para evitar chamadas repetidas

    void Update()
    {
        if (!isGameOver)
        {
            MovimentarBola();

            if (body.transform.position.y < minYPosition)
            {
                EndGame();
            }
        }
    }

    private void MovimentarBola()
    {
        movimentoX = 0f;

        if (Leitor.Instance != null)
        {
            bool? direction = Leitor.Instance.move;

            if (direction == true)
            {
                movimentoX = 1f;
            }
            else if (direction == false)
            {
                movimentoX = -1f;
            }
            else
            {
                movimentoX = 0f;
            }
        }

        body.AddForce(new Vector3(movimentoX * velocidadeDoJogador, 0f, 0f), ForceMode.VelocityChange);

        StabilizeZ();
    }

    private void StabilizeZ()
    {
        Vector3 velocity = body.velocity;
        velocity.z = Mathf.Lerp(velocity.z, 0f, Time.deltaTime * 5f);
        body.velocity = velocity;
    }

    private void EndGame()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            //Debug.Log("Jogo Encerrado!");
            FindObjectOfType<GameManager>().GameOver();
        }
    }
}