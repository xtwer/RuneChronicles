using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 战斗结算UI - 显示胜利/失败
/// </summary>
public class BattleResultUI : MonoBehaviour
{
    public bool isVictory;
    
    private bool initialized = false;

    public void Init()
    {
        if (initialized) return;
        initialized = true;
        CreateResultUI();
    }

    void Start() { Init(); }
    
    void CreateResultUI()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("ResultCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 清除旧UI内容
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(canvas.transform.GetChild(i).gameObject);

        // 半透明背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        var bgImage = bgObj.AddComponent<Image>();
        bgImage.color = new Color(0, 0, 0, 0.8f);
        
        // 结果面板
        var panelObj = new GameObject("ResultPanel");
        panelObj.transform.SetParent(canvas.transform, false);
        var panelRect = panelObj.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.3f, 0.3f);
        panelRect.anchorMax = new Vector2(0.7f, 0.7f);
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        var panelImage = panelObj.AddComponent<Image>();
        panelImage.color = isVictory ? new Color(0.2f, 0.5f, 0.3f) : new Color(0.5f, 0.2f, 0.2f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panelObj.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.6f);
        titleRect.anchorMax = new Vector2(1, 0.9f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<Text>();
        titleText.text = isVictory ? "胜利！" : "失败...";
        titleText.fontSize = 72;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        titleText.font = ChineseUI.GetChineseFont();
        
        // 按钮
        if (isVictory)
        {
            CreateButton(panelObj.transform, "继续冒险", new Vector2(-170, -50), OnContinue);
            CreateButton(panelObj.transform, "返回主菜单", new Vector2(170, -50), OnGameOver);
        }
        else
        {
            CreateButton(panelObj.transform, "返回主菜单", new Vector2(0, -50), OnGameOver);
        }
        
        if (isVictory) AudioManager.Instance?.PlayVictorySFX();
        else AudioManager.Instance?.PlayDefeatSFX();

        Debug.Log($"[BattleResultUI] 战斗结果: {(isVictory ? "胜利" : "失败")}");
    }
    
    void CreateButton(Transform parent, string text, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject($"Button_{text}");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.3f);
        btnRect.anchorMax = new Vector2(0.5f, 0.3f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = new Vector2(300, 80);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.3f, 0.6f, 0.9f);
        
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
        tmp.font = ChineseUI.GetChineseFont();
    }
    
    void OnContinue()
    {
        Debug.Log("[BattleResultUI] 继续 - 显示奖励");
        
        // 销毁结算UI
        Destroy(gameObject);
        
        // 显示奖励UI
        var rewardObj = new GameObject("RewardUI");
        rewardObj.AddComponent<RewardUI>().Init();
    }
    
    void OnGameOver()
    {
        Debug.Log("[BattleResultUI] 游戏结束 - 返回主菜单");

        // 清理所有战斗相关UI
        var battleUI = FindObjectOfType<BattleUI>();
        if (battleUI != null) Destroy(battleUI.gameObject);

        // 销毁结算UI自己（保留Canvas）
        Destroy(gameObject);

        // 返回主菜单
        var menuObj = new GameObject("MainMenuUI");
        menuObj.AddComponent<MainMenuUI_Chinese>().Init();
    }
}
