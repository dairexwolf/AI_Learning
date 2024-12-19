using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixedUpdateMove : MonoBehaviour
{
    public float speed = 0.5f;

    void FixedUpdate()
    {
        //Без синхронизации
        // transform.Translate(0, 0, 0.01f);
        //Синхронизировано со временем
        transform.Translate(0, 0, Time.fixedDeltaTime*speed);
    }
}
