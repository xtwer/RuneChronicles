using UnityEngine;
using UnityEngine.UI;
using TMPro;
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
    private TextMeshProUGUI fusionPointsText;
    
    void Start()
    {
        CreateFusionUI();
    }
    
    void CreateFusionUI()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("FusionCanvas");
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
        bgImage.color = new Color(0.1f, 0.15f, 0.2f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "卡牌融合";
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.6f, 1f);
        
        // 融合点显示
        var fpObj = new GameObject("FusionPoints");
        fpObj.transform.SetParent(canvas.transform, false);
        var fpRect = fpObj.AddComponent<RectTransform>();
        fpRect.anchorMin = new Vector2(0.85f, 0.9f);
        fpRect.anchorMax = new Vector2(1, 0.95f);
        fpRect.offsetMin = Vector2.zero;
        fpRect.offsetMax = Vector2.zero;
        
        fusionPointsText = fpObj.AddComponent<TextMeshProUGUI>();
        int fp = FusionManager.Instance != null ? FusionManager.Instance.GetFusionPoints() : 0;
        fusionPointsText.text = $"融合点: {fp}/10";
        fusionPointsText.fontSize = 28;
        fusionPointsText.alignment = TextAlignmentOptions.Center;
        fusionPointsText.color = new Color(1f, 0.6f, 1f);
        
        // 融合区（顶部中间）
        CreateFusionArea(canvas.transform);
        
        // 牌库区（底部）
        CreateDeckArea(canvas.transform);
        
        // 按钮区
        CreateButtons(canvas.transform);
        
        Debug.Log("[FusionUI] 融合UI已创建");
    }
    
    void CreateFusionArea(Transform parent)
    {
        // 卡槽1
        card1Slot = CreateCardSlot(parent, "卡槽 1", new Vector2(-200, 100));
        
        // "+" 符号
        var plusObj = new GameObject("Plus");
        plusObj.transform.SetParent(parent, false);
        var plusRect = plusObj.AddComponent<RectTransform>();
        plusRect.anchorMin = new Vector2(0.5f, 0.6f);
        plusRect.anchorMax = new Vector2(0.5f, 0.6f);
        plusRect.pivot = new Vector2(0.5f, 0.5f);
        plusRect.anchoredPosition = Vector2.zero;
        plusRect.sizeDelta = new Vector2(100, 100);
        
        var plusText = plusObj.AddComponent<TextMeshProUGUI>();
        plusText.text = "+";
        plusText.fontSize = 72;
        plusText.fontStyle = FontStyles.Bold;
        plusText.alignment = TextAlignmentOptions.Center;
        plusText.color = Color.white;
        
        // 卡槽2
        card2Slot = CreateCardSlot(parent, "卡槽 2", new Vector2(200, 100));
        
        // "=" 符号
        var equalsObj = new GameObject("Equals");
        equalsObj.transform.SetParent(parent, false);
        var equalsRect = equalsObj.AddComponent<RectTransform>();
        equalsRect.anchorMin = new Vector2(0.5f, 0.6f);
        equalsRect.anchorMax = new Vector2(0.5f, 0.6f);
        equalsRect.pivot = new Vector2(0.5f, 0.5f);
        equalsRect.anchoredPosition = new Vector2(0, -150);
        equalsRect.sizeDelta = new Vector2(100, 100);
        
        var equalsText = equalsObj.AddComponent<TextMeshProUGUI>();
        equalsText.text = "=";
        equalsText.fontSize = 72;
        equalsText.fontStyle = FontStyles.Bold;
        equalsText.alignment = TextAlignmentOptions.Center;
        equalsText.color = Color.white;
        
        // 结果槽
        resultSlot = CreateCardSlot(parent, "结果", new Vector2(0, -300));
    }
    
    GameObject CreateCardSlot(Transform parent, string label, Vector2 offset)
    {
        var slotObj = new GameObject($"CardSlot_{label}");
        slotObj.transform.SetParent(parent, false);
        
        var slotRect = slotObj.AddComponent<RectTransform>();
        slotRect.anchorMin = new Vector2(0.5f, 0.6f);
        slotRect.anchorMax = new Vector2(0.5f, 0.6f);
        slotRect.pivot = new Vector2(0.5f, 0.5f);
        slotRect.anchoredPosition = offset;
        slotRect.sizeDelta = new Vector2(180, 260);
        
        var slotImage = slotObj.AddComponent<Image>();
        slotImage.color = new Color(0.2f, 0.2f, 0.3f, 0.5f);
        
        // 标签
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(slotObj.transform, false);
        var labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 0.9f);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.offsetMin = Vector2.zero;
        labelRect.offsetMax = Vector2.zero;
        
        var labelText = labelObj.AddComponent<TextMeshProUGUI>();
        labelText.text = label;
        labelText.fontSize = 20;
        labelText.alignment = TextAlignmentOptions.Center;
        labelText.color = Color.gray;
        
        return slotObj;
    }
    
    void CreateDeckArea(Transform parent)
    {
        // 标题
        var titleObj = new GameObject("DeckTitle");
        titleObj.transform.SetParent(parent, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.25f);
        titleRect.anchorMax = new Vector2(1, 0.3f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "你的牌库（点击选择）";
        titleText.fontSize = 32;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = Color.white;
        
        // 卡牌列表（滚动视图简化版）
        var deckPanel = new GameObject("DeckPanel");
        deckPanel.transform.SetParent(parent, false);
        var deckRect = deckPanel.AddComponent<RectTransform>();
        deckRect.anchorMin = new Vector2(0.1f, 0);
        deckRect.anchorMax = new Vector2(0.9f, 0.25f);
        deckRect.offsetMin = Vector2.zero;
        deckRect.offsetMax = Vector2.zero;
        
        var layout = deckPanel.AddComponent<HorizontalLayoutGroup>();
        layout.spacing = 10;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = false;
        layout.childControlHeight = false;
        
        // 显示牌库中的卡牌
        if (CardManager.Instance != null)
        {
            var deck = CardManager.Instance.playerDeck;
            int displayCount = Mathf.Min(10, deck.Count); // 最多显示10张
            
            for (int i = 0; i < displayCount; i++)
            {
                var card = CardManager.Instance.GetCardById(deck[i]);
                if (card != null)
                {
                    CreateDeckCard(deckPanel.transform, card, i);
                }
            }
        }
    }
    
    void CreateDeckCard(Transform parent, CardData card, int index)
    {
        var cardObj = new GameObject($"DeckCard_{index}");
        cardObj.transform.SetParent(parent, false);
        
        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.sizeDelta = new Vector2(120, 180);
        
        var cardImage = cardObj.AddComponent<Image>();
        cardImage.color = GetCardColor(card.rarity);
        
        var button = cardObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnSelectCard(card));
        
        // 卡牌名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.7f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = card.cardName;
        nameText.fontSize = 14;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.black;
        
        // 费用
        var costObj = new GameObject("Cost");
        costObj.transform.SetParent(cardObj.transform, false);
        var costRect = costObj.AddComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(0.3f, 0.2f);
        costRect.offsetMin = Vector2.zero;
        costRect.offsetMax = Vector2.zero;
        
        var costText = costObj.AddComponent<TextMeshProUGUI>();
        costText.text = card.cost.ToString();
        costText.fontSize = 18;
        costText.alignment = TextAlignmentOptions.Center;
        costText.color = new Color(0, 0.3f, 0.8f);
    }
    
    void CreateButtons(Transform parent)
    {
        // 融合按钮
        CreateButton(parent, "融合", new Vector2(0, -450), OnFuse, new Color(0.6f, 0.2f, 0.8f));
        
        // 清空按钮
        CreateButton(parent, "清空", new Vector2(-150, -550), OnClear, new Color(0.5f, 0.5f, 0.5f));
        
        // 离开按钮
        CreateButton(parent, "离开", new Vector2(150, -550), OnLeave, new Color(0.5f, 0.5f, 0.5f));
    }
    
    void CreateButton(Transform parent, string text, Vector2 offset, UnityEngine.Events.UnityAction action, Color color)
    {
        var btnObj = new GameObject($"Button_{text}");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.5f);
        btnRect.anchorMax = new Vector2(0.5f, 0.5f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = offset;
        btnRect.sizeDelta = new Vector2(250, 70);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = color;
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(action);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 32;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
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
    
    void OnSelectCard(CardData card)
    {
        Debug.Log($"[FusionUI] 选择卡牌: {card.cardName}");
        
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
        // 清除旧内容
        for (int i = slot.transform.childCount - 1; i >= 0; i--)
        {
            var child = slot.transform.GetChild(i);
            if (child.name != "Label")
            {
                Destroy(child.gameObject);
            }
        }
        
        // 显示卡牌信息
        var cardInfoObj = new GameObject("CardInfo");
        cardInfoObj.transform.SetParent(slot.transform, false);
        var cardInfoRect = cardInfoObj.AddComponent<RectTransform>();
        cardInfoRect.anchorMin = new Vector2(0.1f, 0.2f);
        cardInfoRect.anchorMax = new Vector2(0.9f, 0.8f);
        cardInfoRect.offsetMin = Vector2.zero;
        cardInfoRect.offsetMax = Vector2.zero;
        
        var cardInfoText = cardInfoObj.AddComponent<TextMeshProUGUI>();
        cardInfoText.text = $"{card.cardName}\n费用: {card.cost}\n效果: {card.value}";
        cardInfoText.fontSize = 16;
        cardInfoText.alignment = TextAlignmentOptions.Center;
        cardInfoText.color = Color.white;
        
        slot.GetComponent<Image>().color = GetCardColor(card.rarity);
    }
    
    void OnFuse()
    {
        if (selectedCard1 == null || selectedCard2 == null)
        {
            Debug.Log("[FusionUI] 请选择两张卡牌");
            return;
        }
        
        if (FusionManager.Instance == null)
        {
            Debug.Log("[FusionUI] FusionManager不存在");
            return;
        }
        
        // 检查融合点
        if (FusionManager.Instance.GetFusionPoints() < FusionManager.Instance.fusionPointCost)
        {
            Debug.Log("[FusionUI] 融合点不足！");
            return;
        }
        
        // 执行融合
        var result = FusionManager.Instance.FuseCards(selectedCard1.cardId, selectedCard2.cardId);
        
        if (result != null)
        {
            Debug.Log($"[FusionUI] 融合成功: {result.cardName}");
            
            // 显示结果
            ShowCardInSlot(resultSlot, result);
            
            // 添加到牌库
            if (CardManager.Instance != null)
            {
                CardManager.Instance.AddCardToDeck(result.cardId);
            }
            
            // 移除原卡牌
            if (CardManager.Instance != null)
            {
                CardManager.Instance.RemoveCardFromDeck(selectedCard1.cardId);
                CardManager.Instance.RemoveCardFromDeck(selectedCard2.cardId);
            }
            
            // 更新融合点显示
            UpdateFusionPoints();
            
            // 清空选择
            OnClear();
        }
        else
        {
            Debug.Log("[FusionUI] 融合失败！");
        }
    }
    
    void OnClear()
    {
        selectedCard1 = null;
        selectedCard2 = null;
        
        // 清空卡槽
        ClearSlot(card1Slot);
        ClearSlot(card2Slot);
        ClearSlot(resultSlot);
        
        Debug.Log("[FusionUI] 已清空选择");
    }
    
    void ClearSlot(GameObject slot)
    {
        for (int i = slot.transform.childCount - 1; i >= 0; i--)
        {
            var child = slot.transform.GetChild(i);
            if (child.name != "Label")
            {
                Destroy(child.gameObject);
            }
        }
        
        slot.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.3f, 0.5f);
    }
    
    void UpdateFusionPoints()
    {
        if (fusionPointsText != null && FusionManager.Instance != null)
        {
            int fp = FusionManager.Instance.GetFusionPoints();
            fusionPointsText.text = $"融合点: {fp}/10";
        }
    }
    
    void OnLeave()
    {
        Debug.Log("[FusionUI] 离开融合界面");
        
        // 销毁融合UI
        Destroy(gameObject);
        
        // 返回地图
        var mapObj = new GameObject("MapUI");
        mapObj.AddComponent<MapUI>();
    }
}
