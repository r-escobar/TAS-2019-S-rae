using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(Bezier_First))]
public class BezierDraw_First : Editor
{

    private void OnSceneGUI()
    {
        Bezier_First bb = target as Bezier_First;

        bb.startPoint = Handles.PositionHandle(bb.startPoint, Quaternion.identity);
        bb.startTangent = Handles.PositionHandle(bb.startTangent, Quaternion.identity);
        bb.endPoint = Handles.PositionHandle(bb.endPoint, Quaternion.identity);
        bb.endTangent = Handles.PositionHandle(bb.endTangent, Quaternion.identity);

        Handles.DrawBezier(bb.startPoint, bb.endPoint, bb.startTangent, bb.endTangent, Color.red, null, 2f);
        Handles.DrawLine(bb.startPoint, bb.startTangent);
        Handles.DrawLine(bb.endPoint, bb.endTangent);
    }
}

[CustomEditor(typeof(Bezier_Intermediate))]
public class BezierDraw_Intermediate : Editor
{

    private void OnSceneGUI()
    {
        Bezier_Intermediate bb = target as Bezier_Intermediate;

        bb.startPoint = bb.precedingBez.endPoint;

        Vector3 oldTanDir = bb.precedingBez.endPoint - bb.precedingBez.endTangent;
        Vector3 oppositeDir = bb.startPoint + oldTanDir * bb.startTangentMagnitude;

        bb.startTangent = oppositeDir;

        
        bb.startPoint = Handles.PositionHandle(bb.startPoint, Quaternion.identity);
        bb.startTangent = Handles.PositionHandle(bb.startTangent, Quaternion.identity);
        bb.endPoint = Handles.PositionHandle(bb.endPoint, Quaternion.identity);
        bb.endTangent = Handles.PositionHandle(bb.endTangent, Quaternion.identity);

        Handles.DrawBezier(bb.startPoint, bb.endPoint, bb.startTangent, bb.endTangent, Color.red, null, 2f);
        Handles.DrawLine(bb.startPoint, bb.startTangent);
        Handles.DrawLine(bb.endPoint, bb.endTangent);
    }
}

[CustomEditor(typeof(Bezier_LoopConnect))]
public class BezierDraw_LoopConnect : Editor
{

    private void OnSceneGUI()
    {
        Bezier_LoopConnect bb = target as Bezier_LoopConnect;

        bb.startPoint = bb.precedingBez.endPoint;

        Vector3 oldTanDir = bb.startPoint - bb.precedingBez.endTangent;
        Vector3 oppositeDir = bb.startPoint + oldTanDir * bb.startTangentMagnitude;

        bb.startTangent = oppositeDir;

        bb.endPoint = bb.firstBez.startPoint;

        Vector3 firstTanDir = bb.endPoint - bb.firstBez.startTangent;
        Vector3 oppositeDirForFirst = bb.endPoint + firstTanDir * bb.endTangentMagnitude;

        bb.endTangent = oppositeDirForFirst;

        Handles.DrawBezier(bb.startPoint, bb.endPoint, bb.startTangent, bb.endTangent, Color.red, null, 2f);
    }
}
#endif
