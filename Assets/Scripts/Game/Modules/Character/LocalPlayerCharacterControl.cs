using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using UnityEditor;
using UnityEngine.XR;


[RequireComponent(typeof(LocalPlayer))]
[RequireComponent(typeof(PlayerCameraSettings))]
public class LocalPlayerCharacterControl : MonoBehaviour {
    [ConfigVar(Name = "char.showhistory", DefaultValue = "0", Description = "Show last char loco states")]
    public static ConfigVar ShowHistory;

    public Entity lastRegisteredControlledEntity;

    public CharacterHealthUI healthUI;
    public IngameHUD hud;

    public int lastDamageInflictedTick;
    public int lastDamageReceivedTick;

    public List<AbilityUI> registeredCharUIs = new List<AbilityUI>();

    public class FirstPersonData {
        public Entity localPlayerEntity;

        public Entity charEntity;

        public List<CharPresentation> presentations = new List<CharPresentation>();
    }

    public FirstPersonData firstPerson = new FirstPersonData();
}


[DisableAutoCreation]
public class UpdateCharacter1PSpawn : BaseComponentSystem {
    ComponentGroup Group;

    public UpdateCharacter1PSpawn(GameWorld world, BundledResourceManager resourceManager) : base(world) {
        m_ResourceManager = resourceManager;
    }

    protected override void OnCreateManager() {
        base.OnCreateManager();
        Group = GetComponentGroup(typeof(LocalPlayer), typeof(LocalPlayerCharacterControl));
    }


    private List<LocalPlayerCharacterControl> charControlBuffer = new List<LocalPlayerCharacterControl>(2);
    private List<Entity> entityBuffer = new List<Entity>(2);

    protected override void OnUpdate() {
        charControlBuffer.Clear();
        entityBuffer.Clear();
        var charControlArray = Group.GetComponentArray<LocalPlayerCharacterControl>();
        var localPlayerArray = Group.GetComponentArray<LocalPlayer>();
        for (var i = 0; i < charControlArray.Length; i++) {
            var localPlayer = localPlayerArray[i];
            var characterControl = charControlArray[i];

            var controlledChar3PEntity = EntityManager.Exists(localPlayer.controlledEntity) &&
                                         EntityManager.HasComponent<Character>(localPlayer.controlledEntity)
                ? localPlayer.controlledEntity
                : Entity.Null;

            if (characterControl.firstPerson.localPlayerEntity != controlledChar3PEntity) {
                charControlBuffer.Add(characterControl);
                entityBuffer.Add(controlledChar3PEntity);
            }
        }

        if (charControlBuffer.Count > 0) {
            for (var i = 0; i < charControlBuffer.Count; i++) {
                var charCtrl = charControlBuffer[i];
                var charClientEntity = entityBuffer[i];

                foreach (var charPresentation in charCtrl.firstPerson.presentations) {
                    m_world.RequestDespawn(charPresentation.gameObject, PostUpdateCommands);
                }

                charCtrl.firstPerson.presentations.Clear();
                charCtrl.firstPerson.localPlayerEntity = charClientEntity;

                // TODO (mogensh) do all creation of character presentation one place ?
                // Spawn new 1P Presentation
                if (charClientEntity != Entity.Null) {
                    var character = EntityManager.GetComponentObject<Character>(charClientEntity);
                    if (XRSettings.enabled) {
                        GameDebug.Log("VR Enabled, no need to spawn 1P char");
//                        var charVRGUID = character.heroTypeData.character.prefabClient.guid;
//                        var prefabClient = m_ResourceManager.LoadSingleAssetResource(charVRGUID) as GameObject;
//                        var charVRGOE = m_world.Spawn<GameObjectEntity>(prefabClient);
//
//                        var charVREntity = charVRGOE.Entity;
//                        var charVRPresentation = EntityManager.GetComponentObject<CharPresentation>(charVREntity);
//                        charVRPresentation.character = charClientEntity;
//                        charVRPresentation.updateTransform = true;
//                        charCtrl.firstPerson.presentations.Add(charVRPresentation);
//                        charCtrl.firstPerson.charVR = charVREntity;
//                        character.presentationVR = charVREntity;
//                        character.presentations.Add(charVRPresentation);
//                        
//                        
//                        foreach (var itemEntry in character.heroTypeData.items) {
//                            var itemVRGUID = itemEntry.weaponType.prefabClient.guid;
//                            if (itemVRGUID != "") {
//                                var itemPrefabVR = m_ResourceManager.LoadSingleAssetResource(itemVRGUID) as GameObject;
//                                var itemGOE = m_world.Spawn<GameObjectEntity>(itemPrefabVR);
//                                var itemEntity = itemGOE.Entity;
//
//                                var itemPresentation = EntityManager.GetComponentObject<CharPresentation>(itemEntity);
//                                itemPresentation.character = charClientEntity;
//                                itemPresentation.attachToPresentation = charVREntity;
//                                charCtrl.firstPerson.presentations.Add(itemPresentation);
//                                character.presentations.Add(itemPresentation);
//                            }
//                        }
                        charCtrl.firstPerson.charEntity = character.presentation;
                        //charCtrl.firstPerson.presentations.AddRange(character.presentations);
                        var charVR = EntityManager.GetComponentObject<CharacterVR>(character.presentation);
//                        var charPresentation = EntityManager.GetComponentObject<CharPresentation>(character.presentation);
//                        charPresentation.itemAttachBone =
                        charVR.SetAsLocalPlayer();
                    } else {
//                        GameDebug.Log("Spawning 1P char and items");
//                        var char1PGUID = character.heroTypeData.character.prefab1P.guid;
//                        var prefab1P = m_ResourceManager.LoadSingleAssetResource(char1PGUID) as GameObject;
//                        var char1PGOE = m_world.Spawn<GameObjectEntity>(prefab1P);
//
//
//                        var char1PEntity = char1PGOE.Entity;
//                        var char1PPresentation = EntityManager.GetComponentObject<CharPresentation>(char1PEntity);
//                        char1PPresentation.character = charClientEntity;
//                        char1PPresentation.updateTransform = false;
//                        charCtrl.firstPerson.presentations1P.Add(char1PPresentation);
//                        charCtrl.firstPerson.char1P = char1PEntity;

//
//                        // Create items
//                        foreach (var itemEntry in character.heroTypeData.items) {
//                            var item1PGUID = itemEntry.weaponType.prefab1P.guid;
//                            if (item1PGUID != "") {
//                                var itemPrefab1P = m_ResourceManager.LoadSingleAssetResource(item1PGUID) as GameObject;
//                                var itemGOE = m_world.Spawn<GameObjectEntity>(itemPrefab1P);
//                                var itemEntity = itemGOE.Entity;
//
//                                var itemPresentation = EntityManager.GetComponentObject<CharPresentation>(itemEntity);
//                                itemPresentation.character = charClientEntity;
//                                itemPresentation.attachToPresentation = char1PEntity;
//                                charCtrl.firstPerson.presentations1P.Add(itemPresentation);
//                            }
//                        }
                    }
                }
            }
        }
    }

    BundledResourceManager m_ResourceManager;
}


[DisableAutoCreation]
public class
    UpdateCharacterCamera : BaseComponentSystem<LocalPlayer, LocalPlayerCharacterControl, PlayerCameraSettings> {
    private const float k_default3PDisst = 2.5f;
    private float camDist3P = k_default3PDisst;

    public UpdateCharacterCamera(GameWorld world) : base(world) { }

    public void ToggleFOrceThirdPerson() {
        forceThirdPerson = !forceThirdPerson;
    }

    protected override void Update(Entity entity, LocalPlayer localPlayer, LocalPlayerCharacterControl characterControl,
        PlayerCameraSettings cameraSettings) {
        if (localPlayer.controlledEntity == Entity.Null ||
            !EntityManager.HasComponent<Character>(localPlayer.controlledEntity)) {
            controlledEntity = Entity.Null;
            return;
        }

        if (characterControl.firstPerson.charEntity == Entity.Null) {
            controlledEntity = Entity.Null;
            return;
        }

        cameraSettings.isEnabled = true;

        GameDebug.Assert(EntityManager.HasComponent<PresentationState>(localPlayer.controlledEntity),
            "Controlled entity has no animstate");

        var character = EntityManager.GetComponentObject<Character>(localPlayer.controlledEntity);
        var charPredictedState = EntityManager.GetComponentData<CharPredictedStateData>(localPlayer.controlledEntity);

        var animState = EntityManager.GetComponentData<PresentationState>(localPlayer.controlledEntity);


        // Check if this is first time update is called with this controlled entity
        var characterChanged = localPlayer.controlledEntity != controlledEntity;
        if (characterChanged) {
            controlledEntity = localPlayer.controlledEntity;
        }

        // Update character visibility
        var camProfile = (forceThirdPerson ? CameraProfile.ThirdPerson : charPredictedState.cameraProfile);

//        foreach (var charPress in character.presentations) {
//            charPress.SetVisible(camProfile == CameraProfile.ThirdPerson);
//        }
//
//        foreach (var charPress in characterControl.firstPerson.presentations1P) {
//            charPress.SetVisible(camProfile == CameraProfile.FirstPerson);
//        }
//
//        foreach (var charPress in characterControl.firstPerson.presentations) {
//            charPress.SetVisible(camProfile == CameraProfile.FirstPersonVR);
//        }

        // Update camera settings
        var userCommand = EntityManager.GetComponentObject<UserCommandComponent>(localPlayer.controlledEntity);
        var lookRotation = userCommand.command.lookRotation;


        // Update FOV
        //if(characterChanged)
        //    cameraSettings.fieldOfView = Game.configFov.FloatValue;
        //var settings = character.heroTypeData.sprintCameraSettings;
        //var targetFOV = animState.sprinting == 1 ? settings.FOVFactor* Game.configFov.FloatValue : Game.configFov.FloatValue;
        //var speed = targetFOV > cameraSettings.fieldOfView ? settings.FOVInceraetSpeed : settings.FOVDecreaseSpeed;
        //cameraSettings.fieldOfView = Mathf.MoveTowards(cameraSettings.fieldOfView, targetFOV, speed);

        switch (camProfile) {
            case CameraProfile.FirstPerson: {
                var charVRPresentation =
                    EntityManager.GetComponentObject<CharPresentation>(characterControl.firstPerson.charEntity);
                charVRPresentation.transform.rotation = Quaternion.identity;
//                var character1P = EntityManager.GetComponentObject<Character1P>(characterControl.firstPerson.charEntity);
//                var eyePos = charPredictedState.position + Vector3.up * character.eyeHeight;
//
//                // Set camera position and adjust 1P char. As 1P char is scaled down we need to "up-scale" camera
//                // animation to world space. We dont want to upscale cam transform relative to 1PChar so we adjust
//                // position accordingly
//                var camLocalOffset = character1P.cameraTransform.position - character1P.transform.position;
//                var cameraRotationOffset = Quaternion.Inverse(character1P.transform.rotation) *
//                                           character1P.cameraTransform.rotation;
//                var camWorldOffset = camLocalOffset / character1P.transform.localScale.x;
//                var camWorldPos = eyePos + camWorldOffset;
//                var charWorldPos = camWorldPos - camLocalOffset;
//
//                cameraSettings.position = camWorldPos;
//                cameraSettings.rotation = userCommand.command.lookRotation * cameraRotationOffset;
//
//                var char1PPresentation =
//                    EntityManager.GetComponentObject<CharPresentation>(characterControl.firstPerson.charEntity);
//                char1PPresentation.transform.position = charWorldPos;
//                char1PPresentation.transform.rotation = userCommand.command.lookRotation;

                break;
            }

            case CameraProfile.Shoulder:
            case CameraProfile.ThirdPerson: {
#if UNITY_EDITOR
                if (Input.GetAxis("Mouse ScrollWheel") > 0) {
                    camDist3P -= 0.2f;
                }

                if (Input.GetAxis("Mouse ScrollWheel") < 0) {
                    camDist3P += 0.2f;
                }
#endif


                var eyePos = charPredictedState.position + Vector3.up * character.eyeHeight;
                cameraSettings.position = eyePos;
                cameraSettings.rotation = lookRotation;

                // Simpe offset of camera for better 3rd person view. This is only for animation debug atm
                var viewDir = cameraSettings.rotation * Vector3.forward;
                cameraSettings.position += -camDist3P * viewDir;
                cameraSettings.position += lookRotation * Vector3.right * 0.5f + lookRotation * Vector3.up * 0.5f;
                break;
            }
        }


        // TODO (mogensh) find better place to put this. 
        if (LocalPlayerCharacterControl.ShowHistory.IntValue > 0) {
            character.ShowHistory(m_world.worldTime.tick);
        }
    }

    bool forceThirdPerson;
    Entity controlledEntity;
}