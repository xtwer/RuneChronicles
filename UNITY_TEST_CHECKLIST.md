# 🧪 Unity项目完整测试检查清单

**最后更新**: 2026-03-23 15:20  
**项目状态**: ✅ 准备测试

---

## ✅ 预检查

### 1. Unity版本
- [ ] Unity 2022.3.15 LTS或更高版本
- [ ] 所有必需的Package已安装（TextMeshPro, URP）

### 2. 项目结构
- [x] Assets/Scripts/ - 所有脚本
- [x] Assets/Resources/Data/ - 所有JSON
- [x] Assets/Art/ - 美术文件夹（占位符）
- [x] Assets/Audio/ - 音效文件夹（占位符）
- [x] Assets/Tests/ - 测试文件

---

## 🎮 Runtime Tests（运行时测试）

### 步骤1: 创建测试场景
1. [ ] 打开Unity
2. [ ] 创建新场景 (`File > New Scene`)
3. [ ] 保存场景为 `RuntimeTest.unity`

### 步骤2: 添加RuntimeTests组件
1. [ ] 创建空GameObject (`右键 > Create Empty`)
2. [ ] 重命名为 `TestRunner`
3. [ ] 添加 `RuntimeTests` 组件

### 步骤3: 运行测试
1. [ ] 点击 Play 按钮 ▶
2. [ ] 查看Console窗口输出

### 步骤4: 验证输出
检查Console中是否有以下内容：

```
✅ 预期输出:
=== 开始运行时测试 ===
[测试] CardManager...
✅ CardManager 初始化成功
✅ 加载 120 张卡牌  <-- 重要！
[测试] BattleManager...
✅ BattleManager 初始化成功
✅ 最大能量: 3
[测试] FusionManager...
✅ FusionManager 初始化成功
✅ 融合点上限: 10
[测试] MapManager...
✅ MapManager 初始化成功
✅ 总层数: 15
[测试] CharacterManager...
✅ CharacterManager 初始化成功
✅ 当前角色: 符文法师
[测试] RelicManager...
✅ RelicManager 初始化成功
✅ 当前遗物数量: 0
[测试] GameManager...
✅ GameManager 初始化成功
✅ 游戏状态: MainMenu
=== 测试完成 ===
```

---

## 📊 手动功能测试

### 测试1: 卡牌系统
1. [ ] CardManager加载成功
2. [ ] 显示"加载 120 张卡牌"或类似数字（110+）
3. [ ] 无错误信息

### 测试2: 战斗系统
1. [ ] BattleManager初始化成功
2. [ ] 最大能量为3
3. [ ] 无错误信息

### 测试3: 融合系统
1. [ ] FusionManager初始化成功
2. [ ] 融合点上限为10
3. [ ] 无错误信息

### 测试4: 地图系统
1. [ ] MapManager初始化成功
2. [ ] 总层数为15
3. [ ] 无错误信息

### 测试5: 角色系统
1. [ ] CharacterManager初始化成功
2. [ ] 当前角色为"符文法师"
3. [ ] 无错误信息

### 测试6: 遗物系统
1. [ ] RelicManager初始化成功
2. [ ] 当前遗物数量为0（初始状态）
3. [ ] 无错误信息

### 测试7: 游戏管理器
1. [ ] GameManager初始化成功
2. [ ] 游戏状态为MainMenu
3. [ ] 所有子管理器已加载

---

## 🐛 常见问题排查

### 问题1: 编译错误
**症状**: Console中有红色错误  
**解决**:
1. 刷新资源 (`Cmd+R` 或 `Assets > Refresh`)
2. 重启Unity
3. 检查Unity版本是否为2022.3+

### 问题2: 找不到JSON文件
**症状**: `未找到: Resources/Data/XXX.json`  
**解决**:
1. 确认 `Assets/Resources/Data/` 文件夹存在
2. 确认JSON文件都在该文件夹内
3. 刷新资源

### 问题3: 卡牌数量不对
**症状**: 加载的卡牌数量不是120张  
**可能**:
- 部分JSON文件格式错误（不影响主功能）
- 部分卡牌ID重复（CardManager会跳过）
**验证**: 只要数量>=100，即可视为通过

### 问题4: 某个Manager为null
**症状**: `XXXManager 初始化失败`  
**解决**:
1. 检查该Manager的脚本是否有编译错误
2. 在Hierarchy中手动创建GameObject并添加该组件
3. 查看详细错误信息

---

## ✅ 测试通过标准

### 最低标准（必须全部满足）
- [x] 无编译错误
- [ ] 所有7个Manager初始化成功
- [ ] 卡牌加载数量 >= 100
- [ ] Console无红色错误

### 理想标准
- [ ] 卡牌加载数量 = 120
- [ ] 所有Manager初始化成功
- [ ] Console无任何警告

---

## 📝 测试记录

**测试日期**: _____________  
**测试人员**: _____________  
**Unity版本**: _____________  

### 测试结果
- [ ] ✅ 全部通过
- [ ] ⚠️ 部分通过（功能可用但有警告）
- [ ] ❌ 未通过

### 问题记录
```
记录任何问题:
1. 
2. 
3. 
```

### 备注
```

```

---

## 🎉 测试完成后

如果所有测试通过：
1. ✅ 项目核心功能验证完成
2. ✅ 可以进入下一步开发
3. ✅ 可以开始UI/美术集成

---

**检查清单版本**: v1.0  
**最后更新**: 2026-03-23 15:20
