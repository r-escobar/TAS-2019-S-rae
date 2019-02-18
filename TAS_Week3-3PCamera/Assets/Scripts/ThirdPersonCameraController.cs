using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.UIElements;
using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour {

    #region Internal References
    private Transform _app;
    private Transform _view;
    private Transform _cameraBaseTransform;
    private Transform _cameraPivotTransform;
    private Transform _cameraTransform;
    private Transform _cameraLookTarget;
    private Transform _avatarTransform;
    private Rigidbody _avatarRigidbody;
    private Camera mainCam;
    private SkinnedMeshRenderer avatarRend;
    #endregion

    #region Public Tuning Variables
    public Vector3 avatarObservationOffset_Base;
    public float followDistance_Base;
    public float verticalOffset_Base;
    public float pitchGreaterLimit;
    public float pitchLowerLimit;
    public float fovAtUp;
    public float fovAtDown;
    public float camFollowSmoothing = 0.5f;
    public float standingToWalkingSmoothing = 1f;
    public float walkingToStandingSmoothing = 1f;
    public EasingFunction.Ease wToSEase;
    public float standingDistMultiplier = 2f;
    public float standingVertOffsetMultiplier = 4f;
    public float manualCameraLookSmoothing = 0.5f;
    public float standingZoomOutDelay = 1f;
    public float idleDelay = 3f;
    public float objectOfInterestLookupRadius = 10f;
    public float lookAtOOISmoothing = 0.075f;
    public EasingFunction.Ease lookAtOOIEase;
    public float cameraAvoidanceRadius = 1f;
    public float avoidanceSmoothing = 0.5f;
    #endregion

    #region Persistent Outputs
    //Positions
    private Vector3 _camRelativePostion_Auto;

    //Directions
    private Vector3 _avatarLookForward;

    //Scalars
    private float _followDistance_Applied;
    private float _verticalOffset_Applied;
    
    public CameraStates curCamState;

    public float idleTimer = 0f;
    private bool startingIdleTimer = false;

    public Collider[] nearbyObjectsOfInterest = new Collider[50];
    public int numNearbyOOI;
    #endregion

    
    private void Awake()
    {
        _app = GameObject.Find("Application").transform;
        _view = _app.Find("View");
        _cameraBaseTransform = _view.Find("CameraBase");
        _cameraPivotTransform = _cameraBaseTransform.Find("CameraPivot");
        _cameraTransform = _cameraPivotTransform.Find("Camera");
        _cameraLookTarget = _cameraBaseTransform.Find("CameraLookTarget");
        _avatarTransform = _view.Find("AIThirdPersonController");
        _avatarRigidbody = _avatarTransform.GetComponent<Rigidbody>();

        avatarRend = _avatarTransform.GetChild(0).GetComponent<SkinnedMeshRenderer>();
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButton(1))
        {
            curCamState = CameraStates.Manual;
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                _StopIdling();
                curCamState = CameraStates.Auto;
            } else if (idleTimer >= idleDelay)
            {
                curCamState = CameraStates.Idle;
            }
        } 
        
        switch (curCamState)
        {
            case CameraStates.Manual:
                _ManualUpdate();
                break;
            case CameraStates.Auto:
                _AutoUpdate();
                break;
            case CameraStates.Idle:
                _IdleUpdate();
                break;
        }
        
        AvoidObstacle();
    }

    #region States
    private void _AutoUpdate()
    {
        _ComputeData();
        _FollowAvatar();
        _LookAtAvatar();
    }
    private void _ManualUpdate()
    {
        _FollowAvatar();
        _ManualControl();
    }

    private void _IdleUpdate()
    {
        _FindObjectsOfInterest();
        _LookAtNearestOOI();
    }
    #endregion

    #region Internal Logic

    float _standingToWalkingSlider = 0;
    float standingTimer = 0f;
    float zoomOutProgress = 0f;

    private void _ComputeData()
    {
        _avatarLookForward = Vector3.Normalize(Vector3.Scale(_avatarTransform.forward, new Vector3(1, 0, 1)));

        if (_Helper_IsWalking())
        {
            standingTimer = 0f;
            zoomOutProgress = 0f;
            //_standingToWalkingSlider = Mathf.MoveTowards(_standingToWalkingSlider, 1, Time.deltaTime * 3);
            _standingToWalkingSlider = Mathf.Lerp(_standingToWalkingSlider, 1, standingToWalkingSmoothing * 50f * Time.deltaTime);

            idleTimer = 0f;
            startingIdleTimer = false;
        }
        else
        {
            if (standingTimer >= standingZoomOutDelay)
            {
                startingIdleTimer = true;
                //_standingToWalkingSlider = Mathf.MoveTowards(_standingToWalkingSlider, 0, Time.deltaTime);
                //_standingToWalkingSlider = Mathf.Lerp(_standingToWalkingSlider, 0, walkingToStandingSmoothing * 50f * Time.deltaTime);
                
                zoomOutProgress += Time.deltaTime * walkingToStandingSmoothing;
                zoomOutProgress = Mathf.Min(zoomOutProgress, 1f);
                
                EasingFunction.Function func = EasingFunction.GetEasingFunction(wToSEase);
                _standingToWalkingSlider = func(_standingToWalkingSlider, 0f, zoomOutProgress);

                idleTimer += Time.deltaTime;

            } else
            {
                
                standingTimer += Time.deltaTime;
            }
        }

        float _followDistance_Walking = followDistance_Base;
        float _followDistance_Standing = followDistance_Base * standingDistMultiplier;

        float _verticalOffset_Walking = verticalOffset_Base;
        float _verticalOffset_Standing = verticalOffset_Base * standingVertOffsetMultiplier;

        _followDistance_Applied = Mathf.Lerp(_followDistance_Standing, _followDistance_Walking, _standingToWalkingSlider);
        _verticalOffset_Applied = Mathf.Lerp(_verticalOffset_Standing, _verticalOffset_Walking, _standingToWalkingSlider);
    }

    private void _FollowAvatar()
    {
        _camRelativePostion_Auto = _avatarTransform.position;

        _cameraLookTarget.position = Vector3.Lerp(_cameraLookTarget.position, _avatarTransform.position + avatarObservationOffset_Base, camFollowSmoothing * 50f * Time.deltaTime);
        
        Vector3 camPosTarget = _avatarTransform.position - _avatarLookForward * _followDistance_Applied + Vector3.up * _verticalOffset_Applied;
        _cameraBaseTransform.position = Vector3.Lerp(_cameraBaseTransform.position, camPosTarget, camFollowSmoothing * 50f * Time.deltaTime);
    }

    private void _LookAtAvatar()
    {
        //_cameraTransform.LookAt(_cameraLookTarget);
        _cameraPivotTransform.LookAt(_cameraLookTarget);

    }

    private void _ManualControl()
    {
        Vector3 _camEulerHold = _cameraTransform.localEulerAngles;

        if (Input.GetAxis("Mouse X") != 0)
            _camEulerHold.y += Input.GetAxis("Mouse X");

        if (Input.GetAxis("Mouse Y") != 0)
        {
            float temp = _camEulerHold.x - Input.GetAxis("Mouse Y");
            temp = (temp + 360) % 360;

            if (temp < 180)
                temp = Mathf.Clamp(temp, 0, 80);
            else
                temp = Mathf.Clamp(temp, 360 - 80, 360);

            _camEulerHold.x = temp;
        }

        //Debug.Log("The V3 to be applied is " + _camEulerHold);
        _cameraTransform.localRotation = Quaternion.Slerp(_cameraTransform.localRotation, Quaternion.Euler(_camEulerHold), manualCameraLookSmoothing);
    }

    private void _FindObjectsOfInterest()
    {
        int layerMask = 1 << 9;
        
        numNearbyOOI = Physics.OverlapSphereNonAlloc(_avatarTransform.position, objectOfInterestLookupRadius, nearbyObjectsOfInterest, layerMask);
    }

    private float lookAtOOISlider = 0f;
    private float lookAtOOIProgress = 0f;
    Vector3 tgtToOOI = Vector3.zero;
    private Vector3 oOIToCam;
    
    private void _LookAtNearestOOI()
    {
        if (numNearbyOOI > 0)
        {
            Transform nearestOOITransform = GetNearestTransform(_avatarTransform.position, nearbyObjectsOfInterest);


            if (nearestOOITransform != null)
            {
                if (tgtToOOI == Vector3.zero)
                {
                    tgtToOOI = nearestOOITransform.position - _cameraLookTarget.position;
                    tgtToOOI *= 0.5f;

                    oOIToCam = mainCam.transform.position - nearestOOITransform.position;
                    oOIToCam = _cameraBaseTransform.position + (oOIToCam * 0.5f);
                    
                    tgtToOOI = _cameraLookTarget.position + tgtToOOI;
                }
                    
                lookAtOOIProgress += Time.deltaTime * lookAtOOISmoothing;
                lookAtOOIProgress = Mathf.Min(lookAtOOIProgress, 1f);
                
                EasingFunction.Function func = EasingFunction.GetEasingFunction(lookAtOOIEase);
                lookAtOOISlider = func(0f, 1f, lookAtOOIProgress);

                _cameraLookTarget.position = Vector3.Lerp(_cameraLookTarget.position, tgtToOOI, lookAtOOISlider); 
                _cameraBaseTransform.position = Vector3.Lerp(_cameraBaseTransform.position, oOIToCam, lookAtOOISlider); 

                _cameraPivotTransform.LookAt(_cameraLookTarget);
                

            }
        }        
    }

    void _StopIdling()
    {
        tgtToOOI = Vector3.zero;
        startingIdleTimer = false;
        lookAtOOISlider = 0f;
        lookAtOOIProgress = 0f;
        idleTimer = 0f;
    }

    private float avoidTimer;
    private float avoidDelay = 2f;
    void AvoidObstacle()
    {
        Collider[] nearbyObstacles = Physics.OverlapSphere(mainCam.transform.position, cameraAvoidanceRadius);

        if (nearbyObstacles.Length > 0)
        {
           // Debug.Log("cam is colliding with " + nearbyObstacles[0].gameObject.name);
            
            Transform nearestOOITransform = GetNearestTransform(mainCam.transform.position, nearbyObstacles);

            Vector3 objToCam = nearestOOITransform.position - mainCam.transform.position;
            float distMag = objToCam.magnitude;
            float avoidMag = cameraAvoidanceRadius - distMag * 20f;
            
            mainCam.transform.position = Vector3.MoveTowards(mainCam.transform.position, mainCam.transform.position + objToCam.normalized * avoidMag, Time.deltaTime);

            avoidTimer = 0f;
        }
        else
        {
            if (avoidTimer >= avoidDelay)
            {
                mainCam.transform.localPosition = Vector3.MoveTowards(mainCam.transform.localPosition, Vector3.zero, Time.deltaTime * 0.75f);
            }
            else
            {
                avoidTimer += Time.deltaTime;
            }
        }
        

    }
    #endregion

    #region Helper Functions

    private Vector3 _lastPos;
    private Vector3 _currentPos;
    private bool _Helper_IsWalking()
    {
        _lastPos = _currentPos;
        _currentPos = _avatarTransform.position;
        float velInst = Vector3.Distance(_lastPos, _currentPos) / Time.deltaTime;

        if (velInst > .15f)
            return true;
        else return false;
    }

    #endregion

    bool CheckIfVisible(Camera cam, Renderer rend)
    {
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), rend.bounds);

    }

    Transform GetNearestTransform(Vector3 pos, Collider[] collList)
    {
        float minDist = float.MaxValue;
        Transform nearestOOITransform = null;
        
        for (int i = 0; i < collList.Length; i++)
        {
            if (collList[i] != null)
            {
                Transform curOOITransform = collList[i].transform;
                //Debug.Log(curOOITransform.position);

                float dist = Vector3.Distance(pos, curOOITransform.position);
                
                if (dist < minDist)
                {
                    nearestOOITransform = curOOITransform;
                    minDist = dist;
                }
            }
        }

        return nearestOOITransform;
    }

}

public enum CameraStates
{
    Auto,
    Manual,
    Idle
};
