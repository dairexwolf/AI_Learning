using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class AIShell : MonoBehaviour
{
    public GameObject explosion;
    Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Задаем локальный вектор "вперед" равный передвижению снаряда по параболе, чтобы моделька корректно отображала, куда она летит
        transform.forward = rb.velocity;
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.CompareTag("tank"))
        {
            GameObject exp = Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(exp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
