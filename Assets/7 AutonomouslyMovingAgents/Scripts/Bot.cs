using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{

    NavMeshAgent agent;
    public GameObject target;
    DrivePoliceman targetDrive;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        targetDrive = target.GetComponent<DrivePoliceman>();
    }

    // Update is called once per frame
    void Update()
    {
        Evade();
    }

    // ������������� ���� �� ������� ����������
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);

    }

    // ���� �� ���� (������� ��������� �� ������� ����������)
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    // ������������� ����
    void Pursue()
    {
        // ��������� �� ����
        Vector3 targetDir = target.transform.position - this.transform.position;

        // ������������ ���� ����� �������� ������ � �������������� � �������������. ��������� ������, ���������� �� ��� ���� �����������, ����� ������������� ����������
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // ���� ���� ��������� ���������, �� �������������� ������� �� ���
        // � ���� ���� ����� (toTarget > 90) � �� ���� �� ������������ (relativeHeading < 20)
        if ((toTarget > 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }


        // ������������ ���������, ��� ����� �� ����
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ������������� ���� � �������������.
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    // ��������� �� ����
    void Evade()
    {
        // ��������� �� ����
        Vector3 targetDir = target.transform.position - this.transform.position;

        // ������������ ���� ����� �������� ������ � �������������� � �������������. ��������� ������, ���������� �� ��� ���� �����������, ����� ������������� ����������
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // ������������ ���� ����� �������� �������� ������������� � ���������� ��� ����. ��������� ������, ������� ��� ����� ��������� ����
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));
        
        // ���� ���� ��������� ���������, �� ������ �����
        // � ���� ���� ������� (toTarget > 90) � �� ���� �� ������������ (relativeHeading < 20)
        if ((toTarget < 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Flee(target.transform.position);
            return;
        }


        // ������������ ���������, ��� ����� �� ����
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // ���� � �������, ��������� ������������
        Flee(target.transform.position + target.transform.forward * lookAhead * 2);
    }

}
