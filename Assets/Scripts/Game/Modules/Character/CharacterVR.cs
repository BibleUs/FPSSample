using System;
using RootMotion.FinalIK;
using Unity.Entities;
using UnityEngine;
using Valve.VR;

public class CharacterVR : MonoBehaviour {
    public Transform headset;
    public Transform rightController;
    public Transform leftController;

    public GameObject CameraVR;
    public VRIK vrik;
    private Transform rightAttach;
    
    public void Init() {
        var starter = Instantiate(CameraVR, gameObject.transform).GetComponent<VRStarter>();
        headset = starter.headset;
        rightController = starter.right;
        leftController = starter.left;
        vrik.solver.spine.headTarget = starter.headsetOffset;
        vrik.solver.rightArm.target = starter.rightControllerOffset;
        vrik.solver.leftArm.target = starter.leftControllerOffset;
        rightAttach = starter.rightAttach;
        headset.GetComponent<Camera>().enabled = false;
    }

    public Transform SetAsLocalPlayer() {
        headset.GetComponent<Camera>().enabled = true;
        return rightAttach;
    }

    public void UpdatePositions(PresentationState ps) {
        headset.localPosition = ps.headsetPos;
        headset.localRotation = ps.headsetRot;
        rightController.localPosition = ps.rightControllerPos;
        rightController.localRotation = ps.rightControllerRot;
        leftController.localPosition = ps.leftControllerPos;
        leftController.localRotation = ps.leftControllerRot;
    }

    

    
}
