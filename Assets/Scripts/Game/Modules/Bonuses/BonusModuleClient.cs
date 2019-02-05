using System.Collections.Generic;
using Unity.Entities;

public class BonusModuleClient {
    List<ScriptBehaviourManager> m_handleSpawnSystems = new List<ScriptBehaviourManager>();
    List<ScriptBehaviourManager> m_systems = new List<ScriptBehaviourManager>();
    GameWorld m_world;

    public BonusModuleClient (GameWorld world, BundledResourceManager resourceSystem) {
        m_world = world;
        var bonusRegistry = resourceSystem.GetResourceRegistry<BonusRegistry>();
        foreach (var entry in bonusRegistry.entries) {
            resourceSystem.LoadSingleAssetResource(entry.prefabClient.guid);
        }
        
        m_handleSpawnSystems.Add(world.GetECSWorld().CreateManager<HandleBonusSpawnPointInitServer>(world));
        
        m_systems.Add(m_world.GetECSWorld().CreateManager<BonusSpawnerUpdateSystemClient>(world, resourceSystem));
        //m_systems.Add(m_world.GetECSWorld().CreateManager<BonusUpdateSystemServer>(world));

    }


    public void HandleSpawn() {
        foreach (var system in m_handleSpawnSystems)
            system.Update();
    }


    public void Shutdown() {
        foreach (var system in m_systems)
            m_world.GetECSWorld().DestroyManager(system);
        foreach (var system in m_handleSpawnSystems)
            m_world.GetECSWorld().DestroyManager(system);
    }

    public void Update() {
        foreach (var system in m_systems) {
            system.Update();
        }
    }
}