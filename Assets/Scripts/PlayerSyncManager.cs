using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

// ReSharper disable All

public class PlayerSyncManager : MonoBehaviour, IDisposable {
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
	private const int maxReconnectAttempts = 3;
	private const int reconnectDelay = 1000;
	private bool syncInit;
	private bool changeAccount;
	private int roguePoint;
	public bool isOnline;

	public SceneControl SceneControl {
		get => sceneControl;

		set => sceneControl = value;
	}

	public string UUID {
		get => uuid;

		set => uuid = value;
	}

	public int Level {
		get => level;

		set => level = value;
	}

	public int Repeat {
		get => repeat;

		set => repeat = value;
	}

	public string Username {
		get => username;

		set => username = value;
	}

	public int RoguePoint {
		get => roguePoint;

		set => roguePoint = value;
	}

	public static PlayerSyncManager Instance { get; private set; }

	private void Update() {
		if (syncInit) {
			syncInit = false;
			buffer = new byte[1024];
			var bytes = stream.Read(buffer, 0, buffer.Length);

			if (bytes <= 4) {
				return;
			}

			for (int i = 0; i < bytes - 4; i++) {
				buffer[i] = buffer[i + 4];
			}

			for (int i = bytes - 4; i < bytes; i++) {
				buffer[i] = 0;
			}

			if (bytes <= 0) return;

			if (bytes < 20) {
				string code = DeserializeCode(buffer, 16);
				InputBox.Instance.Input.text = code.Substring(0, 8);
			} else {
				var playerData = DeserializePlayerData(buffer, bytes - 4);

				// 서버에서 받은 데이터를 로컬에 반영
				UUID = playerData.uuid;
				Level = playerData.lv;
				Repeat = playerData.repeat;
				Username = playerData.username;
				RoguePoint = playerData.roguePoint;

				// 로컬 데이터 저장
				SaveLocalData(playerData);

				if (changeAccount) {
					changeAccount = false;
					PlayerLocalManager.Instance.CreateNewPlayer();
					SaveLocalData(new PlayerSyncData(UUID, Level, Repeat, Username, RoguePoint));
					Quit();
					CustomLogger.LogWarning("Make NEW");
				}
			}
		}
	}

	private async void Awake() {
		sceneControl = gameObject.AddComponent<SceneControl>();
		persistentDataPath = Application.persistentDataPath;
		filePath = Path.Combine(persistentDataPath, "Player.dat");

		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(this);
		}

		InitializeConnect();

		if (File.Exists(filePath)) {
			await LoadPlayerAsync();
		} else {
			CreateNewPlayer();
		}

		GameStart();
	}

	public void GameStart() {
		if (string.IsNullOrEmpty(UUID)) {
			CreateNewPlayer();
		} else {
			SyncWithServer();
		}
	}

	void InitializeConnect() {
		TryConnect();
	}

	void TryConnect() {
		int reconnectAttempts = 0;

		while (reconnectAttempts < maxReconnectAttempts) {
			bool test = false;
			client = new TcpClient(test ? "127.0.0.1" : "125.191.215.205", 1651);

			// 연결이 성공적으로 이루어졌다면 스트림을 가져옵니다.
			if (client.Connected) {
				stream = client.GetStream();
				CustomLogger.Log("Connected to server.");
				isReconnecting = false; // 재접속 성공 시 플래그 해제
				isOnline = true;

				//new Thread(ReceiveData).Start();
				return;
			}
		}

		if (reconnectAttempts >= maxReconnectAttempts) {
			isOnline = false;
		}

		CustomLogger.LogError("Max reconnect attempts reached. Could not connect to server.");
		isReconnecting = false; // 재접속 시도 실패
	}

	private async Task LoadPlayerAsync() {
		try {
			// 비동기로 파일을 열어 데이터를 읽어옵니다.
			byte[] data;

			using (FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
				long fileSize = file.Length;
				CustomLogger.Log($"File size: {fileSize}");

				if (fileSize == 0) {
					CustomLogger.LogError("Save file is empty.");

					return;
				}

				data = new byte[fileSize];
				int bytesRead = await file.ReadAsync(data, 0, (int)fileSize);

				if (bytesRead < fileSize) {
					CustomLogger.LogError("Failed to read the entire file.");

					return;
				}
			}

			// 읽어온 데이터를 역직렬화합니다.
			PlayerSyncData playerData = PlayerSyncData.Deserialize(data);

			// 역직렬화된 데이터를 클래스 필드에 할당합니다.
			uuid = playerData.uuid;
			level = playerData.lv;
			repeat = playerData.repeat;
			username = playerData.username ?? "プレイヤー";
			roguePoint = playerData.roguePoint;
		} catch (Exception ex) {
			CustomLogger.LogError($"Failed to load save: {ex.Message}");
		}
	}

	private void CreateNewPlayer() {
		UUID = Guid.NewGuid().ToString();

		PlayerSyncData playerData = new PlayerSyncData(UUID, 1, 0, "プレイヤー", 0);

		SaveLocalData(playerData);
		byte[] serializedData = SerializePlayerData(playerData);
		sendStream(serializedData, 0);
	}

	private void SyncWithServer() {
		// 서버에 보내기 위한 더미 데이터 생성
		PlayerSyncData dummyUUID = new PlayerSyncData(UUID, 1, 0, "プレイヤー", 0);
		byte[] serializedData = SerializePlayerData(dummyUUID);
		sendStream(serializedData, 2);
		syncInit = true;
	}

	public void IssueCode() {
		// 서버에 보내기 위한 더미 데이터 생성
		PlayerSyncData dummyUUID = new PlayerSyncData(UUID, 1, 0, "プレイヤー", 0);
		byte[] serializedData = SerializePlayerData(dummyUUID);
		sendStream(serializedData, 3);
		syncInit = true;
	}

	public void ChangeAccount(string code) {
		// 서버에 보내기 위한 더미 데이터 생성
		PlayerSyncData dummyUUID = new PlayerSyncData(UUID, 1, 0, code, 0);
		byte[] serializedData = SerializePlayerData(dummyUUID);
		sendStream(serializedData, 4);
		changeAccount = true;
		syncInit = true;
	}

	private void ChangeNickname(string name) {
		Username = name;
		Save();
	}

	private string DeserializeCode(byte[] data, int length) {
		using (MemoryStream ms = new MemoryStream(data, 0, length)) {
			return PlayerSyncData.DeserializeCode(data);
		}
	}

	private PlayerSyncData DeserializePlayerData(byte[] data, int length) {
		using (MemoryStream ms = new MemoryStream(data, 0, length)) {
			return PlayerSyncData.Deserialize(data);
		}
	}

	public void Save() {
		PlayerSyncData playerData = new PlayerSyncData(UUID, Level, Repeat, Username, RoguePoint);

		SaveLocalData(playerData);
		byte[] serializedData = SerializePlayerData(playerData);
		sendStream(serializedData, 1);
	}

	private void SaveLocalData(PlayerSyncData data) {
		try {
			// 데이터를 직렬화하여 바이트 배열로 변환
			byte[] serializedData = data.Serialize();

			// 파일 스트림을 열어 바이트 배열을 파일에 씁니다.
			using (FileStream file = File.Create(filePath)) {
				file.Write(serializedData, 0, serializedData.Length);
			}

			CustomLogger.Log("Data saved successfully.");
		} catch (Exception ex) {
			CustomLogger.LogError($"Failed to save data: {ex.Message}");
		}
	}

	public void sendStream(byte[] bytesData, int opcode) {
		if (client == null || !client.Connected) {
			if (!isReconnecting) {
				isReconnecting = true;
				TryConnect();
			}
		}

		if (isOnline) {
			byte[] dataWithOpcode = new byte[bytesData.Length + 1];
			dataWithOpcode[0] = (byte)opcode; // 첫 번째 바이트에 opcode 설정
			Buffer.BlockCopy(bytesData, 0, dataWithOpcode, 1, bytesData.Length);

			stream.Write(dataWithOpcode, 0, dataWithOpcode.Length);
			stream.Flush(); // 스트림 플러시
		}
	}

	public void send() {
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
				CustomLogger.Log("Error: " + e.Message);

				break;
			}
		}
	}

	private byte[] SerializePlayerData(PlayerSyncData playerData) {
		// PlayerLocalData 클래스에서 직접 구현한 Serialize 메서드를 사용하여 직렬화
		return playerData.Serialize();
	}

	public void Quit()
	{
		#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
		#else
		    Application.Quit();
		#endif
	}

	public void Dispose()
	{
		Save();
		stream?.Close();
		client?.Close();
	}

	void OnApplicationQuit() {
		Dispose();
	}
}