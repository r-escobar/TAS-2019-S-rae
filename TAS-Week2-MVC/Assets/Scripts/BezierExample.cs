using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BezierExample : MonoBehaviour
{

    public Vector3 startPoint = Vector3.zero;
    public Vector3 startTangent = Vector3.zero;
    public Vector3 endPoint = Vector3.right * 3f;
    public Vector3 endTangent = Vector3.zero;

    public BezierExample prevBez;
    public BezierExample baseBez;
    
    public float startTangentMagnitude = 1f;
    public float endTangentMagnitude = 1f;

    public float approxCurveLength = 0f;

    public void Init(BezierExample pb, BezierExample bb)
    {
        if (pb != null)
        {
            // this is either a middle or loop bezier
            prevBez = pb;
            
            endPoint = pb.endPoint + Vector3.forward;
            endTangent = endPoint - Vector3.forward / 2f;
        }

        if (bb != null)
        {
            // this is definitely a loop bezier
            baseBez = bb;
        }
    }

    public Vector3 EvaluateCurve(float t)
    {
        Vector3 a = Vector3.Lerp(startPoint, startTangent, t);
        Vector3 b = Vector3.Lerp(startTangent, endTangent, t);
        Vector3 c = Vector3.Lerp(endTangent, endPoint, t);

        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    public float GetPercentageProgress(float curDist)
    {
        return curDist / approxCurveLength;
    }
    
    public void ApproximateCurveLength ()
    {
        float dist = 0;

        for (float i = 0; i < 1; i += 0.01f)
        {
            dist += Vector3.Distance(EvaluateCurve(i), EvaluateCurve(i + 0.01f));
        }

        approxCurveLength = dist;
    }
}
