using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierExample))]
public class DrawBezierExample : Editor
{
    private void OnSceneViewGUI(SceneView sv)
    {
        BezierExample be = target as BezierExample;

        if (be.prevBez != null)
        {
            // this is either a middle or loop bezier
            
            be.startPoint = be.prevBez.endPoint;

            Vector3 oldTanDir = be.prevBez.endPoint - be.prevBez.endTangent;
            Vector3 oppositeDir = be.startPoint + oldTanDir * be.startTangentMagnitude;

            be.startTangent = oppositeDir;
            
            Handles.DrawSolidDisc(be.startPoint, Vector3.forward, 0.5f);
            Handles.DrawSolidDisc(be.startTangent, Vector3.back, 0.3f);
        }
        else
        {
            be.startPoint = Handles.PositionHandle(be.startPoint, Quaternion.identity);
            be.startTangent = Handles.PositionHandle(be.startTangent, Quaternion.identity);
        }

        if (be.baseBez != null)
        {
            // this is definitely a loop bezier
            
            be.startPoint = be.prevBez.endPoint;

            Vector3 oldTanDir = be.startPoint - be.prevBez.endTangent;
            Vector3 oppositeDir = be.startPoint + oldTanDir * be.startTangentMagnitude;

            be.startTangent = oppositeDir;

            be.endPoint = be.baseBez.startPoint;

            Vector3 firstTanDir = be.endPoint - be.baseBez.startTangent;
            Vector3 oppositeDirForFirst = be.endPoint + firstTanDir * be.endTangentMagnitude;

            be.endTangent = oppositeDirForFirst;
            
            Handles.DrawSolidDisc(be.endPoint, Vector3.forward, 0.5f);
            Handles.DrawSolidDisc(be.endTangent, Vector3.back, 0.3f);
        }
        else
        {
            be.endPoint = Handles.PositionHandle(be.endPoint, Quaternion.identity);
            be.endTangent = Handles.PositionHandle(be.endTangent, Quaternion.identity);
        }

        Handles.DrawBezier(be.startPoint, be.endPoint, be.startTangent, be.endTangent, Color.red, null, 2f);
        Handles.DrawLine(be.startPoint, be.startTangent);
        Handles.DrawLine(be.endPoint, be.endTangent);
    }

    void OnEnable()
    {
        //Debug.Log("OnEnable");
        SceneView.onSceneGUIDelegate += OnSceneViewGUI;
    }

    void OnDisable()
    {
        //Debug.Log("OnDisable");
        SceneView.onSceneGUIDelegate -= OnSceneViewGUI;
    }
}