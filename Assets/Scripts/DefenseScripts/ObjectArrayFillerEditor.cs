using UnityEditor;
using UnityEngine;

namespace DefenseScripts
{
    [CustomEditor(typeof(AllySpawner))]
    public class ObjectArrayFillerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            AllySpawner script = (AllySpawner)target;

            if (GUILayout.Button("Fill Empty Slots"))
            {
                FillEmptySlots(script);
            }
        }

        private void FillEmptySlots(AllySpawner script)
        {
            for (int i = 0; i < script.allies.Length; i++)
            {
                if (script.allies[i] == null)
                {
                    script.allies[i] = script.defaultObject;
                }
            }

            // 배열의 변경 사항을 저장합니다.
            EditorUtility.SetDirty(script);
        }
    }
}
