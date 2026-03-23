using UnityEngine;
using System;

/// <summary>
/// 卡牌数据结构
/// Week 2: 支持JSON加载
/// </summary>
[CreateAssetMenu(fileName = "New Card", menuName = "RuneChronicles/Card")]
[Serializable]
public class CardData : ScriptableObject
{
    [Header("基础信息")]
    public string cardId;
    public string cardName;
    public string description;
    public string flavor;
    public Sprite cardImage;
    
    [Header("卡牌类型")]
    public CardType cardType = CardType.Attack;
    public CardRarity rarity = CardRarity.Common;
    
    [Header("属性")]
    public int cost = 1;
    public int value = 0;  // 伤害/护盾/效果值
    
    [Header("融合配方")]
    [Tooltip("此卡可以通过哪些卡牌融合得到")]
    public CardData[] fusionRecipe;
    
    /// <summary>
    /// 从JSON创建CardData
    /// </summary>
    public static CardData FromJson(string json)
    {
        var data = JsonUtility.FromJson<CardDataJson>(json);
        var card = CreateInstance<CardData>();
        
        card.cardId = data.cardId;
        card.cardName = data.cardName;
        card.description = data.description;
        card.flavor = data.flavor;
        card.cost = data.cost;
        card.value = data.value;
        
        // 解析CardType
        if (Enum.TryParse(data.cardType, out CardType type))
        {
            card.cardType = type;
        }
        
        // 解析Rarity
        if (Enum.TryParse(data.rarity, out CardRarity rarityValue))
        {
            card.rarity = rarityValue;
        }
        
        return card;
    }
}

/// <summary>
/// 卡牌类型
/// </summary>
public enum CardType
{
    Attack,   // 攻击卡
    Skill,    // 技能卡（防御/抽牌等）
    Power     // 能力卡（持续效果）
}

/// <summary>
/// 卡牌稀有度
/// </summary>
public enum CardRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}

/// <summary>
/// JSON反序列化用的数据结构
/// </summary>
[Serializable]
public class CardDataJson
{
    public string cardId;
    public string cardName;
    public string cardType;
    public string rarity;
    public int cost;
    public int value;
    public string description;
    public string flavor;
}

/// <summary>
/// 卡牌列表JSON结构
/// </summary>
[Serializable]
public class CardListJson
{
    public CardDataJson[] cards;
}
