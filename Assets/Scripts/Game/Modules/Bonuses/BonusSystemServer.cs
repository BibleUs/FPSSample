using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Unity.Entities;

[DisableAutoCreation]
public class BonusUpdateSystemServer : BaseComponentSystem<BonusBehaviour> {
    ComponentGroup Group;

    public BonusUpdateSystemServer(GameWorld gameWorld) : base(gameWorld) {
        m_GameWorld = gameWorld;
    }

    protected override void Update(Entity entity, BonusBehaviour bonus) {
        if (bonus.character == Entity.Null) return;
        bool used = false;
        var spawner = bonus.spawner;
        var settings = EntityManager.GetComponentData<BonusTypeDefinition.Settings>(spawner.bonusE);
        // TODO we should say to character that he get stronger
        switch (settings.type) {
            case BonusTypeDefinition.BonusType.HP: {
                var character = EntityManager.GetComponentObject<Character>(bonus.character);
                var hstate = character.GetComponent<HealthState>();
                var HPKit = EntityManager.GetComponentData<HealthKitBonus.Effect>(spawner.bonusE);
                used = hstate.Heal(HPKit.amount);
                break;
            }
            case BonusTypeDefinition.BonusType.Armor: {
                var character = EntityManager.GetComponentObject<Character>(bonus.character);
                var hstate = character.GetComponent<HealthState>();
                var AKit = EntityManager.GetComponentData<ArmorKitBonus.Effect>(spawner.bonusE);
                used = hstate.AddArmor(AKit.amount);
                break;
            }
            case BonusTypeDefinition.BonusType.Weapon: {
                
                /// TODO короч, тут говорим чару что пора надевать пуху.
                /// потом чар ее надевает и готово.
                /// наверное, стоит сделать какаую-нибудь херную типа переменной с номером пухи.
//                
//                var effect = EntityManager.GetComponentData<WeaponBonus.BonusWeapon>(spawner.bonusE);
//                var itemPrefabGuid = effect.weapon.prefabServer.guid;

//                var itemPrefab = m_resourceManager.LoadSingleAssetResource(itemPrefabGuid) as GameObject;
//                var itemGOE = m_world.Spawn<GameObjectEntity>(itemPrefab);
//
//                var itemCharPresentation = EntityManager.GetComponentObject<CharPresentation>(itemGOE.Entity);
//                itemCharPresentation.character = charEntity;
//                itemCharPresentation.attachToPresentation = charPresentationEntity;
//                character.presentations.Add(itemCharPresentation);
                break;
            }
        }
        
        bonus.character = Entity.Null;

        if (!used) return;
        
        spawner.state = BonusSpawnPoint.State.Reloading;
        spawner.lastTick = m_GameWorld.worldTime.tick;
        m_world.RequestDespawn(bonus.gameObject);
    }

    GameWorld m_GameWorld;
}

[DisableAutoCreation]
public class BonusSpawnerUpdateSystemServer : BaseComponentSystem<BonusSpawnPoint> {
    ComponentGroup Group;
    private BundledResourceManager m_resourceManager;

    public BonusSpawnerUpdateSystemServer(GameWorld world, BundledResourceManager mResourceSystem) : base(world) {
        m_World = world;
        m_resourceManager = mResourceSystem;
    }

    protected override void Update(Entity entity, BonusSpawnPoint spawner) {
        if (spawner.state == BonusSpawnPoint.State.Active) return;

        if (spawner.lastTick == -1 ||
            spawner.state == BonusSpawnPoint.State.Reloading &&
            spawner.reloadTime*m_World.worldTime.tickRate <= (m_World.worldTime.tick - spawner.lastTick)) {
            var bonus = m_World.Spawn<BonusBehaviour>(
                m_resourceManager.LoadSingleAssetResource(spawner.GetBonusServer().guid) as GameObject);
            bonus.transform.SetParent(spawner.transform);
            bonus.transform.localPosition = Vector3.zero;
            spawner.bonus = bonus.GetComponent<GameObjectEntity>();
            bonus.spawner = spawner;
            bonus.collider.size = spawner.btd.settings.colliderSize;
            spawner.state = BonusSpawnPoint.State.Active;
        }
    }

    GameWorld m_World;
}


[DisableAutoCreation]
public class HandleBonusSpawnPointInitServer : InitializeComponentSystem<BonusSpawnPoint> {

    public HandleBonusSpawnPointInitServer(GameWorld world) : base(world) { }


    protected override void Initialize(Entity entity, BonusSpawnPoint spawnPoint) {
        spawnPoint.bonusE = spawnPoint.btd.Create(m_world.GetEntityManager());
        spawnPoint.reloadTime = spawnPoint.btd.settings.reloadTime;
    }

    GameWorld m_GameWorld;
}