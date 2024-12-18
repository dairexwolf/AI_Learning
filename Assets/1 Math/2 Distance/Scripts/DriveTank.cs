using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.UIElements;

// A very simplistic car driving on the x-z plane.

public class DriveTank : MonoBehaviour
{
    public float speed = 10.0f;
    public float rotationSpeed = 20f;
    float rotationSpeedForAutopilot;

    public GameObject fuel;
    bool autopilot = false;

    void Start()
    {
        rotationSpeedForAutopilot = rotationSpeed * 0.00005f;
    }

    void LateUpdate()
    {
        // Get the horizontal and vertical axis.
        // By default they are mapped to the arrow keys.
        // The value is in the range -1 to 1
        float translation = Input.GetAxis("Vertical") * speed;
        float rotation = Input.GetAxis("Horizontal") * rotationSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        transform.Translate(0, translation, 0);

        // Rotate around our y-axis
        transform.Rotate(0, 0, -rotation);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CalculateDistanceDebug();
            CalculateAngleDebug();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            autopilot = !autopilot;
        }

        if (CalculateDistance() < 2f && autopilot)
            autopilot = !autopilot;

        if (autopilot)
            AutoPilot();
    }

    void CalculateDistanceDebug()
    {
        float distance = 0;
        distance = Mathf.Sqrt(Mathf.Pow(fuel.transform.position.x - transform.position.x, 2f) + Mathf.Pow(fuel.transform.position.y - transform.position.y, 2f));
        //Unity distance
        float uDistance = Vector3.Distance(fuel.transform.position, transform.position);        // отличается от нашей дистанции, т.к. еще учитывается z - координата
                                                                                                // можно использовать объекты класса Vector3 и сбросить там z-координату
        Vector3 tankToFuel = fuel.transform.position - transform.position;
        Debug.Log("Distance: " + distance + "; Unity distance: " + uDistance + "; V Magnitude: " + tankToFuel.magnitude + "; V SqrMagnitude: " + tankToFuel.sqrMagnitude);
    }

    float CalculateDistance()
    {
        float distance;
        distance = Mathf.Sqrt(Mathf.Pow(fuel.transform.position.x - transform.position.x, 2f) + Mathf.Pow(fuel.transform.position.y - transform.position.y, 2f));
        return distance;
    }

    void CalculateAngleDebug()
    {
        Vector3 tankForward = transform.up;
        Vector3 fuelDirection = fuel.transform.position - transform.position;

        Debug.DrawRay(transform.position, tankForward * 10, Color.green, 2);
        Debug.DrawRay(transform.position, fuelDirection, Color.red, 2);

        Debug.Log("tankForward x: " + tankForward.x + "; tankForward y: " + tankForward.y);

        // Скалярное произведение
        float dot = tankForward.x * fuelDirection.x + tankForward.y * fuelDirection.y;

        // Это вектора, которые уже имеют длину и идут от танка до своих целей, поэтому нам второй вектор не нужен. Вычисляем магнитуды векторов
        float tankMagitude = Mathf.Sqrt(Mathf.Pow(tankForward.x, 2f) + Mathf.Pow(tankForward.y, 2f));
        float fuelMagnitude = Mathf.Sqrt(Mathf.Pow(fuelDirection.x, 2f) + Mathf.Pow(fuelDirection.y, 2f));

        Debug.Log(tankMagitude + "; Unity tank: " + tankForward.magnitude + "; " + fuelMagnitude + "; Unity fuel: " + fuelDirection.magnitude);

        // Угол
        float angle = Mathf.Acos(dot / (tankMagitude * fuelMagnitude));

        // Угол с магнитудой от юнити
        float angle2 = Mathf.Acos(dot / (tankForward.magnitude * fuelDirection.magnitude));

        Debug.Log("Dot: " + dot + "; Unity dot: " + Vector3.Dot(tankForward, fuelDirection));
        Debug.Log("Angle: " + angle * Mathf.Rad2Deg + "; Angle2 :" + angle2 * Mathf.Rad2Deg + "; Unity Angle: " + Vector3.Angle(tankForward, fuelDirection));


        int clockwise = 1;
        // Если z > 0, то цель находится под углом по часовой стрелки, если z<0, то цель находится под углом против часовой стрелки
        if (Cross(tankForward, fuelDirection).z < 0)
            clockwise = -1;



        // Разворачиваем танк на топливо 
        transform.Rotate(0, 0, angle * Mathf.Rad2Deg * clockwise);
    }

    void CalculateAngle()
    {
        Vector3 tankForward = transform.up;
        Vector3 fuelDirection = fuel.transform.position - transform.position;

        Debug.DrawRay(transform.position, tankForward * 10, Color.green, 2);
        Debug.DrawRay(transform.position, fuelDirection, Color.red, 2);

        float dot = tankForward.x * fuelDirection.x + tankForward.y * fuelDirection.y;
        float angle = Mathf.Acos(dot / (tankForward.magnitude * fuelDirection.magnitude));
        int clockwise = 1;
        // Если z > 0, то цель находится под углом по часовой стрелки, если z<0, то цель находится под углом против часовой стрелки
        if (Cross(tankForward, fuelDirection).z < 0)
            clockwise = -1;
        // Разворачиваем танк на топливо 
        if (angle * Mathf.Rad2Deg > 5)
            // transform.Rotate(0, 0, angle * Mathf.Rad2Deg * clockwise * Time.deltaTime * rotationSpeed * 0.05f); // Вариант из курса, но он не верный, т.к. угол напрямую влияет на скорость поворота
            transform.Rotate(0, 0, clockwise * Time.deltaTime * rotationSpeed); // угол влияет только на то, в какую сторону надо поворачиваться
    } 

    // Нахождение векторного произведения
    Vector3 Cross(Vector3 v, Vector3 w)
    {
        float xMult = v.y * w.z - v.z * w.y;
        float yMult = v.x * w.z - v.z * w.x;
        float zMult = v.x * w.y - v.y * w.x;

        return new Vector3(xMult, yMult, zMult);
    }

    void AutoPilot()
    {
        CalculateAngle();
        transform.position += transform.up * speed * Time.deltaTime;
    }
}