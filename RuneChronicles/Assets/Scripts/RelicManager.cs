using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 遗物管理器
/// Week 7: 遗物系统
/// </summary>
public class RelicManager : MonoBehaviour
{
    public static RelicManager Instance { get; private set; }
    
    [Header("当前遗物")]
    public List<Relic> currentRelics = new List<Relic>();
    
    // 遗物数据库
    private Dictionary<string, RelicData> relicDatabase = new Dictionary<string, RelicData>();
    
    // 事件
    public event Action<Relic> OnRelicObtained;
    
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
        LoadRelics();
    }
    
    /// <summary>
    /// 加载遗物数据
    /// </summary>
    private void LoadRelics()
    {
        // Week 7: 20个遗物
        
        // 通用遗物 (10个)
        AddRelic("RELIC_001", "符文之心", RelicRarity.Common, "每回合开始恢复2点生命");
        AddRelic("RELIC_002", "战争号角", RelicRarity.Common, "每次攻击额外造成1点伤害");
        AddRelic("RELIC_003", "护盾徽记", RelicRarity.Common, "每回合开始获得2点护盾");
        AddRelic("RELIC_004", "能量水晶", RelicRarity.Rare, "每回合额外获得1点能量");
        AddRelic("RELIC_005", "抽卡指环", RelicRarity.Rare, "每回合开始额外抽1张牌");
        AddRelic("RELIC_006", "生命之泉", RelicRarity.Rare, "最大生命+20");
        AddRelic("RELIC_007", "幸运硬币", RelicRarity.Common, "每场战斗额外获得10金币");
        AddRelic("RELIC_008", "融合宝石", RelicRarity.Rare, "融合点上限+5");
        AddRelic("RELIC_009", "时间沙漏", RelicRarity.Epic, "每3回合重置手牌");
        AddRelic("RELIC_010", "不屈之心", RelicRarity.Epic, "首次死亡时恢复50%生命（仅1次）");
        
        // 法师遗物 (5个)
        AddRelic("RELIC_M01", "魔法书", RelicRarity.Rare, "法术伤害+3");
        AddRelic("RELIC_M02", "法杖", RelicRarity.Rare, "首张法术牌费用-1");
        AddRelic("RELIC_M03", "魔法水晶", RelicRarity.Epic, "每打出法术牌抽1张牌");
        AddRelic("RELIC_M04", "元素核心", RelicRarity.Epic, "元素伤害+50%");
        AddRelic("RELIC_M05", "贤者之石", RelicRarity.Legendary, "每回合所有法术牌费用-1");
        
        // 战士遗物 (5个)
        AddRelic("RELIC_W01", "战争勋章", RelicRarity.Rare, "攻击牌伤害+3");
        AddRelic("RELIC_W02", "盾牌碎片", RelicRarity.Rare, "护盾效果+30%");
        AddRelic("RELIC_W03", "狂战士面具", RelicRarity.Epic, "生命低于50%时攻击+5");
        AddRelic("RELIC_W04", "铁血军团", RelicRarity.Epic, "每次获得护盾时造成2点伤害");
        AddRelic("RELIC_W05", "不死图腾", RelicRarity.Legendary, "死亡时复活并恢复30%生命（仅1次）");
        
        Debug.Log($"[RelicManager] 加载 {relicDatabase.Count} 个遗物");
    }
    
    /// <summary>
    /// 添加遗物到数据库
    /// </summary>
    private void AddRelic(string id, string name, RelicRarity rarity, string description)
    {
        relicDatabase[id] = new RelicData
        {
            relicId = id,
            relicName = name,
            rarity = rarity,
            description = description
        };
    }
    
    /// <summary>
    /// 获得遗物
    /// </summary>
    public void ObtainRelic(string relicId)
    {
        if (!relicDatabase.ContainsKey(relicId))
        {
            Debug.LogWarning($"[RelicManager] 遗物不存在: {relicId}");
            return;
        }
        
        var relicData = relicDatabase[relicId];
        var relic = new Relic
        {
            relicId = relicData.relicId,
            relicName = relicData.relicName,
            rarity = relicData.rarity,
            description = relicData.description,
            isActive = true
        };
        
        currentRelics.Add(relic);
        
        Debug.Log($"[RelicManager] 获得遗物: {relic.relicName}");
        OnRelicObtained?.Invoke(relic);
        
        // 应用遗物效果
        ApplyRelicEffect(relic);
    }
    
    /// <summary>
    /// 应用遗物效果
    /// </summary>
    private void ApplyRelicEffect(Relic relic)
    {
        // 根据遗物ID应用不同效果
        switch (relic.relicId)
        {
            case "RELIC_004": // 能量水晶
                if (BattleManager.Instance != null)
                {
                    BattleManager.Instance.maxEnergy += 1;
                }
                break;
            
            case "RELIC_006": // 生命之泉
                if (Player.Instance != null)
                {
                    Player.Instance.maxHP += 20;
                    Player.Instance.currentHP += 20;
                }
                break;
            
            case "RELIC_008": // 融合宝石
                if (FusionManager.Instance != null)
                {
                    FusionManager.Instance.maxFusionPoints += 5;
                }
                break;
            
            // TODO: 实现其他遗物效果
        }
    }
    
    /// <summary>
    /// 检查是否拥有遗物
    /// </summary>
    public bool HasRelic(string relicId)
    {
        foreach (var relic in currentRelics)
        {
            if (relic.relicId == relicId)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// 获取遗物数据
    /// </summary>
    public RelicData GetRelicData(string relicId)
    {
        if (relicDatabase.ContainsKey(relicId))
        {
            return relicDatabase[relicId];
        }
        return null;
    }
    
    /// <summary>
    /// 获取所有遗物数据
    /// </summary>
    public List<RelicData> GetAllRelics()
    {
        return new List<RelicData>(relicDatabase.Values);
    }
}

/// <summary>
/// 遗物数据
/// </summary>
[Serializable]
public class RelicData
{
    public string relicId;
    public string relicName;
    public RelicRarity rarity;
    public string description;
}

/// <summary>
/// 遗物实例
/// </summary>
[Serializable]
public class Relic
{
    public string relicId;
    public string relicName;
    public RelicRarity rarity;
    public string description;
    public bool isActive;
}

/// <summary>
/// 遗物稀有度
/// </summary>
public enum RelicRarity
{
    Common,
    Rare,
    Epic,
    Legendary
}
