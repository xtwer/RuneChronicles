#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

/// <summary>
/// 编辑器工具：检测并强制重新导入 Resources/Art 和 Resources/Audio 资源
/// 解决外部复制文件后 Resources.Load 找不到资源的问题
/// </summary>
[InitializeOnLoad]
public static class ResourceImporter
{
    static ResourceImporter()
    {
        // 编辑器启动或脚本重新编译后延迟检查（必须在非Play模式下）
        EditorApplication.delayCall += CheckAndImport;
    }

    static void CheckAndImport()
    {
        // 只在非Play模式下执行导入
        if (EditorApplication.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
            return;

        // 检查资源是否已正确导入
        bool artImported = AssetDatabase.LoadAssetAtPath<Texture2D>(
            "Assets/Resources/Art/Cards/ATK_001.png") != null;

        if (!artImported)
        {
            Debug.Log("[ResourceImporter] Art资源未导入，开始强制重新导入...");
            ForceReimportArt();
        }
    }

    [MenuItem("Tools/强制重新导入Art资源 %#r")]
    public static void ForceReimportArt()
    {
        if (EditorApplication.isPlaying)
        {
            Debug.LogWarning("[ResourceImporter] 请先停止游戏再执行重导入");
            return;
        }

        string[] folders = {
            "Assets/Resources/Art",
            "Assets/Resources/Audio",
        };

        foreach (var folder in folders)
        {
            if (AssetDatabase.IsValidFolder(folder))
            {
                AssetDatabase.ImportAsset(folder,
                    ImportAssetOptions.ImportRecursive | ImportAssetOptions.ForceUpdate);
                Debug.Log($"[ResourceImporter] 已重新导入: {folder}");
            }
        }

        AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        Debug.Log("[ResourceImporter] 重导入完成，现在可以运行游戏了");
    }
}
#endif
