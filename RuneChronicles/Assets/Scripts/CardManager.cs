using UnityEngine;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// 卡牌管理器 - Week 2版本
/// 支持从JSON加载卡牌数据
/// </summary>
public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    
    [Header("卡牌数据")]
    public TextAsset cardDataJson; // 在Inspector中拖入JSON文件
    private Dictionary<string, CardData> cardDatabase = new Dictionary<string, CardData>();
    
    [Header("当前牌库")]
    public List<CardData> playerDeck = new List<CardData>();
    
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
        LoadCardsFromJson();
    }
    
    #region JSON加载
    
    /// <summary>
    /// 从JSON加载所有卡牌
    /// </summary>
    public void LoadCardsFromJson()
    {
        if (cardDataJson == null)
        {
            // 尝试从Resources加载
            cardDataJson = Resources.Load<TextAsset>("Data/BasicCards");
        }
        
        if (cardDataJson == null)
        {
            Debug.LogError("[CardManager] 未找到卡牌数据JSON文件");
            return;
        }
        
        try
        {
            CardListJson cardList = JsonUtility.FromJson<CardListJson>(cardDataJson.text);
            
            foreach (var cardJson in cardList.cards)
            {
                var card = CreateCardFromJson(cardJson);
                cardDatabase[card.cardId] = card;
            }
            
            Debug.Log($"[CardManager] 成功加载 {cardDatabase.Count} 张卡牌");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CardManager] 加载卡牌数据失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 从JSON数据创建CardData
    /// </summary>
    private CardData CreateCardFromJson(CardDataJson json)
    {
        var card = ScriptableObject.CreateInstance<CardData>();
        
        card.cardId = json.cardId;
        card.cardName = json.cardName;
        card.description = json.description;
        card.flavor = json.flavor;
        card.cost = json.cost;
        card.value = json.value;
        
        // 解析CardType
        if (System.Enum.TryParse(json.cardType, out CardType type))
        {
            card.cardType = type;
        }
        
        // 解析Rarity
        if (System.Enum.TryParse(json.rarity, out CardRarity rarity))
        {
            card.rarity = rarity;
        }
        
        return card;
    }
    
    #endregion
    
    #region 卡牌获取
    
    /// <summary>
    /// 根据ID获取卡牌
    /// </summary>
    public CardData GetCard(string cardId)
    {
        if (cardDatabase.ContainsKey(cardId))
        {
            return cardDatabase[cardId];
        }
        
        Debug.LogWarning($"[CardManager] 未找到卡牌: {cardId}");
        return null;
    }
    
    /// <summary>
    /// 获取所有卡牌
    /// </summary>
    public List<CardData> GetAllCards()
    {
        return new List<CardData>(cardDatabase.Values);
    }
    
    /// <summary>
    /// 根据稀有度获取卡牌
    /// </summary>
    public List<CardData> GetCardsByRarity(CardRarity rarity)
    {
        List<CardData> result = new List<CardData>();
        
        foreach (var card in cardDatabase.Values)
        {
            if (card.rarity == rarity)
            {
                result.Add(card);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// 根据类型获取卡牌
    /// </summary>
    public List<CardData> GetCardsByType(CardType type)
    {
        List<CardData> result = new List<CardData>();
        
        foreach (var card in cardDatabase.Values)
        {
            if (card.cardType == type)
            {
                result.Add(card);
            }
        }
        
        return result;
    }
    
    #endregion
    
    #region 牌库管理
    
    /// <summary>
    /// 创建初始牌库
    /// </summary>
    public List<CardData> CreateStarterDeck()
    {
        List<CardData> deck = new List<CardData>();
        
        // 初始牌库：5张攻击卡 + 3张防御卡 + 2张技能卡
        for (int i = 1; i <= 5; i++)
        {
            deck.Add(GetCard($"ATK_{i:D3}"));
        }
        
        for (int i = 1; i <= 3; i++)
        {
            deck.Add(GetCard($"SKL_{i:D3}"));
        }
        
        deck.Add(GetCard("PWR_001"));
        deck.Add(GetCard("PWR_002"));
        
        Debug.Log($"[CardManager] 创建初始牌库，共 {deck.Count} 张卡");
        
        return deck;
    }
    
    /// <summary>
    /// 添加卡牌到玩家牌库
    /// </summary>
    public void AddCardToDeck(string cardId)
    {
        var card = GetCard(cardId);
        if (card != null)
        {
            playerDeck.Add(card);
            Debug.Log($"[CardManager] 添加卡牌到牌库: {card.cardName}");
        }
    }
    
    /// <summary>
    /// 从玩家牌库移除卡牌
    /// </summary>
    public void RemoveCardFromDeck(CardData card)
    {
        if (playerDeck.Contains(card))
        {
            playerDeck.Remove(card);
            Debug.Log($"[CardManager] 从牌库移除卡牌: {card.cardName}");
        }
    }
    
    #endregion
    
    #region DEBUG接口
    
    /// <summary>
    /// DEBUG: 打印所有卡牌
    /// </summary>
    public void DEBUG_PrintAllCards()
    {
        Debug.Log("=== 所有卡牌 ===");
        foreach (var card in cardDatabase.Values)
        {
            Debug.Log($"{card.cardId}: {card.cardName} (费用{card.cost}, 效果值{card.value})");
        }
    }
    
    #endregion
}
