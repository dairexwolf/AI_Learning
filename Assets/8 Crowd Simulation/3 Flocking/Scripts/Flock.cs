using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{

    public float speed;
    bool turning = false;

    Bounds bounds;

    // Start is called before the first frame update
    void Start()
    {
        speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
        // Устанавливаем границы для плаванья
        bounds = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2);
    }

    // Update is called once per frame
    void Update()
    {
        // Если рыба вышла из области, ставим флаг, что разворачиваем ее обратно
        if (!bounds.Contains(transform.position))
            turning = true;
        else
            turning = false;

        if (turning)
        {
            Vector3 dir = FlockManager.FM.transform.position - this.transform.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), FlockManager.FM.rotationSpeed * Time.deltaTime);
        }
        else
        {

            // Делаем скорость рандомной
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }

            if (Random.Range(0, 100) < 15)
            {
                ApplyRules();
            }
        }
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
        foreach (GameObject go in gameObjects)
        {
            // Если это не эта рыба
            if (go != this.gameObject)
            {
                // Смотрим дистанцию, чтобы образовывать группы
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                // Если образовываем группу
                if (nDistance <= FlockManager.FM.neighbourDistance)
                {
                    // Вычисляем центр группы
                    vCentre += go.transform.position;
                    // Увеличиваем размер группы
                    groupSize++;

                    // Если сосед слишком близко
                    if (nDistance < 1.0f)
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
        if (groupSize > 0)
        {
            // Нормализуем центр группы и скорость, учитывая и цель группы
            vCentre = vCentre / groupSize + (FlockManager.FM.goalLocation - this.transform.position);
            speed = gSpeed / groupSize;
            // Не повзволяем рыбам развивать скорость больше максимальной
            if (speed > FlockManager.FM.maxSpeed)
                speed = FlockManager.FM.maxSpeed;

            // Задаем направление движения
            Vector3 direction = (vCentre + vAvoid) - transform.position;

            // Разворачиваем
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
