using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
// ReSharper disable All

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
    private bool syncInit;

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

    private void Update()
    {
        if (syncInit)
        {
            syncInit = false;
            var bytes = stream.Read(buffer, 0, buffer.Length);
            for (int i = 0; i < bytes - 4; i++)
            {
                buffer[i] = buffer[i + 4];
            }
            for (int i = bytes - 4; i < bytes; i++)
            {
                buffer[i] = 0;
            }
            if (bytes <= 0) return;
            var playerData = DeserializePlayerData(buffer, bytes - 4);

            // 서버에서 받은 데이터를 로컬에 반영
            UUID = playerData.uuid;
            Level = playerData.lv;
            Repeat = playerData.repeat;
            Username = playerData.username;

            // 로컬 데이터 저장
            SaveLocalData(playerData);
        }
    }

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
            // 비동기로 파일을 열어 데이터를 읽어옵니다.
            byte[] data;
            using (FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                long fileSize = file.Length;
                Debug.Log($"File size: {fileSize}");

                if (fileSize == 0)
                {
                    Debug.LogError("Save file is empty.");
                    return;
                }

                data = new byte[fileSize];
                int bytesRead = await file.ReadAsync(data, 0, (int)fileSize);

                if (bytesRead < fileSize)
                {
                    Debug.LogError("Failed to read the entire file.");
                    return;
                }
            }

            // 읽어온 데이터를 역직렬화합니다.
            PlayerLocalData playerData = PlayerLocalData.Deserialize(data);

            // 역직렬화된 데이터를 클래스 필드에 할당합니다.
            uuid = playerData.uuid;
            level = playerData.lv;
            repeat = playerData.repeat;
            username = playerData.username ?? "プレイヤー";
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to load save: {ex.Message}");
        }
    }


    private void CreateNewPlayer()
    {
        UUID = Guid.NewGuid().ToString();

        PlayerLocalData playerData = new PlayerLocalData(UUID, 1, 0, "プレイヤー");

        SaveLocalData(playerData);
        byte[] serializedData = SerializePlayerData(playerData);
        sendStream(serializedData, 0);
    }

    private void SyncWithServer()
    {
            // 서버에 보내기 위한 더미 데이터 생성
            PlayerLocalData dummyUUID = new PlayerLocalData(UUID, 1, 0, "プレイヤー");
            byte[] serializedData = SerializePlayerData(dummyUUID);
            sendStream(serializedData, 2);
            syncInit = true;
    }

    private PlayerLocalData DeserializePlayerData(byte[] data, int length)
    {
        using (MemoryStream ms = new MemoryStream(data, 0, length))
        {
            return PlayerLocalData.Deserialize(data);
        }
    }

    public void Save()
    {
        PlayerLocalData playerData = new PlayerLocalData(UUID, Level, Repeat, Username);

        SaveLocalData(playerData);
        byte[] serializedData = SerializePlayerData(playerData);
        sendStream(serializedData, 1);
    }

    private void SaveLocalData(PlayerLocalData data)
    {
        try
        {
            // 데이터를 직렬화하여 바이트 배열로 변환
            byte[] serializedData = data.Serialize();

            // 파일 스트림을 열어 바이트 배열을 파일에 씁니다.
            using (FileStream file = File.Create(filePath))
            {
                file.Write(serializedData, 0, serializedData.Length);
            }

            Debug.Log("Data saved successfully.");
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

        stream.Write(dataWithOpcode, 0, dataWithOpcode.Length);
        stream.Flush(); // 스트림 플러시
    }

    public void send()
    {
        byte[] data = Encoding.UTF8.GetBytes("TEST");
        sendStream(data, 3);
    }

    private void ReceiveData()
    {
        while (true)
        {
            try
            {
                int bytes = stream.Read(buffer, 0, buffer.Length);

                if (bytes > 0)
                {
                    r_message = Encoding.UTF8.GetString(buffer, 0, bytes);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error: " + e.Message);

                break;
            }
        }
    }

    private byte[] SerializePlayerData(PlayerLocalData playerData)
    {
        // PlayerLocalData 클래스에서 직접 구현한 Serialize 메서드를 사용하여 직렬화
        return playerData.Serialize();
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