using System.Collections.Generic;
using Unity.Entities;

public class WeaponsModule
{
    List<ScriptBehaviourManager> m_handleSpawnSystems = new List<ScriptBehaviourManager>();
    List<ScriptBehaviourManager> m_systems = new List<ScriptBehaviourManager>();
    GameWorld m_world;
    
    public WeaponsModule(GameWorld world, BundledResourceManager resourceSystem, bool server)
    {
        m_world = world;
        
        
        var itemRegistry = resourceSystem.GetResourceRegistry<WeaponRegistry>();
        for (var i = 0; i < itemRegistry.entries.Count; i++)
        {
            resourceSystem.LoadSingleAssetResource(server ? itemRegistry.entries[i].prefabServer.guid : itemRegistry.entries[i].prefabClient.guid);
        }
        
        m_handleSpawnSystems.Add(world.GetECSWorld().CreateManager<HandleWeaponSpawn>(world));
        
        // TODO (mogensh) make server version without all this client stuff
        m_systems.Add(world.GetECSWorld().CreateManager<RobotWeaponClientProjectileSpawnHandler>(world));
        m_systems.Add(world.GetECSWorld().CreateManager<TerraformerWeaponClientProjectileSpawnHandler>(world));
        m_systems.Add(world.GetECSWorld().CreateManager<UpdateTerraformerWeaponA>(world));
        m_systems.Add(world.GetECSWorld().CreateManager<UpdateWeaponActionTimelineTrigger>(world));
        m_systems.Add(world.GetECSWorld().CreateManager<System_RobotWeaponA>(world));
    }

    public void HandleSpawn()
    {
        foreach (var system in m_handleSpawnSystems)
            system.Update();
    }

    public void Shutdown()
    {
        foreach (var system in m_handleSpawnSystems)
            m_world.GetECSWorld().DestroyManager(system);
        foreach (var system in m_systems)
            m_world.GetECSWorld().DestroyManager(system);
    }

    public void LateUpdate()
    {        
        foreach (var system in m_systems)
            system.Update();
    }
}
