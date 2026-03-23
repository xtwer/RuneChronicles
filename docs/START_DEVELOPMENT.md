# 开始开发 - 完整指南

**日期：** 2026-03-19  
**项目：** 符文编年史 (RuneChronicles)  
**目标：** 创建最小原型（2张卡+1融合+1敌人）

---

## 📋 **开发前准备清单**

### **✅ 已完成：**
- [x] 项目文档完整（11份设计文档 + 5份测试文档）
- [x] Git 仓库初始化
- [x] GitHub 仓库创建：https://github.com/xtwer/RuneChronicles
- [x] UiCard 框架下载并评估（4.7/5 ⭐）
- [x] 开发指南准备（UNITY_SETUP_GUIDE.md, MVP_DEVELOPMENT_GUIDE.md）

### **⏳ 待完成：**
- [ ] 安装 Unity 2022.3.15f1
- [ ] 创建 Unity 项目
- [ ] 集成 UiCard 框架
- [ ] 开发最小原型

---

## 🚀 **Step 1: 安装 Unity**

### **1.1 下载 Unity Hub**

访问：https://unity.com/download

或直接下载：
```bash
# macOS
open "https://public-cdn.cloud.unity3d.com/hub/prod/UnityHubSetup.dmg"
```

### **1.2 安装 Unity 2022.3.15f1 LTS**

1. 打开 Unity Hub
2. 点击左侧 "Installs"
3. 点击 "Install Editor"
4. 选择 **2022.3.15f1 LTS**
5. 勾选以下模块：
   - ✅ **2D** (必选)
   - ✅ **Universal RP** (必选)
   - ✅ **Mac Build Support (Mono)** (推荐)
   - ✅ **Windows Build Support (Mono)** (如果需要 Windows 构建)
   - ✅ **Documentation** (推荐)

**安装时间：** 约 10-15 分钟

---

## 🎮 **Step 2: 创建 Unity 项目**

### **2.1 通过 Unity Hub 创建**

1. 打开 Unity Hub
2. 点击 "Projects" → "New project"
3. 选择 **2022.3.15f1**
4. 选择模板：**2D (URP)**
5. 项目名称：`RuneChronicles`
6. 位置：`~/Desktop/RuneChronicles/`
7. 点击 "Create project"

**注意：** Unity 会在 `~/Desktop/RuneChronicles/` 下创建项目文件

### **2.2 项目结构（创建后）**

```
RuneChronicles/
├── Assets/              # Unity 资源文件夹
│   ├── Scenes/         # 场景文件
│   ├── Scripts/        # C# 脚本
│   ├── Prefabs/        # 预制体
│   ├── Resources/      # 资源
│   └── Data/           # 卡牌数据（JSON）
├── Packages/           # Unity 包管理
├── ProjectSettings/    # 项目设置
├── Library/            # Unity 缓存（.gitignore）
├── Logs/               # 日志（.gitignore）
├── Temp/               # 临时文件（.gitignore）
└── UserSettings/       # 用户设置（.gitignore）
```

---

## 🔧 **Step 3: 集成 UiCard 框架**

### **3.1 复制 UiCard 核心代码**

```bash
# 进入项目目录
cd ~/Desktop/RuneChronicles/

# 复制 UiCard 核心脚本
cp -r UiCard-Framework/Assets/Scripts/UICard Assets/Scripts/
cp -r UiCard-Framework/Assets/Scripts/Patterns Assets/Scripts/

# 复制工具脚本
cp -r UiCard-Framework/Assets/Scripts/Tools Assets/Scripts/
cp -r UiCard-Framework/Assets/Scripts/Extensions Assets/Scripts/
```

### **3.2 在 Unity 中验证**

1. 打开 Unity 项目
2. 等待脚本编译完成（约 1-2 分钟）
3. 检查 Console 是否有错误
4. 如果有错误，记录并解决

---

## 🎯 **Step 4: 创建最小原型（MVP）**

### **4.1 创建基础场景**

1. 在 Unity 中创建新场景：`Assets/Scenes/BattleScene.unity`
2. 保存场景

### **4.2 创建核心脚本**

参考 `docs/MVP_DEVELOPMENT_GUIDE.md`，创建以下脚本：

**Day 1（基础框架）：**
- `GameManager.cs` - 游戏管理器
- `Card.cs` - 卡牌数据类
- `Enemy.cs` - 敌人数据类

**Day 2（卡牌系统）：**
- `CardManager.cs` - 卡牌管理器
- `CardUI.cs` - 卡牌 UI（基于 UiCard）

**Day 3（融合系统）：**
- `BattleManager.cs` - 战斗管理器
- `FusionSystem.cs` - 融合系统
- `UiCardFusion.cs` - 融合状态（扩展 UiCard）

### **4.3 创建测试数据**

创建 2 张测试卡牌：
- **火焰弹**（ATK_001）：造成 6 点伤害
- **冰霜箭**（ATK_006）：造成 4 点伤害 + 冻结 1 回合

创建 1 个测试敌人：
- **史莱姆**（ENM_001）：20 HP，每回合攻击 5 点

创建 1 个融合配方：
- **火焰弹 + 冰霜箭 = 冰火箭**：造成 12 点伤害 + 冻结 1 回合

---

## 📊 **开发进度追踪**

### **Week 1 目标（3-5 天）：**
- [ ] Day 1: Unity 安装 + 项目创建 + UiCard 集成
- [ ] Day 2: 基础框架（GameManager, Card, Enemy）
- [ ] Day 3: 卡牌系统（CardManager, CardUI）
- [ ] Day 4: 融合系统（FusionSystem, UiCardFusion）
- [ ] Day 5: 测试与调整

### **验收标准：**
- ✅ 可以抽卡
- ✅ 可以拖拽卡牌
- ✅ 可以选择 2 张卡融合
- ✅ 融合后生成新卡
- ✅ 可以对敌人使用卡牌
- ✅ 敌人 HP 正确减少

---

## 🛠️ **开发工具配置**

### **推荐 IDE：**
- **Visual Studio Code** + C# 扩展
- **Rider**（JetBrains，专业但付费）
- **Visual Studio for Mac**（免费）

### **Unity 插件推荐：**
- **TextMesh Pro**（Unity 自带，用于文字渲染）
- **2D Sprite**（Unity 自带）
- **Universal RP**（已选择）

---

## 📝 **开发日志**

### **2026-03-19 21:40**
- ✅ 准备开发指南
- ⏳ 等待 Unity 安装

---

## 🔗 **相关文档**

- [Unity 安装指南](UNITY_SETUP_GUIDE.md)
- [MVP 开发指南](MVP_DEVELOPMENT_GUIDE.md)
- [UiCard 框架评估](UICARD_EVALUATION.md)
- [技术实现文档](../design/TECH_IMPLEMENTATION.md)

---

## 💡 **重要提醒**

1. **代码优先，美术其次** - 先用白色方块代替卡牌图片
2. **最小原型验证核心玩法** - 融合系统好不好玩？
3. **快速迭代** - 不要追求完美，先做出来
4. **记录问题** - 遇到问题立即记录到 `docs/ISSUES.md`

---

**准备好了吗？让我们开始创造！** 🚀
