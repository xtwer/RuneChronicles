using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 暂停菜单UI - ESC暂停游戏，显示继续/设置/退出选项
/// </summary>
public class PauseMenuUI : MonoBehaviour
{
    public static PauseMenuUI Instance { get; private set; }
    
    private Canvas canvas;
    private GameObject pausePanel;
    private bool isPaused = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        CreatePauseUI();
        pausePanel.SetActive(false);
    }
    
    void Update()
    {
        // ESC键切换暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }
    
    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void Pause()
    {
        if (pausePanel == null) CreatePauseUI();
        
        isPaused = true;
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // 暂停游戏时间
        
        Debug.Log("[PauseMenuUI] 游戏已暂停");
    }
    
    /// <summary>
    /// 继续游戏
    /// </summary>
    public void Resume()
    {
        isPaused = false;
        if (pausePanel != null)
            pausePanel.SetActive(false);
        Time.timeScale = 1f; // 恢复游戏时间
        
        Debug.Log("[PauseMenuUI] 游戏已恢复");
    }
    
    void CreatePauseUI()
    {
        // 查找或创建Canvas
        canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("PauseCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999; // 最顶层
            
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            
            canvasObj.AddComponent<GraphicRaycaster>();
            DontDestroyOnLoad(canvasObj);
        }
        
        // 半透明遮罩背景
        pausePanel = new GameObject("PausePanel");
        pausePanel.transform.SetParent(canvas.transform, false);
        
        var panelRect = pausePanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        var panelImage = pausePanel.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.7f); // 半透明黑色
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(pausePanel.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.3f, 0.7f);
        titleRect.anchorMax = new Vector2(0.7f, 0.85f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        ChineseUI.CreateText(titleObj, "游戏暂停", 72, TextAnchor.MiddleCenter, Color.white)
            .fontStyle = FontStyle.Bold;
        
        // 按钮容器
        float buttonWidth = 400f;
        float buttonHeight = 80f;
        float startY = 0.5f;
        
        // 1. 继续游戏按钮
        CreateButton(pausePanel.transform, "继续游戏", 
            new Vector2(0.5f, startY), new Vector2(buttonWidth, buttonHeight),
            new Color(0.3f, 0.6f, 0.3f), Resume);
        
        // 2. 设置按钮
        CreateButton(pausePanel.transform, "设置", 
            new Vector2(0.5f, startY - 0.12f), new Vector2(buttonWidth, buttonHeight),
            new Color(0.4f, 0.4f, 0.6f), OnSettings);
        
        // 3. 回主菜单按钮
        CreateButton(pausePanel.transform, "回主菜单", 
            new Vector2(0.5f, startY - 0.24f), new Vector2(buttonWidth, buttonHeight),
            new Color(0.6f, 0.4f, 0.3f), OnMainMenu);
        
        // 4. 退出游戏按钮
        CreateButton(pausePanel.transform, "退出游戏", 
            new Vector2(0.5f, startY - 0.36f), new Vector2(buttonWidth, buttonHeight),
            new Color(0.7f, 0.3f, 0.3f), OnQuit);
        
        // 提示文字
        var hintObj = new GameObject("Hint");
        hintObj.transform.SetParent(pausePanel.transform, false);
        var hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.3f, 0.05f);
        hintRect.anchorMax = new Vector2(0.7f, 0.12f);
        hintRect.offsetMin = Vector2.zero;
        hintRect.offsetMax = Vector2.zero;
        
        ChineseUI.CreateText(hintObj, "按 ESC 键继续游戏", 24, TextAnchor.MiddleCenter, new Color(0.8f, 0.8f, 0.8f));
    }
    
    void CreateButton(Transform parent, string label, Vector2 anchorPos, Vector2 sizeDelta, Color color, System.Action onClick)
    {
        var btnObj = new GameObject($"Button_{label}");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = anchorPos;
        btnRect.anchorMax = anchorPos;
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = Vector2.zero;
        btnRect.sizeDelta = sizeDelta;
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = color;
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(() => onClick?.Invoke());
        
        // 悬停效果
        var colors = button.colors;
        colors.highlightedColor = color * 1.2f;
        colors.pressedColor = color * 0.8f;
        button.colors = colors;
        
        // 按钮文字
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        ChineseUI.CreateText(textObj, label, 32, TextAnchor.MiddleCenter, Color.white)
            .fontStyle = FontStyle.Bold;
    }
    
    void OnSettings()
    {
        Debug.Log("[PauseMenuUI] 打开设置");
        
        // 查找或创建SettingsUI
        var settingsUI = FindObjectOfType<SettingsUI>();
        if (settingsUI == null)
        {
            var settingsObj = new GameObject("SettingsUI");
            settingsUI = settingsObj.AddComponent<SettingsUI>();
        }
        
        // 暂时隐藏暂停菜单
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    void OnMainMenu()
    {
        Debug.Log("[PauseMenuUI] 返回主菜单");
        
        // 确认对话框（防止误操作）
        if (!ConfirmAction("确定返回主菜单吗？当前进度可能丢失。"))
            return;
        
        Resume(); // 恢复时间
        
        // 清理当前场景
        if (BattleManager.Instance != null)
            Destroy(BattleManager.Instance.gameObject);
        
        // 加载主菜单场景（如果有）
        // SceneManager.LoadScene("MainMenu");
        
        // 创建中文主菜单
        var mainMenuObj = new GameObject("MainMenuUI");
        mainMenuObj.AddComponent<MainMenuUI_Chinese>();
    }
    
    void OnQuit()
    {
        Debug.Log("[PauseMenuUI] 退出游戏");
        
        if (!ConfirmAction("确定退出游戏吗？"))
            return;
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    /// <summary>
    /// 简单的确认对话框（生产环境应该用更好的UI）
    /// </summary>
    bool ConfirmAction(string message)
    {
        // TODO: 实现更好的确认对话框UI
        // 目前在Editor中使用Unity对话框
        #if UNITY_EDITOR
        return UnityEditor.EditorUtility.DisplayDialog("确认", message, "确定", "取消");
        #else
        // 生产环境直接返回true（或实现自定义对话框）
        return true;
        #endif
    }
    
    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            Time.timeScale = 1f; // 确保时间恢复
        }
    }
}
