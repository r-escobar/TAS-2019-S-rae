using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bezier_LoopConnect : Bezier_Base
{
    public float startTangentMagnitude = 1;
    public float endTangentMagnitude = 1;

    public Bezier_Base precedingBez;
    public Bezier_Base firstBez;

    public override void Init(Bezier_Base bb, Bezier_Base bf)
    {
        precedingBez = bb;
        firstBez = bf;
    }
}
