# Unity 项目创建检查清单

> **目标：** 创建 Unity 2022.3.15f1 项目

---

## ✅ 检查清单

### 1. 安装 Unity Hub（如果未安装）

**下载地址：** https://unity.com/download

**安装步骤：**
1. 下载 Unity Hub
2. 安装并打开
3. 登录你的 Unity 账号（如果没有，免费注册一个）

---

### 2. 安装 Unity 2022.3.15f1

**步骤：**
1. 打开 Unity Hub
2. 点击左侧 "Installs"（安装）
3. 点击右上角 "Install Editor"（安装编辑器）
4. 选择版本：**2022.3.15f1** (LTS)
   - ⚠️ 注意：必须是 **2022.3.15f1**，不要用其他版本
5. 选择模块（勾选以下）：
   - ✅ **Windows Build Support** (IL2CPP)
   - ✅ **Mac Build Support** (Mono + IL2CPP)（如果你在 Mac 上）
   - ✅ **Linux Build Support** (Mono)
   - ✅ **Documentation**（文档）
   - ❌ 其他模块可选（Android/iOS 暂时不需要）
6. 点击 "Continue" → "Install"
7. 等待下载安装（约 10-20 分钟，取决于网速）

---

### 3. 创建项目

**步骤：**
1. 打开 Unity Hub
2. 点击左侧 "Projects"（项目）
3. 点击右上角 "New project"（新建项目）
4. 选择 Unity 版本：**2022.3.15f1**
5. 选择模板：**2D (URP)** ⭐
   - 2D：卡牌游戏用 2D 模板
   - URP：Universal Render Pipeline（更好的性能）
6. 填写项目信息：
   - **Project name:** RuneChronicles
   - **Location:** `/Users/wepie/Desktop/RuneChronicles/`
     - ⚠️ 注意：Unity 会在这个目录下创建项目，请确保路径正确
7. 点击 "Create project"（创建项目）
8. 等待初始化（约 2-5 分钟）

---

### 4. 验证项目创建成功

**检查项：**
- [ ] Unity 编辑器已打开
- [ ] 项目名称显示 "RuneChronicles"
- [ ] Hierarchy 窗口有默认的 "Main Camera" 和 "Directional Light"
- [ ] Project 窗口显示 Assets 文件夹
- [ ] Console 窗口无错误（可能有警告，忽略）

---

### 5. 项目结构验证

**在 Finder/文件管理器中检查：**

```
RuneChronicles/
├── Assets/              ✅ Unity 资源文件夹
├── Packages/            ✅ Unity 包管理
├── ProjectSettings/     ✅ 项目设置
├── docs/                ✅ 我们的文档（已存在）
├── design/              ✅ 我们的设计（已存在）
├── scripts/             ✅ 我们的脚本（已存在）
├── Library/             ⚠️ Unity 生成（Git 已忽略）
├── Temp/                ⚠️ Unity 临时文件（Git 已忽略）
└── .gitignore           ✅ 已配置
```

---

### 6. Git 提交

**在终端执行：**
```bash
cd ~/Desktop/RuneChronicles
git add .
git commit -m "feat: Unity 2022.3.15f1 (2D URP) 项目初始化"
git push
```

---

## ⚠️ 常见问题

### 问题1：找不到 Unity 2022.3.15f1
**解决：** 
- 在 Unity Hub → Installs → Install Editor
- 点击 "Archive" 标签
- 搜索 "2022.3.15"

### 问题2：创建项目时提示路径已存在
**解决：**
- Unity 会在选择的目录下创建项目文件夹
- 如果已经有 `RuneChronicles` 文件夹，Unity 会在里面创建项目
- 这是正常的，我们就是要在现有文件夹内创建

### 问题3：Unity 卡在 "Importing Assets..."
**解决：**
- 第一次打开项目会导入资源，可能需要 5-10 分钟
- 耐心等待，不要强制关闭

### 问题4：Console 显示很多警告
**解决：**
- 第一次创建项目可能有警告，大部分可以忽略
- 如果有红色错误（Error），检查 Unity 版本是否正确

---

## 🎉 完成后

**你应该看到：**
1. Unity 编辑器打开
2. 左侧 Hierarchy 有场景对象
3. 中间 Scene 窗口显示 3D 视图
4. 右侧 Inspector 显示属性
5. 底部 Project 显示 Assets 文件夹
6. 底部 Console 无红色错误

**下一步：**
- ✅ Unity 项目创建完成
- ⏳ 开始创建最小原型（2张卡+1融合+1敌人）

---

**预计时间：** 30-60 分钟（包括下载安装）

**如果遇到问题，随时告诉我！** 🚀
