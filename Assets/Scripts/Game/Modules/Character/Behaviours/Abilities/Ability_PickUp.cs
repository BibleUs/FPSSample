using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability_PickUp", menuName = "FPS Sample/Abilities/Ability_PickUp")]
public class Ability_PickUp : CharBehaviorFactory {
    
    public enum state {
        
    }
    
    
    [Serializable]
    public struct Settings : IComponentData {
        public float stopDelay;
    }

    public struct PredictedState : INetPredicted<PredictedState>, IComponentData {
        public int active;
        public int terminating;
        public int terminateStartTick;

        public void Serialize(ref NetworkWriter writer, IEntityReferenceSerializer refSerializer) {
            writer.WriteBoolean("active", active == 1);
            writer.WriteBoolean("terminating", terminating == 1);
            writer.WriteInt32("terminateStartTick", terminateStartTick);
        }

        public void Deserialize(ref NetworkReader reader, IEntityReferenceSerializer refSerializer, int tick) {
            active = reader.ReadBoolean() ? 1 : 0;
            terminating = reader.ReadBoolean() ? 1 : 0;
            terminateStartTick = reader.ReadInt32();
        }


#if UNITY_EDITOR
        public bool VerifyPrediction(ref PredictedState state) {
            return true;
        }

        public override string ToString() {
            return "Sprint.State active:" + active + " terminating:" + terminating;
        }
#endif
    }

    public Settings settings;

    public override Entity Create(EntityManager entityManager, List<Entity> entities) {
        var entity = CreateCharBehavior(entityManager);
        entities.Add(entity);

        // Ability components
        entityManager.AddComponentData(entity, new PredictedState());
        entityManager.AddComponentData(entity, settings);
        return entity;
    }
}

[DisableAutoCreation]
class Handle_Update : BaseComponentDataSystem<CharBehaviour, AbilityControl, Ability_PickUp.PredictedState,
    Ability_PickUp.Settings> {
    public Handle_Update(GameWorld world) : base(world) {
        ExtraComponentRequirements = new ComponentType[] {typeof(ServerEntity)};
    }

    protected override void Update(Entity abilityEntity, CharBehaviour charAbility, AbilityControl abilityCtrl,
        Ability_PickUp.PredictedState predictedState, Ability_PickUp.Settings settings) {
        if (abilityCtrl.active == 0 && predictedState.active == 0) {
            return;
        }

        var charPredictedState = EntityManager.GetComponentData<CharPredictedStateData>(charAbility.character);

        var command = EntityManager.GetComponentObject<UserCommandComponent>(charAbility.character).command;
        var sprintAllowed = command.moveMagnitude > 0 && (command.moveYaw < 90.0f || command.moveYaw > 270);
        var sprintRequested = sprintAllowed && command.sprint;

        if (sprintRequested && predictedState.active == 0) {
            abilityCtrl.behaviorState = AbilityControl.State.Active;
            predictedState.active = 1;
            predictedState.terminating = 0;
        }

        var startTerminate = !sprintAllowed || abilityCtrl.requestDeactivate == 1;
        if (startTerminate && predictedState.active == 1 && predictedState.terminating == 0) {
            predictedState.terminating = 1;
            predictedState.terminateStartTick = m_world.worldTime.tick;
        }

        if (predictedState.terminating == 1 && m_world.worldTime.DurationSinceTick(predictedState.terminateStartTick) >
            settings.stopDelay) {
            abilityCtrl.behaviorState = AbilityControl.State.Idle;
            predictedState.active = 0;
            abilityCtrl.behaviorState = AbilityControl.State.Idle;
        }

        if (abilityCtrl.active == 0) {
            // Behavior was forcefully deactivated
            abilityCtrl.behaviorState = AbilityControl.State.Idle;
            predictedState.active = 0;
        }

        charPredictedState.sprinting = abilityCtrl.behaviorState == AbilityControl.State.Active &&
                                       predictedState.active == 1 && predictedState.terminating == 0
            ? 1
            : 0;

        EntityManager.SetComponentData(abilityEntity, abilityCtrl);
        EntityManager.SetComponentData(abilityEntity, predictedState);
        EntityManager.SetComponentData(charAbility.character, charPredictedState);
    }
}