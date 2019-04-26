using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMover : MonoBehaviour
{
    public float walkSpeed;
    public float runSpeed;
    public float speedSmoothing = 0.5f;

    public float rotSpeed = 1f;
    
    public Vector3 tgtForward;
    
    private float speed;

    public AnimatorParameterController animScript;
    
    // Start is called before the first frame update
    void Start()
    {
        tgtForward = transform.forward;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }
        } else
        {
            speed = 0f;
        }



        if (speed != 0f)
        {
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(Vector3.up, rotSpeed);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(Vector3.up, -rotSpeed);
            }
            
            animScript.walkRunBlendTotal = (speed - walkSpeed) / (runSpeed - walkSpeed);
            animScript.isIdling = false;
        }
        else
        {
            animScript.isIdling = true;
        }

        transform.position += -transform.forward * speed;

        //transform.rotation = Quaternion.look

    }
}
