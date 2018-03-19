using UnityEngine;

public class Spin : MonoBehaviour {

    public Transform Orbit;

    public float angle = 0;
    public float speedAngle = 0;
    public float radius = 2f;

    void Start() {
        float circonference = (2 * Mathf.PI) * Vector3.Distance(transform.position, Orbit.position);
    }

    void Update() {
        SpinAround();
    }

    void SpinAround() {
        angle += speedAngle * Time.deltaTime;
        float x = radius * Mathf.Cos(angle) + (transform.position.x);
        float y = radius * Mathf.Sin(angle) + (transform.position.z);

        Orbit.transform.position = new Vector3(x, transform.position.y, y);
    }
}
