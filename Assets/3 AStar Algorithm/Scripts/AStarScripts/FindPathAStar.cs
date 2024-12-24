using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PathMarker
{
    public MapLocation location;
    public float G;
    public float H;
    public float F;
    public GameObject marker;
    public PathMarker parent;

    public PathMarker(MapLocation l, float g, float h, float f, GameObject marker, PathMarker p)
    {
        location = l;
        G = g;
        H = h;
        F = f;
        this.marker = marker;
        parent = p;
    }

    public override bool Equals(object obj)
    {
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            return false;
        else
            return location.Equals(((PathMarker)obj).location);
    }
}

public class FindPathAStar : MonoBehaviour
{
    // Скрипт лабиринта, чтобы мы знали его карту (матрицу)
    public Maze maze;
    // Необходимые материалы
    public Material closedMaterial;
    public Material openMaterial;

    // Открытые пути
    public List<PathMarker> open = new List<PathMarker>();
    //Закрытые пути
    public List<PathMarker> closed = new List<PathMarker>();

    // Необходимые ассеты
    public GameObject start;
    public GameObject end;
    public GameObject path;

    // Куда идти
    public PathMarker goalNode;
    // Откуда идти
    public PathMarker startNode;

    // Последний маркер, с которым работали
    public PathMarker lastPos;
    // Найден ли путь?
    bool done = false;

    /// <summary>
    /// Фубирает все маркеры со сцены
    /// </summary>
    void RemoveAllMarkers()
    {
        GameObject[] markers = GameObject.FindGameObjectsWithTag("marker");
        foreach (GameObject marker in markers)
            Destroy(marker);
    }

    /// <summary>
    /// Начинаем поиск
    /// </summary>
    void BeginSearch()
    {
        // Путь не нашли
        done = false;
        // Убираем маркеры
        RemoveAllMarkers();

        // Составляем матрицу потенциальных мест, куда можно поставить маркеры
        List<MapLocation> locations = new List<MapLocation>();
        for (int z = 1; z < maze.depth - 1; z++)
            for (int x = 1; x < maze.width - 1; x++)
            {
                if (maze.map[x, z] != 1)
                    locations.Add(new MapLocation(x, z));
            }

        // Рандомим элементы внутри списка
        locations.Shuffle();

        // Ставим координаты первого маркера
        Vector3 startLocation = new Vector3(locations[0].x * maze.scale, 0, locations[0].z * maze.scale);
        // Ставим старотвый маркер
        startNode = new PathMarker(new MapLocation(locations[0].x, locations[0].z), 0, 0, 0,
            Instantiate(start, startLocation, Quaternion.identity), null);

        // Ставим координаты конечного маркера
        Vector3 goalLocation = new Vector3(locations[1].x * maze.scale, 0, locations[1].z * maze.scale);
        // Ставим конечный маркер
        goalNode = new PathMarker(new MapLocation(locations[1].x, locations[1].z), 0, 0, 0,
            Instantiate(end, goalLocation, Quaternion.identity), null);

        // Чистим списки открытых и закрытых маркеров
        open.Clear();
        closed.Clear();
        // Открываем стартовый маркер
        open.Add(startNode);
        lastPos = startNode;

        // Готовы к поиску
    }

    void Serach(PathMarker thisNode)
    {
        if (thisNode == null)
            return;
        if (thisNode.Equals(goalNode)) // goal has been found
        {
            done = true;
            return;
        }

        foreach (MapLocation dir in maze.directions)
        {
            MapLocation neighbour = dir + thisNode.location;
            // если сосед - это стена, то пропустить
            if (maze.map[neighbour.x, neighbour.z] == 1) continue;
            // Если сосед выходит за пределы лабиринта, то пропустить
            if (neighbour.x >= maze.width || neighbour.z >= maze.depth || neighbour.x < 1 || neighbour.z < 1) continue;
            // Если занят закрытым маркером, то пропустить
            if (IsClosed(neighbour)) continue;

            // Вычисляем шаг (каждый шаг маркера это дистанция, по которой он идет. В нашем случае дистанция всегда будет равна 1)
            float G = Vector2.Distance(thisNode.location.ToVector(), neighbour.ToVector()) + thisNode.G;
            // Вычисляем дистанцию до цели
            float H = Vector2.Distance(neighbour.ToVector(), goalNode.location.ToVector());
            float F = G + H;

            // Ставим соседние маркеры ( по одному )
            GameObject pathBlock = Instantiate(path, new Vector3(neighbour.x * maze.scale, 0, neighbour.z * maze.scale), Quaternion.identity);

            // В маркере есть текст, пишем туда значения
            TextMesh[] values = pathBlock.GetComponentsInChildren<TextMesh>();
            values[0].text = "G: " + G.ToString("0.00");
            values[1].text = "H: " + H.ToString("0.00");
            values[2].text = "F: " + F.ToString("0.00");

            if (!UpdateMarker(neighbour, G, H, F, thisNode))
                open.Add(new PathMarker(neighbour, G, H, F, pathBlock, thisNode));
        }

        // Сортируем открытые маркеры при помощи LINQ
        open = open.OrderBy(p => p.F).ThenBy(p => p.H).ToList<PathMarker>();

        // Берем маркер с минимальным значением F и H
        PathMarker pm = (PathMarker)open.ElementAt(0);

        // И закроем его
        closed.Add(pm);
        open.RemoveAt(0);
        pm.marker.GetComponent<Renderer>().material = closedMaterial;

        lastPos = pm;
    }

    /// <summary>
    /// Функция обновления маркера по позиции
    /// </summary>
    /// <param name="pos">Позиция</param>
    /// <param name="prt">Предыдущий маркер (родитель)</param>
    /// <returns>true, если получилось обновить; False, если не получилось</returns>
    bool UpdateMarker(MapLocation pos, float g, float h, float f, PathMarker prt)
    {
        foreach(PathMarker p in open)
        {
            if(p.location.Equals(pos))
            {
                p.G = g;
                p.H = h;
                p.F = f;
                p.parent = prt;
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Проверяет позицию, нет ли там закрытого маркера
    /// </summary>
    /// <param name="marker">Позиция на карте</param>
    /// <returns>true, если занято; false, если свободно</returns>
    bool IsClosed(MapLocation marker)
    {
        // Хотя конечно это лучше сделать в виде свойства у объекта, чем создавать для этого список
        foreach(PathMarker p in closed)
        {
            if (p.location.Equals(marker)) return true;
        }
        return false;
    }

    void GetPath()
    {
        // Убираем все маркеры, они нам не нужны
        RemoveAllMarkers();
        // Берем последний маркер, который дошел до конца пути
        PathMarker begin = lastPos;
        // Начинаем их ставить, пока не дойдем до начального маркера
        while (!startNode.Equals(begin) && begin != null)
        {
            Instantiate(path, new Vector3(begin.location.x * maze.scale, 0, begin.location.z * maze.scale), Quaternion.identity);
            begin = begin.parent;
        }
        // СТавим начальный маркер
        Instantiate(path, new Vector3(startNode.location.x * maze.scale, 0, startNode.location.z * maze.scale), Quaternion.identity);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            BeginSearch();
        if (Input.GetKeyDown(KeyCode.C) && !done) 
            Serach(lastPos);
        if (Input.GetKeyDown(KeyCode.M)) GetPath();
    }
}
