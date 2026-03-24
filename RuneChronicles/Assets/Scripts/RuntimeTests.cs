using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 运行时测试入口 - 所有关键功能的自动化验证
/// 使用方法：在场景中创建空GameObject，挂载此脚本，运行游戏查看Console
/// </summary>
public class RuntimeTests : MonoBehaviour
{
    [Header("测试配置")]
    [Tooltip("启动时自动运行所有测试")]
    public bool autoRunOnStart = true;
    
    [Tooltip("测试失败时暂停游戏")]
    public bool pauseOnFailure = true;
    
    private int totalTests = 0;
    private int passedTests = 0;
    private int failedTests = 0;
    
    void Start()
    {
        if (autoRunOnStart)
        {
            StartCoroutine(RunAllTests());
        }
    }
    
    /// <summary>
    /// 运行所有测试
    /// </summary>
    public IEnumerator RunAllTests()
    {
        Debug.Log("========== 开始运行时测试 ==========");
        Debug.Log($"测试时间: {System.DateTime.Now}");
        
        totalTests = 0;
        passedTests = 0;
        failedTests = 0;
        
        // 等待所有管理器初始化
        yield return new WaitForSeconds(1f);
        
        // 1. 卡牌系统测试
        Debug.Log("\n[测试套件 1] 卡牌系统");
        yield return TestCardLoading();
        yield return TestCardUniqueness();
        
        // 2. 战斗系统测试
        Debug.Log("\n[测试套件 2] 战斗系统");
        yield return TestBattleInitialization();
        yield return TestBattleFlow();
        yield return TestCardPlaying();
        yield return TestBattleVictoryCondition();
        
        // 3. 融合系统测试
        Debug.Log("\n[测试套件 3] 融合系统");
        yield return TestFusionRecipe();
        yield return TestRandomFusion();
        yield return TestFusionRegistration();
        
        // 4. 地图系统测试
        Debug.Log("\n[测试套件 4] 地图系统");
        yield return TestMapGeneration();
        yield return TestMapNodeDistribution();
        
        // 5. 商店系统测试
        Debug.Log("\n[测试套件 5] 商店系统");
        yield return TestShopNoDuplicates();
        
        // 6. 存档系统测试
        Debug.Log("\n[测试套件 6] 存档系统");
        yield return TestSaveLoad();
        
        // 7. 暂停系统测试
        Debug.Log("\n[测试套件 7] 暂停系统");
        yield return TestPauseResume();
        
        // 测试总结
        Debug.Log("\n========== 测试总结 ==========");
        Debug.Log($"总测试数: {totalTests}");
        Debug.Log($"<color=green>通过: {passedTests}</color>");
        Debug.Log($"<color=red>失败: {failedTests}</color>");
        Debug.Log($"通过率: {(totalTests > 0 ? (passedTests * 100f / totalTests).ToString("F1") : "0")}%");
        Debug.Log("================================\n");
        
        if (failedTests > 0 && pauseOnFailure)
        {
            Debug.LogWarning("检测到测试失败，游戏已暂停。请检查错误信息。");
            Debug.Break();
        }
    }
    
    #region 测试工具方法
    
    /// <summary>
    /// 断言条件为真
    /// </summary>
    private void AssertTrue(bool condition, string testName, string message)
    {
        totalTests++;
        if (condition)
        {
            passedTests++;
            Debug.Log($"<color=green>✓ PASS</color> {testName}");
        }
        else
        {
            failedTests++;
            Debug.LogError($"<color=red>✗ FAIL</color> {testName}: {message}");
        }
    }
    
    /// <summary>
    /// 断言两个值相等
    /// </summary>
    private void AssertEqual<T>(T expected, T actual, string testName)
    {
        totalTests++;
        if (EqualityComparer<T>.Default.Equals(expected, actual))
        {
            passedTests++;
            Debug.Log($"<color=green>✓ PASS</color> {testName}");
        }
        else
        {
            failedTests++;
            Debug.LogError($"<color=red>✗ FAIL</color> {testName}: 期望 {expected}, 实际 {actual}");
        }
    }
    
    /// <summary>
    /// 断言值不为空
    /// </summary>
    private void AssertNotNull(object obj, string testName)
    {
        totalTests++;
        if (obj != null)
        {
            passedTests++;
            Debug.Log($"<color=green>✓ PASS</color> {testName}");
        }
        else
        {
            failedTests++;
            Debug.LogError($"<color=red>✗ FAIL</color> {testName}: 对象为null");
        }
    }
    
    #endregion
    
    #region 1. 卡牌系统测试
    
    /// <summary>
    /// 测试：卡牌加载
    /// </summary>
    private IEnumerator TestCardLoading()
    {
        if (CardManager.Instance == null)
        {
            Debug.LogError("CardManager未初始化，跳过卡牌测试");
            yield break;
        }
        
        var allCards = CardManager.Instance.GetAllCards();
        
        AssertTrue(allCards != null, "卡牌加载 - 列表不为空", "GetAllCards() 返回了 null");
        AssertTrue(allCards.Count >= 30, "卡牌加载 - 数量验证", $"卡牌数量不足，期望≥30，实际{allCards.Count}");
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：卡牌ID唯一性
    /// </summary>
    private IEnumerator TestCardUniqueness()
    {
        if (CardManager.Instance == null) yield break;
        
        var allCards = CardManager.Instance.GetAllCards();
        var ids = new HashSet<string>();
        bool hasDuplicate = false;
        string duplicateId = "";
        
        foreach (var card in allCards)
        {
            if (ids.Contains(card.cardId))
            {
                hasDuplicate = true;
                duplicateId = card.cardId;
                break;
            }
            ids.Add(card.cardId);
        }
        
        AssertTrue(!hasDuplicate, "卡牌唯一性 - ID不重复", $"发现重复ID: {duplicateId}");
        
        yield return null;
    }
    
    #endregion
    
    #region 2. 战斗系统测试
    
    /// <summary>
    /// 测试：战斗初始化
    /// </summary>
    private IEnumerator TestBattleInitialization()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogError("BattleManager未初始化，跳过战斗测试");
            yield break;
        }
        
        AssertNotNull(BattleManager.Instance, "战斗初始化 - 管理器存在");
        AssertEqual(3, BattleManager.Instance.maxEnergy, "战斗初始化 - 最大能量");
        AssertEqual(5, BattleManager.Instance.cardsPerTurn, "战斗初始化 - 每回合抽牌数");
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：战斗流程
    /// </summary>
    private IEnumerator TestBattleFlow()
    {
        if (BattleManager.Instance == null || CardManager.Instance == null) yield break;
        
        // 准备测试数据
        var testDeck = new List<CardData>();
        var allCards = CardManager.Instance.GetAllCards();
        for (int i = 0; i < 10 && i < allCards.Count; i++)
        {
            testDeck.Add(allCards[i]);
        }
        
        var testEnemy = new GameObject("TestEnemy").AddComponent<Enemy>();
        testEnemy.maxHP = 50;
        testEnemy.currentHP = 50;
        testEnemy.minDamage = 5;
        testEnemy.maxDamage = 10;
        
        // 开始战斗
        BattleManager.Instance.StartBattle(testDeck, new List<Enemy> { testEnemy });
        
        yield return new WaitForSeconds(0.5f);
        
        AssertEqual(3, BattleManager.Instance.currentEnergy, "战斗流程 - 初始能量");
        AssertEqual(5, BattleManager.Instance.GetHand().Count, "战斗流程 - 初始手牌数");
        AssertEqual(BattleState.PlayerTurn, BattleManager.Instance.currentState, "战斗流程 - 初始状态");
        
        // 清理
        Destroy(testEnemy.gameObject);
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：卡牌打出
    /// </summary>
    private IEnumerator TestCardPlaying()
    {
        if (BattleManager.Instance == null || CardManager.Instance == null) yield break;
        
        // 准备测试数据
        var attackCard = CardManager.Instance.GetAllCards().Find(c => c.cardType == CardType.Attack && c.cost <= 1);
        if (attackCard == null)
        {
            Debug.LogWarning("未找到费用≤1的攻击牌，跳过测试");
            yield break;
        }
        
        var testEnemy = new GameObject("TestEnemy").AddComponent<Enemy>();
        testEnemy.maxHP = 50;
        testEnemy.currentHP = 50;
        
        var testDeck = new List<CardData> { attackCard };
        BattleManager.Instance.StartBattle(testDeck, new List<Enemy> { testEnemy });
        
        yield return new WaitForSeconds(0.5f);
        
        // 尝试打出攻击牌
        int enemyHpBefore = testEnemy.currentHP;
        bool success = BattleManager.Instance.PlayCard(attackCard, testEnemy);
        
        AssertTrue(success, "卡牌打出 - 操作成功", "PlayCard 返回 false");
        AssertTrue(testEnemy.currentHP < enemyHpBefore, "卡牌打出 - 敌人受伤", "敌人血量未减少");
        
        // 清理
        Destroy(testEnemy.gameObject);
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：战斗胜利条件
    /// </summary>
    private IEnumerator TestBattleVictoryCondition()
    {
        if (BattleManager.Instance == null || CardManager.Instance == null) yield break;
        
        var testEnemy = new GameObject("TestEnemy").AddComponent<Enemy>();
        testEnemy.maxHP = 10;
        testEnemy.currentHP = 10;
        
        var testDeck = CardManager.Instance.GetAllCards().Take(5).ToList();
        BattleManager.Instance.StartBattle(testDeck, new List<Enemy> { testEnemy });
        
        yield return new WaitForSeconds(0.5f);
        
        // 击杀敌人
        testEnemy.TakeDamage(10);
        
        yield return new WaitForSeconds(0.2f);
        
        AssertTrue(testEnemy.currentHP <= 0, "胜利条件 - 敌人死亡", "敌人血量未归零");
        AssertEqual(BattleState.BattleEnd, BattleManager.Instance.currentState, "胜利条件 - 战斗结束");
        
        // 清理
        Destroy(testEnemy.gameObject);
        
        yield return null;
    }
    
    #endregion
    
    #region 3. 融合系统测试
    
    /// <summary>
    /// 测试：预设融合配方
    /// </summary>
    private IEnumerator TestFusionRecipe()
    {
        if (FusionManager.Instance == null || CardManager.Instance == null)
        {
            Debug.LogError("FusionManager或CardManager未初始化，跳过融合测试");
            yield break;
        }
        
        var card1 = CardManager.Instance.GetCard("ATK_001");
        var card2 = CardManager.Instance.GetCard("ATK_002");
        
        if (card1 == null || card2 == null)
        {
            Debug.LogWarning("未找到ATK_001或ATK_002，跳过配方测试");
            yield break;
        }
        
        FusionManager.Instance.currentFusionPoints = 3;
        var result = FusionManager.Instance.FuseCards(card1, card2);
        
        AssertNotNull(result, "融合配方 - 结果不为空");
        AssertEqual("FUSION_001", result?.cardId, "融合配方 - 结果ID正确");
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：随机融合
    /// </summary>
    private IEnumerator TestRandomFusion()
    {
        if (FusionManager.Instance == null || CardManager.Instance == null) yield break;
        
        var allCards = CardManager.Instance.GetAllCards();
        if (allCards.Count < 2)
        {
            Debug.LogWarning("卡牌数量不足，跳过随机融合测试");
            yield break;
        }
        
        var card1 = allCards[0];
        var card2 = allCards[1];
        
        // 确保这对卡没有预设配方
        if (FusionManager.Instance.HasRecipe(card1.cardId, card2.cardId))
        {
            card2 = allCards.Find(c => !FusionManager.Instance.HasRecipe(card1.cardId, c.cardId));
        }
        
        FusionManager.Instance.currentFusionPoints = 3;
        var result = FusionManager.Instance.FuseCards(card1, card2);
        
        AssertNotNull(result, "随机融合 - 结果不为空");
        AssertTrue(result?.cardId.StartsWith("FUSED_") ?? false, "随机融合 - ID前缀正确", "融合卡ID应以FUSED_开头");
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：融合卡注册
    /// </summary>
    private IEnumerator TestFusionRegistration()
    {
        if (FusionManager.Instance == null || CardManager.Instance == null) yield break;
        
        var allCards = CardManager.Instance.GetAllCards();
        if (allCards.Count < 2) yield break;
        
        FusionManager.Instance.currentFusionPoints = 3;
        var result = FusionManager.Instance.FuseCards(allCards[0], allCards[1]);
        
        if (result == null) yield break;
        
        // 验证融合卡已注册到CardManager
        var registered = CardManager.Instance.GetCard(result.cardId);
        AssertNotNull(registered, "融合卡注册 - 可通过ID获取");
        
        yield return null;
    }
    
    #endregion
    
    #region 4. 地图系统测试
    
    /// <summary>
    /// 测试：地图生成
    /// </summary>
    private IEnumerator TestMapGeneration()
    {
        if (MapManager.Instance == null)
        {
            Debug.LogError("MapManager未初始化，跳过地图测试");
            yield break;
        }
        
        MapManager.Instance.GenerateMap();
        yield return new WaitForSeconds(0.5f);
        
        var nodes = MapManager.Instance.GetAllNodes();
        
        AssertNotNull(nodes, "地图生成 - 节点列表不为空");
        AssertEqual(45, nodes?.Count ?? 0, "地图生成 - 节点数量");
        
        // 验证Boss节点
        var bossNode = nodes?.Find(n => n.nodeType == MapNodeType.Boss);
        AssertNotNull(bossNode, "地图生成 - Boss节点存在");
        AssertEqual(4, bossNode?.row ?? -1, "地图生成 - Boss在第5层");
        
        yield return null;
    }
    
    /// <summary>
    /// 测试：节点分布
    /// </summary>
    private IEnumerator TestMapNodeDistribution()
    {
        if (MapManager.Instance == null) yield break;
        
        MapManager.Instance.GenerateMap();
        yield return new WaitForSeconds(0.5f);
        
        var nodes = MapManager.Instance.GetAllNodes();
        if (nodes == null) yield break;
        
        int battleCount = nodes.Count(n => n.nodeType == MapNodeType.Battle);
        int treasureCount = nodes.Count(n => n.nodeType == MapNodeType.Treasure);
        int shopCount = nodes.Count(n => n.nodeType == MapNodeType.Shop);
        
        AssertTrue(battleCount > 0, "节点分布 - 战斗节点存在", $"战斗节点数: {battleCount}");
        AssertTrue(treasureCount > 0, "节点分布 - 宝箱节点存在", $"宝箱节点数: {treasureCount}");
        AssertTrue(shopCount > 0, "节点分布 - 商店节点存在", $"商店节点数: {shopCount}");
        
        yield return null;
    }
    
    #endregion
    
    #region 5. 商店系统测试
    
    /// <summary>
    /// 测试：商店商品不重复
    /// </summary>
    private IEnumerator TestShopNoDuplicates()
    {
        // 创建临时商店
        var shopObj = new GameObject("TestShop");
        var shopUI = shopObj.AddComponent<ShopUI>();
        shopUI.Init();
        
        yield return new WaitForSeconds(0.3f);
        
        if (ShopUI.Instance == null)
        {
            Debug.LogWarning("ShopUI未初始化，跳过商店测试");
            Destroy(shopObj);
            yield break;
        }
        
        // 检查卡牌不重复
        var cards = ShopUI.Instance.GetCurrentCards();
        var cardIds = new HashSet<string>();
        bool hasCardDuplicate = false;
        string duplicateCardId = "";
        
        foreach (var card in cards)
        {
            if (cardIds.Contains(card.cardId))
            {
                hasCardDuplicate = true;
                duplicateCardId = card.cardId;
                break;
            }
            cardIds.Add(card.cardId);
        }
        
        AssertTrue(!hasCardDuplicate, "商店卡牌 - 无重复", $"商店存在重复卡牌: {duplicateCardId}");
        
        // 检查遗物不重复
        var relics = ShopUI.Instance.GetCurrentRelics();
        var relicIds = new HashSet<string>();
        bool hasRelicDuplicate = false;
        string duplicateRelicId = "";
        
        foreach (var relicId in relics)
        {
            if (relicIds.Contains(relicId))
            {
                hasRelicDuplicate = true;
                duplicateRelicId = relicId;
                break;
            }
            relicIds.Add(relicId);
        }
        
        AssertTrue(!hasRelicDuplicate, "商店遗物 - 无重复", $"商店存在重复遗物: {duplicateRelicId}");
        
        // 清理
        Destroy(shopObj);
        
        yield return null;
    }
    
    #endregion
    
    #region 6. 存档系统测试
    
    /// <summary>
    /// 测试：存档保存与加载
    /// </summary>
    private IEnumerator TestSaveLoad()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogWarning("GameManager未初始化，跳过存档测试");
            yield break;
        }
        
        // 保存当前数据
        int originalGold = GameManager.Instance.currentGold;
        
        // 修改数据
        GameManager.Instance.currentGold = 12345;
        GameManager.Instance.currentRun = 99;
        
        // 保存游戏
        GameManager.Instance.SaveGame();
        yield return new WaitForSeconds(0.3f);
        
        // 验证存档文件存在
        AssertTrue(GameManager.Instance.HasSaveFile(), "存档系统 - 存档文件已创建", "SaveGame() 未创建文件");
        
        // 修改数据（准备测试加载）
        GameManager.Instance.currentGold = 0;
        GameManager.Instance.currentRun = 1;
        
        // 加载游戏
        bool loadSuccess = GameManager.Instance.LoadGame();
        yield return new WaitForSeconds(0.3f);
        
        AssertTrue(loadSuccess, "存档系统 - 加载成功", "LoadGame() 返回 false");
        AssertEqual(12345, GameManager.Instance.currentGold, "存档系统 - 金币保存/加载");
        AssertEqual(99, GameManager.Instance.currentRun, "存档系统 - 回合数保存/加载");
        
        // 恢复原值
        GameManager.Instance.currentGold = originalGold;
        GameManager.Instance.currentRun = 1;
        
        // 删除测试存档
        GameManager.Instance.DeleteSave();
        
        yield return null;
    }
    
    #endregion
    
    #region 7. 暂停系统测试
    
    /// <summary>
    /// 测试：暂停与恢复
    /// </summary>
    private IEnumerator TestPauseResume()
    {
        // 创建暂停菜单实例
        if (PauseMenuUI.Instance == null)
        {
            var pauseObj = new GameObject("PauseMenuUI");
            pauseObj.AddComponent<PauseMenuUI>();
            yield return new WaitForSeconds(0.3f);
        }
        
        if (PauseMenuUI.Instance == null)
        {
            Debug.LogWarning("PauseMenuUI未初始化，跳过暂停测试");
            yield break;
        }
        
        float originalTimeScale = Time.timeScale;
        
        // 触发暂停
        PauseMenuUI.Instance.Pause();
        yield return new WaitForSecondsRealtime(0.2f);
        
        AssertEqual(0f, Time.timeScale, "暂停系统 - 时间停止");
        
        // 恢复
        PauseMenuUI.Instance.Resume();
        yield return new WaitForSecondsRealtime(0.2f);
        
        AssertEqual(originalTimeScale, Time.timeScale, "暂停系统 - 时间恢复");
        
        yield return null;
    }
    
    #endregion
}
