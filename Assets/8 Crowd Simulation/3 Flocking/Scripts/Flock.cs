using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        ApplyRules();
        // Двигаем рыбовое вперед
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    /// <summary>
    /// Функция для работы с FlockManager
    /// </summary>
    void ApplyRules()
    {
        GameObject[] gameObjects;
        gameObjects = FlockManager.FM.allFish;

        // Вектор центра группы
        Vector3 vCentre = Vector3.zero;
        // Вектор, чтобы обходить соседей
        Vector3 vAvoid = Vector3.zero;
        // Скорость группы
        float gSpeed = 0.01f;
        // Дистанция между соседями
        float nDistance;
        // Размер группы рыбов
        int groupSize = 0;

        // Перебираем всех рыбов
        foreach(GameObject go in gameObjects)
        {
            // Если это не эта рыба
            if(go != this.gameObject)
            {
                // Смотрим дистанцию, чтобы образовывать группы
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                // Если образовываем группу
                if(nDistance <= FlockManager.FM.neighbourDistance)
                {
                    // Вычисляем центр группы
                    vCentre += go.transform.position;
                    // Увеличиваем размер группы
                    groupSize++;

                    // Если сосед слишком близко
                    if(nDistance<1.0f)
                    {
                        // Уворачиваемся
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }

                    
                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        // Если группа есть
        if(groupSize>0)
        {
            // Нормализуем центр группы и скорость
            vCentre = vCentre / groupSize;
            speed = gSpeed / groupSize;

            // Задаем направление движения
            Vector3 direction = (vCentre + vAvoid) - transform.position;

            // Разворачиваем
            if(direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
