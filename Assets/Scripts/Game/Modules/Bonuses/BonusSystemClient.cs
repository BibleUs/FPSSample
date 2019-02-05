
using Unity.Entities;
using UnityEngine;

[DisableAutoCreation]
public class BonusSpawnerUpdateSystemClient : BaseComponentSystem<BonusSpawnPoint> {
    ComponentGroup Group;
    private BundledResourceManager m_resourceManager;
    private GameObject bonus;

    public BonusSpawnerUpdateSystemClient(GameWorld world, BundledResourceManager mResourceSystem) : base(world) {
        m_World = world;
        m_resourceManager = mResourceSystem;
    }

    protected override void Update(Entity entity, BonusSpawnPoint spawner) {
        if (spawner.state == BonusSpawnPoint.State.Active && spawner.bonus == null) {
            var bonus = m_World.Spawn<GameObjectEntity>(
                m_resourceManager.LoadSingleAssetResource(spawner.GetBonusClient().guid) as GameObject);
            bonus.transform.SetParent(spawner.transform);
            bonus.transform.localPosition = Vector3.zero;
            spawner.bonus = bonus;
        }

        if (spawner.state == BonusSpawnPoint.State.Reloading && spawner.bonus != null) {
            m_World.RequestDespawn(spawner.bonus.gameObject);
        }
    }

    GameWorld m_World;
}
