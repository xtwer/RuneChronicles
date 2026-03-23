using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 商店UI - 购买卡牌、遗物、移除卡牌
/// </summary>
public class ShopUI : MonoBehaviour
{
    private List<CardData> shopCards = new List<CardData>();
    private List<string> shopRelics = new List<string>();
    private const int CARD_PRICE = 50;
    private const int RELIC_PRICE = 150;
    private const int REMOVE_PRICE = 75;
    
    void Start()
    {
        GenerateShopItems();
        CreateShopUI();
    }
    
    void GenerateShopItems()
    {
        if (CardManager.Instance == null) return;
        
        // 随机5张卡牌
        var allCards = CardManager.Instance.GetAllCards();
        for (int i = 0; i < 5 && i < allCards.Count; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            shopCards.Add(allCards[randomIndex]);
        }
        
        // 随机2个遗物
        if (RelicManager.Instance != null)
        {
            var allRelics = RelicManager.Instance.GetAllRelics();
            for (int i = 0; i < 2 && i < allRelics.Count; i++)
            {
                int randomIndex = Random.Range(0, allRelics.Count);
                shopRelics.Add(allRelics[randomIndex].relicId);
            }
        }
        
        Debug.Log($"[ShopUI] 商店生成：{shopCards.Count}张卡牌，{shopRelics.Count}个遗物");
    }
    
    void CreateShopUI()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("ShopCanvas");
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
        bgImage.color = new Color(0.15f, 0.2f, 0.25f);
        
        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.9f);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<TextMeshProUGUI>();
        titleText.text = "商店";
        titleText.fontSize = 56;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.color = new Color(1f, 0.8f, 0.3f);
        
        // 金币显示
        var goldObj = new GameObject("Gold");
        goldObj.transform.SetParent(canvas.transform, false);
        var goldRect = goldObj.AddComponent<RectTransform>();
        goldRect.anchorMin = new Vector2(0.85f, 0.9f);
        goldRect.anchorMax = new Vector2(1, 0.95f);
        goldRect.offsetMin = Vector2.zero;
        goldRect.offsetMax = Vector2.zero;
        
        var goldText = goldObj.AddComponent<TextMeshProUGUI>();
        int currentGold = GameManager.Instance != null ? GameManager.Instance.currentGold : 0;
        goldText.text = $"💰 {currentGold}";
        goldText.fontSize = 32;
        goldText.alignment = TextAlignmentOptions.Center;
        goldText.color = Color.yellow;
        
        // 卡牌区
        CreateSection(canvas.transform, "卡牌 (50金币)", new Vector2(0, 0.65f), new Vector2(1, 0.85f));
        for (int i = 0; i < shopCards.Count; i++)
        {
            CreateShopCard(canvas.transform, shopCards[i], i, shopCards.Count);
        }
        
        // 遗物区
        CreateSection(canvas.transform, "遗物 (150金币)", new Vector2(0, 0.35f), new Vector2(1, 0.55f));
        for (int i = 0; i < shopRelics.Count; i++)
        {
            CreateShopRelic(canvas.transform, shopRelics[i], i, shopRelics.Count);
        }
        
        // 移除卡牌区
        CreateSection(canvas.transform, "移除卡牌 (75金币)", new Vector2(0, 0.15f), new Vector2(1, 0.25f));
        CreateRemoveCardButton(canvas.transform);
        
        // 离开按钮
        CreateLeaveButton(canvas.transform);
        
        Debug.Log("[ShopUI] 商店UI已创建");
    }
    
    void CreateSection(Transform parent, string title, Vector2 anchorMin, Vector2 anchorMax)
    {
        var sectionObj = new GameObject($"Section_{title}");
        sectionObj.transform.SetParent(parent, false);
        var sectionRect = sectionObj.AddComponent<RectTransform>();
        sectionRect.anchorMin = anchorMin;
        sectionRect.anchorMax = anchorMax;
        sectionRect.offsetMin = Vector2.zero;
        sectionRect.offsetMax = Vector2.zero;
        
        var sectionText = sectionObj.AddComponent<TextMeshProUGUI>();
        sectionText.text = title;
        sectionText.fontSize = 36;
        sectionText.fontStyle = FontStyles.Bold;
        sectionText.alignment = TextAlignmentOptions.TopCenter;
        sectionText.color = Color.white;
    }
    
    void CreateShopCard(Transform parent, CardData card, int index, int total)
    {
        var cardObj = new GameObject($"ShopCard_{index}");
        cardObj.transform.SetParent(parent, false);
        
        // 水平排列
        float spacing = 200f;
        float startX = -(total - 1) * spacing / 2f;
        float x = startX + index * spacing;
        
        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.7f);
        cardRect.anchorMax = new Vector2(0.5f, 0.7f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(x, 0);
        cardRect.sizeDelta = new Vector2(150, 220);
        
        var cardImage = cardObj.AddComponent<Image>();
        cardImage.color = GetCardColor(card.rarity);
        
        var button = cardObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnBuyCard(card));
        
        // 卡牌名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.8f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = card.cardName;
        nameText.fontSize = 16;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.black;
        
        // 价格
        var priceObj = new GameObject("Price");
        priceObj.transform.SetParent(cardObj.transform, false);
        var priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.2f);
        priceRect.offsetMin = Vector2.zero;
        priceRect.offsetMax = Vector2.zero;
        
        var priceText = priceObj.AddComponent<TextMeshProUGUI>();
        priceText.text = $"{CARD_PRICE}💰";
        priceText.fontSize = 20;
        priceText.fontStyle = FontStyles.Bold;
        priceText.alignment = TextAlignmentOptions.Center;
        priceText.color = Color.yellow;
    }
    
    void CreateShopRelic(Transform parent, string relicId, int index, int total)
    {
        var relicData = RelicManager.Instance?.GetRelicById(relicId);
        if (relicData == null) return;
        
        var relicObj = new GameObject($"ShopRelic_{index}");
        relicObj.transform.SetParent(parent, false);
        
        // 水平排列
        float spacing = 300f;
        float startX = -(total - 1) * spacing / 2f;
        float x = startX + index * spacing;
        
        var relicRect = relicObj.AddComponent<RectTransform>();
        relicRect.anchorMin = new Vector2(0.5f, 0.4f);
        relicRect.anchorMax = new Vector2(0.5f, 0.4f);
        relicRect.pivot = new Vector2(0.5f, 0.5f);
        relicRect.anchoredPosition = new Vector2(x, 0);
        relicRect.sizeDelta = new Vector2(250, 150);
        
        var relicImage = relicObj.AddComponent<Image>();
        relicImage.color = new Color(0.3f, 0.3f, 0.4f);
        
        var button = relicObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnBuyRelic(relicId));
        
        // 遗物名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(relicObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.6f);
        nameRect.anchorMax = new Vector2(1, 1);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        
        var nameText = nameObj.AddComponent<TextMeshProUGUI>();
        nameText.text = relicData.relicName;
        nameText.fontSize = 20;
        nameText.fontStyle = FontStyles.Bold;
        nameText.alignment = TextAlignmentOptions.Center;
        nameText.color = Color.white;
        
        // 描述
        var descObj = new GameObject("Description");
        descObj.transform.SetParent(relicObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.1f, 0.3f);
        descRect.anchorMax = new Vector2(0.9f, 0.55f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        
        var descText = descObj.AddComponent<TextMeshProUGUI>();
        descText.text = relicData.description;
        descText.fontSize = 14;
        descText.alignment = TextAlignmentOptions.Center;
        descText.color = Color.white;
        
        // 价格
        var priceObj = new GameObject("Price");
        priceObj.transform.SetParent(relicObj.transform, false);
        var priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.25f);
        priceRect.offsetMin = Vector2.zero;
        priceRect.offsetMax = Vector2.zero;
        
        var priceText = priceObj.AddComponent<TextMeshProUGUI>();
        priceText.text = $"{RELIC_PRICE}💰";
        priceText.fontSize = 24;
        priceText.fontStyle = FontStyles.Bold;
        priceText.alignment = TextAlignmentOptions.Center;
        priceText.color = Color.yellow;
    }
    
    void CreateRemoveCardButton(Transform parent)
    {
        var btnObj = new GameObject("RemoveCardButton");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.18f);
        btnRect.anchorMax = new Vector2(0.5f, 0.18f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = Vector2.zero;
        btnRect.sizeDelta = new Vector2(300, 60);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.6f, 0.3f, 0.3f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(OnRemoveCard);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = $"移除卡牌 ({REMOVE_PRICE}💰)";
        text.fontSize = 24;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
    }
    
    void CreateLeaveButton(Transform parent)
    {
        var btnObj = new GameObject("LeaveButton");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.05f);
        btnRect.anchorMax = new Vector2(0.5f, 0.05f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = Vector2.zero;
        btnRect.sizeDelta = new Vector2(250, 60);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.5f, 0.5f, 0.5f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(OnLeave);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "离开商店";
        text.fontSize = 28;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
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
    
    void OnBuyCard(CardData card)
    {
        if (GameManager.Instance == null || CardManager.Instance == null) return;
        
        if (GameManager.Instance.currentGold >= CARD_PRICE)
        {
            GameManager.Instance.RemoveGold(CARD_PRICE);
            CardManager.Instance.AddCardToDeck(card.cardId);
            
            Debug.Log($"[ShopUI] 购买卡牌: {card.cardName}");
            
            // 刷新商店（移除已购买的卡牌）
            shopCards.Remove(card);
            RefreshShop();
        }
        else
        {
            Debug.Log("[ShopUI] 金币不足！");
        }
    }
    
    void OnBuyRelic(string relicId)
    {
        if (GameManager.Instance == null || RelicManager.Instance == null) return;
        
        if (GameManager.Instance.currentGold >= RELIC_PRICE)
        {
            GameManager.Instance.RemoveGold(RELIC_PRICE);
            RelicManager.Instance.AddRelic(relicId);
            
            Debug.Log($"[ShopUI] 购买遗物: {relicId}");
            
            // 刷新商店（移除已购买的遗物）
            shopRelics.Remove(relicId);
            RefreshShop();
        }
        else
        {
            Debug.Log("[ShopUI] 金币不足！");
        }
    }
    
    void OnRemoveCard()
    {
        Debug.Log("[ShopUI] 移除卡牌（暂未实装选择界面）");
        
        if (GameManager.Instance == null || CardManager.Instance == null) return;
        
        if (GameManager.Instance.currentGold >= REMOVE_PRICE)
        {
            // 简单实现：移除牌库中第一张卡
            var deck = CardManager.Instance.playerDeck;
            if (deck != null && deck.Count > 0)
            {
                string removedCard = deck[0];
                CardManager.Instance.RemoveCardFromDeck(removedCard);
                GameManager.Instance.RemoveGold(REMOVE_PRICE);
                
                Debug.Log($"[ShopUI] 已移除卡牌: {removedCard}");
            }
        }
        else
        {
            Debug.Log("[ShopUI] 金币不足！");
        }
    }
    
    void OnLeave()
    {
        Debug.Log("[ShopUI] 离开商店");
        
        // 销毁商店UI
        Destroy(gameObject);
        
        // 前往下一层
        if (MapManager.Instance != null)
        {
            MapManager.Instance.MoveToNextFloor(0);
        }
        
        // 显示地图
        var mapObj = new GameObject("MapUI");
        mapObj.AddComponent<MapUI>();
    }
    
    void RefreshShop()
    {
        // 简单实现：销毁重建
        Destroy(gameObject);
        var newShop = new GameObject("ShopUI");
        var shopUI = newShop.AddComponent<ShopUI>();
        shopUI.shopCards = this.shopCards;
        shopUI.shopRelics = this.shopRelics;
    }
}
