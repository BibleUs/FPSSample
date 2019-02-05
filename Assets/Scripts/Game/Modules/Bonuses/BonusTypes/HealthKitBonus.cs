using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "Bonus_HealthKit", menuName = "FPS Sample/Bonus/Bonus_HealthKit")]
public class HealthKitBonus : BonusTypeDefinition {

       public Effect effect;       

       [Serializable]
       public struct Effect : IComponentData {
              public int amount;
       }


       public override Entity Create(EntityManager entityManager) {
              var entity = CreateBonusBehavior(entityManager);
              
              entityManager.AddComponentData(entity, effect);
              return entity;
       }
}





#if UNITY_EDITOR
[CustomEditor(typeof(HealthKitBonus))]
public class HealthKitBonusEditor : BonusTypeDefinitionEditor
{
    
}
#endif





