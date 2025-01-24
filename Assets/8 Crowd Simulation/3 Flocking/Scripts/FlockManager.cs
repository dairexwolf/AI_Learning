using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{

    public static FlockManager FM;
    // �������� ����
    public GameObject fishPrefab;
    // ���������� �����
    public int numFish = 20;
    // ������ �� ����� ��������
    public GameObject[] allFish;
    // �����, � ������� ����� ������� ������ ���
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

    // ��������� ���������� ����, ���� ����� ���������� �����
    public Vector3 goalLocation = Vector3.zero;


    // Start is called before the first frame update
    void Start()
    {
        // ����������, ������� ����� �����
        allFish = new GameObject[numFish];
        // ������� �����
        for(int i=0; i < numFish; i++)
        {
            // ������� ������� ����� � �������� swim limit
            Vector3 pos = transform.position + new Vector3( Random.Range(-swimLimits.x, swimLimits.x),
                                                            Random.Range(-swimLimits.y, swimLimits.y), 
                                                            Random.Range(-swimLimits.z, swimLimits.z));
            // ������� �������
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
        // �������� ��������, ����� ������ ��, ���� ����� ���� ���������, ��� ���
        if(Random.Range(0,100)<10)
        {
            goalLocation = new Vector3(Random.Range(-swimLimits.x, swimLimits.x),
                                       Random.Range(-swimLimits.y, swimLimits.y),
                                       Random.Range(-swimLimits.z, swimLimits.z));
        }
    }
}
