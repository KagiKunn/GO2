using System.Runtime.CompilerServices;

using UnityEngine;

public static class CustomLogger {
	private static string GetFileName(string filePath) {
		return System.IO.Path.GetFileNameWithoutExtension(filePath);
	}

	public static void Log(string message, string color = "green", [CallerFilePath] string filePath = "") {
		Debug.Log($"<color={color}>[{GetFileName(filePath)}] {message}</color>");
	}

	public static void LogWarning(string message, string color = "yellow", [CallerFilePath] string filePath = "") {
		Debug.LogWarning($"<color={color}>[{GetFileName(filePath)}] {message}</color>");
	}

	public static void LogError(string message, string color = "red", [CallerFilePath] string filePath = "") {
		Debug.LogError($"<color={color}>[{GetFileName(filePath)}] {message}</color>");
	}
}