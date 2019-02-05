using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class HealthState : MonoBehaviour, INetSerialized      
{
    [NonSerialized] public float health = 100;
    [NonSerialized] public float maxHealth = 100;  
    [NonSerialized] public float armor = 100;
    [NonSerialized] public float maxArmor = 100;     
    [NonSerialized] public int deathTick;
    [NonSerialized] public Entity killedBy;


    public void Serialize(ref NetworkWriter writer, IEntityReferenceSerializer refSerializer)
    {
        writer.WriteFloat("health", health);
        writer.WriteFloat("armor", armor);
    }

    public void Deserialize(ref NetworkReader reader, IEntityReferenceSerializer refSerializer, int tick)
    {
        health = reader.ReadFloat();
        armor = reader.ReadFloat();
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
        health = maxHealth;
    }
    public void SetMaxArmor(float maxArmor)
    {
        this.maxArmor = maxArmor;
        armor = maxArmor/2f;
    }
    
    public void ApplyDamage(ref DamageEvent damageEvent, int tick)
    {
        if (health <= 0)
            return;

        var dmg = damageEvent.damage;
        
        if (armor > 0) {
            var reducedDmg = math.min(dmg * 0.75f, armor);
            armor -= reducedDmg;
            dmg -= reducedDmg;

        }

        health -= dmg;
        
        if (health <= 0)
        {
            killedBy = damageEvent.instigator;
            deathTick = tick;
            health = 0;
        }
    }

    public bool Heal(float amount) {
        if (health == maxHealth) return false;

        health = math.min(health + amount, maxHealth);
        return true;
    }
    
    public bool AddArmor(float amount) {
        if (armor == maxArmor) return false;

        armor = math.min(armor + amount, maxArmor);
        return true;

    }
}
