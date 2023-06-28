using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondsUpdate : MonoBehaviour
{
    float timeStartOffset = 0f;
    bool gotStartTime = false;

    void Update()
    {
        // Фиксируем время, когда запусается эта функция
        if(!gotStartTime)
        {
            timeStartOffset = Time.realtimeSinceStartup;
            gotStartTime = true;
        }


        // Изменеие координаты в зависимости от текущего времени
        transform.position = new Vector3(transform.position.x, transform.position.y,
            Time.realtimeSinceStartup - timeStartOffset);
    }
}
