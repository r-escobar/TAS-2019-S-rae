using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitAroundPoint : MonoBehaviour
{

    public Vector3 origin = new Vector3(-0.325474f, 0f, 0f);
    public Vector3 rotationAxis = Vector3.forward;
    public float radius = 10f;
    public float rotationSpeed = 10f;
    public float radiusSpeed = 10f;

    // Start is called before the first frame update
    void Start()
    {
        if(origin.x == -0.325474f)
            origin = transform.position;
        transform.position = (transform.position - origin).normalized * radius + origin;
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround (origin, rotationAxis, rotationSpeed * Time.deltaTime);
        var desiredPosition = (transform.position - origin).normalized * radius + origin;
        transform.position = Vector3.MoveTowards(transform.position, desiredPosition, Time.deltaTime * radiusSpeed);
    }
}
