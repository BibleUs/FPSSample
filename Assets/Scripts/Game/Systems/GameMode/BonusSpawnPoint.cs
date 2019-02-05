using Unity.Entities;
using UnityEngine;

public class BonusSpawnPoint : MonoBehaviour, INetSerialized {
    
    public enum State : byte {
        Active,
        Reloading
    }

    
    
    public Entity bonusE;
    public BonusTypeDefinition btd;

    public GameObjectEntity bonus;

    public State state;

    public int reloadTime;

    public int lastTick = -1;    
 
    public void Serialize(ref NetworkWriter writer, IEntityReferenceSerializer refSerializer) {
        writer.WriteByte("state", (byte)state);
    }

    public void Deserialize(ref NetworkReader reader, IEntityReferenceSerializer refSerializer, int tick) {
        state = (State)reader.ReadByte();
    }


    public WeakAssetReference GetBonusServer() {
        return btd.prefabServer;
    }
    
    public WeakAssetReference GetBonusClient() {
        return btd.prefabClient;
    }

    public void Reset() {
        Destroy(bonus.gameObject);
        bonus = null;
    }
       

}
