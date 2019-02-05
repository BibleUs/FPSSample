using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class VRStarter : MonoBehaviour {

    public Transform headset;
    public Transform left;
    public Transform right;
    
    
    public SteamVR.InitializedStates initialized;

    public bool XRState;

    // Start is called before the first frame update
    void Start() {
        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess) {
            SteamVR.Initialize(true);
        }
    }

    // Update is called once per frame
    void Update() {
        initialized = SteamVR.initializedState;
        XRState = XRSettings.enabled;
    }
}