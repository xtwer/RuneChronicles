using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 融合管理器
/// Week 3: 卡牌融合系统
/// </summary>
public class FusionManager : MonoBehaviour
{
    public static FusionManager Instance { get; private set; }
    
    [Header("融合配置")]
    public int fusionPointCost = 3; // 每次融合消耗的融合点
    public int maxFusionPoints = 10; // 最大融合点
    
    [Header("当前状态")]
    public int currentFusionPoints = 0;
    
    // 融合配方数据库
    private Dictionary<string, FusionRecipe> fusionRecipes = new Dictionary<string, FusionRecipe>();
    
    // 事件
    public event Action<int> OnFusionPointsChanged;
    public event Action<CardData> OnCardFused;
    
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
        LoadFusionRecipes();
    }
    
    #region 融合点管理
    
    /// <summary>
    /// 获得融合点
    /// </summary>
    public void GainFusionPoints(int amount)
    {
        currentFusionPoints += amount;
        currentFusionPoints = Mathf.Min(currentFusionPoints, maxFusionPoints);
        
        Debug.Log($"[FusionManager] 获得 {amount} 融合点，当前: {currentFusionPoints}/{maxFusionPoints}");
        
        OnFusionPointsChanged?.Invoke(currentFusionPoints);
    }
    
    /// <summary>
    /// 消耗融合点
    /// </summary>
    public bool SpendFusionPoints(int amount)
    {
        if (currentFusionPoints >= amount)
        {
            currentFusionPoints -= amount;
            OnFusionPointsChanged?.Invoke(currentFusionPoints);
            return true;
        }
        
        Debug.LogWarning($"[FusionManager] 融合点不足: {currentFusionPoints}/{amount}");
        return false;
    }
    
    /// <summary>
    /// 获取当前融合点
    /// </summary>
    public int GetFusionPoints()
    {
        return currentFusionPoints;
    }
    
    #endregion
    
    #region 融合系统
    
    /// <summary>
    /// 尝试融合两张卡牌
    /// </summary>
    public CardData FuseCards(CardData card1, CardData card2)
    {
        // 检查融合点
        if (currentFusionPoints < fusionPointCost)
        {
            Debug.LogWarning($"[FusionManager] 融合点不足，需要 {fusionPointCost}点");
            return null;
        }
        
        // 检查是否有预设配方
        string recipeKey = GetRecipeKey(card1.cardId, card2.cardId);
        
        if (fusionRecipes.ContainsKey(recipeKey))
        {
            // 预设配方融合
            var recipe = fusionRecipes[recipeKey];
            if (CardManager.Instance == null) return null;
            var resultCard = CardManager.Instance.GetCard(recipe.resultCardId);
            
            if (resultCard != null)
            {
                SpendFusionPoints(fusionPointCost);
                Debug.Log($"[FusionManager] 融合成功: {card1.cardName} + {card2.cardName} → {resultCard.cardName}");
                
                OnCardFused?.Invoke(resultCard);
                return resultCard;
            }
        }
        
        // 随机融合
        var randomResult = RandomFusion(card1, card2);
        
        if (randomResult != null)
        {
            SpendFusionPoints(fusionPointCost);
            Debug.Log($"[FusionManager] 随机融合: {card1.cardName} + {card2.cardName} → {randomResult.cardName}");
            
            OnCardFused?.Invoke(randomResult);
            return randomResult;
        }
        
        Debug.LogWarning("[FusionManager] 融合失败");
        return null;
    }
    
    /// <summary>
    /// 随机融合算法
    /// </summary>
    private CardData RandomFusion(CardData card1, CardData card2)
    {
        // 计算融合后的属性
        int newCost = Mathf.CeilToInt((card1.cost + card2.cost) / 2f);
        int newValue = card1.value + card2.value;
        
        // 随机选择一个稀有度（优先高稀有度）
        CardRarity newRarity = (CardRarity)Mathf.Max((int)card1.rarity, (int)card2.rarity);
        
        // 随机选择一个类型
        CardType newType = UnityEngine.Random.value > 0.5f ? card1.cardType : card2.cardType;
        
        // 创建融合卡
        var fusedCard = ScriptableObject.CreateInstance<CardData>();
        fusedCard.cardId = $"FUSED_{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
        fusedCard.cardName = $"{card1.cardName}+{card2.cardName}";
        fusedCard.cardType = newType;
        fusedCard.rarity = newRarity;
        fusedCard.cost = newCost;
        fusedCard.value = newValue;
        fusedCard.description = $"融合卡：{card1.cardName} 与 {card2.cardName} 的结合";
        fusedCard.flavor = "强大的融合之力";
        
        // 注册到CardManager（重要！）
        if (CardManager.Instance != null)
        {
            CardManager.Instance.RegisterCard(fusedCard);
        }
        
        return fusedCard;
    }
    
    /// <summary>
    /// 生成配方键
    /// </summary>
    private string GetRecipeKey(string card1Id, string card2Id)
    {
        // 确保顺序一致
        if (string.Compare(card1Id, card2Id) > 0)
        {
            return $"{card2Id}+{card1Id}";
        }
        return $"{card1Id}+{card2Id}";
    }
    
    #endregion
    
    #region 融合配方管理
    
    /// <summary>
    /// 加载融合配方
    /// </summary>
    private void LoadFusionRecipes()
    {
        // Week 3: 10种预设配方
        
        // 1. 烈火斩 + 冰霜打击 = 冰火交融
        AddRecipe("ATK_001", "ATK_002", "FUSION_001");
        
        // 2. 烈火斩 + 雷霆一击 = 雷炎斩
        AddRecipe("ATK_001", "ATK_003", "FUSION_002");
        
        // 3. 冰霜打击 + 雷霆一击 = 冰雷爆
        AddRecipe("ATK_002", "ATK_003", "FUSION_003");
        
        // 4. 重击 + 连斩 = 狂暴连击
        AddRecipe("ATK_004", "ATK_005", "FUSION_004");
        
        // 5. 防御姿态 + 符文护盾 = 坚不可摧
        AddRecipe("SKL_001", "SKL_002", "FUSION_005");
        
        // 6. 烈火斩 + 防御姿态 = 火焰屏障
        AddRecipe("ATK_001", "SKL_001", "FUSION_006");
        
        // 7. 生命汲取 + 力量觉醒 = 狂战士之怒
        AddRecipe("SKL_003", "PWR_001", "FUSION_007");
        
        // 8. 力量觉醒 + 坚韧之心 = 不屈斗志
        AddRecipe("PWR_001", "PWR_002", "FUSION_008");
        
        // 9. 烈火斩 + 生命汲取 = 炎之吞噬
        AddRecipe("ATK_001", "SKL_003", "FUSION_009");
        
        // 10. 雷霆一击 + 力量觉醒 = 雷神之力
        AddRecipe("ATK_003", "PWR_001", "FUSION_010");
        
        Debug.Log($"[FusionManager] 加载 {fusionRecipes.Count} 个融合配方");
    }
    
    /// <summary>
    /// 添加融合配方
    /// </summary>
    private void AddRecipe(string card1Id, string card2Id, string resultId)
    {
        string key = GetRecipeKey(card1Id, card2Id);
        
        var recipe = new FusionRecipe
        {
            card1Id = card1Id,
            card2Id = card2Id,
            resultCardId = resultId
        };
        
        fusionRecipes[key] = recipe;
    }
    
    /// <summary>
    /// 获取配方（如果存在）
    /// </summary>
    public FusionRecipe GetRecipe(string card1Id, string card2Id)
    {
        string key = GetRecipeKey(card1Id, card2Id);
        
        if (fusionRecipes.ContainsKey(key))
        {
            return fusionRecipes[key];
        }
        
        return null;
    }
    
    /// <summary>
    /// 是否有预设配方
    /// </summary>
    public bool HasRecipe(string card1Id, string card2Id)
    {
        string key = GetRecipeKey(card1Id, card2Id);
        return fusionRecipes.ContainsKey(key);
    }
    
    #endregion
    
    #region DEBUG接口
    
    /// <summary>
    /// DEBUG: 设置融合点
    /// </summary>
    public void DEBUG_SetFusionPoints(int amount)
    {
        currentFusionPoints = Mathf.Clamp(amount, 0, maxFusionPoints);
        OnFusionPointsChanged?.Invoke(currentFusionPoints);
    }
    
    /// <summary>
    /// DEBUG: 打印所有配方
    /// </summary>
    public void DEBUG_PrintAllRecipes()
    {
        Debug.Log("=== 融合配方 ===");
        foreach (var recipe in fusionRecipes.Values)
        {
            var card1 = CardManager.Instance.GetCard(recipe.card1Id);
            var card2 = CardManager.Instance.GetCard(recipe.card2Id);
            var result = CardManager.Instance.GetCard(recipe.resultCardId);
            
            Debug.Log($"{card1?.cardName} + {card2?.cardName} → {result?.cardName}");
        }
    }
    
    #endregion
}

/// <summary>
/// 融合配方数据结构
/// </summary>
[Serializable]
public class FusionRecipe
{
    public string card1Id;
    public string card2Id;
    public string resultCardId;
}
