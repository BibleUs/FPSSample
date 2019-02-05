using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


[CreateAssetMenu(fileName = "Ability_None",menuName = "FPS Sample/Abilities/Ability_None")]
public class Ability_None : CharBehaviorFactory{
    public override Entity Create(EntityManager entityManager, List<Entity> entities) {
        
        var entity = CreateCharBehavior(entityManager);
        entities.Add(entity);

        return entity;
    }
}


