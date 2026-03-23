using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 角色管理器
/// Week 6: 多角色系统
/// </summary>
public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }
    
    [Header("当前角色")]
    public CharacterClass currentCharacter = CharacterClass.Mage;
    
    // 角色数据
    private Dictionary<CharacterClass, CharacterData> characterDatabase = new Dictionary<CharacterClass, CharacterData>();
    
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
        InitializeCharacters();
    }
    
    /// <summary>
    /// 初始化所有角色
    /// </summary>
    private void InitializeCharacters()
    {
        // 法师
        characterDatabase[CharacterClass.Mage] = new CharacterData
        {
            characterClass = CharacterClass.Mage,
            characterName = "符文法师",
            maxHP = 80,
            maxEnergy = 3,
            startingDeck = GetMageStarterDeck(),
            startingRelic = "RELIC_001" // 符文之心
        };
        
        // 战士
        characterDatabase[CharacterClass.Warrior] = new CharacterData
        {
            characterClass = CharacterClass.Warrior,
            characterName = "符文战士",
            maxHP = 100,
            maxEnergy = 3,
            startingDeck = GetWarriorStarterDeck(),
            startingRelic = "RELIC_002" // 战争号角
        };
        
        Debug.Log($"[CharacterManager] 初始化 {characterDatabase.Count} 个角色");
    }
    
    /// <summary>
    /// 获取法师初始卡组
    /// </summary>
    private List<string> GetMageStarterDeck()
    {
        return new List<string>
        {
            "ATK_001", "ATK_001", "ATK_002", "ATK_002", "ATK_003",
            "SKL_001", "SKL_001", "SKL_002", 
            "PWR_001", "PWR_002"
        };
    }
    
    /// <summary>
    /// 获取战士初始卡组
    /// </summary>
    private List<string> GetWarriorStarterDeck()
    {
        return new List<string>
        {
            "ATK_004", "ATK_004", "ATK_005", "ATK_005", "ATK_025",
            "SKL_008", "SKL_008", "SKL_004",
            "PWR_010", "PWR_009"
        };
    }
    
    /// <summary>
    /// 选择角色
    /// </summary>
    public void SelectCharacter(CharacterClass characterClass)
    {
        currentCharacter = characterClass;
        Debug.Log($"[CharacterManager] 选择角色: {GetCharacterData().characterName}");
    }
    
    /// <summary>
    /// 获取当前角色数据
    /// </summary>
    public CharacterData GetCharacterData()
    {
        if (characterDatabase.ContainsKey(currentCharacter))
        {
            return characterDatabase[currentCharacter];
        }
        
        return characterDatabase[CharacterClass.Mage];
    }
}

/// <summary>
/// 角色类型枚举
/// </summary>
public enum CharacterClass
{
    Mage,       // 法师
    Warrior     // 战士
}

/// <summary>
/// 角色数据
/// </summary>
[System.Serializable]
public class CharacterData
{
    public CharacterClass characterClass;
    public string characterName;
    public int maxHP;
    public int maxEnergy;
    public List<string> startingDeck;
    public string startingRelic;
}
