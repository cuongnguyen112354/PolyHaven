using UnityEngine;
using System.IO;

public class IconCapture : MonoBehaviour
{
    public Camera iconCamera;
    public Transform prefabParent;
    public int resolution = 512;

    public void CaptureIcon()
    {
        string iconName = prefabParent.GetChild(0).gameObject.name;

        if (iconCamera == null || iconName == "")
        {
            Debug.LogError("Chưa gán Camera hoặc đặt tên Icon!");
            return;
        }

        RenderTexture rt = new RenderTexture(resolution, resolution, 24);
        iconCamera.targetTexture = rt;

        Texture2D screenshot = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        iconCamera.Render();
        RenderTexture.active = rt;
        screenshot.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        screenshot.Apply();

        iconCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);

        string folderPath = Application.dataPath + "/Resources/_Icons";
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string filePath = folderPath + "/" + iconName + ".png";
        File.WriteAllBytes(filePath, screenshot.EncodeToPNG());

        Debug.Log("Icon đã lưu tại: " + filePath);
    }
}
