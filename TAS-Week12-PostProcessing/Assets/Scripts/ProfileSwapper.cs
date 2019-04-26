using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class ProfileSwapper : MonoBehaviour
{
    public PostProcessProfile profile1;
    public PostProcessProfile profile2;
    public PostProcessProfile profile3;
    public PostProcessProfile profile0;

    public PostProcessVolume pVol;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            pVol.enabled = true;
            pVol.profile = profile1;
            RenderSettings.fog = true;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            pVol.enabled = true;
            pVol.profile = profile2;
            RenderSettings.fog = false;
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            pVol.enabled = true;
            pVol.profile = profile3;
            RenderSettings.fog = true;
        }
        
        
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            pVol.enabled = false;
            pVol.profile = profile0;
            RenderSettings.fog = false;
        }
    }
}
