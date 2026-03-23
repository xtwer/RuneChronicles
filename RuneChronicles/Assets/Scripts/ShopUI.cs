using UnityEngine;
using UnityEngine.UI;
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

    private bool initialized = false;

    public void Init()
    {
        if (initialized) return;
        initialized = true;
        GenerateShopItems();
        CreateShopUI();
    }

    void Start() { Init(); }

    void GenerateShopItems()
    {
        if (CardManager.Instance == null) return;

        var allCards = new List<CardData>(CardManager.Instance.GetAllCards());
        for (int i = 0; i < 5 && allCards.Count > 0; i++)
        {
            int randomIndex = Random.Range(0, allCards.Count);
            shopCards.Add(allCards[randomIndex]);
            allCards.RemoveAt(randomIndex);
        }

        if (RelicManager.Instance != null)
        {
            var allRelics = new List<RelicData>(RelicManager.Instance.GetAllRelics());
            for (int i = 0; i < 2 && allRelics.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, allRelics.Count);
                shopRelics.Add(allRelics[randomIndex].relicId);
                allRelics.RemoveAt(randomIndex);
            }
        }

        Debug.Log($"[ShopUI] 商店生成：{shopCards.Count}张卡牌，{shopRelics.Count}个遗物");
    }

    void CreateShopUI()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            var canvasObj = new GameObject("ShopCanvas");
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
        bgObj.AddComponent<Image>().color = new Color(0.15f, 0.2f, 0.25f);

        // 标题
        var titleObj = new GameObject("Title");
        titleObj.transform.SetParent(canvas.transform, false);
        var titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 0.92f);
        titleRect.anchorMax = new Vector2(0.7f, 1f);
        titleRect.offsetMin = Vector2.zero;
        titleRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(titleObj, "神秘商店", 52, TextAnchor.MiddleCenter, new Color(1f, 0.8f, 0.3f))
            .fontStyle = FontStyle.Bold;

        // 金币显示
        var goldObj = new GameObject("Gold");
        goldObj.transform.SetParent(canvas.transform, false);
        var goldRect = goldObj.AddComponent<RectTransform>();
        goldRect.anchorMin = new Vector2(0.7f, 0.92f);
        goldRect.anchorMax = new Vector2(1f, 1f);
        goldRect.offsetMin = Vector2.zero;
        goldRect.offsetMax = Vector2.zero;
        int currentGold = GameManager.Instance != null ? GameManager.Instance.currentGold : 0;
        ChineseUI.CreateText(goldObj, $"金币: {currentGold}", 32, TextAnchor.MiddleCenter, Color.yellow);

        // 卡牌区标题
        CreateSectionLabel(canvas.transform, $"卡牌 (每张{CARD_PRICE}金币)",
            new Vector2(0, 0.82f), new Vector2(1f, 0.9f));

        for (int i = 0; i < shopCards.Count; i++)
            CreateShopCard(canvas.transform, shopCards[i], i, shopCards.Count);

        // 遗物区标题
        CreateSectionLabel(canvas.transform, $"遗物 (每个{RELIC_PRICE}金币)",
            new Vector2(0, 0.52f), new Vector2(1f, 0.6f));

        for (int i = 0; i < shopRelics.Count; i++)
            CreateShopRelic(canvas.transform, shopRelics[i], i, shopRelics.Count);

        // 移除卡牌区
        CreateSectionLabel(canvas.transform, $"移除卡牌 ({REMOVE_PRICE}金币)",
            new Vector2(0, 0.3f), new Vector2(1f, 0.38f));
        CreateRemoveCardButton(canvas.transform);

        // 离开按钮
        CreateLeaveButton(canvas.transform);

        Debug.Log("[ShopUI] 商店UI已创建");
    }

    void CreateSectionLabel(Transform parent, string title, Vector2 anchorMin, Vector2 anchorMax)
    {
        var obj = new GameObject($"Section_{title}");
        obj.transform.SetParent(parent, false);
        var rect = obj.AddComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(obj, title, 30, TextAnchor.MiddleCenter, new Color(0.9f, 0.9f, 0.6f))
            .fontStyle = FontStyle.Bold;
    }

    void CreateShopCard(Transform parent, CardData card, int index, int total)
    {
        var cardObj = new GameObject($"ShopCard_{index}");
        cardObj.transform.SetParent(parent, false);

        float spacing = 200f;
        float startX = -(total - 1) * spacing / 2f;
        float x = startX + index * spacing;

        var cardRect = cardObj.AddComponent<RectTransform>();
        cardRect.anchorMin = new Vector2(0.5f, 0.62f);
        cardRect.anchorMax = new Vector2(0.5f, 0.62f);
        cardRect.pivot = new Vector2(0.5f, 0.5f);
        cardRect.anchoredPosition = new Vector2(x, 0);
        cardRect.sizeDelta = new Vector2(160, 230);

        cardObj.AddComponent<Image>().color = GetCardColor(card.rarity);
        cardObj.AddComponent<Button>().onClick.AddListener(() => OnBuyCard(card));

        // 卡名
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(cardObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.78f);
        nameRect.anchorMax = new Vector2(1, 1f);
        nameRect.offsetMin = new Vector2(2, 0);
        nameRect.offsetMax = new Vector2(-2, 0);
        ChineseUI.CreateText(nameObj, card.cardName, 15, TextAnchor.MiddleCenter, Color.black)
            .fontStyle = FontStyle.Bold;

        // 卡牌信息
        var infoObj = new GameObject("Info");
        infoObj.transform.SetParent(cardObj.transform, false);
        var infoRect = infoObj.AddComponent<RectTransform>();
        infoRect.anchorMin = new Vector2(0.05f, 0.25f);
        infoRect.anchorMax = new Vector2(0.95f, 0.75f);
        infoRect.offsetMin = Vector2.zero;
        infoRect.offsetMax = Vector2.zero;
        string typeLabel = card.cardType == CardType.Attack ? "攻击" :
                           card.cardType == CardType.Skill ? "技能" : "能力";
        ChineseUI.CreateText(infoObj, $"{typeLabel}\n费用:{card.cost}\n效果:{card.value}",
            12, TextAnchor.MiddleCenter, new Color(0.15f, 0.15f, 0.15f));

        // 价格
        var priceObj = new GameObject("Price");
        priceObj.transform.SetParent(cardObj.transform, false);
        var priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.22f);
        priceRect.offsetMin = Vector2.zero;
        priceRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(priceObj, $"{CARD_PRICE}金", 18, TextAnchor.MiddleCenter, Color.yellow)
            .fontStyle = FontStyle.Bold;
    }

    void CreateShopRelic(Transform parent, string relicId, int index, int total)
    {
        var relicData = RelicManager.Instance?.GetRelicData(relicId);
        if (relicData == null) return;

        var relicObj = new GameObject($"ShopRelic_{index}");
        relicObj.transform.SetParent(parent, false);

        float spacing = 320f;
        float startX = -(total - 1) * spacing / 2f;
        float x = startX + index * spacing;

        var relicRect = relicObj.AddComponent<RectTransform>();
        relicRect.anchorMin = new Vector2(0.5f, 0.38f);
        relicRect.anchorMax = new Vector2(0.5f, 0.38f);
        relicRect.pivot = new Vector2(0.5f, 0.5f);
        relicRect.anchoredPosition = new Vector2(x, 0);
        relicRect.sizeDelta = new Vector2(280, 140);

        relicObj.AddComponent<Image>().color = new Color(0.3f, 0.3f, 0.4f);
        relicObj.AddComponent<Button>().onClick.AddListener(() => OnBuyRelic(relicId));

        // 遗物名称
        var nameObj = new GameObject("Name");
        nameObj.transform.SetParent(relicObj.transform, false);
        var nameRect = nameObj.AddComponent<RectTransform>();
        nameRect.anchorMin = new Vector2(0, 0.65f);
        nameRect.anchorMax = new Vector2(1, 1f);
        nameRect.offsetMin = Vector2.zero;
        nameRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(nameObj, relicData.relicName, 20, TextAnchor.MiddleCenter, new Color(1f, 0.9f, 0.5f))
            .fontStyle = FontStyle.Bold;

        // 描述
        var descObj = new GameObject("Description");
        descObj.transform.SetParent(relicObj.transform, false);
        var descRect = descObj.AddComponent<RectTransform>();
        descRect.anchorMin = new Vector2(0.05f, 0.28f);
        descRect.anchorMax = new Vector2(0.95f, 0.62f);
        descRect.offsetMin = Vector2.zero;
        descRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(descObj, relicData.description, 13, TextAnchor.MiddleCenter, Color.white);

        // 价格
        var priceObj = new GameObject("Price");
        priceObj.transform.SetParent(relicObj.transform, false);
        var priceRect = priceObj.AddComponent<RectTransform>();
        priceRect.anchorMin = new Vector2(0, 0);
        priceRect.anchorMax = new Vector2(1, 0.25f);
        priceRect.offsetMin = Vector2.zero;
        priceRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(priceObj, $"{RELIC_PRICE}金", 22, TextAnchor.MiddleCenter, Color.yellow)
            .fontStyle = FontStyle.Bold;
    }

    void CreateRemoveCardButton(Transform parent)
    {
        var btnObj = new GameObject("RemoveCardButton");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.22f);
        btnRect.anchorMax = new Vector2(0.65f, 0.29f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        btnObj.AddComponent<Image>().color = new Color(0.6f, 0.3f, 0.3f);
        btnObj.AddComponent<Button>().onClick.AddListener(OnRemoveCard);

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(textObj, $"移除一张牌 ({REMOVE_PRICE}金)", 22, TextAnchor.MiddleCenter, Color.white);
    }

    void CreateLeaveButton(Transform parent)
    {
        var btnObj = new GameObject("LeaveButton");
        btnObj.transform.SetParent(parent, false);
        var btnRect = btnObj.AddComponent<RectTransform>();
        btnRect.anchorMin = new Vector2(0.35f, 0.02f);
        btnRect.anchorMax = new Vector2(0.65f, 0.1f);
        btnRect.offsetMin = Vector2.zero;
        btnRect.offsetMax = Vector2.zero;
        btnObj.AddComponent<Image>().color = new Color(0.3f, 0.5f, 0.3f);
        btnObj.AddComponent<Button>().onClick.AddListener(OnLeave);

        var textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        var textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        ChineseUI.CreateText(textObj, "离开商店", 28, TextAnchor.MiddleCenter, Color.white);
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
            GameManager.Instance.SpendGold(CARD_PRICE);
            CardManager.Instance.AddCardToDeck(card.cardId);
            Debug.Log($"[ShopUI] 购买卡牌: {card.cardName}");
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
            GameManager.Instance.SpendGold(RELIC_PRICE);
            RelicManager.Instance.ObtainRelic(relicId);
            Debug.Log($"[ShopUI] 购买遗物: {relicId}");
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
        if (GameManager.Instance == null || CardManager.Instance == null) return;

        if (GameManager.Instance.currentGold >= REMOVE_PRICE)
        {
            var deck = CardManager.Instance.playerDeck;
            if (deck != null && deck.Count > 0)
            {
                CardData removedCard = deck[0];
                CardManager.Instance.RemoveCardFromDeck(removedCard);
                GameManager.Instance.SpendGold(REMOVE_PRICE);
                Debug.Log($"[ShopUI] 已移除卡牌: {removedCard.cardName}");
                RefreshShop();
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
        Destroy(gameObject);

        if (MapManager.Instance != null)
            MapManager.Instance.MoveToNextFloor(0);

        var mapObj = new GameObject("MapUI");
        mapObj.AddComponent<MapUI>().Init();
    }

    void RefreshShop()
    {
        shopCards.Clear();
        shopRelics.Clear();
        GenerateShopItems();
        CreateShopUI();
    }
}
