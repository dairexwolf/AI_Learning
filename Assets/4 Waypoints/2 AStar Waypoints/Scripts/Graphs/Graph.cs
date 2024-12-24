using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Graph
{
    // All info
    List<Edge> edges = new List<Edge>();
    List<Node> nodes = new List<Node>();

    // Path info
    List<Node> pathList = new List<Node>();

    public List<Node> PathList
    {
        get
        {

            return pathList;
        }
        set
        {
            if (value.GetType() == typeof(List<Node>))
                pathList = value;
        }
    }

    public Graph() { }

    public void AddNode(GameObject id)
    {
        Node node = new Node(id);
        nodes.Add(node);
    }

    public void AddEdge(GameObject fromNode, GameObject toNode)
    {
        Node from = FindNode(fromNode);
        Node to = FindNode(toNode);

        if (from != null && to != null)
        {
            Edge e = new Edge(from, to);
            edges.Add(e);
            // �������� � ����, ��� �� ��� ���� �����
            from.edgeList.Add(e);
        }
    }

    Node FindNode(GameObject id)
    {
        foreach (Node n in nodes)
        {
            if (n.getId() == id)
                return n;
        }
        return null;
    }

    // ���������� A* ��������� �� ����������
    public bool AStar(GameObject startId, GameObject endId)
    {
        if(startId == endId)
        {
            pathList.Clear();
            return false;
        }

        // ������ ����
        Node start = FindNode(startId);
        // ����� ����
        Node end = FindNode(endId);

        if (start == null || end == null)
            return false;                   // ���� ��� ������ ��� �����, �� ����� ���� ����

        // �������� ����
        List<Node> open = new List<Node>();
        // �������� ����
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0;
        bool tentative_is_better;

        // �������������� ��������� ������
        // G - ���
        // H - ��������� �� ����� ����
        // F = G+H
        start.g = 0;
        start.h = Distance(start, end);
        start.f = start.h;

        // ��������� ��������� ����
        open.Add(start);

        // ���� ������� �� �������� �����
        while (open.Count > 0)
        {
            // ����� ���� � ����������� F
            int i = LowestF(open);
            Node thisNode = open[i];
            // ���� ���� ��������, �� ��� �������
            if (thisNode.getId() == endId)
            {
                ReconstructPath(start, end);
                return true;
            }

            // ��������� ����
            open.RemoveAt(i);
            closed.Add(thisNode);
            // �������� �������� �� ������� ������ ��� �������� ����
            Node neighbor;
            // ����� �������� �� ������, ������� ���� �� ����
            foreach (Edge e in thisNode.edgeList)
            {
                neighbor = e.endNode;

                // ���� ���� �������, ��� ��� �� �����
                if (closed.IndexOf(neighbor) > -1)
                    continue;

                // ���� ���, �� ����������� �� ��������� �������� �� ������� ���� �� ������, �������� �� �������
                // ����� �����, ����� ����� ������, ����� �� ��� �� ��� ����� ����
                tentative_g_score = thisNode.g + Distance(thisNode, neighbor);

                // ���� ���� ��� �� �������, �� �� ���������, ����� ���������� �� ��������� G, H � F
                if (open.IndexOf(neighbor) == -1)
                {
                    open.Add(neighbor);
                    tentative_is_better = true;
                }
                // ���� ��������� �������� ������, ��� ��� ����������� g (��� ���� ���� �������), �� ����� �� ����������������.
                else if (tentative_g_score < neighbor.g)
                {
                    tentative_is_better = true;
                }
                // ���� ����, �� �� ����� ���������������
                else
                {
                    tentative_is_better = false;
                }

                // ����������� �������������� ����
                if (tentative_is_better)
                {
                    neighbor.cameFrom = thisNode;
                    neighbor.g = tentative_g_score;
                    neighbor.h = Distance(thisNode, end);
                    neighbor.f = neighbor.g + neighbor.h;
                }
            }
        }
        return false;
    }

    float Distance(Node a, Node b)
    {
        return Vector3.SqrMagnitude(a.getId().transform.position - b.getId().transform.position);
    }

    int LowestF(List<Node> l)
    {
        float lowestf = 0;
        int count = 0;
        int iteratorCount = 0;

        lowestf = l[0].f;

        for (int i = 1; i < l.Count; i++)
        {
            if (l[i].f < lowestf)
            {
                lowestf = l[i].f;
                iteratorCount = count;
            }
            count++;
        }

        return iteratorCount;
    }

    // ������ ����
    public void ReconstructPath(Node startId, Node endId)
    {
        // ������� ������ ���� � ��������� �������� ��������
        pathList.Clear();
        pathList.Add(endId);

        var p = endId.cameFrom;
        // ���� �� ��������� ���������� ���������
        while (p != startId && p != null)
        {
            // ��������� � ������ ������ ��������, � �������� ������� �� �������
            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startId);
    }
}
