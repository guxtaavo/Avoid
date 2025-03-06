using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovimentaPlayer : MonoBehaviour
{
    public float velocidadeDoJogador; // Velocidade de movimento do jogador
    public Rigidbody body; // Referência ao Rigidbody da bola
    private float movimentoX; // Controle do movimento no eixo X
    private float minYPosition = -3f; // Posição mínima no eixo Y para encerrar o jogo

    void Update()
    {
        // Movimenta a bola
        MovimentarBola();

        // Verifica se a posição da bola no eixo Y está abaixo do limite
        if (body.transform.position.y < minYPosition)
        {
            EndGame();
        }
    }

    private void MovimentarBola()
    {
        // Captura o movimento no eixo horizontal
        movimentoX = Input.GetAxis("Horizontal") * velocidadeDoJogador;

        // Adiciona força no eixo X para movimentar a bola
        body.AddForce(new Vector3(movimentoX, 0f, 0f), ForceMode.Force);

        // Restringe o movimento no eixo Z para evitar problemas ao rolar para trás
        StabilizeZ();
    }

    private void StabilizeZ()
    {
        // Estabiliza o movimento no eixo Z ao reduzir a velocidade nesse eixo
        Vector3 velocity = body.velocity;
        velocity.z = Mathf.Lerp(velocity.z, 0f, Time.deltaTime * 5f); // Suaviza o movimento
        body.velocity = velocity;
    }

    private void EndGame()
    {
        Debug.Log("Jogo Encerrado!");
        FindObjectOfType<GameManager>().GameOver(); // Chama o painel de Game Over
    }

}
