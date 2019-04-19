using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorParameterController : MonoBehaviour
{
    private float walk_Blend_X;
    private float walk_Blend_Y;

    private float time;

    private Animator myAnimator;

    [Header("Tuning Values")] 
    [Range(0.001f, 10f)]
    public float walkCycleTime;
    
    [Range(0.001f, 10f)]
    public float stepsPerSecond;
    
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        time += (Mathf.PI * 2f * Time.deltaTime) / walkCycleTime;

        walk_Blend_X = Mathf.Cos(time);
        walk_Blend_Y = Mathf.Sin(time);
        
        myAnimator.SetFloat("Walk_TreeVal_X", walk_Blend_X);
        myAnimator.SetFloat("Walk_TreeVal_Y", walk_Blend_Y);


    }
}
