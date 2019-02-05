using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public struct UserCommand : INetSerialized {
    public int checkTick; // For debug purposes
    public int renderTick;
    public float moveYaw;
    public float moveMagnitude;
    public float lookYaw;
    public float lookPitch;
    public bool jump;
    public bool boost;
    public bool sprint;
    public bool primaryFire;
    public bool secondaryFire;
    public bool abilityA;
    public bool reload;
    public bool melee;
    public bool use;

    public CharacterEmote emote;

    //VR controls
    public Vector3 rightControllerPos;
    public Vector3 leftControllerPos;
    public Vector3 headsetPos;

    public Quaternion rightControllerRot;
    public Quaternion leftControllerRot;
    public Quaternion headsetRot;


    public static readonly UserCommand defaultCommand = new UserCommand(0);

    private UserCommand(int i) {
        checkTick = 0;
        renderTick = 0;
        moveYaw = 0;
        moveMagnitude = 0;
        lookYaw = 0;
        lookPitch = 90;
        jump = false;
        boost = false;
        sprint = false;
        primaryFire = false;
        secondaryFire = false;
        abilityA = false;
        reload = false;
        melee = false;
        use = false;
        emote = CharacterEmote.None;
        //VR controls
        rightControllerPos = Vector3.zero;
        leftControllerPos = Vector3.zero;
        headsetPos = Vector3.zero;
        rightControllerRot = Quaternion.identity;
        leftControllerRot = Quaternion.identity;
        headsetRot = Quaternion.identity;
    }

    public void ClearCommand() {
        jump = false;
        boost = false;
        sprint = false;
        primaryFire = false;
        secondaryFire = false;
        abilityA = false;
        reload = false;
        melee = false;
        use = false;
        emote = CharacterEmote.None;
    }


    public Vector3 lookDir {
        get { return Quaternion.Euler(new Vector3(-lookPitch, lookYaw, 0)) * Vector3.down; }
    }

    public Quaternion lookRotation {
        get { return Quaternion.Euler(new Vector3(90 - lookPitch, lookYaw, 0)); }
    }

    public void Serialize(ref NetworkWriter writer, IEntityReferenceSerializer refSerializer) {
        writer.WriteInt32("tick", checkTick);
        writer.WriteInt32("renderTick", renderTick);
        writer.WriteFloatQ("moveYaw", moveYaw, 0);
        writer.WriteFloatQ("moveMagnitude", moveMagnitude, 2);
        writer.WriteFloat("lookYaw", lookYaw);
        writer.WriteFloat("lookPitch", lookPitch);
        writer.WriteBoolean("jump", jump);
        writer.WriteBoolean("boost", boost);
        writer.WriteBoolean("sprint", sprint);
        writer.WriteBoolean("primaryFire", primaryFire);
        writer.WriteBoolean("secondaryFire", secondaryFire);
        writer.WriteBoolean("abilityA", abilityA);
        writer.WriteBoolean("reload", reload);
        writer.WriteBoolean("melee", melee);
        writer.WriteBoolean("use", use);
        writer.WriteByte("emote", (byte) emote);
        //VR controls
        writer.WriteVector3Q("rightControllerPos", rightControllerPos, 2);
        writer.WriteVector3Q("leftControllerPos", leftControllerPos, 2);
        writer.WriteVector3Q("headsetPos", headsetPos, 2);
        writer.WriteQuaternion("rightControllerRot", rightControllerRot);
        writer.WriteQuaternion("leftControllerRot", leftControllerRot);
        writer.WriteQuaternion("headsetRot", headsetRot);
    }

    public void Deserialize(ref NetworkReader reader, IEntityReferenceSerializer refSerializer, int tick) {
        checkTick = reader.ReadInt32();
        renderTick = reader.ReadInt32();
        moveYaw = reader.ReadFloatQ();
        moveMagnitude = reader.ReadFloatQ();
        lookYaw = reader.ReadFloat();
        lookPitch = reader.ReadFloat();
        jump = reader.ReadBoolean();
        boost = reader.ReadBoolean();
        sprint = reader.ReadBoolean();
        primaryFire = reader.ReadBoolean();
        secondaryFire = reader.ReadBoolean();
        abilityA = reader.ReadBoolean();
        reload = reader.ReadBoolean();
        melee = reader.ReadBoolean();
        use = reader.ReadBoolean();
        emote = (CharacterEmote) reader.ReadByte();
        //VR controls
        rightControllerPos = reader.ReadVector3Q();
        leftControllerPos = reader.ReadVector3Q();
        headsetPos = reader.ReadVector3Q();
        rightControllerRot = reader.ReadQuaternion();
        leftControllerRot = reader.ReadQuaternion();
        headsetRot = reader.ReadQuaternion();
    }

    public override string ToString() {
        System.Text.StringBuilder strBuilder = new System.Text.StringBuilder();
        strBuilder.AppendLine("tick:" + checkTick);
        strBuilder.AppendLine("moveYaw:" + moveYaw);
        strBuilder.AppendLine("moveMagnitude:" + moveMagnitude);
        strBuilder.AppendLine("lookYaw:" + lookYaw);
        strBuilder.AppendLine("lookPitch:" + lookPitch);
        strBuilder.AppendLine("jump:" + jump);
        strBuilder.AppendLine("boost:" + boost);
        strBuilder.AppendLine("sprint:" + sprint);
        strBuilder.AppendLine("primaryFire:" + primaryFire);
        strBuilder.AppendLine("secondaryFire:" + secondaryFire);
        strBuilder.AppendLine("abilityA:" + abilityA);
        strBuilder.AppendLine("reload:" + reload);
        strBuilder.AppendLine("melee:" + melee);
        strBuilder.AppendLine("use:" + use);
        strBuilder.AppendLine("emote:" + emote);

        strBuilder.AppendLine("rightControllerPos:" + rightControllerPos);
        strBuilder.AppendLine("leftControllerPos:" + leftControllerPos);
        strBuilder.AppendLine("headsetPos:" + headsetPos);

        strBuilder.AppendLine("rightControllerRot:" + rightControllerRot);
        strBuilder.AppendLine("leftControllerRot:" + leftControllerRot);
        strBuilder.AppendLine("headsetRot:" + headsetRot);
        return strBuilder.ToString();
    }
}