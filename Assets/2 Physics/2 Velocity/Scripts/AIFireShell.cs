using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFireShell : MonoBehaviour
{
    public GameObject bullet;
    public GameObject turret;
    public GameObject enemy;
    public Transform turretBase;

    public float rotationSpeed = 1f;
    float speed = 15;

    float moveSpeed = 3;

    // Update is called once per frame
    void Update()
    {
        // Вычисление векторов и позиции, куда надо смотреть
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Сглаживание, чтобы поворот был не рванным
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);

        float? angle = RotateTurret();
        if (Input.GetKeyDown(KeyCode.Space) && angle is not null)
        {
            CreateBullet();
        }
        else if(angle is null)
        {
            transform.Translate(0, 0, Time.deltaTime * moveSpeed);
        }
    }

    void CreateBullet()
    {
        GameObject shell = Instantiate(bullet, turret.transform.position, turret.transform.rotation);
        // Дадим ему ускорение по U физике
        shell.GetComponent<Rigidbody>().velocity = speed * turretBase.forward;
    }

    float? CalculateAngle(bool low)
    {
        Vector3 targetDir = enemy.transform.position - transform.position;
        float y = targetDir.y;
        targetDir.y = 0f;
        float x = targetDir.magnitude - 1.5f;
        float gravity = 9.8f;
        float sSqr = speed * speed;
        float underSqrRoot = (sSqr * sSqr) - gravity * (gravity * x * x + 2 * y * sSqr);

        // Чтобы не было проблем с минусом
        if (underSqrRoot >= 0f)
        {
            float root = Mathf.Sqrt(underSqrRoot);
            // Самый высокий угол
            float hightAngle = sSqr + root;
            // Низкий угол
            float lowAngle = sSqr - root;

            if (low)
                return Mathf.Atan2(lowAngle, gravity * x) * Mathf.Rad2Deg;
            else
                return Mathf.Atan2(hightAngle, gravity * x) * Mathf.Rad2Deg;
        }
        else
            return null;
    }

    float? RotateTurret()
    {
        float? angle = CalculateAngle(true);
        if(angle is not null)
        {
            // вращаем турельку по x коордиате
            turretBase.localEulerAngles = new Vector3(360f - (float)angle, 0f, 0f);
        }
        return angle;
    }
}
