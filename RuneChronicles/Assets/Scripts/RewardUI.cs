using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 奖励UI - 战斗后选择卡牌
/// </summary>
public class RewardUI : MonoBehaviour
{
    private List<CardData> rewardCards = new List<CardData>();
    
    void Start()
    {
        GenerateRewards();
        CreateRewardUI();
    }
    
    void GenerateRewards()
    {
        if (CardManager.Instance == null) return;
        
        // 随机3张卡作为奖励
        var allCards = CardManager.Instance.GetAllCards();
        
        for (int i = 0; i < 3 && i < allCards.Count; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            rewardCards.Add(allCards[randomIndex]);
        }
        
        Debug.Log($"[RewardUI] 生成 {rewardCards.Count} 张奖励卡");
    }
    
    void CreateRewardUI()
    {
        // 创建Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("RewardCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;
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
        titleRect.anchorMin = new Vector2(0, 0.8f);
        titleRect.anchorMax = new Vector2(1, 0.95f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        
        var titleText = titleObj.AddComponent<Text>();
        titleText.text = "选择一张卡牌加入牌库";
        titleText.fontSize = 48;
        titleText.fontStyle = FontStyle.Bold;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.color = Color.white;
        
        // 显示奖励卡牌
        for (int i = 0; i < rewardCards.Count; i++)
        {
            CreateRewardCard(canvas.transform, rewardCards[i], i);
        }
        
        // 跳过按钮
        CreateSkipButton(canvas.transform);
        
        Debug.Log("[RewardUI] 奖励UI已创建");
    }
    
    void CreateRewardCard(Transform parent, CardData card, int index)
    {
        var cardObj = new GameObject($"RewardCard_{index}");
        cardObj.transform.SetParent(parent, false);
        
        // 水平排列
        float spacing = 300f;
        float startX = -(rewardCards.Count - 1) * spacing / 2f;
        float x = startX + index * spacing;
        
        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.5f);
        cardRect.anchorMax = new Vector2(0.5f, 0.5f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(x, 0);
        cardRect.sizeDelta = new Vector2(200, 300);
        
        var cardImage = cardObj.AddComponent<Image>();
        cardImage.color = GetCardColor(card.rarity);
        
        var button = cardObj.AddComponent<Button>();
        button.onClick.AddListener(() => OnCardSelected(card));
        
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
        nameText.fontSize = 20;
        nameText.fontStyle = FontStyle.Bold;
        nameText.alignment = TextAnchor.MiddleCenter;
        nameText.color = Color.black;
        
        // 描述
        var descObj = new GameObject("Description");
        descObj.transform.SetParent(cardObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.1f, 0.2f);
        descRect.anchorMax = new Vector2(0.9f, 0.7f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        
        var descText = descObj.AddComponent<Text>();
        descText.text = $"费用: {card.cost}\n效果: {card.value}\n\n{card.description}";
        descText.fontSize = 16;
        descText.alignment = TextAnchor.UpperLeft;
        descText.color = Color.black;
    }
    
    void CreateSkipButton(Transform parent)
    {
        var btnObj = new GameObject("SkipButton");
        btnObj.transform.SetParent(parent, false);
        
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.5f, 0.1f);
        btnRect.anchorMax = new Vector2(0.5f, 0.1f);
        btnRect.pivot = new Vector2(0.5f, 0.5f);
        btnRect.anchoredPosition = Vector2.zero;
        btnRect.sizeDelta = new Vector2(300, 70);
        
        var btnImage = btnObj.AddComponent<Image>();
        btnImage.color = new Color(0.5f, 0.5f, 0.5f);
        
        var button = btnObj.AddComponent<Button>();
        button.onClick.AddListener(OnSkip);
        
        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        var text = textObj.AddComponent<Text>();
        text.text = "跳过";
        text.fontSize = 32;
        text.alignment = TextAnchor.MiddleCenter;
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
    
    void OnCardSelected(CardData card)
    {
        Debug.Log($"[RewardUI] 选择卡牌: {card.cardName}");
        
        // 添加到牌库
        if (CardManager.Instance != null)
        {
            CardManager.Instance.AddCardToDeck(card.cardId);
        }
        
        ContinueToNextFloor();
    }
    
    void OnSkip()
    {
        Debug.Log("[RewardUI] 跳过奖励");
        
        ContinueToNextFloor();
    }
    
    void ContinueToNextFloor()
    {
        // 销毁奖励UI
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
}
