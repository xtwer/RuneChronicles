# Unity 测试指南

**最后更新**: 2026-03-23 14:55

---

## 🚀 快速开始

### 1. 在Unity中打开项目

```bash
cd ~/Desktop/RuneChronicles/RuneChronicles
# Unity Hub: Add Project -> 选择 RuneChronicles 文件夹
```

### 2. 检查项目设置

- Unity版本: **2022.3.15 LTS** 或更高
- 平台: PC/Mac/Linux
- 分辨率: 1920x1080

---

## 🧪 运行单元测试

### 方法1: Unity Test Runner

1. 打开 `Window > General > Test Runner`
2. 切换到 `PlayMode` 标签
3. 点击 `Run All` 运行所有测试
4. 查看测试结果

**预期结果**: ✅ 所有测试通过

### 方法2: 命令行运行

```bash
# 在Unity中运行测试
/Applications/Unity/Hub/Editor/2022.3.15f1/Unity.app/Contents/MacOS/Unity \
  -runTests \
  -batchmode \
  -projectPath ~/Desktop/RuneChronicles/RuneChronicles \
  -testResults ~/Desktop/RuneChronicles/TestResults.xml \
  -testPlatform PlayMode
```

---

## ✅ 手动功能验证

### 步骤1: 创建测试场景

1. 创建新场景: `File > New Scene`
2. 添加空GameObject: `GameObject > Create Empty`
3. 重命名为 `GameManager`
4. 添加组件: `Add Component > GameManager`

### 步骤2: 运行游戏

1. 点击 Play 按钮
2. 检查Console输出

**预期输出**:
```
[GameManager] 游戏初始化
[GameManager] 所有管理器已加载
[CardManager] 成功加载 X 张卡牌
[FusionManager] 加载 10 个融合配方
[RelicManager] 加载 20 个遗物
[MapManager] 生成地图：15层，每层3个节点
```

### 步骤3: 测试DEBUG接口

在Unity Console中运行:

```csharp
// 快速开始游戏（法师）
GameManager.Instance.DEBUG_QuickStartMage();

// 快速开始游戏（战士）
GameManager.Instance.DEBUG_QuickStartWarrior();

// 解锁所有卡牌
GameManager.Instance.DEBUG_UnlockAllCards();

// 解锁所有遗物
GameManager.Instance.DEBUG_UnlockAllRelics();
```

---

## 🔍 关键系统验证

### CardManager 测试

```csharp
// 在Unity Console中运行
CardManager.Instance.DEBUG_PrintAllCards();
```

**预期**: 打印所有卡牌（120张）

### FusionManager 测试

```csharp
// 测试融合
var card1 = CardManager.Instance.GetCard("ATK_001");
var card2 = CardManager.Instance.GetCard("ATK_002");
var result = FusionManager.Instance.FuseCards(card1, card2);
```

**预期**: 成功融合卡牌

### MapManager 测试

```csharp
// 打印地图
MapManager.Instance.DEBUG_PrintMap();
```

**预期**: 打印15层地图结构

---

## 🎮 完整游戏流程测试

### 测试步骤

1. **开始新游戏**
   ```csharp
   GameManager.Instance.StartNewRun(CharacterClass.Mage);
   ```

2. **检查初始状态**
   - 玩家HP: 80（法师）或 100（战士）
   - 初始卡组: 10张
   - 初始遗物: 1个
   - 融合点: 3

3. **模拟战斗**
   ```csharp
   // 创建敌人
   var enemy = new GameObject("Enemy").AddComponent<Enemy>();
   enemy.enemyId = "ENM_001";
   enemy.maxHP = 50;
   
   // 开始战斗
   var playerDeck = CardManager.Instance.playerDeck;
   var enemies = new List<Enemy> { enemy };
   BattleManager.Instance.StartBattle(playerDeck, enemies);
   ```

4. **测试融合**
   ```csharp
   var card1 = CardManager.Instance.GetCard("ATK_001");
   var card2 = CardManager.Instance.GetCard("ATK_002");
   FusionManager.Instance.FuseCards(card1, card2);
   ```

5. **测试地图导航**
   ```csharp
   var nodes = MapManager.Instance.GetCurrentFloorNodes();
   MapManager.Instance.EnterNode(nodes[0]);
   ```

---

## 📊 性能测试

### 帧率测试

1. 在Game视图启用 `Stats`
2. 运行游戏
3. 检查FPS

**目标**: ≥60 FPS

### 内存测试

1. 打开 `Window > Analysis > Profiler`
2. 运行游戏10分钟
3. 检查内存使用

**目标**: <500MB

---

## 🐛 常见问题

### Q: 找不到卡牌数据
**A**: 检查 `Assets/Data/` 下的JSON文件是否存在

### Q: 脚本编译错误
**A**: 确保使用 Unity 2022.3 LTS

### Q: 测试失败
**A**: 检查Test Runner中的错误信息

---

## ✅ 验收检查清单

- [ ] Unity项目正常打开
- [ ] 无编译错误
- [ ] 单元测试全部通过（50+个）
- [ ] 所有Manager正常初始化
- [ ] 120张卡牌加载成功
- [ ] 21种敌人加载成功
- [ ] 20个遗物加载成功
- [ ] 地图生成正常（15层）
- [ ] 融合系统可用
- [ ] 战斗系统可用
- [ ] FPS ≥ 60
- [ ] 内存 < 500MB
- [ ] DEBUG接口可用

---

## 🎉 测试完成

如果所有检查项都通过，恭喜！**项目验证成功** ✅

---

**最后更新**: 2026-03-23 14:55  
**负责人**: 李嘉图
