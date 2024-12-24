using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowNavMesh : MonoBehaviour
{
    public GameObject wpManager;
    GameObject[] wps;
    [SerializeField]
    GameObject currentNode;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        // Откуда стартует танк
        currentNode = wps[15];

        agent = GetComponent<NavMeshAgent>();

        // Invoke("GoToTown", 2);
    }

    public void GoToFirstNode()
    {
        // graph.AStar(currentNode, wps[15]);
        agent.SetDestination(wps[15].transform.position);
    }

    public void GoToHeli()
    {
        //graph.AStar(currentNode, wps[1]);
        // Устанавливаем позицию, куда хотим отправиться
        agent.SetDestination(wps[1].transform.position);
    }

    public void GoToRuin()
    {
        //graph.AStar(currentNode, wps[12]);
        agent.SetDestination(wps[12].transform.position);
    }

    public void GoToAngar()
    {
        //graph.AStar(currentNode, wps[14]);
        agent.SetDestination(wps[14].transform.position);
    }

    public void GoToTown()
    {
        //graph.AStar(currentNode, wps[13]);
        agent.SetDestination(wps[13].transform.position);
    }

    public void GoToOil()
    {
        //graph.AStar(currentNode, wps[10]);
        agent.SetDestination(wps[10].transform.position);
    }

    public void GoToFactory()
    {
        //graph.AStar(currentNode, wps[9]);
        agent.SetDestination(wps[9].transform.position);
    }

    void LateUpdate()
    {
        
    }
}
