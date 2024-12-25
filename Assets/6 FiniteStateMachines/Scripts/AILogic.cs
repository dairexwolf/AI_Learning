using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �����, ������� ��������� ��������� ���������� ��� ������ " �������� ���������"
/// </summary>
public class AILogic : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    public State currentState;
    public string currentEvent;


    // ����������
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = new Idle(this.gameObject, agent, anim, player);
    }

    // Update is called once per frame
    void Update()
    {
        // ���������� � ������ �����, ��� ������ ��� � ����������� �� �������� ���������
        currentState = currentState.Process();
        currentEvent = currentState.stage.ToString();
    }
}
