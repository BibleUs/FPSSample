using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Bonus_Weapon", menuName = "FPS Sample/Bonus/Bonus_Weapon")]
public class WeaponBonus : BonusTypeDefinition {
    [Serializable]
    public struct BonusWeapon : IComponentData {
        public WeaponTypeDefinition weapon;
    }


    public BonusWeapon effect;


//    
//    public override Entity Create(EntityManager entityManager, List<Entity> entities) {
//        return Entity.Null;
//    }

    //public WeaponTypeDefinition weapon;
    public override Entity Create(EntityManager entityManager) {
        var entity = CreateBonusBehavior(entityManager);
        entityManager.AddComponentData(entity, effect);
        return entity;
    }
}


#if UNITY_EDITOR
[CustomEditor(typeof(WeaponBonus))]
public class WeaponBonusEditor : BonusTypeDefinitionEditor { }
#endif