using System.IO;
using UnityEngine;

public class PlayerLocalData
{
    public string uuid;
    public int lv;
    public int repeat;
    public string username;

    public PlayerLocalData(string uuid, int lv, int repeat, string username)
    {
        this.uuid = uuid;
        this.lv = lv;
        this.repeat = repeat;
        this.username = username;
    }

    public byte[] Serialize()
    {
        using (MemoryStream stream = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(uuid.Length);
                writer.Write(uuid);
                writer.Write(lv);
                writer.Write(repeat);
                writer.Write(username.Length);
                writer.Write(username);
            }
            return stream.ToArray();
        }
    }

    public static PlayerLocalData Deserialize(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                reader.ReadInt32();
                int uuidLength = reader.ReadByte();
                char[] uuidChars = reader.ReadChars(uuidLength);
                string uuid = new string(uuidChars);

                int lv = reader.ReadInt32();
                int repeat = reader.ReadInt32();
                
                reader.ReadInt32();
                int usernameLength = reader.ReadByte();
                char[] usernameChars = reader.ReadChars(usernameLength);
                string username = new string(usernameChars);
                
                CustomLogger.LogWarning($"UUID: {uuid}");
                CustomLogger.LogWarning($"Username: {username}");

                return new PlayerLocalData(uuid, lv, repeat, username);
            }
        }
    }
}