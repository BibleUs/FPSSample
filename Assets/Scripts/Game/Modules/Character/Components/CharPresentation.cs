using System;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class CharPresentation : MonoBehaviour {
    public GameObject geomtry;
    public Transform itemAttachBone;

    public bool attached;
    
    public AbilityUI[]
        uiPrefabs; // TODO (mogensh) perhaps move UI to their own char presentation (so they are just just char and items)

    public Transform weaponBoneDebug; // TODO (mogensh) put these two debug properties somewhere appropriate
    public Vector3 weaponOffsetDebug;

    [NonSerialized] public Entity character;

    /*[NonSerialized]*/
    public bool updateTransform = true;
    [NonSerialized] public Entity attachToPresentation;

    public bool IsVisible {
        get { return isVisible; }
    }

    public void SetVisible(bool visible) {
        isVisible = visible;
        if (geomtry != null && geomtry.activeSelf != visible)
            geomtry.SetActive(visible);
    }

    [NonSerialized] bool isVisible = true;
}

[DisableAutoCreation]
public class UpdatePresentationRootTransform : BaseComponentSystem<CharPresentation> {
    private ComponentGroup Group;

    public UpdatePresentationRootTransform(GameWorld world) : base(world) { }

    protected override void Update(Entity entity, CharPresentation charPresentation) {
        if (!charPresentation.updateTransform)
            return;

        if (charPresentation.attachToPresentation != Entity.Null)
            return;
        
        var animState = EntityManager.GetComponentData<PresentationState>(charPresentation.character);
        charPresentation.transform.position = animState.position;
        var CharVR = charPresentation.GetComponent<CharacterVR>();
        if (CharVR == null) {
            charPresentation.transform.rotation = Quaternion.Euler(0f, animState.rotation, 0f);
        } else {
            CharVR.UpdatePositions(animState);
        }
    }
}

[DisableAutoCreation]
public class UpdatePresentationAttachmentTransform : BaseComponentSystem<CharPresentation> {
    public UpdatePresentationAttachmentTransform(GameWorld world) : base(world) { }

    protected override void Update(Entity entity, CharPresentation charPresentation) {
        if (charPresentation.attached) return;

        if (!charPresentation.updateTransform)
            return;

        if (charPresentation.attachToPresentation == Entity.Null)
            return;


        if (!EntityManager.Exists(charPresentation.attachToPresentation)) {
            GameDebug.LogWarning("Huhb ?");
            return;
        } else {
            var refPresentation =
                EntityManager.GetComponentObject<CharPresentation>(charPresentation.attachToPresentation);
            charPresentation.transform.parent = refPresentation.itemAttachBone.transform;
            charPresentation.transform.localPosition = Vector3.zero;
            charPresentation.transform.localRotation = Quaternion.identity;
            charPresentation.attached = true;
            Debug.Log("Attached");
        }

//        var refPresentation =
//            EntityManager.GetComponentObject<CharPresentation>(charPresentation.attachToPresentation);
//
//        charPresentation.transform.position = refPresentation.itemAttachBone.position;
//        charPresentation.transform.rotation = refPresentation.itemAttachBone.rotation;
    }
}