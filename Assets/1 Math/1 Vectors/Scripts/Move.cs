using UnityEngine;

public class Move : MonoBehaviour {

    public GameObject goal;
    Vector3 direction;
    public float speed = 3f;

    void Start()
    {

        direction = goal.transform.position - this.transform.position;
        direction.y = 0;
        // transform.position += direction; // Это тоже самое, что и translate

    }

    private void LateUpdate() 
    {
        this.transform.LookAt(goal.transform.position);
        direction = goal.transform.position - this.transform.position;
        direction.y = 0;
        if (direction.magnitude > 2f)
        {
            Vector3 velocity = direction.normalized * speed * Time.deltaTime;
            this.transform.position += velocity;
        }
    }
}
