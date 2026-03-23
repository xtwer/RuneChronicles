using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 卡牌管理器 - 自动加载所有JSON
/// </summary>
public class CardManager : MonoBehaviour
{
    public static CardManager Instance { get; private set; }
    
    private Dictionary<string, CardData> cardDatabase = new Dictionary<string, CardData>();
    private List<CardData> allCards = new List<CardData>();
    
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
        LoadAllCards();
    }
    
    /// <summary>
    /// 加载所有卡牌JSON
    /// </summary>
    public void LoadAllCards()
    {
        Debug.Log("[CardManager] 开始加载卡牌...");
        
        // 加载所有JSON文件
        string[] jsonFiles = {
            "BasicCards",
            "FusionCards",
            "WarriorCards",
            "ExtendedAttackCards",
            "ExtendedSkillCards",
            "ExtendedPowerCards"
        };
        
        int totalLoaded = 0;
        
        foreach (var jsonFile in jsonFiles)
        {
            var textAsset = Resources.Load<TextAsset>($"Data/{jsonFile}");
            
            if (textAsset != null)
            {
                try
                {
                    CardListJson cardList = JsonUtility.FromJson<CardListJson>(textAsset.text);
                    
                    if (cardList != null && cardList.cards != null)
                    {
                        foreach (var cardJson in cardList.cards)
                        {
                            var card = CreateCardFromJson(cardJson);
                            
                            if (card != null && !cardDatabase.ContainsKey(card.cardId))
                            {
                                cardDatabase[card.cardId] = card;
                                totalLoaded++;
                            }
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[CardManager] 加载 {jsonFile} 失败: {e.Message}");
                }
            }
            else
            {
                Debug.LogWarning($"[CardManager] 未找到: Resources/Data/{jsonFile}.json");
            }
        }
        
        Debug.Log($"[CardManager] 成功加载 {totalLoaded} 张卡牌");
    }
    
    /// <summary>
    /// 从JSON创建CardData
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
    
    /// <summary>
    /// 创建初始牌库
    /// </summary>
    public List<CardData> CreateStarterDeck()
    {
        List<CardData> deck = new List<CardData>();
        
        // 初始牌库：5张攻击卡 + 3张防御卡 + 2张能力卡
        for (int i = 1; i <= 5; i++)
        {
            var card = GetCard($"ATK_{i:D3}");
            if (card != null) deck.Add(card);
        }
        
        for (int i = 1; i <= 3; i++)
        {
            var card = GetCard($"SKL_{i:D3}");
            if (card != null) deck.Add(card);
        }
        
        var pwr1 = GetCard("PWR_001");
        var pwr2 = GetCard("PWR_002");
        if (pwr1 != null) deck.Add(pwr1);
        if (pwr2 != null) deck.Add(pwr2);
        
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
    
    /// <summary>
    /// 注册新卡牌到数据库（用于融合卡）
    /// </summary>
    public void RegisterCard(CardData card)
    {
        if (card != null && !cardDatabase.ContainsKey(card.cardId))
        {
            cardDatabase[card.cardId] = card;
            allCards.Add(card);
            Debug.Log($"[CardManager] 注册新卡牌: {card.cardName} ({card.cardId})");
        }
    }
    
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
}
