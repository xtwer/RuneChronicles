using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 游戏主管理器
/// 整合所有子系统
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("游戏状态")]
    public GameState currentState = GameState.MainMenu;
    public int currentGold = 0;
    public int currentRun = 1;
    
    [Header("难度设置")]
    public DifficultyLevel difficulty = DifficultyLevel.Normal;
    
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
        Initialize();
    }
    
    /// <summary>
    /// 初始化游戏
    /// </summary>
    private void Initialize()
    {
        Debug.Log("[GameManager] 游戏初始化");
        
        // 确保所有管理器都已创建
        EnsureManagersExist();
        
        // 切换到主菜单
        ChangeState(GameState.MainMenu);
    }
    
    /// <summary>
    /// 确保所有管理器存在
    /// </summary>
    private void EnsureManagersExist()
    {
        // 如果管理器不存在，创建它们
        if (CardManager.Instance == null)
        {
            new GameObject("CardManager").AddComponent<CardManager>();
        }
        
        if (BattleManager.Instance == null)
        {
            new GameObject("BattleManager").AddComponent<BattleManager>();
        }
        
        if (FusionManager.Instance == null)
        {
            new GameObject("FusionManager").AddComponent<FusionManager>();
        }
        
        if (MapManager.Instance == null)
        {
            new GameObject("MapManager").AddComponent<MapManager>();
        }
        
        if (CharacterManager.Instance == null)
        {
            new GameObject("CharacterManager").AddComponent<CharacterManager>();
        }
        
        if (RelicManager.Instance == null)
        {
            new GameObject("RelicManager").AddComponent<RelicManager>();
        }
        
        Debug.Log("[GameManager] 所有管理器已加载");
    }
    
    #region 游戏状态管理
    
    /// <summary>
    /// 改变游戏状态
    /// </summary>
    public void ChangeState(GameState newState)
    {
        currentState = newState;
        Debug.Log($"[GameManager] 游戏状态: {currentState}");
        
        switch (newState)
        {
            case GameState.MainMenu:
                OnMainMenu();
                break;
            
            case GameState.CharacterSelect:
                OnCharacterSelect();
                break;
            
            case GameState.Map:
                OnMap();
                break;
            
            case GameState.Battle:
                OnBattle();
                break;
            
            case GameState.Reward:
                OnReward();
                break;
            
            case GameState.Shop:
                OnShop();
                break;
            
            case GameState.GameOver:
                OnGameOver();
                break;
            
            case GameState.Victory:
                OnVictory();
                break;
        }
    }
    
    private void OnMainMenu()
    {
        // TODO: 显示主菜单UI
    }
    
    private void OnCharacterSelect()
    {
        // TODO: 显示角色选择UI
    }
    
    private void OnMap()
    {
        // TODO: 显示地图UI
    }
    
    private void OnBattle()
    {
        // TODO: 开始战斗
    }
    
    private void OnReward()
    {
        // TODO: 显示奖励UI
    }
    
    private void OnShop()
    {
        // TODO: 显示商店UI
    }
    
    private void OnGameOver()
    {
        Debug.Log("[GameManager] 游戏结束 - 失败");
        // TODO: 显示失败UI
    }
    
    private void OnVictory()
    {
        Debug.Log("[GameManager] 游戏结束 - 胜利！");
        // TODO: 显示胜利UI
    }
    
    #endregion
    
    #region 新游戏
    
    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewRun(CharacterClass characterClass)
    {
        Debug.Log($"[GameManager] 开始新游戏 - 角色: {characterClass}");
        
        // 选择角色
        CharacterManager.Instance.SelectCharacter(characterClass);
        
        // 获取角色数据
        var characterData = CharacterManager.Instance.GetCharacterData();
        
        // 初始化玩家
        if (Player.Instance != null)
        {
            Player.Instance.maxHP = characterData.maxHP;
            Player.Instance.currentHP = characterData.maxHP;
        }
        
        // 初始化牌库
        if (CardManager.Instance != null)
        {
            CardManager.Instance.playerDeck.Clear();
            foreach (var cardId in characterData.startingDeck)
            {
                var card = CardManager.Instance.GetCard(cardId);
                if (card != null)
                {
                    CardManager.Instance.playerDeck.Add(card);
                }
            }
        }
        
        // 获得初始遗物
        if (RelicManager.Instance != null)
        {
            RelicManager.Instance.ObtainRelic(characterData.startingRelic);
        }
        
        // 初始化地图
        if (MapManager.Instance != null)
        {
            MapManager.Instance.GenerateMap();
            MapManager.Instance.currentFloor = 0;
        }
        
        // 重置金币和融合点
        currentGold = 0;
        if (FusionManager.Instance != null)
        {
            FusionManager.Instance.currentFusionPoints = 3; // 初始3点
        }
        
        // 进入地图
        ChangeState(GameState.Map);
    }
    
    #endregion
    
    #region 金币系统
    
    /// <summary>
    /// 获得金币
    /// </summary>
    public void AddGold(int amount)
    {
        currentGold += amount;
        Debug.Log($"[GameManager] 获得 {amount} 金币，当前: {currentGold}");
    }
    
    /// <summary>
    /// 消耗金币
    /// </summary>
    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"[GameManager] 消耗 {amount} 金币，剩余: {currentGold}");
            return true;
        }
        
        Debug.LogWarning($"[GameManager] 金币不足: {currentGold}/{amount}");
        return false;
    }
    
    #endregion
    
    #region 存档系统
    
    private const string SAVE_FILE_NAME = "savegame.json";
    
    /// <summary>
    /// 保存游戏数据类
    /// </summary>
    [System.Serializable]
    public class SaveData
    {
        // 游戏状态
        public int currentGold;
        public int currentRun;
        public string difficulty;
        public string gameState;
        
        // 角色状态
        public string characterClass;
        public int currentHP;
        public int maxHP;
        
        // 地图进度
        public int currentFloor;
        
        // 牌库（只存ID）
        public List<string> playerDeckIds = new List<string>();
        
        // 遗物（只存ID）
        public List<string> relicIds = new List<string>();
        
        // 融合点
        public int fusionPoints;
        
        // 时间戳
        public string saveTime;
    }
    
    /// <summary>
    /// 保存游戏
    /// </summary>
    public void SaveGame()
    {
        try
        {
            SaveData data = new SaveData();
            
            // 基础数据
            data.currentGold = currentGold;
            data.currentRun = currentRun;
            data.difficulty = difficulty.ToString();
            data.gameState = currentState.ToString();
            
            // 角色数据
            if (CharacterManager.Instance != null)
            {
                data.characterClass = CharacterManager.Instance.currentCharacter.ToString();
            }
            
            if (Player.Instance != null)
            {
                data.currentHP = Player.Instance.currentHP;
                data.maxHP = Player.Instance.maxHP;
            }
            
            // 地图进度
            if (MapManager.Instance != null)
            {
                data.currentFloor = MapManager.Instance.currentFloor;
            }
            
            // 牌库
            if (CardManager.Instance != null && CardManager.Instance.playerDeck != null)
            {
                foreach (var card in CardManager.Instance.playerDeck)
                {
                    data.playerDeckIds.Add(card.cardId);
                }
            }
            
            // 遗物
            if (RelicManager.Instance != null)
            {
                data.relicIds = new List<string>(RelicManager.Instance.GetOwnedRelics());
            }
            
            // 融合点
            if (FusionManager.Instance != null)
            {
                data.fusionPoints = FusionManager.Instance.currentFusionPoints;
            }
            
            // 时间戳
            data.saveTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            
            // 序列化为JSON
            string json = JsonUtility.ToJson(data, true);
            
            // 保存到文件
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            System.IO.File.WriteAllText(savePath, json);
            
            Debug.Log($"[GameManager] 游戏已保存: {savePath}");
            Debug.Log($"[GameManager] 存档时间: {data.saveTime}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] 保存失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 加载游戏
    /// </summary>
    public bool LoadGame()
    {
        try
        {
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            
            if (!System.IO.File.Exists(savePath))
            {
                Debug.LogWarning("[GameManager] 存档文件不存在");
                return false;
            }
            
            // 读取文件
            string json = System.IO.File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            
            if (data == null)
            {
                Debug.LogError("[GameManager] 存档数据解析失败");
                return false;
            }
            
            Debug.Log($"[GameManager] 加载存档: {data.saveTime}");
            
            // 恢复基础数据
            currentGold = data.currentGold;
            currentRun = data.currentRun;
            difficulty = (DifficultyLevel)System.Enum.Parse(typeof(DifficultyLevel), data.difficulty);
            
            // 恢复角色
            if (CharacterManager.Instance != null)
            {
                CharacterClass charClass = (CharacterClass)System.Enum.Parse(typeof(CharacterClass), data.characterClass);
                CharacterManager.Instance.SelectCharacter(charClass);
            }
            
            if (Player.Instance != null)
            {
                Player.Instance.currentHP = data.currentHP;
                Player.Instance.maxHP = data.maxHP;
            }
            
            // 恢复地图进度
            if (MapManager.Instance != null)
            {
                MapManager.Instance.currentFloor = data.currentFloor;
                MapManager.Instance.GenerateMap(); // 重新生成地图
            }
            
            // 恢复牌库
            if (CardManager.Instance != null)
            {
                CardManager.Instance.playerDeck.Clear();
                foreach (var cardId in data.playerDeckIds)
                {
                    var card = CardManager.Instance.GetCard(cardId);
                    if (card != null)
                    {
                        CardManager.Instance.playerDeck.Add(card);
                    }
                }
                Debug.Log($"[GameManager] 恢复牌库: {CardManager.Instance.playerDeck.Count}张");
            }
            
            // 恢复遗物
            if (RelicManager.Instance != null)
            {
                RelicManager.Instance.ClearAllRelics();
                foreach (var relicId in data.relicIds)
                {
                    RelicManager.Instance.ObtainRelic(relicId);
                }
                Debug.Log($"[GameManager] 恢复遗物: {data.relicIds.Count}个");
            }
            
            // 恢复融合点
            if (FusionManager.Instance != null)
            {
                FusionManager.Instance.currentFusionPoints = data.fusionPoints;
            }
            
            // 恢复游戏状态
            GameState loadedState = (GameState)System.Enum.Parse(typeof(GameState), data.gameState);
            ChangeState(loadedState);
            
            Debug.Log("[GameManager] 游戏加载成功！");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] 加载失败: {e.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// 检查是否有存档
    /// </summary>
    public bool HasSaveFile()
    {
        string savePath = System.IO.Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
        return System.IO.File.Exists(savePath);
    }
    
    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSave()
    {
        try
        {
            string savePath = System.IO.Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            if (System.IO.File.Exists(savePath))
            {
                System.IO.File.Delete(savePath);
                Debug.Log("[GameManager] 存档已删除");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[GameManager] 删除存档失败: {e.Message}");
        }
    }
    
    /// <summary>
    /// 自动保存（在关键节点触发）
    /// </summary>
    public void AutoSave()
    {
        // 只在特定状态下自动保存
        if (currentState == GameState.Map || 
            currentState == GameState.Reward || 
            currentState == GameState.Shop)
        {
            SaveGame();
            Debug.Log("[GameManager] 自动保存完成");
        }
    }
    
    #endregion
    
    #region DEBUG接口
    
    /// <summary>
    /// DEBUG: 快速开始游戏（法师）
    /// </summary>
    public void DEBUG_QuickStartMage()
    {
        StartNewRun(CharacterClass.Mage);
        Debug.Log("[DEBUG] 快速开始 - 法师");
    }
    
    /// <summary>
    /// DEBUG: 快速开始游戏（战士）
    /// </summary>
    public void DEBUG_QuickStartWarrior()
    {
        StartNewRun(CharacterClass.Warrior);
        Debug.Log("[DEBUG] 快速开始 - 战士");
    }
    
    /// <summary>
    /// DEBUG: 获得所有卡牌
    /// </summary>
    public void DEBUG_UnlockAllCards()
    {
        var allCards = CardManager.Instance.GetAllCards();
        foreach (var card in allCards)
        {
            if (!CardManager.Instance.playerDeck.Contains(card))
            {
                CardManager.Instance.playerDeck.Add(card);
            }
        }
        Debug.Log($"[DEBUG] 解锁所有卡牌，当前牌库: {CardManager.Instance.playerDeck.Count}张");
    }
    
    /// <summary>
    /// DEBUG: 获得所有遗物
    /// </summary>
    public void DEBUG_UnlockAllRelics()
    {
        for (int i = 1; i <= 10; i++)
        {
            string relicId = $"RELIC_{i:D3}";
            if (!RelicManager.Instance.HasRelic(relicId))
            {
                RelicManager.Instance.ObtainRelic(relicId);
            }
        }
        Debug.Log("[DEBUG] 解锁所有遗物");
    }
    
    #endregion
}

/// <summary>
/// 游戏状态枚举
/// </summary>
public enum GameState
{
    MainMenu,         // 主菜单
    CharacterSelect,  // 角色选择
    Map,              // 地图
    Battle,           // 战斗
    Reward,           // 奖励
    Shop,             // 商店
    GameOver,         // 游戏结束
    Victory           // 胜利
}

/// <summary>
/// 难度等级
/// </summary>
public enum DifficultyLevel
{
    Easy,     // 简单
    Normal,   // 普通
    Hard      // 困难
}
