using UnityEngine;

public class Move : MonoBehaviour {

    public GameObject goal;
    Vector3 direction;
    float speed = 0.5f;

    void Start() 
    {

        direction = goal.transform.position - transform.position;
        // transform.position += direction; // Это тоже самое, что и translate
        
    }

    private void LateUpdate() 
    {
        Vector3 velocity = direction.normalized * speed * Time.deltaTime;
        transform.Translate(velocity);
    }
}
