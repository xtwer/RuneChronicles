using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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
    private List<Text> enemyHPTexts = new List<Text>();
    private List<Slider> enemyHPBars = new List<Slider>();
    private GameObject draggedCard = null;
    public Enemy targetEnemy = null;
    private Text combatLogText;
    
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
            BattleManager.Instance.OnBattleEnd += OnBattleEnd;
            BattleManager.Instance.OnEnemyTurnEnd += OnEnemyTurnEnd;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnHPChanged += UpdatePlayerHP;
            Player.Instance.OnGainBlock += UpdatePlayerBlock;
            Player.Instance.OnTakeDamage += OnPlayerTakeDamage;
        }

        // 事件可能在 Start() 之前已触发（第一回合），手动同步一次
        RefreshHand();
        UpdateUI();
    }

    void OnDestroy()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.OnPlayerTurnStart -= OnPlayerTurnStart;
            BattleManager.Instance.OnEnergyChanged -= UpdateEnergy;
            BattleManager.Instance.OnCardDrawn -= OnCardDrawn;
            BattleManager.Instance.OnCardPlayed -= OnCardPlayed;
            BattleManager.Instance.OnBattleEnd -= OnBattleEnd;
            BattleManager.Instance.OnEnemyTurnEnd -= OnEnemyTurnEnd;
        }

        if (Player.Instance != null)
        {
            Player.Instance.OnHPChanged -= UpdatePlayerHP;
            Player.Instance.OnGainBlock -= UpdatePlayerBlock;
            Player.Instance.OnTakeDamage -= OnPlayerTakeDamage;
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
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        // 清除旧UI内容
        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(canvas.transform.GetChild(i).gameObject);

        // 战斗背景
        var bgObj = new GameObject("BattleBG");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.12f, 0.15f, 0.2f);

        // 玩家面板（底部）
        CreatePlayerPanel(canvas.transform);

        // 敌人面板（顶部）
        CreateEnemyPanel(canvas.transform);

        // 填充敌人UI
        PopulateEnemyPanel();

        // 手牌面板（底部中间）
        CreateHandPanel(canvas.transform);

        // 按钮面板（右下）
        CreateButtonPanel(canvas.transform);

        // 初始化UI数值
        UpdateUI();

        Debug.Log("[BattleUI] 战斗UI创建完成");
    }
    
    void CreatePlayerPanel(Transform parent)
    {
        playerPanel = new GameObject("PlayerPanel");
        playerPanel.transform.SetParent(parent, false);
        
        var rect = playerPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.05f, 0.05f);
        rect.anchorMax = new Vector2(0.25f, 0.25f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var bg = playerPanel.AddComponent<Image>();
        bg.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        // 生命值
        CreatePlayerStat(playerPanel.transform, "HP", new Vector2(0, 0.6f), out playerHPText, out playerHPBar);
        
        // 护盾
        CreatePlayerStat(playerPanel.transform, "Block", new Vector2(0, 0.3f), out playerBlockText, out _);

        // 战斗日志（玩家面板正下方）
        var logObj = new GameObject("CombatLog");
        logObj.transform.SetParent(parent, false);
        var logRect = logObj.AddComponent<RectTransform>();
        logRect.anchorMin = new Vector2(0.05f, 0.27f);
        logRect.anchorMax = new Vector2(0.25f, 0.38f);
        logRect.offsetMin = Vector2.zero;
        logRect.offsetMax = Vector2.zero;
        logObj.AddComponent<Image>().color = new Color(0, 0, 0, 0.5f);

        var logTextObj = new GameObject("LogText");
        logTextObj.transform.SetParent(logObj.transform, false);
        var logTextRect = logTextObj.AddComponent<RectTransform>();
        logTextRect.anchorMin = Vector2.zero;
        logTextRect.anchorMax = Vector2.one;
        logTextRect.offsetMin = new Vector2(4, 2);
        logTextRect.offsetMax = new Vector2(-4, -2);
        combatLogText = logTextObj.AddComponent<Text>();
        combatLogText.text = "";
        combatLogText.fontSize = 16;
        combatLogText.alignment = TextAnchor.MiddleCenter;
        combatLogText.color = new Color(1f, 0.8f, 0.6f);
        combatLogText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
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
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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
        rect.anchorMin = new Vector2(0.25f, 0.65f);
        rect.anchorMax = new Vector2(0.75f, 0.95f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
        var layout = enemyPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 20;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
    }
    
    void PopulateEnemyPanel()
    {
        if (BattleManager.Instance == null || enemyPanel == null) return;

        enemyObjects.Clear();
        enemyHPTexts.Clear();
        enemyHPBars.Clear();

        var enemies = BattleManager.Instance.GetAliveEnemies();
        foreach (var enemy in enemies)
        {
            if (enemy == null) continue;

            var cardObj = new GameObject($"Enemy_{enemy.enemyName}");
            cardObj.transform.SetParent(enemyPanel.transform, false);

            var rect = cardObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(200, 250);

            var bg = cardObj.AddComponent<Image>();
            bg.color = new Color(0.4f, 0.15f, 0.15f, 0.9f);

            // 敌人名称
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(cardObj.transform, false);
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.8f);
            nameRect.anchorMax = new Vector2(1, 1);
            nameRect.offsetMin = Vector2.zero;
            nameRect.offsetMax = Vector2.zero;
            var nameText = nameObj.AddComponent<Text>();
            nameText.text = enemy.enemyName;
            nameText.fontSize = 22;
            nameText.fontStyle = FontStyle.Bold;
            nameText.alignment = TextAnchor.MiddleCenter;
            nameText.color = new Color(1f, 0.7f, 0.7f);
            nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            // HP文本
            var hpObj = new GameObject("HP");
            hpObj.transform.SetParent(cardObj.transform, false);
            var hpRect = hpObj.AddComponent<RectTransform>();
            hpRect.anchorMin = new Vector2(0, 0.6f);
            hpRect.anchorMax = new Vector2(1, 0.78f);
            hpRect.offsetMin = Vector2.zero;
            hpRect.offsetMax = Vector2.zero;
            var hpText = hpObj.AddComponent<Text>();
            hpText.text = $"HP: {enemy.currentHP}/{enemy.maxHP}";
            hpText.fontSize = 20;
            hpText.alignment = TextAnchor.MiddleCenter;
            hpText.color = Color.white;
            hpText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            enemyHPTexts.Add(hpText);

            // HP条背景
            var barBgObj = new GameObject("HPBarBG");
            barBgObj.transform.SetParent(cardObj.transform, false);
            var barBgRect = barBgObj.AddComponent<RectTransform>();
            barBgRect.anchorMin = new Vector2(0.05f, 0.5f);
            barBgRect.anchorMax = new Vector2(0.95f, 0.58f);
            barBgRect.offsetMin = Vector2.zero;
            barBgRect.offsetMax = Vector2.zero;
            barBgObj.AddComponent<Image>().color = Color.red;

            // HP条填充区
            var fillAreaObj = new GameObject("FillArea");
            fillAreaObj.transform.SetParent(barBgObj.transform, false);
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
            fillObj.AddComponent<Image>().color = new Color(0.8f, 0.2f, 0.2f);

            var slider = barBgObj.AddComponent<Slider>();
            slider.fillRect = fillRect;
            slider.minValue = 0;
            slider.maxValue = enemy.maxHP;
            slider.value = enemy.currentHP;
            enemyHPBars.Add(slider);

            // 攻击意图
            var intentObj = new GameObject("Intent");
            intentObj.transform.SetParent(cardObj.transform, false);
            var intentRect = intentObj.AddComponent<RectTransform>();
            intentRect.anchorMin = new Vector2(0, 0.25f);
            intentRect.anchorMax = new Vector2(1, 0.45f);
            intentRect.offsetMin = Vector2.zero;
            intentRect.offsetMax = Vector2.zero;
            var intentText = intentObj.AddComponent<Text>();
            intentText.text = $"攻击: {enemy.minDamage}-{enemy.maxDamage}";
            intentText.fontSize = 18;
            intentText.alignment = TextAnchor.MiddleCenter;
            intentText.color = new Color(1f, 0.5f, 0.5f);
            intentText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            enemyObjects.Add(cardObj);

            // 点击敌人时设为目标
            var button = cardObj.AddComponent<Button>();
            var capturedEnemy = enemy;
            button.onClick.AddListener(() => { targetEnemy = capturedEnemy; });
        }
    }

    void RefreshEnemyPanel()
    {
        if (BattleManager.Instance == null) return;

        var enemies = BattleManager.Instance.GetAliveEnemies();
        for (int i = 0; i < enemyHPTexts.Count && i < enemies.Count; i++)
        {
            if (enemyHPTexts[i] != null)
                enemyHPTexts[i].text = $"HP: {enemies[i].currentHP}/{enemies[i].maxHP}";
            if (enemyHPBars[i] != null)
                enemyHPBars[i].value = enemies[i].currentHP;
        }
    }

    void CreateHandPanel(Transform parent)
    {
        handPanel = new GameObject("HandPanel");
        handPanel.transform.SetParent(parent, false);
        
        var rect = handPanel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.25f, 0.05f);
        rect.anchorMax = new Vector2(0.75f, 0.35f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;        
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
        rect.anchorMin = new Vector2(0.80f, 0.05f);
        rect.anchorMax = new Vector2(0.95f, 0.25f);
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        
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
        energyText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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
        turnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

        // 投降按钮
        var surrenderObj = new GameObject("SurrenderButton");
        surrenderObj.transform.SetParent(buttonPanel.transform, false);
        var surrenderRect = surrenderObj.AddComponent<RectTransform>();
        surrenderRect.anchorMin = new Vector2(0, 0.42f);
        surrenderRect.anchorMax = new Vector2(1, 0.55f);
        surrenderRect.offsetMin = Vector2.zero;
        surrenderRect.offsetMax = Vector2.zero;
        surrenderObj.AddComponent<Image>().color = new Color(0.4f, 0.4f, 0.4f);
        surrenderObj.AddComponent<Button>().onClick.AddListener(OnSurrender);

        var surTextObj = new GameObject("Text");
        surTextObj.transform.SetParent(surrenderObj.transform, false);
        var surTextRect = surTextObj.AddComponent<RectTransform>();
        surTextRect.anchorMin = Vector2.zero;
        surTextRect.anchorMax = Vector2.one;
        surTextRect.offsetMin = Vector2.zero;
        surTextRect.offsetMax = Vector2.zero;
        var surText = surTextObj.AddComponent<Text>();
        surText.text = "投降";
        surText.fontSize = 20;
        surText.alignment = TextAnchor.MiddleCenter;
        surText.color = Color.white;
        surText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
    }
    
    // 事件处理
    void OnPlayerTurnStart()
    {
        RefreshHand();
        UpdateUI();
        // 新回合开始时清除战斗日志
        if (combatLogText != null) combatLogText.text = "";
    }

    void OnEnemyTurnEnd()
    {
        // 强制刷新玩家HP和护盾显示
        if (Player.Instance != null)
            UpdatePlayerHP(Player.Instance.currentHP, Player.Instance.maxHP);
        UpdatePlayerBlock(0);
    }

    void OnPlayerTakeDamage(int damage)
    {
        if (combatLogText == null || Player.Instance == null) return;
        if (damage <= 0)
            combatLogText.text = $"护盾吸收了攻击！\n护盾剩余: {Player.Instance.currentBlock}";
        else
            combatLogText.text = $"受到 {damage} 点伤害！\nHP: {Player.Instance.currentHP}/{Player.Instance.maxHP}";
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
            playerHPText.text = $"生命: {currentHP}/{maxHP}";

        if (playerHPBar != null)
            playerHPBar.value = maxHP > 0 ? (float)currentHP / maxHP : 0;

        // 同步刷新护盾（OnHPChanged 在护盾变化时也会触发）
        UpdatePlayerBlock(0);
    }

    void UpdatePlayerBlock(int block)
    {
        if (playerBlockText != null && Player.Instance != null)
            playerBlockText.text = $"护盾: {Player.Instance.currentBlock}";
    }
    
    void OnCardDrawn(CardData card)
    {
        CreateCardUI(card);
    }
    
    void OnCardPlayed(CardData card)
    {
        RefreshHand();
        RefreshEnemyPanel();
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
        nameText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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
        costText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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
        valueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
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

        // 从 BattleManager 重建手牌
        if (BattleManager.Instance != null)
        {
            foreach (var card in BattleManager.Instance.GetHand())
            {
                CreateCardUI(card);
            }
        }
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
                turnText.text = $"回合: {BattleManager.Instance.currentTurn}";

            // 更新敌人HP显示
            RefreshEnemyPanel();

            // 默认目标为第一个存活敌人
            if (targetEnemy == null || targetEnemy.currentHP <= 0)
            {
                var alive = BattleManager.Instance.GetAliveEnemies();
                targetEnemy = alive.Count > 0 ? alive[0] : null;
            }
        }

        if (Player.Instance != null)
        {
            UpdatePlayerHP(Player.Instance.currentHP, Player.Instance.maxHP);
            UpdatePlayerBlock(0);
        }
    }
    
    void OnBattleEnd()
    {
        Debug.Log("[BattleUI] 战斗结束，显示结算UI");

        bool isVictory = BattleManager.Instance == null ||
                         BattleManager.Instance.GetAliveEnemies().Count == 0;

        // 销毁战斗UI
        Destroy(gameObject);

        // 显示战斗结算UI
        var resultObj = new GameObject("BattleResultUI");
        var resultUI = resultObj.AddComponent<BattleResultUI>();
        resultUI.isVictory = isVictory;
        resultUI.Init();
    }

    void OnEndTurn()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.EndPlayerTurn();
        }
    }

    void OnSurrender()
    {
        Debug.Log("[BattleUI] 玩家投降");
        Destroy(gameObject);

        var canvas = FindObjectOfType<Canvas>();
        if (canvas != null) Destroy(canvas.gameObject);

        var menuObj = new GameObject("MainMenuUI");
        menuObj.AddComponent<MainMenuUI_Chinese>().Init();
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
        
        // 打出到当前选中目标（默认第一个存活敌人）
        if (battleUI != null)
        {
            var target = battleUI.targetEnemy;
            if (target == null || target.currentHP <= 0)
            {
                var enemies = BattleManager.Instance?.GetAliveEnemies();
                target = (enemies != null && enemies.Count > 0) ? enemies[0] : null;
            }
            battleUI.PlayCard(card, target);
        }
        
        // 回到原位
        rectTransform.anchoredPosition = originalPosition;
    }
}
