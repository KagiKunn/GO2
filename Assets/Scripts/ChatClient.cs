using System;
using System.Collections;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using Button = UnityEngine.UI.Button;

#pragma warning disable CS0168 // 변수가 선언되었지만 사용되지 않았습니다.

public class ChatClient : MonoBehaviour {
	[SerializeField] private TMP_InputField input;
	[SerializeField] private TextMeshProUGUI chatBox;
	[SerializeField] private Button chatButton;
	[SerializeField] private ScrollRect chatScroll;
	private string username;
	private TcpClient client;
	private NetworkStream stream;
	private byte[] buffer = new byte[1024];
	private string r_message = String.Empty;

	void Start() {
		if (PlayerDataControl.Instance != null) {
			username = PlayerDataControl.Instance.Username;
		} else {
			username = "Guest";
		}

		InitializeConnect();
		input.onSubmit.AddListener(delegate { enterSend(); });
	}

	void Update() {
		if (!string.IsNullOrEmpty(r_message)) {
			chatBox.text += r_message;
			r_message = string.Empty;
			StartCoroutine(chatMove());
		}
	}

	IEnumerator chatMove() {
		yield return new WaitForSeconds(0.01f);

		chatScroll.verticalNormalizedPosition = 0.0f;
	}

	void InitializeConnect() {
		try {
			client = new TcpClient("192.168.0.32", 1650);
			stream = client.GetStream();
			new Thread(ReceiveData).Start();
		} catch (Exception e) {
			chatBox.text = "Connect Failed";

			throw;
		}
	}

	void enterSend() {
		StartCoroutine(coEnter());
	}

	IEnumerator coEnter() {
		if (!string.IsNullOrEmpty(Input.compositionString)) {
			input.text += Input.compositionString;
		}

		yield return new WaitForEndOfFrame();

		input.text = input.text.Trim();
		sendMessage();
		input.ActivateInputField();
	}

	public void sendMessage() {
		if (client == null) {
			return;
		}

		if (!string.IsNullOrEmpty(input.text)) {
			string message = username + ": " + input.text + "\n"; // 개행 문자 추가
			byte[] data = Encoding.UTF8.GetBytes(message);
			stream.Write(data, 0, data.Length);
			stream.Flush(); // 스트림 플러시
			input.text = string.Empty;
		}
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

	void OnApplicationQuit() {
		stream.Close();
		client.Close();
	}
}