using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{

    NavMeshAgent agent;
    public GameObject target;
    DrivePoliceman targetDrive;

    // Переменные для функции Wonder
    /// <summary>
    /// Радиус круга
    /// </summary>
    public float wanderRadius = 10f;
    /// <summary>
    /// Дистанция до середины вооброжаемого круга
    /// </summary>
    public float wanderDistance = 20f;
    /// <summary>
    /// Значение, которая устанавливает, насколько сильно НИП будет поворачиваться 
    /// </summary>
    public float wanderJitter = 1f;
    /// <summary>
    /// Место, куда пойдет бродить НИП
    /// </summary>
    Vector3 wanderTarget = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        targetDrive = target.GetComponent<DrivePoliceman>();
    }

    // Update is called once per frame
    void Update()
    {
        CleverHide();
    }

    bool CanSeeTarget()
    {
        RaycastHit raycastInfo;
        // Vector3 rayToTarget = this.tr;
        return true;
    }

    /// <summary>
    /// Преследование цели по заданой координате
    /// </summary>
    /// <param name="location"></param>
    void Seek(Vector3 location)
    {
        agent.SetDestination(location);

    }

    // Уход от цели (который находится на заданой координате)
    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - this.transform.position;
        agent.SetDestination(this.transform.position - fleeVector);
    }

    /// <summary>
    /// Преследование цели
    /// </summary>
    void Pursue()
    {
        // Дистанция до цели
        Vector3 targetDir = target.transform.position - this.transform.position;

        // Рассчитываем угол между вектором вперед у преследователя и преследуемого. Позволяет понять, достаточно ли они идут параллельно, чтобы предсказывать траекторию
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // Рассчитываем угол между вектором движения преследующего и дистанцией для цели. Позволяет понять, спереди или сзади находится цель
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // Если цель перестала двигаться, то преследователь следует за ней
        // И если цель сзади (toTarget > 90) и их пути не пересекаются (relativeHeading < 20)
        if ((toTarget > 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }


        // Предсказание магнитуды, как дойти до цели
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // Преследование цели с предсказанием.
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    /// <summary>
    /// Уклонение от цели
    /// </summary>
    void Evade()
    {
        // Дистанция до цели
        Vector3 targetDir = target.transform.position - this.transform.position;

        // Рассчитываем угол между вектором вперед у преследователя и преследуемого. Позволяет понять, достаточно ли они идут параллельно, чтобы предсказывать траекторию
        float relativeHeading = Vector3.Angle(transform.forward, this.transform.TransformVector(target.transform.forward));
        // Рассчитываем угол между вектором движения преследующего и дистанцией для цели. Позволяет понять, спереди или сзади находится цель
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        // Если цель перестала двигаться, то просто бежим
        // И если цель спереди (toTarget > 90) и их пути не пересекаются (relativeHeading < 20)
        if ((toTarget < 90 && relativeHeading < 20) || targetDrive.currentSpeed < 0.01f)
        {
            Flee(target.transform.position);
            return;
        }


        // Предсказание магнитуды, как дойти до цели
        float lookAhead = targetDir.magnitude / (agent.speed + targetDrive.currentSpeed);
        // Уход в сторону, используя предсказание
        Flee(target.transform.position + target.transform.forward * lookAhead * 2);
    }

    /// <summary>
    /// Брождение НИПа
    /// </summary>
    void Wander()
    {
        // Устанавливаем направление, куда пойдет НИП. Делается это при помощи рандома
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        // Нормализуем магнитуду вектора
        wanderTarget.Normalize();
        // И устанавливаем в радиус круга
        wanderTarget *= wanderRadius;

        // Через какое расстояние будет центр круга в локальных координатах
        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        // Переводим их в глобальные координаты, чтобы послать их в NavMeshAgent
        Vector3 targetWorld = gameObject.transform.InverseTransformVector(targetLocal);

        // И ведем туда НИПа
        Seek(targetWorld);
    }

    /// <summary>
    /// Попытка НИПа спрятаться
    /// </summary> 
    void Hide()
    {
        // Инициализация переменных
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;

        // Получение всех объектов, за которыми можно спрятаться
        int len = World.Instance.GetHidingSpots().Length;

        // Перебор всех препятсвий и выбор ближайшего
        for (int i = 0; i < len; i++)
        {
            Vector3 hideDir = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            // Умножаем на 10, чтобы вынести координату за препятствие
            Vector3 hidePos = World.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10;

            if (Vector3.Distance(this.transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                dist = Vector3.Distance(this.transform.position, hidePos);
            }
        }

        Seek(chosenSpot);
    }

    /// <summary>
    /// Более продвинутое поведение НИП при попытке спрятаться.
    /// </summary>
    void CleverHide()
    {
        // Инициализация переменных
        float dist = Mathf.Infinity;
        Vector3 chosenSpot = Vector3.zero;
        // Запоминаем направление, куда выберет идти НИП
        Vector3 chosenDir = Vector3.zero;
        // Выбранное НИПом препятствие
        GameObject chosenGO = World.Instance.GetHidingSpots()[0];

        // Получение всех объектов, за которыми можно спрятаться
        int len = World.Instance.GetHidingSpots().Length;

        // Перебор всех препятствий и выбор ближайшего
        for (int i = 0; i < len; i++)
        {
            Vector3 hideDir = World.Instance.GetHidingSpots()[i].transform.position - target.transform.position;
            // Умножаем на 10, чтобы вынести координату за препятствие
            Vector3 hidePos = World.Instance.GetHidingSpots()[i].transform.position + hideDir.normalized * 10;

            // Если выбираем, то записываем все в ранее обозначенные позиции
            if (Vector3.Distance(this.transform.position, hidePos) < dist)
            {
                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = World.Instance.GetHidingSpots()[i];
                dist = Vector3.Distance(this.transform.position, hidePos);

            }
        }

        // Берем коллайдер выбранного объекта
        Collider hideCol = chosenGO.GetComponent<Collider>();
        // Создаем объект луча из выбранного направления и направляем его обратно
        Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
        // Инициализируем параметры луча
        RaycastHit info;
        float rayDistance = 100f;
        // Создаем луч. Отличие от  Physics коллайдера в том, что он игнорирует все коллайдеры, кроме этого
        hideCol.Raycast(backRay, out info, rayDistance);
        Debug.DrawRay(chosenSpot, -chosenDir.normalized * 10, Color.red);

        // Направляем НИПа к точке, куда попал луч, плюс небольшое смещение в направлении chosenDir.
        Seek(info.point + chosenDir.normalized * 5);
    }

}
