using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerDataControl : MonoBehaviour, IDisposable
{
    private SceneControl sceneControl;
    private static string persistentDataPath;
    private string filePath;
    private string uuid;
    private string username;
    private int level;
    private int repeat;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];
    private string r_message = String.Empty;
    private bool isReconnecting;
    private const int maxReconnectAttempts = 5;
    private const int reconnectDelay = 5000;

    public SceneControl SceneControl
    {
        get => sceneControl;
        set => sceneControl = value;
    }

    public string UUID
    {
        get => uuid;
        set => uuid = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int Repeat
    {
        get => repeat;
        set => repeat = value;
    }

    public string Username
    {
        get => username;
        set => username = value;
    }

    public static PlayerDataControl Instance { get; private set; }

    private async void Awake()
    {
        sceneControl = gameObject.AddComponent<SceneControl>();
        persistentDataPath = Application.persistentDataPath;
        filePath = Path.Combine(persistentDataPath, "Player.dat");

        InitializeConnect();
        if (File.Exists(filePath))
        {
            await LoadPlayerAsync();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }

        GameStart();
    }

    public void GameStart()
    {
        if (string.IsNullOrEmpty(UUID))
        {
            CreateNewPlayer();
        }
        else
        {
            CustomLogger.LogWarning(UUID);
            SyncWithServer();
        }
    }


    void InitializeConnect()
    {
        TryConnect();
    }

    void TryConnect()
    {
        int reconnectAttempts = 0;

        while (reconnectAttempts < maxReconnectAttempts)
        {
                client = new TcpClient("192.168.0.32", 1651);

                // 연결이 성공적으로 이루어졌다면 스트림을 가져옵니다.
                if (client.Connected)
                {
                        stream = client.GetStream();
                        Debug.Log("Connected to server.");
                        isReconnecting = false; // 재접속 성공 시 플래그 해제
                        //new Thread(ReceiveData).Start();
                        return;
                }
        }

        Debug.LogError("Max reconnect attempts reached. Could not connect to server.");
        isReconnecting = false; // 재접속 시도 실패
    }


    private async Task LoadPlayerAsync()
    {
        try
        {
            using (FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                PlayerLocalData playerData = (PlayerLocalData)formatter.Deserialize(file);

                uuid = playerData.uuid;
                level = playerData.lv;
                repeat = playerData.repeat;
                username = playerData.username ?? "プレイヤー";

                await Task.CompletedTask; // 비동기 작업 유지
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save: {ex.Message}");
        }
    }

    private void CreateNewPlayer()
    {
        UUID = Guid.NewGuid().ToString();

        PlayerLocalData playerData = new PlayerLocalData
        {
            uuid = UUID,
            lv = 1,
            repeat = 0,
            username = "プレイヤー"
        };

        SaveLocalData(playerData);
        byte[] serializedData = SerializePlayerData(playerData);
        sendStream(serializedData, 0);
    }
    private async void SyncWithServer()
    {
        try
        {
            // 서버에 보내기 위한 더미 데이터 생성
            PlayerLocalData dummyUUID = new PlayerLocalData
            {
                uuid = UUID,
                lv = 1,
                repeat = 0,
                username = "プレイヤー"
            };
            byte[] serializedData = SerializePlayerData(dummyUUID);
            sendStream(serializedData, 2);
            CustomLogger.LogWarning($"Send Stream {serializedData}");
        
            // 서버로부터 응답을 비동기적으로 기다림
            byte[] responseBuffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(responseBuffer, 0, responseBuffer.Length);
        
            // 응답된 데이터를 역직렬화하여 PlayerLocalData 객체로 변환
            PlayerLocalData playerData = DeserializePlayerData(responseBuffer, bytesRead);
        
            // 서버에서 받은 데이터를 로컬에 반영
            UUID = playerData.uuid;
            Level = playerData.lv;
            Repeat = playerData.repeat;
            Username = playerData.username;

            // 로컬 데이터 저장
            SaveLocalData(playerData);

            Debug.Log("Player data synchronized with server.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to sync with server: {ex.Message}");
        }
    }

    private PlayerLocalData DeserializePlayerData(byte[] data, int length)
    {
        using (MemoryStream ms = new MemoryStream(data, 0, length))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            return (PlayerLocalData)formatter.Deserialize(ms);
        }
    }

    public void Save()
    {
        PlayerLocalData playerData = new PlayerLocalData
        {
            uuid = UUID,
            lv = Level,
            repeat = Repeat,
            username = Username
        };

        SaveLocalData(playerData);
        byte[] serializedData = SerializePlayerData(playerData);
        sendStream(serializedData, 1);
    }

    private void SaveLocalData(PlayerLocalData data)
    {
        try
        {
            using (FileStream file = File.Create(filePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(file, data);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to save data: {ex.Message}");
        }
    }

    public void sendStream(byte[] bytesData, int opcode)
    {
        if (client == null || !client.Connected)
        {
            if (!isReconnecting)
            {
                isReconnecting = true;
                TryConnect();
            }
        }

        byte[] dataWithOpcode = new byte[bytesData.Length + 1];
        dataWithOpcode[0] = (byte)opcode; // 첫 번째 바이트에 opcode 설정
        Buffer.BlockCopy(bytesData, 0, dataWithOpcode, 1, bytesData.Length);
        
        CustomLogger.LogWarning("sendStream"+ dataWithOpcode.Length);
        stream.Write(dataWithOpcode, 0, dataWithOpcode.Length);
        stream.Flush(); // 스트림 플러시
    }

    public void send()
    {
        byte[] data = Encoding.UTF8.GetBytes("TEST");
        sendStream(data, 3);
    }

    private void ReceiveData() {
        while (true) {
            try {
                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes > 0) {
                    r_message = Encoding.UTF8.GetString(buffer, 0, bytes);
                }
            } catch (Exception e) {
                Debug.Log("Error: " + e.Message);

                break;
            }
        }
    }

    private byte[] SerializePlayerData(PlayerLocalData playerData)
    {
        using (MemoryStream ms = new MemoryStream())
        {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, playerData);
            return ms.ToArray();
        }
    }

    public void Dispose()
    {
        stream?.Close();
        client?.Close();
    }

    void OnApplicationQuit()
    {
        Dispose();
    }
}