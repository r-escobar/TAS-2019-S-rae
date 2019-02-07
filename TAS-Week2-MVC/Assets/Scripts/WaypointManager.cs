using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WaypointManager : MonoBehaviour {

    public List<Bezier_Base> waypoints = new List<Bezier_Base>();
    public float speed;



    private bool _loopClosed;

    public void MakeWaypoint()
    {
        if (!_loopClosed)
        {
            if (waypoints.Count == 0)
            {
                waypoints.Add(GameObject.Find("Model").AddComponent<Bezier_First>());
                waypoints[0].Init(null, null);
            }
            else
            {
                waypoints.Add(GameObject.Find("Model").AddComponent<Bezier_Intermediate>());
                waypoints[waypoints.Count - 1].Init(waypoints[waypoints.Count - 2], null);
            }
        }
    }

    public void CloseLoop()
    {
        _loopClosed = true;
        waypoints.Add(GameObject.Find("Model").AddComponent<Bezier_LoopConnect>());
        waypoints[waypoints.Count - 1].Init(waypoints[waypoints.Count - 2], waypoints[0]);

    }

    public void RemoveLastPoint()
    {
        _loopClosed = false;
        Bezier_Base bb = waypoints[waypoints.Count - 1];
        waypoints.Remove(bb);
        DestroyImmediate(bb);
        Debug.Log("Destroyed");
    }

    public void GrabLostWaypoints()
    {
        waypoints.Clear();
        Transform model = GameObject.Find("Model").transform;
        Bezier_Base[] all = model.GetComponents<Bezier_Base>();

        for (int i = 0; i < all.Length; i++)
        {
            waypoints.Add(all[i]);
        }
    }

    public Transform cameraFlyover;

    private int waypointIndex;
    private float t;
    private float distToTravel;
    private float percToTravel;
    private Vector3 lastPos;

    public void Update()
    {
        distToTravel = Time.deltaTime * speed;
        percToTravel = waypoints[waypointIndex].GetPercForDist(distToTravel);

        //Check if you overtravel
        if (t + percToTravel > 1)
        {
            float PercLeftOnFirstLeg = 1 - t;
            float DistLeftOnFirstLeg = waypoints[waypointIndex].GetDistForPerc(PercLeftOnFirstLeg);
            float DistCarryover = distToTravel - DistLeftOnFirstLeg;

            waypointIndex++;
            if (waypointIndex > waypoints.Count - 1)
                waypointIndex = 0;

            t = waypoints[waypointIndex].GetPercForDist(DistCarryover);
        }
        else
            t += percToTravel;

        Vector3 spotOnTrack = waypoints[waypointIndex].GetPositionOnPath(t);

        lastPos = cameraFlyover.position;
        cameraFlyover.position = spotOnTrack;

        Debug.DrawRay(spotOnTrack, spotOnTrack - lastPos, Color.blue, 10);

        cameraFlyover.rotation = Quaternion.Slerp(cameraFlyover.rotation, Quaternion.LookRotation((spotOnTrack - lastPos), Vector3.up), .05f);
    }
}
#if UNITY_EDITOR
[CustomEditor(typeof(WaypointManager))]
public class WaypointEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WaypointManager _myWPM = (WaypointManager)target;

        if (GUILayout.Button("Make a waypoint"))
            _myWPM.MakeWaypoint();

        if (GUILayout.Button("Close waypoint loop"))
            _myWPM.CloseLoop();

        if (GUILayout.Button("Remove waypoint"))
            _myWPM.RemoveLastPoint();

        if (GUILayout.Button("Recalculate linear distance"))
            foreach (Bezier_Base b in _myWPM.waypoints)
                b.RecalculateLinearDist();

        if (GUILayout.Button("Get lost waypoints"))
            _myWPM.GrabLostWaypoints();
    }
}
#endif
