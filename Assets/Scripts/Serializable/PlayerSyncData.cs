using System;
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
                writer.Write(uuid);
                writer.Write(lv);
                writer.Write(repeat);
                writer.Write(username);
                writer.Write(roguePoint);
            }

            return stream.ToArray();
        }
    }

    public static string DeserializeCode(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    byte[] codeBytes = reader.ReadBytes(8);
                    string code = System.Text.Encoding.UTF8.GetString(codeBytes);
                    return code;
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to deserialize data: {ex.Message}", ex);
                }
            }
        }
    }
    public static PlayerSyncData Deserialize(byte[] data)
    {
        using (MemoryStream stream = new MemoryStream(data))
        {
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    int uuidLength = reader.ReadByte();

                    char[] uuidChars = reader.ReadChars(uuidLength);
                    string uuid = new string(uuidChars);

                    byte[] lvBytes = reader.ReadBytes(4);
                    int lv = ReadInt32WithCondition(lvBytes);

                    byte[] repeatBytes = reader.ReadBytes(4);
                    int repeat = ReadInt32WithCondition(repeatBytes);

                    int usernameLength = reader.ReadByte();

                    byte[] usernameBytes = reader.ReadBytes(usernameLength);
                    string username = System.Text.Encoding.UTF8.GetString(usernameBytes);

                    byte[] roguePointBytes = reader.ReadBytes(4);
                    int roguePoint = ReadInt32WithCondition(roguePointBytes);

                    return new PlayerSyncData(uuid, lv, repeat, username, roguePoint);
                }
                catch (Exception ex)
                {
                    throw new Exception($"Failed to deserialize data: {ex.Message}", ex);
                }
            }
        }
    }
    
    static int ReadInt32WithCondition(Byte[] bytes)
    {

        // 4바이트 배열을 역순으로 변환
        if (BitConverter.ToInt32(bytes, 0) > 1000)
        {
            Array.Reverse(bytes);
        }

        // 역순 또는 원래 순서의 배열로부터 Int32로 변환
        return BitConverter.ToInt32(bytes, 0);
    }
}