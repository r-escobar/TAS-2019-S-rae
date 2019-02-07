using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_Intermediate : Bezier_Base
{
    public float startTangentMagnitude = 1;

    public Bezier_Base precedingBez;

    public override void Init(Bezier_Base bb, Bezier_Base bf) {
        precedingBez = bb;

        endPoint = precedingBez.endPoint + Vector3.forward;
        endTangent = endPoint - Vector3.forward / 2;
    }

}
