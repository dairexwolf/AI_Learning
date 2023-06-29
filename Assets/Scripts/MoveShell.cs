using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveShell : MonoBehaviour
{
    // это стандартная скорость
    public float speed = 1.0f;

    // Update is called once per frame
    void Update()
    {
        // это velocity
        transform.Translate(0, Time.deltaTime * speed * -0.5f, Time.deltaTime * speed);
    }
}
