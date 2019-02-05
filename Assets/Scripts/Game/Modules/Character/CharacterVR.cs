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


    public void Init() {
        var starter = Instantiate(CameraVR, gameObject.transform).GetComponent<VRStarter>();
        headset = starter.headset;
        rightController = starter.right;
        leftController = starter.left;
        vrik.solver.spine.headTarget = headset;
        vrik.solver.rightArm.target = rightController;
        vrik.solver.leftArm.target = leftController;
        headset.GetComponent<Camera>().enabled = false;
    }

    public void SetAsLocalPlayer() {
        headset.GetComponent<Camera>().enabled = true;
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
