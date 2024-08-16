using UnityEditor;
using UnityEngine;

public class ItemSO_IDAssigner : EditorWindow
{
    [MenuItem("Tools/Assign ItemSO IDs")]
    public static void AssignIDs()
    {
        string[] guids = AssetDatabase.FindAssets("t:ItemSO");

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ItemSO itemSO = AssetDatabase.LoadAssetAtPath<ItemSO>(path);

            if (itemSO != null && string.IsNullOrEmpty(itemSO.ItemID))
            {
                Undo.RecordObject(itemSO, "Assign Item ID");
                itemSO.OnValidate();
                EditorUtility.SetDirty(itemSO);
                Debug.Log($"Assigned ID to {itemSO.name}");
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("ID assignment complete.");
    }
}
