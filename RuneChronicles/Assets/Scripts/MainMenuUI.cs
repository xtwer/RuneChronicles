using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;

/// <summary>
/// 主菜单UI
/// </summary>
public class MainMenuUI : MonoBehaviour
{
    void Start()
    {
        CreateMainMenu();
    }
    
    void CreateMainMenu()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("MainMenuCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.15f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.9f);
        titleRect.pivot = new Vector2(0.5f, 0.5f);
        titleRect.sizeDelta = new Vector2(800, 200);
        
        var titleText = titleObj.AddComponent<Text>();
        titleText.text = "符文编年史\nRUNE CHRONICLES";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = new Color(1f, 0.8f, 0.3f);
        
        // 按钮组
        CreateMenuButton(canvas.transform, "开始游戏", new Vector2(0, 50), OnStartGame);
        CreateMenuButton(canvas.transform, "设置", new Vector2(0, -50), OnSettings);
        CreateMenuButton(canvas.transform, "退出游戏", new Vector2(0, -150), OnQuitGame);
        
        // 版本信息
        var versionObj = new GameObject("Version");
        versionObj.transform.SetParent(canvas.transform, false);
        var versionRect = versionObj.AddComponent<RectTransform>();
        versionRect.anchorMin = new Vector2(1, 0);
        versionRect.anchorMax = new Vector2(1, 0);
        versionRect.pivot = new Vector2(1, 0);
        versionRect.anchoredPosition = new Vector2(-20, 20);
        versionRect.sizeDelta = new Vector2(200, 30);
        
        var versionText = versionObj.AddComponent<Text>();
        versionText.text = "v1.0 - MVP";
        versionText.fontSize = 18;
        versionText.alignment = TextAnchor.LowerRight;
        versionText.color = new Color(0.5f, 0.5f, 0.5f);
        
        Debug.Log("[MainMenuUI] 主菜单已创建");
    }
    
    void CreateMenuButton(Transform parent, string text, Vector2 offset, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject($"Button_{text}");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.4f);
        btnRect.anchorMax = new Vector2(0.5f, 0.4f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = offset;
        btnRect.sizeDelta = new Vector2(400, 80);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.4f, 0.8f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(action);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var tmp = textObj.AddComponent<Text>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAnchor.MiddleCenter;
        tmp.color = Color.white;
    }
    
    void OnStartGame()
    {
        Debug.Log("[MainMenuUI] 开始游戏 - 切换到角色选择");
        
        // 销毁主菜单
        Destroy(gameObject);
        
        // 创建角色选择UI
        var charSelectObj = new GameObject("CharacterSelectUI");
        charSelectObj.AddComponent<CharacterSelectUI>();
    }
    
    void OnSettings()
    {
        Debug.Log("[MainMenuUI] 设置（未实现）");
    }
    
    void OnQuitGame()
    {
        Debug.Log("[MainMenuUI] 退出游戏");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
