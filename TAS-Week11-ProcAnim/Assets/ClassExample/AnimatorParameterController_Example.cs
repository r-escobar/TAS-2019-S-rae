using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterController_Example : MonoBehaviour
{
    private float walkRun_TreeVal_X;
    private float walkRun_TreeVal_Y;

    private float time;

    private float idleTime;

    private Animator _myAnimator;

    [Header("Tuning Values")]
    [Range(0.001f, 10.0f)] public float walkCycleTime;
    [Range(0.00f, 1.00f)] public float walkRunMagnitude;

    [Range(0.00f, 1.00f)] public float walkRunBlendTotal;


    //[Range(0.001f, 10.0f)] public float stepsPerSecond; // 2 * (1/(walkCycleTime)

    //Soh - opposite/hypotenuse
    //Cah - adjacent/hypotenuse
    //Toa - opposite/adjacent

    private void Start()
    {
        _myAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.W))
            _myAnimator.SetBool("Idle_False_Move_True", true);
        else
            _myAnimator.SetBool("Idle_False_Move_True", false);

        idleTime += Time.deltaTime * 6;
        _myAnimator.SetFloat("Idle_TreeVal_X", (Mathf.Sin(idleTime)+1)/2);
  


        walkCycleTime = 1 - (.5f * walkRunBlendTotal);
        walkRunMagnitude = .25f + (.75f * walkRunBlendTotal);

        time += (Mathf.PI * 2 * Time.deltaTime) / walkCycleTime;

        walkRun_TreeVal_X = Mathf.Cos(time) * walkRunMagnitude;
        walkRun_TreeVal_Y = Mathf.Sin(time) * walkRunMagnitude;

        _myAnimator.SetFloat("WalkRun_TreeVal_X", walkRun_TreeVal_X);
        _myAnimator.SetFloat("WalkRun_TreeVal_Y", walkRun_TreeVal_Y);
    }
}
