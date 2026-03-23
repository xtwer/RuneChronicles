using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RuneChronicles
{
    public class CleanupMissingScripts
    {
#if UNITY_EDITOR
        [MenuItem("RuneChronicles/Cleanup Missing Scripts")]
        public static void CleanupMissing()
        {
            var allObjects = Object.FindObjectsOfType<GameObject>();
            int count = 0;
            
            foreach (var obj in allObjects)
            {
                // 获取所有组件
                var components = obj.GetComponents<Component>();
                
                // 移除丢失的脚本
                for (int i = components.Length - 1; i >= 0; i--)
                {
                    if (components[i] == null)
                    {
                        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
                        count++;
                        Debug.Log($"Removed missing script from: {obj.name}");
                        break;
                    }
                }
            }
            
            if (count > 0)
            {
                Debug.Log($"✅ Cleaned up {count} missing scripts!");
            }
            else
            {
                Debug.Log("No missing scripts found.");
            }
        }
#endif
    }
}
