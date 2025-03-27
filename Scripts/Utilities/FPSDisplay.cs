using UnityEngine;

public class FPSDisplay : MonoBehaviour
{
    [SerializeField] private bool useTargetFramerate;
    [SerializeField] private int targetFrameRate = 30;
    private float deltaTime = 0.0f;

    private void Start()
    {
        if (useTargetFramerate)
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = targetFrameRate;
        }
    }

    void Update()
    {
        deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int width = Screen.width, height = Screen.height;
        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, width, height * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = height * 2 / 100;
        style.normal.textColor = Color.white;
        float fps = 1.0f / deltaTime;
        string text = $"{fps:0.} FPS";
        GUI.Label(rect, text, style);
    }
}
