using System.Runtime.CompilerServices;

using UnityEngine;

public static class CustomLogger {
	private static string GetFileName(string filePath) {
		return System.IO.Path.GetFileNameWithoutExtension(filePath);
	}

	private static string ColorToHex(object color) {
		if (color is Color col) {
			return "#" + ColorUtility.ToHtmlStringRGBA(col);
		} else if (color is string str) {
			if (ColorUtility.TryParseHtmlString(str, out Color namedColor)) {
				return "#" + ColorUtility.ToHtmlStringRGBA(namedColor);
			}

			string[] rgbParts = str.Split(',');

			if (rgbParts.Length == 3) {
				bool isValidR = byte.TryParse(rgbParts[0], out byte r);
				bool isValidG = byte.TryParse(rgbParts[1], out byte g);
				bool isValidB = byte.TryParse(rgbParts[2], out byte b);

				if (isValidR && isValidG && isValidB) {
					return "#" + ColorUtility.ToHtmlStringRGBA(new Color32(r, g, b, 255));
				}
			}

			return str.StartsWith("#") ? str : "#" + str;
		} else {
			return "#ffffff";
		}
	}

	public static void Log(object message, object color = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
		Debug.Log($"<i><b><color={ColorToHex(color != null ? color : "green")}>[{GetFileName(filePath)}::{memberName}] {message}</color></b></i>");
	}

	public static void LogWarning(object message, object color = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
		Debug.LogWarning($"<i><b><color={ColorToHex(color != null ? color : "yellow")}>[{GetFileName(filePath)}::{memberName}] {message}</color></b></i>");
	}

	public static void LogError(object message, object color = null, [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "") {
		Debug.LogError($"<i><b><color={ColorToHex(color != null ? color : "red")}>[{GetFileName(filePath)}::{memberName}] {message}</color></b></i>");
	}
}