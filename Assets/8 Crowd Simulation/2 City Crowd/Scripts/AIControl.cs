using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour {

    public GameObject[] goalLocations;
    NavMeshAgent agent;
    Animator anim;
    /// <summary>
    /// Радиус затрагиваемых НИПов вокруг опасного объекта
    /// </summary>
    float detectionRadius = 20;
    /// <summary>
    /// Радиус побега НИПа от опасного объекта
    /// </summary>
    float fleeRadius = 30;

    float speedMulti;

    void Start() {

        agent = GetComponent<NavMeshAgent>();
        // Собираем все доступные локации, куда может пойти НИП
        goalLocations = GameObject.FindGameObjectsWithTag("goal");
        // Устанавливаем агенту, чтобы он шел к нему. К какому именно - выбираем рандомно
        agent.SetDestination(goalLocations[Random.Range(0, goalLocations.Length)].transform.position);
        anim = GetComponent<Animator>();
        // Делаем так, чтобы скорость анимации была рандомной
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
    /// Перезапускаем агент НИПа
    /// </summary>
    void ResetAgent()
    {
        // Запускаем анимацию ходьбы
        anim.SetTrigger("isWalking");
        // Делаем скорости НИП разными рандомно
        speedMulti = Random.Range(0.5f, 2f);
        anim.SetFloat("speedMult", speedMulti);
        agent.speed *= speedMulti;
        agent.angularSpeed = 120;
        // Удаляем путь
        agent.ResetPath();
    }

    public void DetectNewObstacle(Vector3 obsPos)
    {
        if(Vector3.Distance(obsPos, this.transform.position)< detectionRadius)
        {
            Vector3 fleeDir = (this.transform.position - obsPos).normalized;
            Vector3 newGoal = this.transform.position + fleeDir * fleeRadius;

            // Загружаем новый компонент пути
            NavMeshPath path = new NavMeshPath();
            // Расчитываем путь
            agent.CalculatePath(newGoal, path);

            // Если путь доступен, то направляем
            if(path.status != NavMeshPathStatus.PathInvalid)
            {
                // Берем последнюю точку из списка вейпоинтов пути объекта
                agent.SetDestination(path.corners[path.corners.Length - 1]);
                anim.SetTrigger("isRunning");
                agent.speed = 10;
                agent.angularSpeed = 500;
            }
        }
    }

}