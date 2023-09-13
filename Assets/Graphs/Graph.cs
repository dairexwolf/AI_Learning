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
            // Добавить к ноде, что из нее идет грань
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

    // Реализация A* алгоритма на вейпоинтах
    public bool AStar(GameObject startId, GameObject endId)
    {
        if(startId == endId)
        {
            pathList.Clear();
            return false;
        }

        // Начало пути
        Node start = FindNode(startId);
        // конец пути
        Node end = FindNode(endId);

        if (start == null || end == null)
            return false;                   // если нет начала или конца, не может быть пути

        // Открытые ноды
        List<Node> open = new List<Node>();
        // закрытые ноды
        List<Node> closed = new List<Node>();
        float tentative_g_score = 0;
        bool tentative_is_better;

        // инициализируем начальные данные
        // G - шаг
        // H - дистанция до конца пути
        // F = G+H
        start.g = 0;
        start.h = Distance(start, end);
        start.f = start.h;

        // Открываем начальную ноду
        open.Add(start);

        // Цикл прохода по открытым нодам
        while (open.Count > 0)
        {
            // Берем ноду с минимальным F
            int i = LowestF(open);
            Node thisNode = open[i];
            // Если нода конечная, то все сделали
            if (thisNode.getId() == endId)
            {
                ReconstructPath(start, end);
                return true;
            }

            // Закрываем ноду
            open.RemoveAt(i);
            closed.Add(thisNode);
            // Начинаем проходку по соседям только что закрытой ноды
            Node neighbor;
            // Будем смотреть по граням, которые идут от ноды
            foreach (Edge e in thisNode.edgeList)
            {
                neighbor = e.endNode;

                // Если нода закрыта, она нам не нужна
                if (closed.IndexOf(neighbor) > -1)
                    continue;

                // Если нет, то рассчитывем ее временное значение от текущей ноды до соседа, которого мы смотрим
                // Нужно будет, чтобы потом понять, нужно ли нам по ней снова идти
                tentative_g_score = thisNode.g + Distance(thisNode, neighbor);

                // Если нода еще не открыта, мы ее открываем, чтобы рассчитать ее параметры G, H и F
                if (open.IndexOf(neighbor) == -1)
                {
                    open.Add(neighbor);
                    tentative_is_better = true;
                }
                // Если временное значение меньше, чем уже высчитанная g (все таки нода открыта), то будем ее перерассчитывать.
                else if (tentative_g_score < neighbor.g)
                {
                    tentative_is_better = true;
                }
                // Если хуже, то не будем перерасчитывать
                else
                {
                    tentative_is_better = false;
                }

                // Расчитываем характеристики ноды
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

    // Ставим путь
    public void ReconstructPath(Node startId, Node endId)
    {
        // Очищаем список пути и добавляем конечный вейпоинт
        pathList.Clear();
        pathList.Add(endId);

        var p = endId.cameFrom;
        // Пока не достигнем стартового вейпоинта
        while (p != startId && p != null)
        {
            // Вставляем в начало списка вейпоинт, с которого перешли на текущий
            pathList.Insert(0, p);
            p = p.cameFrom;
        }
        pathList.Insert(0, startId);
    }
}
