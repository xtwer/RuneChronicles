# 验收标准 (Acceptance Criteria)

> **目标：** 定义"项目完成"的可验证标准

---

## 1. 卡牌系统 (Week 1)

### 1.1 卡牌数据完整性
- [ ] 至少 30 张基础卡牌（攻击/技能/能力各 10 张）
- [ ] 每张卡牌有唯一 ID（格式：`ATK_001`, `SKL_001`, `PWR_001`）
- [ ] 每张卡牌有完整属性：名称、类型、稀有度、费用、数值、描述
- [ ] 所有卡牌可通过 `CardManager.GetCard(id)` 正确加载

### 1.2 卡牌加载验证
**自动化测试：**
```csharp
// RuntimeTests.cs
[Test] void TestCardLoading()
{
    // 加载所有卡牌
    var allCards = CardManager.Instance.GetAllCards();
    Assert.IsTrue(allCards.Count >= 30, "卡牌数量不足30张");
    
    // 验证ID唯一性
    var ids = new HashSet<string>();
    foreach (var card in allCards)
    {
        Assert.IsFalse(ids.Contains(card.cardId), $"重复ID: {card.cardId}");
        ids.Add(card.cardId);
    }
}
```

---

## 2. 战斗系统 (Week 2)

### 2.1 回合流程完整性
- [ ] **玩家回合流程：** 开始 → 恢复能量 → 抽牌 → 出牌 → 结束
- [ ] **敌人回合流程：** 开始 → 执行行动 → 结算伤害 → 结束
- [ ] **战斗结束条件：** 敌人全灭（胜利）或玩家死亡（失败）

### 2.2 战斗核心逻辑验证
**自动化测试：**
```csharp
[Test] void TestBattleFlow()
{
    // 初始化战斗
    var testDeck = new List<CardData> { /* 测试卡组 */ };
    var testEnemy = new Enemy { maxHP = 50, currentHP = 50 };
    BattleManager.Instance.StartBattle(testDeck, new List<Enemy> { testEnemy });
    
    // 验证初始状态
    Assert.AreEqual(3, BattleManager.Instance.currentEnergy, "初始能量应为3");
    Assert.AreEqual(5, BattleManager.Instance.GetHand().Count, "初始手牌应为5张");
    
    // 打出攻击牌
    var attackCard = testDeck.Find(c => c.cardType == CardType.Attack);
    bool success = BattleManager.Instance.PlayCard(attackCard, testEnemy);
    Assert.IsTrue(success, "应能成功打出攻击牌");
    Assert.Less(testEnemy.currentHP, 50, "敌人应受到伤害");
}

[Test] void TestBattleVictory()
{
    var testEnemy = new Enemy { maxHP = 10, currentHP = 10 };
    BattleManager.Instance.StartBattle(testDeck, new List<Enemy> { testEnemy });
    
    // 击杀敌人
    testEnemy.TakeDamage(10);
    Assert.IsTrue(testEnemy.currentHP <= 0, "敌人应死亡");
    
    // 战斗应自动结束
    Assert.AreEqual(BattleState.BattleEnd, BattleManager.Instance.currentState);
}
```

---

## 3. 融合系统 (Week 3)

### 3.1 融合功能完整性
- [ ] 10 种预设融合配方（已在 `FusionManager` 中定义）
- [ ] 随机融合机制（无配方时生成随机融合卡）
- [ ] 融合点消耗与恢复机制
- [ ] 融合结果可加入牌库并在战斗中使用

### 3.2 融合系统验证
**自动化测试：**
```csharp
[Test] void TestFusionRecipe()
{
    // 测试预设配方：烈火斩 + 冰霜打击 = 冰火交融
    var card1 = CardManager.Instance.GetCard("ATK_001");
    var card2 = CardManager.Instance.GetCard("ATK_002");
    
    FusionManager.Instance.currentFusionPoints = 3;
    var result = FusionManager.Instance.FuseCards(card1, card2);
    
    Assert.IsNotNull(result, "融合应成功");
    Assert.AreEqual("FUSION_001", result.cardId, "应生成预设融合卡");
}

[Test] void TestRandomFusion()
{
    var card1 = CardManager.Instance.GetCard("ATK_001");
    var card2 = CardManager.Instance.GetCard("ATK_004"); // 无预设配方
    
    FusionManager.Instance.currentFusionPoints = 3;
    var result = FusionManager.Instance.FuseCards(card1, card2);
    
    Assert.IsNotNull(result, "随机融合应成功");
    Assert.IsTrue(result.cardId.StartsWith("FUSED_"), "应生成随机融合卡");
    
    // 验证融合卡已注册
    var registered = CardManager.Instance.GetCard(result.cardId);
    Assert.IsNotNull(registered, "融合卡应自动注册到CardManager");
}
```

---

## 4. 地图系统 (Week 4)

### 4.1 地图生成稳定性
- [ ] 每次生成 15 层节点（3 列 × 5 行）
- [ ] 节点类型分布合理（战斗 > 奖励 > 商店 > Boss）
- [ ] 第 15 层必定为 Boss 节点
- [ ] 节点间连接路径清晰且可通行

### 4.2 地图系统验证
**自动化测试：**
```csharp
[Test] void TestMapGeneration()
{
    MapManager.Instance.GenerateMap();
    var nodes = MapManager.Instance.GetAllNodes();
    
    Assert.AreEqual(15, nodes.Count, "应生成15个节点");
    
    // 验证Boss节点
    var bossNode = nodes.Find(n => n.nodeType == NodeType.Boss);
    Assert.IsNotNull(bossNode, "应有Boss节点");
    Assert.AreEqual(4, bossNode.row, "Boss应在第5层（row=4）");
}

[Test] void TestNodeDistribution()
{
    MapManager.Instance.GenerateMap();
    var nodes = MapManager.Instance.GetAllNodes();
    
    int battleCount = nodes.Count(n => n.nodeType == NodeType.Battle);
    int rewardCount = nodes.Count(n => n.nodeType == NodeType.Reward);
    int shopCount = nodes.Count(n => n.nodeType == NodeType.Shop);
    
    Assert.Greater(battleCount, rewardCount, "战斗节点应最多");
    Assert.Greater(rewardCount, shopCount, "奖励节点应多于商店");
}
```

---

## 5. 商店与奖励系统

### 5.1 商店功能完整性
- [ ] 商店刷新时不重复展示相同商品
- [ ] 购买逻辑正确（金币检查、物品获取、库存扣减）
- [ ] 商店关闭后可正常返回地图

### 5.2 奖励系统完整性
- [ ] 奖励选项不重复（3选1机制）
- [ ] 选择后正确加入牌库或背包
- [ ] 可拒绝奖励并继续游戏

### 5.3 商店验证
**自动化测试：**
```csharp
[Test] void TestShopNoDuplicates()
{
    ShopUI.Instance.RefreshShop();
    var items = ShopUI.Instance.GetCurrentItems();
    
    var ids = new HashSet<string>();
    foreach (var item in items)
    {
        Assert.IsFalse(ids.Contains(item.id), $"商店重复商品: {item.id}");
        ids.Add(item.id);
    }
}

[Test] void TestPurchaseFlow()
{
    GameManager.Instance.currentGold = 100;
    var item = new ShopItem { id = "CARD_TEST", price = 50 };
    
    bool success = ShopUI.Instance.PurchaseItem(item);
    Assert.IsTrue(success, "购买应成功");
    Assert.AreEqual(50, GameManager.Instance.currentGold, "金币应扣减");
}
```

---

## 6. 游戏外壳与流程

### 6.1 完整游戏流程
- [ ] **主菜单 → 角色选择 → 地图 → 战斗/商店/奖励 → 下一层 → Boss → 胜利/失败**
- [ ] 每个场景有明确的入口和出口（不出现死路）

### 6.2 设置与暂停系统
- [ ] ESC 键可打开暂停菜单
- [ ] 暂停菜单包含：继续游戏、设置、返回主菜单、退出游戏
- [ ] 设置界面可调整音量并持久化保存
- [ ] 退出游戏前有确认弹窗

### 6.3 游戏外壳验证
**手动测试清单：**
```
1. 启动游戏 → 显示主菜单
2. 点击"开始游戏" → 进入角色选择
3. 选择角色 → 进入地图
4. 点击战斗节点 → 进入战斗
5. 战斗胜利 → 显示奖励界面
6. 选择奖励 → 返回地图
7. 按ESC → 显示暂停菜单
8. 点击"设置" → 可调整音量
9. 返回游戏 → 音量设置已保存
10. 击败Boss → 显示胜利界面
11. 点击"返回主菜单" → 回到主菜单
```

---

## 7. 性能与稳定性

### 7.1 性能要求
- [ ] 战斗场景帧率 ≥ 30 FPS（中等配置）
- [ ] 场景切换时间 ≤ 2 秒
- [ ] 无明显内存泄漏（长时间运行后内存稳定）

### 7.2 异常处理
- [ ] 加载失败时有友好提示（不直接崩溃）
- [ ] 战斗中异常操作不导致状态错乱
- [ ] 存档损坏时可正常重置

---

## 8. 文档完整性

- [ ] README.md：项目介绍、如何运行、控制说明
- [ ] GDD.md：游戏设计文档
- [ ] ROADMAP.md：开发路线图与里程碑
- [ ] ACCEPTANCE_CRITERIA.md：本文档
- [ ] 代码注释覆盖率 ≥ 60%

---

## ✅ 验收流程

### 自动化测试
```bash
# Unity Test Runner
1. 打开 Unity Editor
2. Window → General → Test Runner
3. 运行 "PlayMode Tests"
4. 所有测试应通过（绿色）
```

### 手动验收
```
1. 按照"游戏外壳验证"清单逐项测试
2. 记录发现的问题到 Issues 列表
3. 修复后重新验收
```

### 完成标准
- **自动化测试通过率 ≥ 90%**
- **手动验收清单全部通过**
- **无阻塞性 Bug（P0/P1）**
- **文档完整且与代码同步**

---

**最后更新：** 2026-03-24  
**维护者：** 李嘉图
