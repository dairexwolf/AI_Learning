using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// ������� ����� ������, ���������������, ����� � ���� ������ �����������
/// </summary>
public class State
{
    public enum STATE
    {
        IDLE, PATROL, PURSUE, ATTACK, SLEEP, RUNAWAY
    };

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    };

    // ��������� ����������� ����������
    // �������� ������
    public STATE name;
    //�������� ������� � ������
    public EVENT stage;
    //������ �� ���
    protected GameObject npc;
    protected Animator anim;
    // ������� ������
    protected Transform player;
    // ��������� ������, ������� ����� ����
    protected State nextState;
    // ����� �������� � ���������� ������, ���� ����������


    protected NavMeshAgent agent;

    // ���������, ��� ������� ������ ����� ��������
    // ��������� �� ������
    float visDist = 10;
    // ���� ���������
    float visAngle = 30f;
    // ��������� ��������
    float shootDist = 7f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    // ����������� ������ ��� ���� ������� � ������
    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    // ������� �������� �� ������ � ������
    public State Process()
    {
        if (stage == EVENT.ENTER) Enter();
        if (stage == EVENT.UPDATE) Update();
        if (stage == EVENT.EXIT)

        {
            Exit();
            return nextState;
        }
        return this;
    }

    /// <summary>
    /// ����� �� ��� ������?
    /// </summary>
    /// <returns>true, ���� ��; false, ���� ���</returns>
    public bool CanSeePlayer()
    {
        // ������ ����������� �� ��� � ������
        Vector3 direction = player.position - npc.transform.position;
        // ���� �� ������ ��� � ������
        float angle = Vector3.Angle(direction, npc.transform.forward);

        // ���� ����� ������ � ��� ����� ������, �� �� ��� �����
        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }

        // ���� ���, �� �� �����
        return false;
    }

    /// <summary>
    /// ����� �� ��������� ��� ������
    /// </summary>
    /// <returns>true, ���� ��; false - ���� ���</returns>
    public bool CanAttackPlayer()
    {
        // ������ ����������� �� ��� � ������
        Vector3 direction = player.position - npc.transform.position;
        // ���� ��������� ������ ������, ��� ��������� ��� ��������, �� �����
        if (direction.magnitude < shootDist && CanSeePlayer())
        {
            return true;
        }
        return false;
    }

    public bool CanBeScared()
    {
        // ������ ����������� �� ��� � ������. ���������, ����� ������ ��� ����� ����
        Vector3 direction = npc.transform.position - player.position;
        // ���� ��������� ������ ������, ��� ��������� ��� ��������, �� �����
        // if(direction.magnitude < 2 && angle < 30)
        if (direction.magnitude < 2 && !CanSeePlayer())
        {
            return true;
        }
        return false;
    }

}

/// <summary>
/// ��������� "�����"
/// </summary>
public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
    }

    // ��� ������ � ���������, ���������� ��������
    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        base.Enter();
    }

    // ��� ������� ������, ��� ����� � ���� ��������� �����������.
    public override void Update()
    {
        //���� ��������������� ����� ������ 10, �� ���������� npc  � ��������� "�������".
        if (Random.Range(0, 100) < 10)
        {
            nextState = new Patrol(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }

        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    // ��� ������ �� ��������� ���������� ������� ��������.
    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

/// <summary>
/// ��������� ��������������
/// </summary>
public class Patrol : State
{
    // ������ ���������
    int currentIndex = -1;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                    : base(_npc, _agent, _anim, _player)
    {
        // ������ �������
        name = STATE.PATROL;
        // ������ �������� ���������
        agent.speed = 2;
        // ������� "����" � ������
        agent.isStopped = false;
    }

    // ��� ������ � ���������, ���������� ��������
    public override void Enter()
    {
        float lastDist = Mathf.Infinity;
        // �������� ��������� Waypoint, � �������� ������ ���
        for (int i = 0; i < GameEnviroment.Singleton.Checkpoints.Count; i++)
        {
            // ���������� ��� ��������� � ����������� ���������� �� ���
            GameObject thisWP = GameEnviroment.Singleton.Checkpoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            // ���������� ����������
            if (distance < lastDist)
            {
                // ��� ������� � ��� ��������� ��������, � ��� ������ � ���������� �� ����������
                currentIndex = i - 1;
                lastDist = distance;
            }
        }
        currentIndex = 0;
        anim.SetTrigger("isWalking");
        base.Enter();

    }

    // ��� ������� ������, ��� ����� � ���� ��������� �����������.
    public override void Update()
    {
        // ���������, ������ �� ����� ���������
        if (agent.remainingDistance < 1)
        {
            // ���� ��, ����� ���� ������ �� �����
            if (currentIndex >= GameEnviroment.Singleton.Checkpoints.Count - 1)
                currentIndex = 0;
            // ���� ���, �� � ����������
            else
                currentIndex++;

            // ���������� ������ � ���������
            agent.SetDestination(GameEnviroment.Singleton.Checkpoints[currentIndex].transform.position);
        }

        if (CanSeePlayer())
        {
            nextState = new Pursue(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }

        if (CanBeScared())
        {
            nextState = new RunAway(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }

    }

    // ��� ������ �� ��������� ���������� ������� ��������.
    public override void Exit()
    {
        base.Exit();
        anim.ResetTrigger("isWalking");
    }
}

/// <summary>
/// ��������� ������������� ������
/// </summary>
public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                    : base(_npc, _agent, _anim, _player)
    {
        // ������ �������
        name = STATE.PURSUE;
        // ������ �������� ��������� (�����)
        agent.speed = 5;
        // ������� "����" � ������
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        // ������ �����, ���� ������ ���� ��� (�� ������ ������)
        agent.SetDestination(player.position);

        // ���� �� ����� ����
        if (agent.hasPath)
        {
            // � ����� ��������� ������
            if (CanAttackPlayer())
            {
                // ��������� � �����
                nextState = new Attack(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
            else if (!CanSeePlayer())
            {
                // ��������� � ��������������
                nextState = new Patrol(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isRunning");
        base.Exit();
    }
}

/// <summary>
/// ��������� ����� ������
/// </summary>
public class Attack : State
{
    float rotationSpeed = 2f;
    AudioSource shoot;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                   : base(_npc, _agent, _anim, _player)
    {
        // ������ ��������� �����
        name = STATE.ATTACK;
        shoot = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isShooting");
        // �� ������� ����
        agent.isStopped = true;
        // shoot.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.position);
        // �� ����� ��� ������� �� y ����������
        direction.y = 0;

        // ���������� �������� ���
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotationSpeed);

        // ����� � ���� �������, ���� ���, �� ����� ������ ����� ���� ���� �������
        if (!CanAttackPlayer())
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isShooting");
        base.Exit();
    }
}

public class RunAway : State
{
    // ������, ���� ������ ������ ���� � GameEnviroment, �� ��� �������
    Transform safeCube;
    float rotationSpeed = 2f;

    public RunAway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                   : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RUNAWAY;
        // ������ �������� ��������� (�����)
        agent.speed = 5;
        // ������� "����" � ������
        agent.isStopped = false;
        safeCube = GameObject.FindGameObjectWithTag("Safe").GetComponent<Transform>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        // ������ ���������, ����� 1 ��� ������ ����������
        agent.SetDestination(safeCube.position);
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = safeCube.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.position);

        // �� ����� ��� ������� �� y ����������
        direction.y = 0;

        // ���������� �������� ���
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotationSpeed);

        // ������, ����� �� ������ ���� ����� ������������� ����������
        // agent.SetDestination(safeCube.position);

        if (agent.remainingDistance < 1)
        {
            nextState = new Idle(npc, agent, anim, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        anim.ResetTrigger("isShooting");
        base.Exit();
    }
}