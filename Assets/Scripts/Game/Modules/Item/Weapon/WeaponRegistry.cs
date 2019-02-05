using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "FPS Sample/Weapon/WeaponTypeRegistry", fileName = "WeaponTypeRegistry")]
public class WeaponRegistry : ScriptableObjectRegistry<WeaponTypeDefinition>
{
    public int GetIndexByClientGUID(string guid)
    {
        for (var i = 0; i < entries.Count; i++)
        {
            if (entries[i].prefabClient.guid == guid)
                return i;
        }

        return -1;
    }

#if UNITY_EDITOR
    public override void GetSingleAssetGUIDs(List<string> guids, bool server)
    {
        foreach (var entry in entries)
        {
            if (server && entry.prefabServer.guid != "")
                guids.Add(entry.prefabServer.guid);
            if (!server && entry.prefabClient.guid != "")
                guids.Add(entry.prefabClient.guid);
        }
    }
#endif
}