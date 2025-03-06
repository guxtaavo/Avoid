using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [Header("Configurações de Spawn")]
    public GameObject[] obstacles; // Array de obstáculos (Prefabs)
    public Transform obstacleParent; // Parent para organizar os obstáculos na Hierarquia
    public float spawnIntervalMin = 0.9f;
    public float spawnIntervalMax = 1.9f;

    [Header("Configurações do Caminho")]
    public float pathWidth = 9f; // Largura total do caminho (ex.: 4.5 para cada lado)
    public float obstacleSpeed = 8f; // Velocidade dos obstáculos
    public float minGapBetweenObstacles = 2f; // Espaço mínimo entre obstáculos

    private float halfPathWidth;

    void Start()
    {
        halfPathWidth = pathWidth / 2f;
        StartCoroutine(SpawnObstacle());
    }

    private IEnumerator SpawnObstacle()
    {
        while (true)
        {
            float spawnDelay = Random.Range(spawnIntervalMin, spawnIntervalMax);
            GenerateObstacles();
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void GenerateObstacles()
    {
        // Lista de posições ocupadas na linha do spawn
        List<Vector2> occupiedRanges = new List<Vector2>();

        // Decide quantos obstáculos spawnar (1 ou 2) para a linha
        int obstacleCount = Random.Range(1, 3);

        for (int i = 0; i < obstacleCount; i++)
        {
            // Escolhe um obstáculo aleatório
            int randomIndex = Random.Range(0, obstacles.Length);
            GameObject selectedObstacle = obstacles[randomIndex];

            // Obtém o tamanho do obstáculo no eixo X
            float obstacleWidth = selectedObstacle.transform.localScale.x;

            // Tenta encontrar uma posição válida
            float spawnX;
            int attempts = 0;
            do
            {
                spawnX = Random.Range(-halfPathWidth + obstacleWidth / 2, halfPathWidth - obstacleWidth / 2);
                attempts++;
            } while (!IsPositionValid(spawnX, obstacleWidth, occupiedRanges) && attempts < 10);

            if (attempts < 10)
            {
                // Instancia o obstáculo
                GameObject obstacle = Instantiate(selectedObstacle, transform.position, Quaternion.identity, obstacleParent);

                // Ajusta a posição do obstáculo
                obstacle.transform.position = new Vector3(spawnX, transform.position.y, transform.position.z);

                // Adiciona o movimento
                obstacle.AddComponent<ObstacleMover>().Initialize(obstacleSpeed);

                // Adiciona a posição e largura à lista de ranges ocupados
                occupiedRanges.Add(new Vector2(spawnX - obstacleWidth / 2, spawnX + obstacleWidth / 2));
            }
        }
    }

    private bool IsPositionValid(float spawnX, float obstacleWidth, List<Vector2> occupiedRanges)
    {
        float leftEdge = spawnX - obstacleWidth / 2;
        float rightEdge = spawnX + obstacleWidth / 2;

        foreach (Vector2 range in occupiedRanges)
        {
            // Verifica se os ranges se sobrepõem
            if (leftEdge < range.y && rightEdge > range.x)
            {
                return false; // Sobreposição detectada
            }
        }
        return true;
    }
}
