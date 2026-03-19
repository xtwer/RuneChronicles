# 技术方案文档（Technical Solution / TSD）
# 符文编年史 Rune Chronicles

**版本：** v0.1  
**状态：** Draft（开发基线）  
**最后更新：** 2026-03-18  
**约束：** 后续开发如实现方案变更，必须同步修订本文件（见“变更流程”）

---

## 0. 文档目标与约束

### 0.1 目标
- 作为后续开发的**唯一技术基线**（Single Source of Truth）。
- 降低返工：所有关键技术决策在此落地，可追踪。
- 提升质量：保证可验证性、可观测性、可回归性。

### 0.2 非目标
- 不替代 GDD（玩法设计）、不展开所有内容配置细节（卡牌/敌人表已在 design/ 内）。

### 0.3 变更流程（强制）
- 任意技术方案变更（例如：Unity 版本、存档格式、事件系统、融合算法实现）必须：
  1) 在 PR/提交信息中标注：`[TSD]`  
  2) 同步更新本文件对应章节与“变更记录”。

---

## 1. 总体架构

### 1.1 架构分层
- **表现层（UI）**：战斗 UI、地图 UI、卡牌 UI、提示/日志。
- **领域层（Domain）**：战斗规则、卡牌效果、状态（Buff/Debuff）、融合配方、敌人 AI。
- **数据层（Data）**：JSON 配置加载、运行态数据、存档序列化。
- **基础设施层（Infra）**：日志、埋点、资源加载、对象池、输入、平台适配。

### 1.2 运行态模块（建议划分）
- `GameManager`：全局状态机（菜单/地图/战斗/结算）。
- `BattleManager`：战斗生命周期 + 回合切换。
- `CardSystem`：牌堆/手牌/弃牌堆 + 出牌。
- `EffectSystem`：卡牌效果执行（命令/策略模式）。
- `StatusSystem`：状态（灼烧/冻结/麻痹/护盾/力量等）。
- `FusionManager`：融合点与融合配方（预设 + 随机）。
- `EnemyAI`：敌人意图与行动。
- `ProgressionManager`：奖励、商店、篝火、解锁。
- `SaveManager`：存档/读档/版本迁移。
- `Telemetry`：日志/指标/回放（至少日志+可重放基础）。

### 1.3 关键性技术选择
- 引擎：Unity 2022.3 LTS（见 TDD，若变更需更新本章）。
- 语言：C#。
- 数据：JSON 作为配置源（卡牌/敌人/配方/平衡参数）。
- 目标平台：Windows/macOS/Linux（Steam）。

---

## 2. 核心数据模型（Domain Model）

### 2.1 基础类型
- `Energy`：能量（int）。
- `FusionPoints`：融合点（int, 0~10）。
- `HP`：生命值 + 最大生命。
- `Block`：护盾（回合结束清空/或按规则清空，需在此固定）。

### 2.2 战斗实体
- `Combatant`（基类）：
  - `hp`, `maxHp`, `block` 
  - `statuses: List<StatusInstance>`
  - `TakeDamage(DamagePacket)` / `GainBlock(int)` / `ApplyStatus(Status)`
- `Player : Combatant`
- `Enemy : Combatant`

### 2.3 卡牌
- `CardInstance`：运行态卡牌
  - `cardId`（指向静态配置）
  - `upgradeLevel`
  - `isTemporary`（融合卡/一次性）

- `CardDef`：静态卡牌定义（JSON）
  - `id, name, type, rarity, cost`
  - `effects[]`（效果列表）
  - `tags[]`（元素/关键词，便于融合与协同）

### 2.4 状态（Buff/Debuff）
- `StatusDef`：定义（如 Burn, Freeze, Paralysis, Weak, Strength）。
- `StatusInstance`：实例（层数/持续回合/特殊参数）。

**统一规则：**
- 状态的触发点必须标准化：
  - OnTurnStart / OnTurnEnd / OnDealDamage / OnTakeDamage / OnCardPlayed / OnEnemyDefeated / OnFusion 等。

---

## 3. 战斗系统实现方案

### 3.1 战斗状态机
建议用显式状态机（便于测试/回放/排查）：
- `BattleState.Init`
- `BattleState.PlayerTurnStart`
- `BattleState.PlayerActing`
- `BattleState.PlayerTurnEnd`
- `BattleState.EnemyTurnStart`
- `BattleState.EnemyActing`
- `BattleState.EnemyTurnEnd`
- `BattleState.ResolveEnd`

**关键不变量（Invariant）：**
- 任一时刻只能有一个“行动者”。
- 出牌与效果执行必须串行化（避免竞态/动画回调乱序）。

### 3.2 牌堆模型
- `DrawPile`（抽牌堆）
- `Hand`（手牌，上限 10）
- `DiscardPile`（弃牌堆）

**抽牌规则：**
- 回合开始抽 5（可配置）。
- 抽牌堆为空时：弃牌堆洗回抽牌堆。

### 3.3 出牌与效果执行
- 出牌流程：
  1) 校验 `cost <= currentEnergy`
  2) 扣能量
  3) 触发 `OnCardPlayed`
  4) 执行 `effects[]`（按顺序）
  5) 将牌移动到弃牌堆（或消耗区）

**效果执行建议：命令模式/策略模式**
- `IEffect.Execute(Context ctx, Target t)`
- `Context` 包括：战斗引用、随机源、执行栈、日志器。

### 3.4 伤害计算
统一 DamagePipeline（否则后期会失控）：
- 输入：`DamagePacket { base, type, source, tags }`
- 修正：
  - attacker statuses（Strength/FireMastery 等）
  - target statuses（Weak/Vulnerable/ShieldType 等）
  - relic/passives
- 应用护盾：
  - `block` 抵扣 → 剩余进 HP
- 触发：OnTakeDamage/OnDealDamage

---

## 4. 融合系统实现方案

### 4.1 融合点系统
- 上限 10
- 来源：
  - 每打出 1 张卡：+1
  - 每击败 1 个敌人：+2
  - 遗物/事件额外加成（可配置）
- 消耗：默认 3 点/次（可配置）

### 4.2 融合操作（UX 与逻辑）
- UI：卡牌 A 拖到卡牌 B → 展示预览 → 确认。
- 逻辑：
  1) 校验融合点
  2) 校验两张卡可融合（禁用：特殊卡/诅咒卡等策略）
  3) 计算融合结果卡
  4) 消耗两张卡（从手牌移除）
  5) 扣融合点
  6) 将新卡加入手牌（或弃牌堆，需在此固定）

**推荐：加入手牌**（即时爽感，符合卖点）。

### 4.3 配方查找
- Key 规则：`min(cardA.id, cardB.id) + "+" + max(...)`
- 预设配方：来自 `fusion_recipes.json`
- 未命中：走随机融合算法

### 4.4 随机融合算法（可测试、可复现）
- 必须注入 `SeededRandom`，战斗开始产生 `battleSeed`，确保回放一致。

建议算法（与 design 文档一致）：
- `newCost = ceil((c1.cost + c2.cost)/1.5)`
- `newPower = (power(c1)+power(c2))*1.2`
- 效果合并规则按类别（伤害/护盾/抽牌/控制）

**落地要求：**
- 每次随机融合必须写入 `CombatLog`（含 seed、输入、输出）

---

## 5. 敌人 AI 与意图系统

### 5.1 意图显示
- `EnemyIntent` 作为下一回合行动的可视化。
- 敌人行动前，UI 必须能读取 `GetNextIntent()`。

### 5.2 行为模型
- MVP：Pattern-based（按配置循环）。
- 进阶：Conditional（HP 阶段/状态驱动）。

### 5.3 数据驱动
- `enemy.actions[]` 允许配置：Attack/Defend/ApplyStatus/Summon/Heal/Special。

---

## 6. 配置与数据方案

### 6.1 配置文件清单
- `cards.json`
- `enemies.json`
- `fusion_recipes.json`
- `balance_config.json`（建议单独）
- `loot_tables.json`（掉落/商店）

### 6.2 配置加载策略
- 启动加载静态配置到内存（小规模内容可行）。
- 运行态只引用 `id`，避免对象引用污染存档。

### 6.3 配置校验（强烈建议）
- 启动时做 schema 校验 + 关键字段校验：
  - cardId 唯一
  - effect 类型合法
  - 配方引用的 cardId 存在
  - 平衡表数值范围

---

## 7. 存档方案（版本化 + 迁移）

### 7.1 存档粒度
- `MetaSave`：解锁、设置、统计。
- `RunSave`：本局进度（楼层、HP、金币、牌组 list、遗物 list、地图状态）。

### 7.2 JSON 存档与版本号
- 存档必须包含：`saveVersion`。
- 每次结构变更，提供 `Migrate(vX→vY)`。

### 7.3 防坏档策略
- 写入：先写临时文件 `save.tmp`，成功后原子替换。
- 保留 `save.bak`（最后一次成功存档）。

---

## 8. 可观测性与回放（最低可用）

### 8.1 日志
- `CombatLog`：战斗内事件（抽牌/出牌/伤害/状态/融合/敌人行动）。
- 关联字段：`runId`, `battleId`, `turn`, `seed`。

### 8.2 事件队列与可重放
- 所有战斗事件必须通过单一事件队列派发。
- 回放：用同 `seed` + 同顺序输入（卡牌选择/目标）可重建关键状态。

---

## 9. 质量防线（测试与CI）

### 9.1 单元测试（建议比例：50%）
- DamagePipeline
- Status 触发点
- 融合配方命中与随机融合可复现
- 配置校验

### 9.2 集成测试（建议比例：30%）
- 一场战斗从 Init 到 End 的完整流程
- 多敌人 + AOE + 状态叠加

### 9.3 E2E / 手工验收（建议比例：20%）
- 关键路径：开局 → 打第一场 → 获得奖励 → 进地图 → 下一场

### 9.4 CI 建议
- GitHub Actions：
  - lint（csharpier/format）
  - build（Unity batchmode）
  - test（Unity Test Runner）

---

## 10. 风险图与关键失败路径

### 10.1 高风险点（需要预置防线）
- 融合：随机性导致不可复现 → 必须 seed + 记录。
- 状态系统：触发点复杂 → 必须统一生命周期。
- 动画回调：容易乱序 → 逻辑/动画解耦，逻辑先行。
- 配置膨胀：卡牌/配方数量大 → 必须校验 + 工具化。

### 10.2 关键失败路径
- “一张卡触发多个效果 + 状态叠加 + AOE” → 回合结束状态不一致
- 解决：效果执行栈 + 明确事件顺序 + 快照断言

---

## 11. 目录规范（建议）

```
/dev
  /Runtime
    /Core
    /Battle
    /Cards
    /Enemies
    /Fusion
    /Status
    /UI
    /Save
  /Tests
    /EditMode
    /PlayMode
/Configs
  cards.json
  enemies.json
  fusion_recipes.json
  balance_config.json
```

---

## 12. 变更记录（Changelog）

- v0.1 (2026-03-18)
  - 初始化技术方案：战斗状态机、效果系统、融合系统、存档、可观测性、测试策略。

---

## 13. 下一步（落地清单）

1. 建立 Unity 工程骨架与目录结构
2. 实现 BattleStateMachine + CardSystem 最小闭环
3. 引入 SeededRandom + CombatLog
4. 配置加载与校验（先简单，后引入 schema）
5. 补齐单测：DamagePipeline / Fusion 可复现

---

> 备注：本文件是“开发时必须跟随的技术基线”。任何偏离都要在本文件中记录原因、替代方案与影响面。
