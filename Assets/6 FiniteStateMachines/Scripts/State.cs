using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Базовый класс стадии, преднозначенный, чтобы с него делали наследников
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

    // Добавляем необходимые переменные
    // Название стадии
    public STATE name;
    //Название события в стадии
    public EVENT stage;
    //Ссылка на НИП
    protected GameObject npc;
    protected Animator anim;
    // Позиция игрока
    protected Transform player;
    // Следующая стадия, которая может быть
    protected State nextState;
    // Можно добавить и предыдущую стадию, если необходимо


    protected NavMeshAgent agent;

    // Параметры, при которых стадии будут меняться
    // Дистанция до игрока
    float visDist = 10;
    // Поле видимости
    float visAngle = 30f;
    // Дистанция выстрела
    float shootDist = 7f;

    public State(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
    {
        npc = _npc;
        agent = _agent;
        anim = _anim;
        stage = EVENT.ENTER;
        player = _player;
    }

    // Виртуальные методы для всех событий в стадии
    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }
    public virtual void Update() { stage = EVENT.UPDATE; }
    public virtual void Exit() { stage = EVENT.EXIT; }

    // Функция перевода из стадии в стадию
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
    /// Видит ли НИП игрока?
    /// </summary>
    /// <returns>true, если да; false, если нет</returns>
    public bool CanSeePlayer()
    {
        // Вектор направления от НИП к игроку
        Vector3 direction = player.position - npc.transform.position;
        // Угол от переда НИП к игроку
        float angle = Vector3.Angle(direction, npc.transform.forward);

        // Если игрок близко и под углом зрения, то он его видит
        if (direction.magnitude < visDist && angle < visAngle)
        {
            return true;
        }

        // Если нет, то не видит
        return false;
    }

    /// <summary>
    /// Может ли атаковать НИП игрока
    /// </summary>
    /// <returns>true, если да; false - если нет</returns>
    public bool CanAttackPlayer()
    {
        // Вектор направления от НИП к игроку
        Vector3 direction = player.position - npc.transform.position;
        // Если дистанция игрока меньше, чем дистанция его стрельбы, то может
        if (direction.magnitude < shootDist && CanSeePlayer())
        {
            return true;
        }
        return false;
    }

    public bool CanBeScared()
    {
        // Вектор направления от НИП к игроку. Реверснем, чтобы вектор был сзади НИПа
        Vector3 direction = npc.transform.position - player.position;
        // Если дистанция игрока меньше, чем дистанция его стрельбы, то может
        // if(direction.magnitude < 2 && angle < 30)
        if (direction.magnitude < 2 && !CanSeePlayer())
        {
            return true;
        }
        return false;
    }

}

/// <summary>
/// Состояние "Покой"
/// </summary>
public class Idle : State
{
    public Idle(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                : base(_npc, _agent, _anim, _player)
    {
        name = STATE.IDLE;
    }

    // При заходе в состояние, активируем анимацию
    public override void Enter()
    {
        anim.SetTrigger("isIdle");
        base.Enter();
    }

    // Тут пишется логика, что будет в этом состоянии происходить.
    public override void Update()
    {
        //Если сгенерировалось число меньше 10, то отправляем npc  в состояние "Патруль".
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

    // При выходе из состояние сбрасываем триггер анимации.
    public override void Exit()
    {
        anim.ResetTrigger("isIdle");
        base.Exit();
    }
}

/// <summary>
/// Состояние патрулирования
/// </summary>
public class Patrol : State
{
    // Индекс вейпоинта
    int currentIndex = -1;

    public Patrol(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                    : base(_npc, _agent, _anim, _player)
    {
        // Ставим патруль
        name = STATE.PATROL;
        // Ставим скорость персонажа
        agent.speed = 2;
        // Убираем "стоп" с агента
        agent.isStopped = false;
    }

    // При заходе в состояние, активируем анимацию
    public override void Enter()
    {
        float lastDist = Mathf.Infinity;
        // Выбираем ближайший Waypoint, к которому пойдет НИП
        for (int i = 0; i < GameEnviroment.Singleton.Checkpoints.Count; i++)
        {
            // Перебираем все вейпоинты и расчитываем расстояние до них
            GameObject thisWP = GameEnviroment.Singleton.Checkpoints[i];
            float distance = Vector3.Distance(npc.transform.position, thisWP.transform.position);
            // Сравниваем расстояния
            if (distance < lastDist)
            {
                // При апдейте у нас добавится вейпоинт, и НИП пойдет к следующему от ближайщего
                currentIndex = i - 1;
                lastDist = distance;
            }
        }
        currentIndex = 0;
        anim.SetTrigger("isWalking");
        base.Enter();

    }

    // Тут пишется логика, что будет в этом состоянии происходить.
    public override void Update()
    {
        // Проверяем, достиг ли агент чекпоинта
        if (agent.remainingDistance < 1)
        {
            // Если да, пусть идет дальше по кругу
            if (currentIndex >= GameEnviroment.Singleton.Checkpoints.Count - 1)
                currentIndex = 0;
            // Если нет, то к следующему
            else
                currentIndex++;

            // Направляем агента к вейпоинту
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

    // При выходе из состояние сбрасываем триггер анимации.
    public override void Exit()
    {
        base.Exit();
        anim.ResetTrigger("isWalking");
    }
}

/// <summary>
/// Состояние преследования игрока
/// </summary>
public class Pursue : State
{
    public Pursue(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                    : base(_npc, _agent, _anim, _player)
    {
        // Ставим патруль
        name = STATE.PURSUE;
        // Ставим скорость персонажа (бежит)
        agent.speed = 5;
        // Убираем "стоп" с агента
        agent.isStopped = false;
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        base.Enter();
    }

    public override void Update()
    {
        // Ставим точку, куда должен идти НИП (до нашего игрока)
        agent.SetDestination(player.position);

        // Если он нашел путь
        if (agent.hasPath)
        {
            // И может атаковать игрока
            if (CanAttackPlayer())
            {
                // Переходит в атаку
                nextState = new Attack(npc, agent, anim, player);
                stage = EVENT.EXIT;
            }
            else if (!CanSeePlayer())
            {
                // Переходит в патрулирование
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
/// Состояние атаки игрока
/// </summary>
public class Attack : State
{
    float rotationSpeed = 2f;
    AudioSource shoot;

    public Attack(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                   : base(_npc, _agent, _anim, _player)
    {
        // Ставим состояние атаки
        name = STATE.ATTACK;
        shoot = _npc.GetComponent<AudioSource>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isShooting");
        // Он атакует стоя
        agent.isStopped = true;
        // shoot.Play();
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = player.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.position);
        // Не хотим его вращать по y координате
        direction.y = 0;

        // Сглаживаем вращение НИП
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotationSpeed);

        // Лучше в айдл перейти, если что, он может оттуда снова хоть куда перейти
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
    // Вообще, этот объект должен быть в GameEnviroment, но так быстрее
    Transform safeCube;
    float rotationSpeed = 2f;

    public RunAway(GameObject _npc, NavMeshAgent _agent, Animator _anim, Transform _player)
                   : base(_npc, _agent, _anim, _player)
    {
        name = STATE.RUNAWAY;
        // Ставим скорость персонажа (бежит)
        agent.speed = 5;
        // Убираем "стоп" с агента
        agent.isStopped = false;
        safeCube = GameObject.FindGameObjectWithTag("Safe").GetComponent<Transform>();
    }

    public override void Enter()
    {
        anim.SetTrigger("isRunning");
        // Объект статичный, можно 1 раз задать координату
        agent.SetDestination(safeCube.position);
        base.Enter();
    }

    public override void Update()
    {
        Vector3 direction = safeCube.position - npc.transform.position;
        float angle = Vector3.Angle(direction, npc.transform.position);

        // Не хотим его вращать по y координате
        direction.y = 0;

        // Сглаживаем вращение НИП
        npc.transform.rotation = Quaternion.Slerp(npc.transform.rotation,
            Quaternion.LookRotation(direction),
            Time.deltaTime * rotationSpeed);

        // Молжно, чтобы он каждый цикл менял окончательную координату
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