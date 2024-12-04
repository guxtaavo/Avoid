using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    private float speed;

    public void Initialize(float moveSpeed)
    {
        speed = moveSpeed;
    }

    void Update()
    {
        // Move o obstáculo na direção do jogador (eixo Z negativo)
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Destroi o obstáculo ao sair do campo de visão
        if (transform.position.z < -60f)
        {
            Destroy(gameObject);
        }
    }
}
