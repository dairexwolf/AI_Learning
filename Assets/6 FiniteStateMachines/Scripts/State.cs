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
        IDLE, PATROL, PURSUE, ATTACK, SLEEP
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
    }

    // При выходе из состояние сбрасываем триггер анимации.
    public override void Exit()
    {
        base.Exit();
        anim.ResetTrigger("isWalking");
    }
}
