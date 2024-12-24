using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AIController : MonoBehaviour
{
    public NavMeshAgent agent;
    Animator anim;
    public GameObject target;   // Позиция игрока


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        agent.SetDestination(target.transform.position);    // Цель - игрок
        // Проверяем расстояние до точки. Если большое, запускаем анимацию ходьбы
        // если блико, то анимцию покоя
        if (agent.remainingDistance < 2)
            anim.SetBool("isMoving", false);
        else
            anim.SetBool("isMoving", true);
    }
}
