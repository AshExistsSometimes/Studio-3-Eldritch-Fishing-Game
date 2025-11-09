using UnityEngine;
using UnityEditor;
using System.IO;

public class IconCamera : EditorWindow
{
    private GameObject targetObject;
    private Camera renderCamera;
    private int resolution = 256;
    private string saveFolder = "Assets/Icons";

    [MenuItem("Tools/Icon Renderer")]
    public static void ShowWindow()
    {
        GetWindow<IconCamera>("Icon Renderer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Render GameObject to PNG Icon", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target Object", targetObject, typeof(GameObject), true);
        renderCamera = (Camera)EditorGUILayout.ObjectField("Render Camera", renderCamera, typeof(Camera), true);
        resolution = EditorGUILayout.IntField("Resolution", resolution);
        saveFolder = EditorGUILayout.TextField("Save Folder", saveFolder);

        GUILayout.Space(10);

        if (GUILayout.Button("Render Icon"))
        {
            if (targetObject == null || renderCamera == null)
            {
                Debug.LogError("Please assign both a Target Object and a Render Camera!");
                return;
            }

            RenderAndSaveIcon();
        }
    }

    private void RenderAndSaveIcon()
    {
        // Create temporary RenderTexture
        RenderTexture rt = new RenderTexture(resolution, resolution, 24, RenderTextureFormat.ARGB32);
        rt.antiAliasing = 8;

        renderCamera.targetTexture = rt;
        renderCamera.backgroundColor = new Color(0, 0, 0, 0); // transparent
        renderCamera.clearFlags = CameraClearFlags.SolidColor;

        // Just render whatever the camera sees (object should be manually placed)
        renderCamera.Render();

        // Read pixels
        RenderTexture.active = rt;
        Texture2D tex = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        tex.ReadPixels(new Rect(0, 0, resolution, resolution), 0, 0);
        tex.Apply();

        // Save as PNG
        if (!Directory.Exists(saveFolder))
            Directory.CreateDirectory(saveFolder);

        string path = Path.Combine(saveFolder, targetObject.name + "_Icon.png");
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Debug.Log($"Icon saved to {path}");

        // Cleanup
        renderCamera.targetTexture = null;
        RenderTexture.active = null;
        DestroyImmediate(rt);
        DestroyImmediate(tex);

        AssetDatabase.Refresh();
    }
}