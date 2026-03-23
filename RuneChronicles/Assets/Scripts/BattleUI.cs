using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 完整的战斗UI系统
/// </summary>
public class BattleUI : MonoBehaviour
{
    [Header("UI面板")]
    public GameObject handPanel;
    public GameObject enemyPanel;
    public GameObject playerPanel;
    public GameObject buttonPanel;
    
    [Header("玩家信息")]
    public Text playerHPText;
    public Text playerBlockText;
    public Slider playerHPBar;
    
    [Header("能量信息")]
    public Text energyText;
    
    [Header("回合信息")]
    public Text turnText;
    public Button endTurnButton;
    
    [Header("卡牌预制体")]
    public GameObject cardPrefab;
    
    [Header("敌人预制体")]
    public GameObject enemyPrefab;
    
    private List<GameObject> handCardObjects = new List<GameObject>();
    private List<GameObject> enemyObjects = new List<GameObject>();
    private GameObject draggedCard = null;
    private Enemy targetEnemy = null;
    
    void Start()
    {
        CreateBattleUI();
        
        // 订阅事件
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart += OnPlayerTurnStart;
            BattleManager.Instance.OnEnergyChanged += UpdateEnergy;
            BattleManager.Instance.OnCardDrawn += OnCardDrawn;
            BattleManager.Instance.OnCardPlayed += OnCardPlayed;
        }
        
        if (Player.Instance != null)
        {
            Player.Instance.OnHPChanged += UpdatePlayerHP;
            Player.Instance.OnGainBlock += UpdatePlayerBlock;
        }
    }
    
    void CreateBattleUI()
    {
        Debug.Log("[BattleUI] 创建战斗UI...");
        
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("BattleCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
        }
        
        // 玩家面板（底部）
        CreatePlayerPanel(canvas.transform);
        
        // 敌人面板（顶部）
        CreateEnemyPanel(canvas.transform);
        
        // 手牌面板（底部中间）
        CreateHandPanel(canvas.transform);
        
        // 按钮面板（右下）
        CreateButtonPanel(canvas.transform);
        
        Debug.Log("[BattleUI] 战斗UI创建完成");
    }
    
    void CreatePlayerPanel(Transform parent)
    {
        playerPanel = new GameObject("PlayerPanel");
        playerPanel.transform.SetParent(parent, false);
        
        var rect = playerPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0.3f, 0.15f);
        rect.offsetMin = new Vector2(20, 20);
        rect.offsetMax = new Vector2(-20, -20);
        
        var bg = playerPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // 生命值
        CreatePlayerStat(playerPanel.transform, "HP", new Vector2(0, 0.6f), out playerHPText, out playerHPBar);
        
        // 护盾
        CreatePlayerStat(playerPanel.transform, "Block", new Vector2(0, 0.3f), out playerBlockText, out _);
    }
    
    void CreatePlayerStat(Transform parent, string label, Vector2 yPos, out Text text, out Slider slider)
    {
        var statObj = new GameObject(label);
        statObj.transform.SetParent(parent, false);
        
        var rect = statObj.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.1f, yPos.y);
        rect.anchorMax = new Vector2(0.9f, yPos.y + 0.15f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        text = statObj.AddComponent<Text>();
        text.text = $"{label}: 0";
        text.fontSize = 24;
        text.alignment = TextAnchor.MiddleLeft;
        text.color = Color.white;
        
        // 血条（只给HP）
        if (label == "HP")
        {
            var sliderObj = new GameObject("HPBar");
            sliderObj.transform.SetParent(parent, false);
            
            var sliderRect = sliderObj.AddComponent<RectTransform>();
            sliderRect.anchorMin = new Vector2(0.1f, 0.45f);
            sliderRect.anchorMax = new Vector2(0.9f, 0.5f);
            sliderRect.offsetMin = Vector2.zero;
            sliderRect.offsetMax = Vector2.zero;
            
            slider = sliderObj.AddComponent<Slider>();
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
            
            // Background
            var bgObj = new GameObject("Background");
            bgObj.transform.SetParent(sliderObj.transform, false);
            var bgRect = bgObj.AddComponent<RectTransform>();
            bgRect.anchorMin = Vector2.zero;
            bgRect.anchorMax = Vector2.one;
            bgRect.offsetMin = Vector2.zero;
            bgRect.offsetMax = Vector2.zero;
            var bgImage = bgObj.AddComponent<Image>();
            bgImage.color = Color.red;
            
            // Fill
            var fillAreaObj = new GameObject("Fill Area");
            fillAreaObj.transform.SetParent(sliderObj.transform, false);
            var fillAreaRect = fillAreaObj.AddComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = Vector2.zero;
            fillAreaRect.offsetMax = Vector2.zero;
            
            var fillObj = new GameObject("Fill");
            fillObj.transform.SetParent(fillAreaObj.transform, false);
            var fillRect = fillObj.AddComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
            var fillImage = fillObj.AddComponent<Image>();
            fillImage.color = Color.green;
            
            slider.fillRect = fillRect;
        }
        else
        {
            slider = null;
        }
    }
    
    void CreateEnemyPanel(Transform parent)
    {
        enemyPanel = new GameObject("EnemyPanel");
        enemyPanel.transform.SetParent(parent, false);
        
        var rect = enemyPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0.6f);
        rect.anchorMax = new Vector2(0.8f, 0.9f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var layout = enemyPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }
    
    void CreateHandPanel(Transform parent)
    {
        handPanel = new GameObject("HandPanel");
        handPanel.transform.SetParent(parent, false);
        
        var rect = handPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.2f, 0);
        rect.anchorMax = new Vector2(0.8f, 0.3f);
        rect.offsetMin = new Vector2(0, 20);
        rect.offsetMax = new Vector2(0, -20);
        
        var layout = handPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.LowerCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }
    
    void CreateButtonPanel(Transform parent)
    {
        buttonPanel = new GameObject("ButtonPanel");
        buttonPanel.transform.SetParent(parent, false);
        
        var rect = buttonPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.85f, 0);
        rect.anchorMax = new Vector2(1, 0.2f);
        rect.offsetMin = new Vector2(-20, 20);
        rect.offsetMax = new Vector2(-20, -20);
        
        // 能量显示
        var energyObj = new GameObject("EnergyText");
        energyObj.transform.SetParent(buttonPanel.transform, false);
        var energyRect = energyObj.AddComponent<RectTransform>();
        energyRect.anchorMin = new Vector2(0, 0.7f);
        energyRect.anchorMax = new Vector2(1, 1);
        energyRect.offsetMin = Vector2.zero;
        energyRect.offsetMax = Vector2.zero;
        
        energyText = energyObj.AddComponent<Text>();
        energyText.text = "能量: 3/3";
        energyText.fontSize = 28;
        energyText.alignment = TextAnchor.MiddleCenter;
        energyText.color = new Color(0.3f, 0.6f, 1f);
        
        // 回合显示
        var turnObj = new GameObject("TurnText");
        turnObj.transform.SetParent(buttonPanel.transform, false);
        var turnRect = turnObj.AddComponent<RectTransform>();
        turnRect.anchorMin = new Vector2(0, 0.5f);
        turnRect.anchorMax = new Vector2(1, 0.65f);
        turnRect.offsetMin = Vector2.zero;
        turnRect.offsetMax = Vector2.zero;
        
        turnText = turnObj.AddComponent<Text>();
        turnText.text = "回合: 1";
        turnText.fontSize = 20;
        turnText.alignment = TextAnchor.MiddleCenter;
        turnText.color = Color.white;
        
        // 结束回合按钮
        var btnObj = new GameObject("EndTurnButton");
        btnObj.transform.SetParent(buttonPanel.transform, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0, 0);
        btnRect.anchorMax = new Vector2(1, 0.4f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.8f, 0.2f, 0.2f);
        
        endTurnButton = btnObj.AddComponent<Button>();
        endTurnButton.onClick.AddListener(OnEndTurn);
        
        var btnTextObj = new GameObject("Text");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        var btnTextRect = btnTextObj.AddComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero;
        btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero;
        btnTextRect.offsetMax = Vector2.zero;
        
        var btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "结束回合";
        btnText.fontSize = 24;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.color = Color.white;
    }
    
    // 事件处理
    void OnPlayerTurnStart()
    {
        UpdateUI();
    }
    
    void UpdateEnergy(int energy)
    {
        if (energyText != null && BattleManager.Instance != null)
        {
            energyText.text = $"能量: {energy}/{BattleManager.Instance.maxEnergy}";
        }
    }
    
    void UpdatePlayerHP(int currentHP, int maxHP)
    {
        if (playerHPText != null)
        {
            playerHPText.text = $"生命: {currentHP}/{maxHP}";
        }
        
        if (playerHPBar != null)
        {
            playerHPBar.value = (float)currentHP / maxHP;
        }
    }
    
    void UpdatePlayerBlock(int block)
    {
        if (playerBlockText != null && Player.Instance != null)
        {
            playerBlockText.text = $"护盾: {Player.Instance.currentBlock}";
        }
    }
    
    void OnCardDrawn(CardData card)
    {
        CreateCardUI(card);
    }
    
    void OnCardPlayed(CardData card)
    {
        // 移除打出的卡牌UI
        RefreshHand();
    }
    
    void CreateCardUI(CardData card)
    {
        var cardObj = new GameObject($"Card_{card.cardName}");
        cardObj.transform.SetParent(handPanel.transform, false);
        
        var rect = cardObj.AddComponent<RectTransform>();
        rect.sizeDelta = new Vector2(150, 220);
        
        var bg = cardObj.AddComponent<Image>();
        bg.color = GetCardColor(card.rarity);
        
        // 卡牌名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.8f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        var nameText = nameObj.AddComponent<Text>();
        nameText.text = card.cardName;
        nameText.fontSize = 18;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        // 费用
        var costObj = new GameObject("Cost");
        costObj.transform.SetParent(cardObj.transform, false);
        var costRect = costObj.AddComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(0.3f, 0.2f);
        costRect.offsetMin = Vector2.zero;
        costRect.offsetMax = Vector2.zero;
        
        var costText = costObj.AddComponent<Text>();
        costText.text = card.cost.ToString();
        costText.fontSize = 24;
        costText.fontStyle = FontStyle.Bold;
        costText.alignment = TextAnchor.MiddleCenter;
        costText.color = new Color(0, 0.3f, 0.8f);
        
        // 效果值
        var valueObj = new GameObject("Value");
        valueObj.transform.SetParent(cardObj.transform, false);
        var valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0.7f, 0);
        valueRect.anchorMax = new Vector2(1, 0.2f);
        valueRect.offsetMin = Vector2.zero;
        valueRect.offsetMax = Vector2.zero;
        
        var valueText = valueObj.AddComponent<Text>();
        valueText.text = card.value.ToString();
        valueText.fontSize = 24;
        valueText.fontStyle = FontStyle.Bold;
        valueText.alignment = TextAnchor.MiddleCenter;
        valueText.color = Color.red;
        
        // 添加拖拽功能
        var dragger = cardObj.AddComponent<CardDragger>();
        dragger.card = card;
        dragger.battleUI = this;
        
        handCardObjects.Add(cardObj);
    }
    
    void RefreshHand()
    {
        // 清除所有手牌UI
        foreach (var cardObj in handCardObjects)
        {
            if (cardObj != null) Destroy(cardObj);
        }
        handCardObjects.Clear();
        
        // 重新创建（从BattleManager获取）
        // TODO: 实现从BattleManager.hand重建
    }
    
    Color GetCardColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return new Color(0.9f, 0.9f, 0.9f);
            case CardRarity.Rare: return new Color(0.7f, 0.85f, 1f);
            case CardRarity.Epic: return new Color(0.8f, 0.6f, 1f);
            case CardRarity.Legendary: return new Color(1f, 0.8f, 0.3f);
            default: return Color.white;
        }
    }
    
    void UpdateUI()
    {
        if (BattleManager.Instance != null)
        {
            UpdateEnergy(BattleManager.Instance.currentEnergy);
            
            if (turnText != null)
            {
                turnText.text = $"回合: {BattleManager.Instance.currentTurn}";
            }
        }
        
        if (Player.Instance != null)
        {
            UpdatePlayerHP(Player.Instance.currentHP, Player.Instance.maxHP);
            UpdatePlayerBlock(0);
        }
    }
    
    void OnEndTurn()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPlayerTurn();
        }
    }
    
    public void PlayCard(CardData card, Enemy target)
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.PlayCard(card, target);
        }
    }
}

/// <summary>
/// 卡牌拖拽组件
/// </summary>
public class CardDragger : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public CardData card;
    public BattleUI battleUI;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;
    
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }
    
    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        
        // TODO: 检查是否拖到敌人上
        // 暂时直接打出到第一个敌人
        if (battleUI != null)
        {
            var enemies = BattleManager.Instance?.GetAliveEnemies();
            if (enemies != null && enemies.Count > 0)
            {
                battleUI.PlayCard(card, enemies[0]);
            }
        }
        
        // 回到原位
        rectTransform.anchoredPosition = originalPosition;
    }
}
