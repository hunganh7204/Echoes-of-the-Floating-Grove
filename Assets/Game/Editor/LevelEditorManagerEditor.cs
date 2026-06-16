using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelEditorManager))]
public class LevelEditorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        LevelEditorManager manager = (LevelEditorManager)target;

        GUILayout.Space(20);
        GUILayout.Label("--- CÔNG CỤ VẼ MAP ---", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Ground", GUILayout.Height(30))) manager.SetBrushType(1);
        if (GUILayout.Button("Checkpoint", GUILayout.Height(30))) manager.SetBrushType(2);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button(" Quái ", GUILayout.Height(30))) manager.SetBrushType(3);
        if (GUILayout.Button("Rương ", GUILayout.Height(30))) manager.SetBrushType(4);
        if (GUILayout.Button("NPC ", GUILayout.Height(30))) manager.SetBrushType(5);
        if (GUILayout.Button("Điểm Kết Thúc", GUILayout.Height(30))) manager.SetBrushType(-2);
        if (GUILayout.Button("Vùng Chết", GUILayout.Height(30))) manager.SetBrushType(0);
        GUILayout.EndHorizontal();

        GUILayout.Space(5);

        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Điểm Bắt Đầu", GUILayout.Height(30))) manager.SetBrushType(-1);

        GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
        if (GUILayout.Button("Erase", GUILayout.Height(30))) manager.SetBrushType(99);

        GUI.backgroundColor = Color.white;
        GUILayout.EndHorizontal();

        GUILayout.Space(20);
        GUILayout.Label("--- QUẢN LÝ FILE MAP ---", EditorStyles.boldLabel);

        manager.levelNameToLoad = EditorGUILayout.TextField("Tên Map cần Load:", manager.levelNameToLoad);
        if (GUILayout.Button(" TẢI MAP TỪ JSON", GUILayout.Height(30)))
        {
            manager.LoadMapFromFile();
        }

        GUILayout.Space(20);
        GUILayout.Label("--- HỆ THỐNG ---", EditorStyles.boldLabel);

        if (GUILayout.Button(manager.IsTesting ? "DỪNG TEST" : "▶CHƠI THỬ", GUILayout.Height(40)))
        {
            manager.ToggleTestPlay();
        }

        GUILayout.Space(5);

        GUI.backgroundColor = Color.cyan;
        if (GUILayout.Button("LƯU MAP VÀO JSON", GUILayout.Height(40)))
        {
            manager.SaveMap();
        }
        GUI.backgroundColor = Color.white;
    }
}