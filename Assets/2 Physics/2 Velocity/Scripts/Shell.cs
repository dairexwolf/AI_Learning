using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shell : MonoBehaviour
{
    public GameObject explosion;
    float speed = 0f;
    float ySpeed = 0f;

    public float force = 10;
    float mass = 10;
    float acceleration;
    float drag = 1;
    float gravity = -9.80665f;
    float gAccel;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("tank"))
        {
            GameObject exp = Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(exp, 0.5f);
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ускорение
        acceleration = force / mass;
        // Скорость
        speed += acceleration * 1;
        // Ускорение гравитационное
        gAccel = gravity / mass;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // Замедление снаряда со временем
        speed *= (1 - Time.deltaTime * drag);
        // Увеличение скорости падения снаряда со временем
        ySpeed += gAccel * Time.deltaTime;
        // Движение
        transform.Translate(0, ySpeed, speed * Time.deltaTime);
    }
}
