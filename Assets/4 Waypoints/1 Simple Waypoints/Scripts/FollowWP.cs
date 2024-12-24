using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowWP : MonoBehaviour
{
    public GameObject[] waypoints;
    int currentWP = 0;

    public float speed = 10f;
    public float rotSpeed = 10f;
    public float lookAhead = 10f;

    GameObject tracker;

    private void Start()
    {
        tracker = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        DestroyImmediate(tracker.GetComponent<Collider>());
        tracker.GetComponent<MeshRenderer>().enabled = false;
        tracker.transform.position = this.transform.position;
        tracker.transform.rotation = this.transform.rotation;
    }

    void ProgressTracker()
    {
        // Если трекер уедет слишком далеко, то он остановится
        if (Vector3.Distance(tracker.transform.position, this.transform.position) > lookAhead) return;

        if (Vector3.Distance(tracker.transform.position, waypoints[currentWP].transform.position) < 3)
            currentWP++;

        if (currentWP >= waypoints.Length)
            currentWP = 0;
        tracker.transform.LookAt(waypoints[currentWP].transform);

        // Трекер должен быть немного быстрее, чем объект, чтобы тот успевал перейти на другой вейпоинт. 
        // Если трекер будет меделнный, то объект развернется на предыдущий вейпоинт
        // Если трекер будет слишком быстрый, то объект будет пропускать вейпоинты
        tracker.transform.Translate(0, 0, (speed + 2f) * Time.deltaTime);
    }

    void Update()
    {
        ProgressTracker();

        Quaternion lookAtWP = Quaternion.LookRotation(tracker.transform.position - this.transform.position);
        this.transform.rotation = Quaternion.Slerp(transform.rotation, lookAtWP, rotSpeed * Time.deltaTime);

        this.transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
