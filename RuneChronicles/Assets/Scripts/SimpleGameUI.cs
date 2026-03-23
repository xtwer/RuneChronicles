using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 最简单的游戏UI - 可以实际玩游戏
/// </summary>
public class SimpleGameUI : MonoBehaviour
{
    [Header("UI引用")]
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI cardInfoText;
    public Button startButton;
    public Button drawButton;
    public Button endTurnButton;
    
    void Start()
    {
        CreateUI();
    }
    
    void CreateUI()
    {
        // 创建Canvas（如果不存在）
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 状态文本（顶部）
        var statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(canvas.transform, false);
        var statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 1);
        statusRect.anchorMax = new Vector2(1, 1);
        statusRect.pivot = new Vector2(0.5f, 1);
        statusRect.anchoredPosition = new Vector2(0, -20);
        statusRect.sizeDelta = new Vector2(-40, 100);
        
        statusText = statusObj.AddComponent<TextMeshProUGUI>();
        statusText.text = "符文编年史 - MVP测试版\n点击\"开始游戏\"开始";
        statusText.fontSize = 24;
        statusText.alignment = TextAlignmentOptions.Top;
        statusText.color = Color.white;
        
        // 卡牌信息文本（中间）
        var cardInfoObj = new GameObject("CardInfoText");
        cardInfoObj.transform.SetParent(canvas.transform, false);
        var cardInfoRect = cardInfoObj.AddComponent<RectTransform>();
        cardInfoRect.anchorMin = new Vector2(0, 0);
        cardInfoRect.anchorMax = new Vector2(1, 1);
        cardInfoRect.pivot = new Vector2(0.5f, 0.5f);
        cardInfoRect.anchoredPosition = Vector2.zero;
        cardInfoRect.sizeDelta = new Vector2(-100, -300);
        
        cardInfoText = cardInfoObj.AddComponent<TextMeshProUGUI>();
        cardInfoText.text = "等待开始...";
        cardInfoText.fontSize = 20;
        cardInfoText.alignment = TextAlignmentOptions.TopLeft;
        cardInfoText.color = new Color(0.9f, 0.9f, 0.9f);
        
        // 按钮区域（底部）
        CreateButton(canvas.transform, "StartButton", "开始游戏", new Vector2(-250, 50), OnStartGame);
        CreateButton(canvas.transform, "DrawButton", "抽牌", new Vector2(0, 50), OnDrawCard);
        CreateButton(canvas.transform, "EndTurnButton", "结束回合", new Vector2(250, 50), OnEndTurn);
        
        Debug.Log("[SimpleGameUI] UI已创建");
    }
    
    void CreateButton(Transform parent, string name, string label, Vector2 pos, UnityEngine.Events.UnityAction action)
    {
        var btnObj = new GameObject(name);
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0);
        btnRect.anchorMax = new Vector2(0.5f, 0);
        btnRect.pivot = new Vector2(0.5f, 0);
        btnRect.anchoredPosition = pos;
        btnRect.sizeDelta = new Vector2(200, 60);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.2f, 0.4f, 0.8f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(action);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 24;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
    }
    
    void Update()
    {
        UpdateStatus();
    }
    
    void UpdateStatus()
    {
        if (GameManager.Instance == null || CardManager.Instance == null) return;
        
        string status = "=== 游戏状态 ===\n";
        status += $"状态: {GameManager.Instance.currentState}\n";
        status += $"金币: {GameManager.Instance.currentGold}\n";
        
        if (Player.Instance != null)
        {
            status += $"生命: {Player.Instance.currentHP}/{Player.Instance.maxHP}\n";
            status += $"护盾: {Player.Instance.currentBlock}\n";
        }
        
        if (BattleManager.Instance != null)
        {
            status += $"能量: {BattleManager.Instance.currentEnergy}/{BattleManager.Instance.maxEnergy}\n";
            status += $"回合: {BattleManager.Instance.currentTurn}\n";
        }
        
        if (statusText != null)
        {
            statusText.text = status;
        }
        
        // 显示卡牌信息
        UpdateCardInfo();
    }
    
    void UpdateCardInfo()
    {
        if (CardManager.Instance == null || cardInfoText == null) return;
        
        string info = "=== 可用卡牌 ===\n\n";
        
        var cards = CardManager.Instance.GetAllCards();
        int count = Mathf.Min(10, cards.Count); // 只显示前10张
        
        for (int i = 0; i < count; i++)
        {
            var card = cards[i];
            info += $"{i+1}. {card.cardName} (费用:{card.cost}, 效果:{card.value})\n";
            info += $"   {card.description}\n\n";
        }
        
        info += $"...共{cards.Count}张卡牌";
        
        cardInfoText.text = info;
    }
    
    void OnStartGame()
    {
        Debug.Log("[SimpleGameUI] 开始游戏");
        
        if (GameManager.Instance != null)
        {
            GameManager.Instance.DEBUG_QuickStartMage();
            Debug.Log("游戏已开始！选择了法师角色");
        }
    }
    
    void OnDrawCard()
    {
        Debug.Log("[SimpleGameUI] 抽牌");
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.DrawCard();
        }
    }
    
    void OnEndTurn()
    {
        Debug.Log("[SimpleGameUI] 结束回合");
        
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPlayerTurn();
        }
    }
}
