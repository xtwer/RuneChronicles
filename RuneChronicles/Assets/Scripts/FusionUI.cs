using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 融合UI - 选择两张卡进行融合
/// </summary>
public class FusionUI : MonoBehaviour
{
    private CardData selectedCard1 = null;
    private CardData selectedCard2 = null;
    private GameObject card1Slot;
    private GameObject card2Slot;
    private GameObject resultSlot;
    private GameObject deckPanel;
    private Text fusionPointsText;
    private Text fusionStatusText;

    private bool initialized = false;

    public void Init()
    {
        if (initialized) return;
        initialized = true;
        CreateFusionUI();
    }

    void Start() { Init(); }

    void CreateFusionUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("FusionCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
            canvasObj.AddComponent<GraphicRaycaster>();
        }

        for (int i = canvas.transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(canvas.transform.GetChild(i).gameObject);

        // 背景
        var bgObj = new GameObject("Background");
        bgObj.transform.SetParent(canvas.transform, false);
        var bgRect = bgObj.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.offsetMin = Vector2.zero;
        bgRect.offsetMax = Vector2.zero;
        bgObj.AddComponent<Image>().color = new Color(0.1f, 0.12f, 0.18f);

        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.92f);
        titleRect.anchorMax = new Vector2(0.7f, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(titleObj, "卡牌融合", 52, TextAnchor.MiddleCenter, new Color(1f, 0.6f, 1f))
            .fontStyle = FontStyle.Bold;

        // 融合点
        var fpObj = new GameObject("FusionPoints");
        fpObj.transform.SetParent(canvas.transform, false);
        var fpRect = fpObj.AddComponent<RectTransform>();
        fpRect.anchorMin = new Vector2(0.7f, 0.92f);
        fpRect.anchorMax = new Vector2(1f, 1f);
        fpRect.offsetMin = Vector2.zero;
        fpRect.offsetMax = Vector2.zero;
        int fp = FusionManager.Instance != null ? FusionManager.Instance.GetFusionPoints() : 0;
        fusionPointsText = ChineseUI.CreateText(fpObj, $"融合点: {fp}/10", 28, TextAnchor.MiddleCenter, new Color(1f, 0.6f, 1f));

        // 融合区（上半部分）
        CreateFusionArea(canvas.transform);

        // 牌库标题
        var deckTitleObj = new GameObject("DeckTitle");
        deckTitleObj.transform.SetParent(canvas.transform, false);
        var deckTitleRect = deckTitleObj.AddComponent<RectTransform>();
        deckTitleRect.anchorMin = new Vector2(0, 0.27f);
        deckTitleRect.anchorMax = new Vector2(1, 0.34f);
        deckTitleRect.offsetMin = Vector2.zero;
        deckTitleRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(deckTitleObj, "你的牌库（点击选择）", 30, TextAnchor.MiddleCenter, Color.white);

        // 牌库卡牌区
        deckPanel = new GameObject("DeckPanel");
        deckPanel.transform.SetParent(canvas.transform, false);
        var deckRect = deckPanel.AddComponent<RectTransform>();
        deckRect.anchorMin = new Vector2(0.02f, 0.1f);
        deckRect.anchorMax = new Vector2(0.98f, 0.27f);
        deckRect.offsetMin = Vector2.zero;
        deckRect.offsetMax = Vector2.zero;
        var layout = deckPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 8;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        layout.padding = new RectOffset(10, 10, 5, 5);

        if (CardManager.Instance != null)
        {
            var deck = CardManager.Instance.playerDeck;
            int displayCount = Mathf.Min(12, deck.Count);
            for (int i = 0; i < displayCount; i++)
            {
                if (deck[i] != null)
                    CreateDeckCard(deckPanel.transform, deck[i]);
            }
        }

        // 状态提示文字
        var statusObj = new GameObject("StatusText");
        statusObj.transform.SetParent(canvas.transform, false);
        var statusRect = statusObj.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(0, 0.09f);
        statusRect.anchorMax = new Vector2(1f, 0.16f);
        statusRect.offsetMin = Vector2.zero;
        statusRect.offsetMax = Vector2.zero;
        fusionStatusText = ChineseUI.CreateText(statusObj, "", 26, TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.3f));

        // 按钮区（底部）
        CreateActionButtons(canvas.transform);

        Debug.Log("[FusionUI] 融合UI已创建");
    }

    void CreateFusionArea(Transform parent)
    {
        // 卡槽1（左）
        card1Slot = CreateCardSlot(parent, "卡槽 1", new Vector2(-220, 0));

        // "+" 符号
        var plusObj = new GameObject("Plus");
        plusObj.transform.SetParent(parent, false);
        var plusRect = plusObj.AddComponent<RectTransform>();
        plusRect.anchorMin = new Vector2(0.5f, 0.55f);
        plusRect.anchorMax = new Vector2(0.5f, 0.55f);
        plusRect.pivot = new Vector2(0.5f, 0.5f);
        plusRect.anchoredPosition = Vector2.zero;
        plusRect.sizeDelta = new Vector2(80, 80);
        ChineseUI.CreateText(plusObj, "+", 60, TextAnchor.MiddleCenter, Color.white)
            .fontStyle = FontStyle.Bold;

        // 卡槽2（右）
        card2Slot = CreateCardSlot(parent, "卡槽 2", new Vector2(220, 0));

        // "=" 符号
        var eqObj = new GameObject("Equals");
        eqObj.transform.SetParent(parent, false);
        var eqRect = eqObj.AddComponent<RectTransform>();
        eqRect.anchorMin = new Vector2(0.5f, 0.55f);
        eqRect.anchorMax = new Vector2(0.5f, 0.55f);
        eqRect.pivot = new Vector2(0.5f, 0.5f);
        eqRect.anchoredPosition = new Vector2(450, 0);
        eqRect.sizeDelta = new Vector2(80, 80);
        ChineseUI.CreateText(eqObj, "=", 60, TextAnchor.MiddleCenter, Color.white)
            .fontStyle = FontStyle.Bold;

        // 结果槽（右侧）
        resultSlot = CreateCardSlot(parent, "结果", new Vector2(680, 0));
    }

    GameObject CreateCardSlot(Transform parent, string label, Vector2 offset)
    {
        var slotObj = new GameObject($"CardSlot_{label}");
        slotObj.transform.SetParent(parent, false);
        var slotRect = slotObj.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0.5f, 0.55f);
        slotRect.anchorMax = new Vector2(0.5f, 0.55f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.anchoredPosition = offset;
        slotRect.sizeDelta = new Vector2(170, 240);
        slotObj.AddComponent<Image>().color = new Color(0.2f, 0.2f, 0.32f, 0.8f);

        // 标签
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(slotObj.transform, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.88f);
        labelRect.anchorMax = new Vector2(1, 1f);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(labelObj, label, 20, TextAnchor.MiddleCenter, Color.gray);

        // 提示文字
        var hintObj = new GameObject("Hint");
        hintObj.transform.SetParent(slotObj.transform, false);
        var hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.05f, 0.1f);
        hintRect.anchorMax = new Vector2(0.95f, 0.85f);
        hintRect.offsetMin = Vector2.zero;
        hintRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(hintObj, "点击下方\n卡牌选择", 18, TextAnchor.MiddleCenter, new Color(0.5f, 0.5f, 0.6f));

        return slotObj;
    }

    void CreateDeckCard(Transform parent, CardData card)
    {
        var cardObj = new GameObject($"DeckCard_{card.cardId}");
        cardObj.transform.SetParent(parent, false);
        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(110, 160);
        cardObj.AddComponent<Image>().color = GetCardColor(card.rarity);

        var button = cardObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnSelectCard(card));

        // 卡名
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.72f);
        nameRect.anchorMax = new Vector2(1, 1f);
        nameRect.offsetMin = new Vector2(2, 0);
        nameRect.offsetMax = new Vector2(-2, 0);
        ChineseUI.CreateText(nameObj, card.cardName, 13, TextAnchor.MiddleCenter, Color.black);

        // 描述
        var descObj = new GameObject("Desc");
        descObj.transform.SetParent(cardObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.05f, 0.2f);
        descRect.anchorMax = new Vector2(0.95f, 0.7f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        string typeLabel = card.cardType == CardType.Attack ? "攻击" : card.cardType == CardType.Skill ? "技能" : "能力";
        ChineseUI.CreateText(descObj, $"{typeLabel}\n效果:{card.value}", 12, TextAnchor.MiddleCenter, new Color(0.15f, 0.15f, 0.15f));

        // 费用角标
        var costObj = new GameObject("Cost");
        costObj.transform.SetParent(cardObj.transform, false);
        var costRect = costObj.AddComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(0.35f, 0.2f);
        costRect.offsetMin = Vector2.zero;
        costRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(costObj, card.cost.ToString(), 18, TextAnchor.MiddleCenter, new Color(0, 0.2f, 0.9f))
            .fontStyle = FontStyle.Bold;
    }

    void CreateActionButtons(Transform parent)
    {
        CreateButton(parent, "融合",
            new Vector2(0.35f, 0.02f), new Vector2(0.65f, 0.09f),
            OnFuse, new Color(0.6f, 0.2f, 0.85f));

        CreateButton(parent, "清空选择",
            new Vector2(0.05f, 0.02f), new Vector2(0.3f, 0.09f),
            OnClear, new Color(0.4f, 0.4f, 0.4f));

        CreateButton(parent, "离开",
            new Vector2(0.7f, 0.02f), new Vector2(0.95f, 0.09f),
            OnLeave, new Color(0.3f, 0.5f, 0.3f));
    }

    void CreateButton(Transform parent, string label,
        Vector2 anchorMin, Vector2 anchorMax,
        UnityEngine.Events.UnityAction action, Color color)
    {
        var btnObj = new GameObject($"Btn_{label}");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = anchorMin;
        btnRect.anchorMax = anchorMax;
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        btnObj.AddComponent<Image>().color = color;
        btnObj.AddComponent<Button>().onClick.AddListener(action);

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(textObj, label, 28, TextAnchor.MiddleCenter, Color.white);
    }

    Color GetCardColor(CardRarity rarity)
    {
        switch (rarity)
        {
            case CardRarity.Common: return new Color(0.88f, 0.88f, 0.88f);
            case CardRarity.Rare: return new Color(0.7f, 0.85f, 1f);
            case CardRarity.Epic: return new Color(0.8f, 0.6f, 1f);
            case CardRarity.Legendary: return new Color(1f, 0.8f, 0.3f);
            default: return Color.white;
        }
    }

    void OnSelectCard(CardData card)
    {
        if (selectedCard1 == null)
        {
            selectedCard1 = card;
            ShowCardInSlot(card1Slot, card);
            Debug.Log($"[FusionUI] 卡槽1: {card.cardName}");
        }
        else if (selectedCard2 == null && card.cardId != selectedCard1.cardId)
        {
            selectedCard2 = card;
            ShowCardInSlot(card2Slot, card);
            Debug.Log($"[FusionUI] 卡槽2: {card.cardName}");
        }
        else
        {
            Debug.Log("[FusionUI] 已选择两张卡，或重复选择");
        }
    }

    void ShowCardInSlot(GameObject slot, CardData card)
    {
        // 清除旧内容（保留Label）
        for (int i = slot.transform.childCount - 1; i >= 0; i--)
        {
            var child = slot.transform.GetChild(i);
            if (child.name != "Label")
                Destroy(child.gameObject);
        }

        slot.GetComponent<Image>().color = GetCardColor(card.rarity);

        var infoObj = new GameObject("CardInfo");
        infoObj.transform.SetParent(slot.transform, false);
        var infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0.05f, 0.05f);
        infoRect.anchorMax = new Vector2(0.95f, 0.85f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
        string typeLabel = card.cardType == CardType.Attack ? "攻击" : card.cardType == CardType.Skill ? "技能" : "能力";
        ChineseUI.CreateText(infoObj,
            $"{card.cardName}\n\n{typeLabel}\n费用: {card.cost}\n效果: {card.value}",
            15, TextAnchor.MiddleCenter, Color.white);
    }

    void ClearSlot(GameObject slot)
    {
        for (int i = slot.transform.childCount - 1; i >= 0; i--)
        {
            var child = slot.transform.GetChild(i);
            if (child.name != "Label")
                Destroy(child.gameObject);
        }
        slot.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.32f, 0.8f);

        // 恢复提示文字
        var hintObj = new GameObject("Hint");
        hintObj.transform.SetParent(slot.transform, false);
        var hintRect = hintObj.AddComponent<RectTransform>();
        hintRect.anchorMin = new Vector2(0.05f, 0.1f);
        hintRect.anchorMax = new Vector2(0.95f, 0.85f);
        hintRect.offsetMin = Vector2.zero;
        hintRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(hintObj, "点击下方\n卡牌选择", 18, TextAnchor.MiddleCenter, new Color(0.5f, 0.5f, 0.6f));
    }

    void UpdateFusionPoints()
    {
        if (fusionPointsText != null && FusionManager.Instance != null)
        {
            int fp = FusionManager.Instance.GetFusionPoints();
            fusionPointsText.text = $"融合点: {fp}/10";
        }
    }

    void OnFuse()
    {
        if (selectedCard1 == null || selectedCard2 == null)
        {
            SetStatus("请先选择两张卡牌！", new Color(1f, 0.4f, 0.4f));
            return;
        }
        if (FusionManager.Instance == null) return;
        if (FusionManager.Instance.GetFusionPoints() < FusionManager.Instance.fusionPointCost)
        {
            SetStatus($"融合点不足！需要 {FusionManager.Instance.fusionPointCost} 点", new Color(1f, 0.4f, 0.4f));
            return;
        }

        var result = FusionManager.Instance.FuseCards(selectedCard1, selectedCard2);
        if (result != null)
        {
            Debug.Log($"[FusionUI] 融合成功: {result.cardName}");
            ShowCardInSlot(resultSlot, result);

            if (CardManager.Instance != null)
            {
                CardManager.Instance.AddCardToDeck(result.cardId);
                CardManager.Instance.RemoveCardFromDeck(selectedCard1);
                CardManager.Instance.RemoveCardFromDeck(selectedCard2);
            }

            // 只清空选择槽，保留结果槽展示
            selectedCard1 = null;
            selectedCard2 = null;
            ClearSlot(card1Slot);
            ClearSlot(card2Slot);

            UpdateFusionPoints();
            RefreshDeckPanel();
            AudioManager.Instance?.PlayFusionSFX();
            SetStatus($"融合成功！获得【{result.cardName}】", new Color(0.3f, 1f, 0.5f));
        }
        else
        {
            SetStatus("融合失败！", new Color(1f, 0.4f, 0.4f));
        }
    }

    void SetStatus(string msg, Color color)
    {
        if (fusionStatusText != null)
        {
            fusionStatusText.text = msg;
            fusionStatusText.color = color;
        }
        Debug.Log($"[FusionUI] {msg}");
    }

    void RefreshDeckPanel()
    {
        if (deckPanel == null || CardManager.Instance == null) return;

        for (int i = deckPanel.transform.childCount - 1; i >= 0; i--)
            Destroy(deckPanel.transform.GetChild(i).gameObject);

        var deck = CardManager.Instance.playerDeck;
        int displayCount = Mathf.Min(12, deck.Count);
        for (int i = 0; i < displayCount; i++)
        {
            if (deck[i] != null)
                CreateDeckCard(deckPanel.transform, deck[i]);
        }
    }

    void OnClear()
    {
        selectedCard1 = null;
        selectedCard2 = null;
        ClearSlot(card1Slot);
        ClearSlot(card2Slot);
        ClearSlot(resultSlot);
        SetStatus("", Color.white);
        Debug.Log("[FusionUI] 已清空选择");
    }

    void OnLeave()
    {
        Debug.Log("[FusionUI] 离开融合界面");
        Destroy(gameObject);
        var mapObj = new GameObject("MapUI");
        mapObj.AddComponent<MapUI>().Init();
    }
}
