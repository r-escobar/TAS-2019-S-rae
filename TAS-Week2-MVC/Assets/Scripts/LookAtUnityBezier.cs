using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LookAtUnityBezier : MonoBehaviour
{
    [Header("Public References")]
    public GameObject marker;
    //public BezierExample bezEx;
    public Transform myModel;
    public Transform camTransform;

    [Header("List of Curves")]
    public List<BezierExample> curveList = new List<BezierExample>();

    
    [Header("Movement Variables")]
    private float t;
    private Vector3 oldCamPos;

    public float moveSpeed = 10f;
    public float rotationSmoothing = 0.1f;

    private void Update()
    {
        int curCurveIndex = (int) t;

        Vector3 newCamPos;
        oldCamPos = camTransform.position;
        
        if (curCurveIndex >= curveList.Count)
        {
            t -= curCurveIndex;
            curCurveIndex = 0;
        }
        
        newCamPos = curveList[curCurveIndex].EvaluateCurve(t - curCurveIndex);
        camTransform.position = newCamPos;

        Vector3 lookVector = (newCamPos - oldCamPos).normalized;   
        camTransform.rotation = Quaternion.Slerp(camTransform.rotation, Quaternion.LookRotation(lookVector, Vector3.up), rotationSmoothing);


        t += Time.deltaTime * moveSpeed;
    }
}

[CustomEditor(typeof(LookAtUnityBezier))]
public class ButtonsForLAUB : Editor
{
    private bool hasBeenClosed = false;
    
    public override void OnInspectorGUI()
    {
        LookAtUnityBezier myLAUB = (LookAtUnityBezier) target;
        
        DrawDefaultInspector();
        
        if (GUILayout.Button("New Curve"))
        {
            if (!hasBeenClosed)
            {
                BezierExample newBezEx = myLAUB.myModel.gameObject.AddComponent<BezierExample>();

                if (myLAUB.curveList.Count > 0)
                {                
                    newBezEx.Init(myLAUB.curveList[myLAUB.curveList.Count - 1], null);
                }
            
                myLAUB.curveList.Add(newBezEx);
            }
            else
            {
                Debug.LogError("Cannot add a curve to a closed loop!");
            }
            

        }

        if (GUILayout.Button("Close Curve"))
        {
            if (!hasBeenClosed)
            {
                BezierExample newBezEx = myLAUB.myModel.gameObject.AddComponent<BezierExample>();

                if (myLAUB.curveList.Count > 0)
                {
                    newBezEx.Init(myLAUB.curveList[myLAUB.curveList.Count - 1], myLAUB.curveList[0]);
                }
            
                myLAUB.curveList.Add(newBezEx);

                hasBeenClosed = true;
            }
            else
            {
                Debug.LogError("The loop has already been closed!");
            }

            
        }

        if (GUILayout.Button("Delete Last Curve"))
        {
            if (myLAUB.curveList.Count > 0)
            {
                BezierExample bezEzToRemove = myLAUB.curveList[myLAUB.curveList.Count - 1];
                myLAUB.curveList.RemoveAt(myLAUB.curveList.Count - 1);
                DestroyImmediate(bezEzToRemove);

                hasBeenClosed = false;
            }
            else
            {
                Debug.LogError("There are no curves to delete!");
            }
        }

        if (GUILayout.Button("Calculate Curve Lengths"))
        {
            if (myLAUB.curveList.Count > 0)
            {
                for (int i = 0; i < myLAUB.curveList.Count; i++)
                {
                    myLAUB.curveList[i].ApproximateCurveLength();
                }
            }
            else
            {
                Debug.LogError("There are no curves to calculate!");
            }
        }


    }
}