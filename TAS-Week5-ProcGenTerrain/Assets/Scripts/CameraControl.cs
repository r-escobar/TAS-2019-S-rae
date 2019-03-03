using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{

    public float moveSpeed = 10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Input.GetAxis("Horizontal") * Vector3.right * moveSpeed * Time.deltaTime;
        transform.position += Input.GetAxis("Vertical") * Vector3.forward * moveSpeed * Time.deltaTime;
    }
}
