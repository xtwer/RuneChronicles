using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
#endif

namespace RuneChronicles
{
    public class SceneHelper
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Create Clean Test Scene")]
        public static void CreateCleanTestScene()
        {
            // 创建新场景
            var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            
            // 构建测试场景
            TestSceneBuilder.BuildTestScene();
            
            // 保存场景
            string scenePath = "Assets/Scenes/TestScene.unity";
            if (!AssetDatabase.IsValidFolder("Assets/Scenes"))
                AssetDatabase.CreateFolder("Assets", "Scenes");
            
            EditorSceneManager.SaveScene(newScene, scenePath);
            
            Debug.Log($"✅ Clean test scene created and saved to {scenePath}");
        }
#endif
    }
}
