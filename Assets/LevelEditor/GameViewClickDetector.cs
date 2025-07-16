using UnityEditor;
using UnityEngine;
public class GameViewClickDetector : EditorWindow
{
    [MenuItem("Tools/Game View Click Detector")]
    public static void ShowWindow()
    {
        GetWindow<GameViewClickDetector>("Game View Click");
    }

    private void OnGUI()
    {
        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            // 获取在当前窗口中的点击位置
            Vector2 mousePosition = e.mousePosition;

            // 处理点击事件
            Debug.Log($"模拟Game窗口点击位置: {mousePosition}");

            // 标记事件已处理
            e.Use();
        }

        // 绘制窗口内容
        GUILayout.Label("点击此窗口模拟Game窗口点击", EditorStyles.boldLabel);
    }
}