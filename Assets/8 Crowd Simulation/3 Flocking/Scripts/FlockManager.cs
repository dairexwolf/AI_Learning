using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public static FlockManager FM;
    // Моделька рыбы
    public GameObject fishPrefab;
    // Количество рыбов
    public int numFish = 20;
    // Массив со всеми рыбовыми
    public GameObject[] allFish;
    // Лимит, в котором будет плавать группа рыб
    public Vector3 swimLimits = new Vector3(5, 5, 5);

    [Header("Fish Settings")]
    [Range(0.0f, 5.0f)]
    public float minSpeed;
    [Range(0.0f, 6.0f)]
    public float maxSpeed;
    [Range(0.0f, 10.0f)]
    public float neighbourDistance;
    [Range(0.5f, 5.0f)]
    public float rotationSpeed;

    // Добавляем коррдинаты цели, куда будут стремиться рыбов
    public Vector3 goalLocation = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        // Определяем, сколько будет рыбов
        allFish = new GameObject[numFish];
        // Создаем рыбов
        for(int i=0; i < numFish; i++)
        {
            // Создаем позицию рыбов в пределах swim limit
            Vector3 pos = transform.position + new Vector3( Random.Range(-swimLimits.x, swimLimits.x),
                                                            Random.Range(-swimLimits.y, swimLimits.y), 
                                                            Random.Range(-swimLimits.z, swimLimits.z));
            // Создаем рыбовое
            allFish[i] = Instantiate(fishPrefab, pos, Quaternion.identity);
            goalLocation = this.transform.position;
        }

        FM = this;

        if (maxSpeed<minSpeed)
        {
            maxSpeed = minSpeed + 0.5f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Выбираем рандомно, хотим менять то, куда будут рыбы двиатьсяя, или нет
        if(Random.Range(0,100)<10)
        {
            goalLocation = new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                       Random.Range(-swimLimits.y, swimLimits.y),
                                       Random.Range(-swimLimits.z, swimLimits.z));
        }
    }
}
