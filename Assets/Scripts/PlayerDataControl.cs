using System;
using System.Data;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

using UnityEngine;

public class PlayerDataControl : MonoBehaviour {
	private SceneControl sceneControl;
	private static string persistentDataPath;
	private string filePath;
	private string uuid;
	private string username;
	private int level;
	private int repeat;

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

	public static PlayerDataControl Instance { get; private set; }

	private async void Awake() {
		sceneControl = gameObject.AddComponent<SceneControl>();
		persistentDataPath = Application.persistentDataPath;
		filePath = Path.Combine(persistentDataPath, "Player.dat");

		if (File.Exists(filePath)) {
			await LoadPlayerAsync();
		}

		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(this);
		} else {
			Destroy(this);
		}

		GameStart();
	}

	public async void GameStart() {
		if (string.IsNullOrEmpty(UUID)) {
			CreateNewPlayer();
		} else {
			await SyncWithServer();
		}
	}

	private async Task LoadPlayerAsync() {
		try {
			using (FileStream file = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None)) {
				BinaryFormatter formatter = new BinaryFormatter();
				PlayerLocalData playerData = (PlayerLocalData)formatter.Deserialize(file);

				uuid = playerData.uuid;
				level = playerData.lv;
				repeat = playerData.repeat;
				username = playerData.username ?? "プレイヤー";

				await Task.CompletedTask; // 비동기 작업 유지
			}
		} catch (Exception ex) {
			Debug.LogError($"Failed to load save: {ex.Message}");
		}
	}

	private void CreateNewPlayer() {
		UUID = Guid.NewGuid().ToString();

		PlayerLocalData playerData = new PlayerLocalData {
			uuid = UUID,
			lv = 1,
			repeat = 0,
			username = "プレイヤー"
		};

		SaveLocalData(playerData);

		DBControl.OnCUD($"Insert into account values (null, '{UUID}', {playerData.lv}, {playerData.repeat})");
	}

	private async Task SyncWithServer() {
		DataSet ds = DBControl.OnRead($"Select * from account where uuid = '{UUID}'", "account");

		if (ds != null && ds.Tables.Contains("account")) {
			DataTable dt = ds.Tables["account"];
			DataRow row = dt.Rows[0];

			string serverUUID = row["uuid"].ToString();
			int serverLevel = Convert.ToInt32(row["lv"]);
			int serverRepeat = Convert.ToInt32(row["repeat"]);
			string serverUsername = row["username"].ToString();

			if (UUID != serverUUID || Level != serverLevel || Repeat != serverRepeat || Username != serverUsername) {
				UUID = serverUUID;
				Level = serverLevel;
				Repeat = serverRepeat;
				Username = serverUsername;

				SaveLocalData(new PlayerLocalData {
					uuid = UUID,
					lv = Level,
					repeat = Repeat,
					username = Username
				});
			}
		} else {
			Debug.LogWarning("No matching server data found.");
		}

		await Task.CompletedTask;
	}

	public void Save() {
		PlayerLocalData playerData = new PlayerLocalData {
			uuid = UUID,
			lv = Level,
			repeat = Repeat,
			username = Username
		};

		SaveLocalData(playerData);
		DBControl.OnCUD($"Update account set lv = {playerData.lv}, username = '{playerData.username}', `repeat` = {playerData.repeat} where uuid = '{playerData.uuid}'");
	}

	private void SaveLocalData(PlayerLocalData data) {
		try {
			using (FileStream file = File.Create(filePath)) {
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(file, data);
			}
		} catch (Exception ex) {
			Debug.LogError($"Failed to save data: {ex.Message}");
		}
	}
}