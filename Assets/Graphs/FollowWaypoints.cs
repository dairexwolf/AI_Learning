using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWaypoints : MonoBehaviour
{
    Transform goal;
    [SerializeField]
    float speed = 5f;
    [SerializeField]
    float accuracy = 3f;
    [SerializeField]
    float rotSpeed = 2f;

    public GameObject wpManager;
    GameObject[] wps;
    [SerializeField]
    GameObject currentNode;
    // ������ ���������
    [SerializeField]
    int currentWP = 0;
    Graph graph;

    // Start is called before the first frame update
    void Start()
    {
        wps = wpManager.GetComponent<WPManager>().waypoints;
        graph = wpManager.GetComponent<WPManager>().graph;
        // ������ �������� ����
        currentNode = wps[15];

        // Invoke("GoToTown", 2);
    }

    public void GoToFirstNode()
    {
        graph.AStar(currentNode, wps[15]);
        currentWP = 0;
    }

    public void GoToHeli()
    {
        graph.AStar(currentNode, wps[1]);
        currentWP = 0;
    }

    public void GoToRuin()
    {
        graph.AStar(currentNode, wps[12]);
        currentWP = 0;
    }

    public void GoToAngar()
    {
        graph.AStar(currentNode, wps[14]);
        currentWP = 0;
    }

    public void GoToTown()
    {
        graph.AStar(currentNode, wps[13]);
        currentWP = 0;
    }

    public void GoToOil()
    {
        graph.AStar(currentNode, wps[10]);
        currentWP = 0;
    }

    public void GoToFactory()
    {
        graph.AStar(currentNode, wps[9]);
        currentWP = 0;
    }

    void LateUpdate()
    {
        // ���� ���� ������, �� ���
        if (graph.PathList.Count == 0 || currentWP == graph.PathList.Count)
            return;

        // ���� ������� ��������� ��� ������, �������� ��������� ��������
        if (Vector3.Distance(graph.PathList[currentWP].getId().transform.position,
            transform.position) < accuracy)
        {
            currentNode = graph.PathList[currentWP].getId();
            currentWP++;
        }

        // ���� �� �� �������� ����� ����, ����
        if (currentWP < graph.PathList.Count)
        {
            // ����� ������� ���������, ���� ����
            goal = graph.PathList[currentWP].getId().transform;

            Vector3 lookAtGoal = new Vector3(goal.position.x, transform.position.y,
                                                goal.position.z);
            // ������������ ���� �� ��������
            Vector3 direction = lookAtGoal - transform.position;
           
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(direction), Time.deltaTime * rotSpeed);

            // ���� ���� ������
            transform.Translate(0, 0, speed * Time.deltaTime);
        }

    }
}
