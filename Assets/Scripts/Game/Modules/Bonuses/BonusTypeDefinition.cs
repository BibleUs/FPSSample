    using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif



[CreateAssetMenu(fileName = "BonusTypeDefinition", menuName = "FPS Sample/Bonus/BonusTypeDefinition")]
public abstract class BonusTypeDefinition : ScriptableObjectRegistryEntry {

    public Settings settings;
    public WeakAssetReference prefabServer;
    public WeakAssetReference prefabClient;
    
    public enum BonusPickUpType {
        OnTrigger,
        OnTake,
        OnHold
    }
    
    public enum BonusType {
        HP,
        Armor,
        Weapon,
        Boost
    }

    public abstract Entity Create(EntityManager entityManager);
    
    public Entity CreateBonusBehavior(EntityManager entityManager)
    {
        var entity = entityManager.CreateEntity();

        entityManager.AddComponentData(entity, new Settings());

        return entity;
    }
    
    
    [Serializable]
    public struct Settings : IComponentData {
        
        public int reloadTime;
        public BonusPickUpType pickRule;
        public BonusType type;
        public Vector3 colliderSize;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BonusTypeDefinition))]
public class BonusTypeDefinitionEditor : ScriptableObjectRegistryEntryEditor<BonusRegistry, BonusTypeDefinition>
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawDefaultInspector();
    }
}
#endif


//
//[DisableAutoCreation]
//public class HandleBonusSpawn : InitializeComponentDataSystem<BonusTypeDefinition.State, HandleBonusSpawn.Initialized>
//{
//    public struct Initialized : IComponentData{}
//    
//    public HandleBonusSpawn(GameWorld gameWorld) : base(gameWorld)
//    {}
//
//    protected override void Initialize(Entity entity, BonusTypeDefinition.State component)
//    {
//        // Update character references on all behaviours
//        var buffer = EntityManager.GetBuffer<EntityGroupChildren>(entity);
//        for (int j = 0; j < buffer.Length; j++)
//        {
//            var childEntity = buffer[j].entity;
//            if (EntityManager.HasComponent<CharBehaviour>(childEntity))
//            {
//                var charBehaviour = EntityManager.GetComponentData<CharBehaviour>(childEntity);
//                charBehaviour.character = component.character;
//                EntityManager.SetComponentData(childEntity, charBehaviour);
//            }
//        }
//        
//        // TODO (mogensh) this is a very hacked approach to setting item abilities. Make more general way that supports item swap
//        var internalState = EntityManager.GetComponentData<WeaponTypeDefinition.InternalState>(entity);
//        var character = EntityManager.GetComponentObject<Character>(component.character);
//        character.item = entity;
//        var behaviourCtrlInternalState =
//            EntityManager.GetComponentData<DefaultCharBehaviourController.InternalState>(character.behaviourController);
//
//        behaviourCtrlInternalState.abilityPrimFire = internalState.abilityPrimFire;
//        behaviourCtrlInternalState.abilitySecFire = internalState.abilitySecFire;
//        EntityManager.SetComponentData(character.behaviourController, behaviourCtrlInternalState);
//        
//    }
//}
