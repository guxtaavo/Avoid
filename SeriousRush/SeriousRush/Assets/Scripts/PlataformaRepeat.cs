using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRepeat : MonoBehaviour
{
    private float height;

    // Start is called before the first frame update
    void Start()
    {
        BoxCollider checkCollider = GetComponent<BoxCollider>();
        height = checkCollider.size.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x < -height)
        {
            transform.Translate(Vector2.right * 2f * height);
        }
    }
}