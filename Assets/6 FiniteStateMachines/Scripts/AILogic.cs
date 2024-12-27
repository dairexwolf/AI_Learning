using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// Класс, который позволяет управлять персонажем при помощи " конечных состояний"
/// </summary>
public class AILogic : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    public Transform player;
    public State currentState;
    public string currentEvent;

    public List<GameObject> checkpoints;


    // Подготовка
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentState = new Idle(this.gameObject, agent, anim, player);

        checkpoints = GameEnviroment.Singleton.Checkpoints;
    }

    // Update is called once per frame
    void Update()
    {
        // Обновление в каждом кадре, что делает НИП в зависимости от текущего состояния
        currentState = currentState.Process();
        currentEvent = currentState.stage.ToString();
    }
}
