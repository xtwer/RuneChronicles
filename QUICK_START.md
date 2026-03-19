# ⚡ 快速开始指南

## 📋 你需要知道的

### 项目是什么？
一款**卡牌 Roguelike** 游戏，核心特色是**符文融合系统**（任意2张卡牌可融合产生新效果）。

### 目标是什么？
**12周后在 Steam 上线 Early Access**，定价 $6.99。

### 现在在哪个阶段？
**前期策划完成**，准备开始编码（Week 1）。

---

## 📁 重要文档位置

```
~/Desktop/RuneChronicles/
├── PROJECT_INDEX.md        ← 📍 从这里开始！
├── docs/
│   ├── README.md           ← 项目概述
│   ├── GDD.md              ← 游戏设计（重要！）
│   ├── TDD.md              ← 技术设计
│   └── ROADMAP.md          ← 12周计划
```

---

## 🎯 下周任务（Week 1）

### Day 1-2：Unity 项目搭建
- [ ] 创建 Unity 2022.3 LTS 项目
- [ ] 导入必要的 Package
- [ ] 建立文件夹结构

### Day 3-4：核心框架
- [ ] GameManager（单例）
- [ ] BattleManager（战斗管理）
- [ ] Card/Enemy/Player 类

### Day 5-7：简单UI
- [ ] 战斗界面
- [ ] 卡牌显示
- [ ] 生命值显示

**验收标准：** Unity 项目可运行，基础框架完整。

---

## 💡 核心设计要点

### 符文融合机制
```
火焰弹（8伤害）+ 冰霜箭（6伤害+冻结）
= 冰火符文（12伤害+灼烧2回合）
```

**融合点系统：**
- 初始：0点
- 打出卡牌 +1点
- 融合消耗 3点
- 上限：10点

### 战斗流程
```
回合开始 → 抽5张卡 → 恢复3点能量 →
玩家打牌 → 结束回合 → 敌人行动 → 循环
```

### 地图结构
```
起点 → 3层（各5个节点）→ BOSS
节点类型：战斗、精英、商店、宝箱、事件、篝火
```

---

## 🛠️ 技术要点

### 架构模式
- **单例模式：** GameManager, AudioManager
- **观察者模式：** 事件系统（HP变化、卡牌使用）
- **工厂模式：** CardFactory, EnemyFactory
- **状态机：** 战斗状态管理

### 数据格式
所有配置使用 **JSON**：
- `cards.json` - 卡牌数据
- `enemies.json` - 敌人数据
- `fusion_recipes.json` - 融合配方
- `save.json` - 存档数据

### 核心类结构
```csharp
// 卡牌
public class Card {
    public string id;
    public string name;
    public CardType type;
    public int energyCost;
    public List<CardEffect> effects;
}

// 敌人
public class Enemy {
    public int currentHP;
    public List<EnemyAction> actionPattern;
    public void TakeTurn();
}

// 战斗管理
public class BattleManager {
    public void StartBattle();
    public void PlayCard(Card card, Target target);
    public void FuseCards(Card card1, Card card2);
}
```

---

## 📊 关键数值

### 玩家
- 初始生命：70
- 初始能量：3
- 每回合抽牌：5张

### 卡牌
- 普通卡（40张）- 能量1-2
- 罕见卡（25张）- 能量2-3
- 史诗卡（12张）- 能量3-4
- 传说卡（3张）- 能量4+

### 敌人
- 普通敌人：30-60 HP
- 精英敌人：100-150 HP
- BOSS：250-400 HP

---

## ⚠️ 重要原则

### 开发原则
1. **先做核心玩法**，后做美术和音效
2. **数据驱动设计**，所有内容配置化
3. **快速迭代**，每周可玩版本
4. **及时测试**，避免积累Bug

### 设计原则
1. **简单清晰**，效果一目了然
2. **数值平衡**，能量与效果匹配
3. **协同效应**，卡牌之间有配合
4. **融合潜力**，考虑融合后的效果

---

## 📞 需要帮助？

### 技术问题
→ 查看 `docs/TDD.md`（技术设计文档）

### 玩法问题
→ 查看 `docs/GDD.md`（游戏设计文档）

### 计划问题
→ 查看 `docs/ROADMAP.md`（开发路线图）

### AI 协作
→ 直接问李嘉图（AI助手）

---

## 🎉 准备好了吗？

**下一步：**
1. 阅读 `PROJECT_INDEX.md`
2. 详细阅读 `docs/GDD.md`（游戏设计）
3. 开始 Week 1 开发！

**让我们开始制作游戏！** 🚀
