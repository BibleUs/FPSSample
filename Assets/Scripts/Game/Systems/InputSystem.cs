using UnityEngine;
using UnityEngine.XR;
using Valve.VR;

public class InputSystem {
    // TODO: these should be put in some global setting
    public static Vector2 s_JoystickLookSensitivity = new Vector2(90.0f, 60.0f);

    static float maxMoveYaw;
    static float maxMoveMagnitude;

    private SteamVR_Action_Boolean primaryFireVR;
    private SteamVR_Action_Boolean secondaryFireVR;
    private SteamVR_Action_Vector2 moveVR;
    private SteamVR_Action_Pose defaultPosesVR;


    public void AccumulateInput(ref UserCommand command, float deltaTime) {
        // To accumulate move we store the input with max magnitude and uses that
        Vector2 moveInput = new Vector2(Game.Input.GetAxisRaw("Horizontal"), Game.Input.GetAxisRaw("Vertical"));

        // Using input that stronger
        if (XRSettings.enabled)
            moveInput = moveVR[SteamVR_Input_Sources.LeftHand].lastAxis;

        float angle = Vector2.Angle(Vector2.up, moveInput);
        if (moveInput.x < 0)
            angle = 360 - angle;
        float magnitude = Mathf.Clamp(moveInput.magnitude, 0, 1);
        if (magnitude > maxMoveMagnitude) {
            maxMoveYaw = angle;
            maxMoveMagnitude = magnitude;
        }

        command.moveYaw = maxMoveYaw;
        command.moveMagnitude = maxMoveMagnitude;

        float invertY = Game.configInvertY.IntValue > 0 ? -1.0f : 1.0f;

        Vector2 deltaMousePos = new Vector2(0, 0);
        if (deltaTime > 0.0f)
            deltaMousePos += new Vector2(Game.Input.GetAxisRaw("Mouse X"), Game.Input.GetAxisRaw("Mouse Y") * invertY);
        deltaMousePos += deltaTime * (new Vector2(Game.Input.GetAxisRaw("RightStickX") * s_JoystickLookSensitivity.x,
                             -invertY * Game.Input.GetAxisRaw("RightStickY") * s_JoystickLookSensitivity.y));
        deltaMousePos += deltaTime * (new Vector2(
                             ((Game.Input.GetKey(KeyCode.Keypad4) ? -1.0f : 0.0f) +
                              (Game.Input.GetKey(KeyCode.Keypad6) ? 1.0f : 0.0f)) * s_JoystickLookSensitivity.x,
                             -invertY * Game.Input.GetAxisRaw("RightStickY") * s_JoystickLookSensitivity.y));

        command.lookYaw += deltaMousePos.x * Game.configMouseSensitivity.FloatValue;
        command.lookYaw = command.lookYaw % 360;

        if (XRSettings.enabled) {
            command.lookYaw = defaultPosesVR[SteamVR_Input_Sources.Head].localRotation.eulerAngles.y;
        }


        while (command.lookYaw < 0.0f) command.lookYaw += 360.0f;

        command.lookPitch += deltaMousePos.y * Game.configMouseSensitivity.FloatValue;
        command.lookPitch = Mathf.Clamp(command.lookPitch, 0, 180);

        if (XRSettings.enabled) {
            command.lookPitch = defaultPosesVR[SteamVR_Input_Sources.Head].localRotation.eulerAngles.x;
        }

        command.jump = command.jump || Game.Input.GetKeyDown(KeyCode.Space) ||
                       Game.Input.GetKeyDown(KeyCode.Joystick1Button0);
        command.boost = command.boost || Game.Input.GetKey(KeyCode.LeftControl) ||
                        Game.Input.GetKey(KeyCode.Joystick1Button4);
        command.sprint = command.sprint || Game.Input.GetKey(KeyCode.LeftShift);
        command.primaryFire = command.primaryFire || (Game.Input.GetMouseButton(0) && Game.GetMousePointerLock()) ||
                              (Game.Input.GetAxisRaw("Trigger") < -0.5f);
        command.secondaryFire = command.secondaryFire || Game.Input.GetMouseButton(1) ||
                                Game.Input.GetKey(KeyCode.Joystick1Button5);
        command.abilityA = command.abilityA || Game.Input.GetKey(KeyCode.Q);
        command.reload = command.reload || Game.Input.GetKey(KeyCode.R) || Game.Input.GetKey(KeyCode.Joystick1Button2);

        command.melee = command.melee || Game.Input.GetKey(KeyCode.V) || Game.Input.GetKey(KeyCode.Joystick1Button1);
        command.use = command.use || Game.Input.GetKey(KeyCode.E);

        command.emote = Game.Input.GetKeyDown(KeyCode.J) ? CharacterEmote.Victory : CharacterEmote.None;
        command.emote = Game.Input.GetKeyDown(KeyCode.K) ? CharacterEmote.Defeat : command.emote;

        // VR Inputs Fire
        if (XRSettings.enabled) {
            command.primaryFire = command.primaryFire || primaryFireVR.state;
            command.secondaryFire = command.secondaryFire || secondaryFireVR.state;
            command.headsetPos = defaultPosesVR[SteamVR_Input_Sources.Head].localPosition;
            command.headsetRot = defaultPosesVR[SteamVR_Input_Sources.Head].localRotation;
            command.rightControllerPos = defaultPosesVR[SteamVR_Input_Sources.RightHand].localPosition;
            command.rightControllerRot = defaultPosesVR[SteamVR_Input_Sources.RightHand].localRotation;
            command.leftControllerPos = defaultPosesVR[SteamVR_Input_Sources.LeftHand].localPosition;
            command.leftControllerRot = defaultPosesVR[SteamVR_Input_Sources.LeftHand].localRotation;
        }
    }

    private bool VRInputInitialized;

    public void SetVRInput() {
        if (VRInputInitialized) return;
        if (!XRSettings.enabled) return;

        if (SteamVR.initializedState != SteamVR.InitializedStates.InitializeSuccess)
            SteamVR.Initialize(true);

        primaryFireVR = SteamVR_Actions.default_GrabPinch;
        secondaryFireVR = SteamVR_Actions.default_GrabGrip;
        moveVR = SteamVR_Actions.default_TrackPad;
        defaultPosesVR = SteamVR_Actions.default_Pose;


        VRInputInitialized = true;
        Debug.Log("VR Input Initialized");
    }

    public void ClearInput(ref UserCommand command) {
        maxMoveMagnitude = 0;
        command.ClearCommand();
    }
}