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
        // ������������� ������� ��� ��������
        bounds = new Bounds(FlockManager.FM.transform.position, FlockManager.FM.swimLimits * 2);
    }

    // Update is called once per frame
    void Update()
    {
        // ���� ���� ����� �� �������, ������ ����, ��� ������������� �� �������
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

            // ������ �������� ���������
            if (Random.Range(0, 100) < 10)
            {
                speed = Random.Range(FlockManager.FM.minSpeed, FlockManager.FM.maxSpeed);
            }

            if (Random.Range(0, 100) < 15)
            {
                ApplyRules();
            }
        }
        // ������� ������� ������
        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }

    /// <summary>
    /// ������� ��� ������ � FlockManager
    /// </summary>
    void ApplyRules()
    {
        GameObject[] gameObjects;
        gameObjects = FlockManager.FM.allFish;

        // ������ ������ ������
        Vector3 vCentre = Vector3.zero;
        // ������, ����� �������� �������
        Vector3 vAvoid = Vector3.zero;
        // �������� ������
        float gSpeed = 0.01f;
        // ��������� ����� ��������
        float nDistance;
        // ������ ������ �����
        int groupSize = 0;

        // ���������� ���� �����
        foreach (GameObject go in gameObjects)
        {
            // ���� ��� �� ��� ����
            if (go != this.gameObject)
            {
                // ������� ���������, ����� ������������ ������
                nDistance = Vector3.Distance(go.transform.position, this.transform.position);
                // ���� ������������ ������
                if (nDistance <= FlockManager.FM.neighbourDistance)
                {
                    // ��������� ����� ������
                    vCentre += go.transform.position;
                    // ����������� ������ ������
                    groupSize++;

                    // ���� ����� ������� ������
                    if (nDistance < 1.0f)
                    {
                        // �������������
                        vAvoid = vAvoid + (this.transform.position - go.transform.position);
                    }


                    Flock anotherFlock = go.GetComponent<Flock>();
                    gSpeed = gSpeed + anotherFlock.speed;
                }
            }
        }

        // ���� ������ ����
        if (groupSize > 0)
        {
            // ����������� ����� ������ � ��������, �������� � ���� ������
            vCentre = vCentre / groupSize + (FlockManager.FM.goalLocation - this.transform.position);
            speed = gSpeed / groupSize;
            // �� ���������� ����� ��������� �������� ������ ������������
            if (speed > FlockManager.FM.maxSpeed)
                speed = FlockManager.FM.maxSpeed;

            // ������ ����������� ��������
            Vector3 direction = (vCentre + vAvoid) - transform.position;

            // �������������
            if (direction != Vector3.zero)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation,
                                                      Quaternion.LookRotation(direction),
                                                      FlockManager.FM.rotationSpeed * Time.deltaTime);
            }
        }
    }
}
