using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_Base : MonoBehaviour {

    public Vector3 startPoint = Vector3.zero;
    public Vector3 startTangent = Vector3.zero;
    public Vector3 endPoint = Vector3.right * 3f;
    public Vector3 endTangent = Vector3.zero;

    public float linearDist;

    public virtual void Init(Bezier_Base preceding, Bezier_Base first) {
    }

    public Vector3 GetPositionOnPath (float t)
    {
        Vector3 a = Vector3.Lerp(startPoint, startTangent, t);
        Vector3 b = Vector3.Lerp(startTangent, endTangent, t);
        Vector3 c = Vector3.Lerp(endTangent, endPoint, t);

        Vector3 ab = Vector3.Lerp(a, b, t);
        Vector3 bc = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(ab, bc, t);
    }

    public float GetPercForDist(float dist)
    {
        return dist / linearDist;
    }

    public float GetDistForPerc(float perc)
    {
        return perc * linearDist;
    }

    public void RecalculateLinearDist ()
    {
        float dist = 0;

        for (float i = 0; i < 1; i += .01f)
        {
            dist += Vector3.Distance(GetPositionOnPath(i), GetPositionOnPath(i + .01f));
        }

        linearDist = dist;
    }
}
