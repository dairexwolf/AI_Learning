using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWaypoints : MonoBehaviour
{
    Transform goal;
    float speed = 5f;
    float accuracy = 1f;
    float rotSpeed = 2f;

    public GameObject wpManager;
    GameObject[] wps;
    GameObject currentNode;
    // Индекс вейпоинта
    [SerializeField]
    int currentWP = 0;
    Graph graph;

    // Start is called before the first frame update
    void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        graph = wpManager.GetComponent<WPManager>().graph;
        // Откуда стартует танк
        currentNode = wps[15];
    }

    public void GoToFirstNode()
    {
        graph.AStar(currentNode, wps[15]);
        currentWP = 15;
    }

    public void GoToHeli()
    {
        graph.AStar(currentNode, wps[1]);
        currentWP = 15;
    }

    public void GoToRuin()
    {
        graph.AStar(currentNode, wps[12]);
        currentWP = 15;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
