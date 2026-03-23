using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class CameraFix
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Add Camera")]
        public static void AddCamera()
        {
            // 查找是否已有摄像机
            if (Object.FindObjectOfType<Camera>() != null)
            {
                Debug.Log("Camera already exists!");
                return;
            }
            
            // 创建摄像机
            var cameraObj = new GameObject("Main Camera");
            var camera = cameraObj.AddComponent<Camera>();
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.1f, 0.1f, 0.1f);
            camera.orthographic = true;
            camera.orthographicSize = 5;
            
            cameraObj.tag = "MainCamera";
            cameraObj.transform.position = new Vector3(0, 0, -10);
            
            Debug.Log("✅ Camera added!");
        }
#endif
    }
}
