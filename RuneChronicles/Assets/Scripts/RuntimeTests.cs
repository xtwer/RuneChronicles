using UnityEngine;

/// <summary>
/// 简单的运行时测试 - 不需要Test Framework
/// 直接在场景中运行验证
/// </summary>
public class RuntimeTests : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 开始运行时测试 ===");
        
        RunAllTests();
        
        Debug.Log("=== 测试完成 ===");
    }
    
    void RunAllTests()
    {
        // 测试1: 卡牌管理器
        Test_CardManager();
        
        // 测试2: 战斗管理器
        Test_BattleManager();
        
        // 测试3: 融合管理器
        Test_FusionManager();
        
        // 测试4: 地图管理器
        Test_MapManager();
        
        // 测试5: 角色管理器
        Test_CharacterManager();
        
        // 测试6: 遗物管理器
        Test_RelicManager();
        
        // 测试7: 游戏管理器
        Test_GameManager();
    }
    
    void Test_CardManager()
    {
        Debug.Log("[测试] CardManager...");
        
        if (CardManager.Instance == null)
        {
            var go = new GameObject("CardManager");
            go.AddComponent<CardManager>();
        }
        
        if (CardManager.Instance != null)
        {
            Debug.Log("✅ CardManager 初始化成功");
            
            // 测试卡牌加载
            var allCards = CardManager.Instance.GetAllCards();
            Debug.Log($"✅ 加载 {allCards.Count} 张卡牌");
        }
        else
        {
            Debug.LogError("❌ CardManager 初始化失败");
        }
    }
    
    void Test_BattleManager()
    {
        Debug.Log("[测试] BattleManager...");
        
        if (BattleManager.Instance == null)
        {
            var go = new GameObject("BattleManager");
            go.AddComponent<BattleManager>();
        }
        
        if (BattleManager.Instance != null)
        {
            Debug.Log("✅ BattleManager 初始化成功");
            Debug.Log($"✅ 最大能量: {BattleManager.Instance.maxEnergy}");
        }
        else
        {
            Debug.LogError("❌ BattleManager 初始化失败");
        }
    }
    
    void Test_FusionManager()
    {
        Debug.Log("[测试] FusionManager...");
        
        if (FusionManager.Instance == null)
        {
            var go = new GameObject("FusionManager");
            go.AddComponent<FusionManager>();
        }
        
        if (FusionManager.Instance != null)
        {
            Debug.Log("✅ FusionManager 初始化成功");
            Debug.Log($"✅ 融合点上限: {FusionManager.Instance.maxFusionPoints}");
        }
        else
        {
            Debug.LogError("❌ FusionManager 初始化失败");
        }
    }
    
    void Test_MapManager()
    {
        Debug.Log("[测试] MapManager...");
        
        if (MapManager.Instance == null)
        {
            var go = new GameObject("MapManager");
            go.AddComponent<MapManager>();
        }
        
        if (MapManager.Instance != null)
        {
            Debug.Log("✅ MapManager 初始化成功");
            Debug.Log($"✅ 总层数: {MapManager.Instance.totalFloors}");
        }
        else
        {
            Debug.LogError("❌ MapManager 初始化失败");
        }
    }
    
    void Test_CharacterManager()
    {
        Debug.Log("[测试] CharacterManager...");
        
        if (CharacterManager.Instance == null)
        {
            var go = new GameObject("CharacterManager");
            go.AddComponent<CharacterManager>();
        }
        
        if (CharacterManager.Instance != null)
        {
            Debug.Log("✅ CharacterManager 初始化成功");
            
            var characterData = CharacterManager.Instance.GetCharacterData();
            Debug.Log($"✅ 当前角色: {characterData.characterName}");
        }
        else
        {
            Debug.LogError("❌ CharacterManager 初始化失败");
        }
    }
    
    void Test_RelicManager()
    {
        Debug.Log("[测试] RelicManager...");
        
        if (RelicManager.Instance == null)
        {
            var go = new GameObject("RelicManager");
            go.AddComponent<RelicManager>();
        }
        
        if (RelicManager.Instance != null)
        {
            Debug.Log("✅ RelicManager 初始化成功");
            Debug.Log($"✅ 当前遗物数量: {RelicManager.Instance.currentRelics.Count}");
        }
        else
        {
            Debug.LogError("❌ RelicManager 初始化失败");
        }
    }
    
    void Test_GameManager()
    {
        Debug.Log("[测试] GameManager...");
        
        if (GameManager.Instance == null)
        {
            var go = new GameObject("GameManager");
            go.AddComponent<GameManager>();
        }
        
        if (GameManager.Instance != null)
        {
            Debug.Log("✅ GameManager 初始化成功");
            Debug.Log($"✅ 游戏状态: {GameManager.Instance.currentState}");
        }
        else
        {
            Debug.LogError("❌ GameManager 初始化失败");
        }
    }
}
