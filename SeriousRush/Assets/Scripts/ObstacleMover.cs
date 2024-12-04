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
        // Move o obst�culo na dire��o do jogador (eixo Z negativo)
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        // Destroi o obst�culo ao sair do campo de vis�o
        if (transform.position.z < -60f)
        {
            Destroy(gameObject);
        }
    }
}
