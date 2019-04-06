using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class AutoAgentBehavior : MonoBehaviour
{
    public Vector3 moveDirection;
    public float moveVelocityMagnitude;

    public Transform myModelTransform;

    [Range(0.0f, 1.0f)] public float clumpStrength;
    [Range(0.0f, 1.0f)] public float alignStrength;
    [Range(0.0f, 1.0f)] public float avoidStrength;
    [Range(0.0f, 1.0f)] public float originStrength;
    [Range(0f, 10f)] public float clumpStrengthMod;
    public AnimationCurve clumpStrengthCurve;
    public int clumpSizeCap = 50;
    [Range(0.0000001f, 1000f)] public float originStrengthMod;

    public float positionSmoothing = 0.05f;
    public float rotationSmoothing = 0.05f;

    public float defaultAnimFrequency = 10f;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;
        
    void Start()
    {
        myModelTransform = transform.GetChild(0);
        rend = myModelTransform.GetComponentInChildren<Renderer>();
        propBlock = new MaterialPropertyBlock();

        myModelTransform.localScale *= Random.Range(0.5f, 1f);
        
        ChangeFlapOffset();
    }

    public void PassArrayOfContext(Collider[] context)
    {
        List<Collider> contextWithoutMe = new List<Collider>();

        foreach (Collider c in context)
        {
            if (c != null && c.gameObject != gameObject)
                contextWithoutMe.Add(c);
        }

        CalcMyDir(contextWithoutMe.ToArray());
        UpdateFlapSpeed(contextWithoutMe.Count);
        MoveInMyAssignedDirection(moveDirection, moveVelocityMagnitude);
    }

    void ChangeFlapOffset()
    {
        rend.GetPropertyBlock(propBlock);
        
        propBlock.SetFloat("_PhaseShift", Random.Range(0f, 100f));
        
        rend.SetPropertyBlock(propBlock);
    }
    
    void UpdateFlapSpeed(int clumpSize)
    {
        rend.GetPropertyBlock(propBlock);
        
        propBlock.SetFloat("_AnimFrequency", Mathf.Clamp(Mathf.Pow((clumpSize * 1.0f) / clumpSizeCap, 8f), 0f, 1f) * defaultAnimFrequency);
        
        rend.SetPropertyBlock(propBlock);
    }
    
    void CalcMyDir(Collider[] context)
    {
        moveDirection = Vector3.Lerp(
            moveDirection, 
            Vector3.Normalize(
                ClumpDir(context) * clumpStrength * clumpStrengthCurve.Evaluate(Mathf.Clamp(context.Length, 0f, clumpSizeCap)) + 
                Align(context) * alignStrength +
                Avoidance(context) * avoidStrength +
                MoveTowardsOrigin() * originStrength * Vector3.Magnitude(transform.position) / originStrengthMod), 
            positionSmoothing);
    }

    Vector3 ClumpDir (Collider[] context)
    {
        if (context.Length == 0)
            return Vector3.zero;
        
        Vector3 midpoint = Vector3.zero;

        foreach (Collider c in context)
        {
            midpoint += c.transform.position;
        }

        midpoint /= context.Length;

        Vector3 dirIWantToGo = midpoint - transform.position;

        Vector3 normalizedDirIWantToGo = Vector3.Normalize(dirIWantToGo);

        return normalizedDirIWantToGo;
    }

    Vector3 Align (Collider[] context)
    {
        Vector3 headings = Vector3.zero;

        foreach (Collider c in context)
        {
            headings += c.transform.GetChild(0).forward;
        }

        headings /= context.Length;

        return Vector3.Normalize(headings);
    }

    Vector3 Avoidance (Collider[] context)
    {
        if (context.Length == 0)
            return Vector3.zero;
        
        Vector3 midpoint = Vector3.zero;

        foreach (Collider c in context)
        {
            midpoint += c.transform.position;
        }

        midpoint /= context.Length;

        Vector3 dirIWantToGo = midpoint - transform.position;

        Vector3 normalizedDirIWantToGo = Vector3.Normalize(dirIWantToGo);

        return -normalizedDirIWantToGo;
    }

    Vector3 MoveTowardsOrigin()
    {
        return Vector3.zero - transform.position;
    }

    void MoveInMyAssignedDirection(Vector3 direction, float magnitude)
    {
        transform.position += direction * magnitude * Time.deltaTime;
        myModelTransform.rotation = Quaternion.Slerp(myModelTransform.rotation, Quaternion.LookRotation(direction), rotationSmoothing);
    }
}
