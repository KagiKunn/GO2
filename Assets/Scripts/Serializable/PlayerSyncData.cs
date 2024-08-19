using System.IO;

public class PlayerSyncData
{
    public string uuid;
    public int lv;
    public int repeat;
    public string username;
    public int roguePoint;

    public PlayerSyncData(string uuid, int lv, int repeat, string username, int roguePoint)
    {
        this.uuid = uuid;
        this.lv = lv;
        this.repeat = repeat;
        this.username = username;
        this.roguePoint = roguePoint;
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
                writer.Write(roguePoint);
            }
            return stream.ToArray();
        }
    }

    public static PlayerSyncData Deserialize(byte[] data)
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

                int roguePoint = reader.ReadInt32();

                return new PlayerSyncData(uuid, lv, repeat, username, roguePoint);
            }
        }
    }
}