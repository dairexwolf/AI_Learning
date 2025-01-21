using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {

    public GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator anim;
    /// <summary>
    /// ������ ������������� ����� ������ �������� �������
    /// </summary>
    float detectionRadius = 20;
    /// <summary>
    /// ������ ������ ���� �� �������� �������
    /// </summary>
    float fleeRadius = 30;

    float speedMulti;

    void Start() {

        agent = GetComponent<NavMeshAgent>();
        // �������� ��� ��������� �������, ���� ����� ����� ���
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        // ������������� ������, ����� �� ��� � ����. � ������ ������ - �������� ��������
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        anim = GetComponent<Animator>();
        // ������ ���, ����� �������� �������� ���� ���������
        anim.SetFloat("wOffset", Random.Range(0.0f, 1.0f));
        ResetAgent();
    }


    void Update() {
        if (agent.remainingDistance < 1)
        {
            ResetAgent();
            agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        }
    }

    /// <summary>
    /// ������������� ����� ����
    /// </summary>
    void ResetAgent()
    {
        // ��������� �������� ������
        anim.SetTrigger("isWalking");
        // ������ �������� ��� ������� ��������
        speedMulti = Random.Range(0.5f, 2f);
        anim.SetFloat("speedMult", speedMulti);
        agent.speed *= speedMulti;
        agent.angularSpeed = 120;
        // ������� ����
        agent.ResetPath();
    }

    public void DetectNewObstacle(Vector3 obsPos)
    {
        if(Vector3.Distance(obsPos, this.transform.position)< detectionRadius)
        {
            Vector3 fleeDir = (this.transform.position - obsPos).normalized;
            Vector3 newGoal = this.transform.position + fleeDir * fleeRadius;

            // ��������� ����� ��������� ����
            NavMeshPath path = new NavMeshPath();
            // ����������� ����
            agent.CalculatePath(newGoal, path);

            // ���� ���� ��������, �� ����������
            if(path.status != NavMeshPathStatus.PathInvalid)
            {
                // ����� ��������� ����� �� ������ ���������� ���� �������
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                anim.SetTrigger("isRunning");
                agent.speed = 10;
                agent.angularSpeed = 500;
            }
        }
    }

}