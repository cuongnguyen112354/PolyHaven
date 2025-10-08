using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(IconCapture))]
public class IconCaptureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        IconCapture script = (IconCapture)target;
        if (GUILayout.Button("Capture Icon"))
        {
            script.CaptureIcon();
        }
    }
}