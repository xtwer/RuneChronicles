#!/bin/bash

echo "========================================"
echo "  Unity项目完整性检查"
echo "========================================"
echo ""

cd ~/Desktop/RuneChronicles/RuneChronicles

# 1. 检查核心脚本
echo "✓ 检查核心脚本..."
CORE_SCRIPTS=(
  "Assets/Scripts/GameManager.cs"
  "Assets/Scripts/BattleManager.cs"
  "Assets/Scripts/CardManager.cs"
  "Assets/Scripts/FusionManager.cs"
  "Assets/Scripts/MapManager.cs"
  "Assets/Scripts/CharacterManager.cs"
  "Assets/Scripts/RelicManager.cs"
  "Assets/Scripts/Player.cs"
  "Assets/Scripts/Enemy.cs"
  "Assets/Scripts/CardData.cs"
  "Assets/Scripts/RuntimeTests.cs"
  "Assets/Scripts/AudioManager.cs"
  "Assets/Scripts/DebugManager.cs"
)

MISSING=0
for script in "${CORE_SCRIPTS[@]}"; do
  if [ ! -f "$script" ]; then
    echo "  ✗ 缺失: $script"
    MISSING=1
  fi
done

if [ $MISSING -eq 0 ]; then
  echo "  ✓ 所有核心脚本存在 (${#CORE_SCRIPTS[@]}个)"
fi

# 2. 检查JSON数据
echo ""
echo "✓ 检查JSON数据..."
JSON_FILES=(
  "Assets/Resources/Data/BasicCards.json"
  "Assets/Resources/Data/FusionCards.json"
  "Assets/Resources/Data/WarriorCards.json"
  "Assets/Resources/Data/ExtendedAttackCards.json"
  "Assets/Resources/Data/ExtendedSkillCards.json"
  "Assets/Resources/Data/ExtendedPowerCards.json"
  "Assets/Resources/Data/BasicEnemies.json"
  "Assets/Resources/Data/ExtendedEnemies.json"
)

MISSING_JSON=0
for json in "${JSON_FILES[@]}"; do
  if [ ! -f "$json" ]; then
    echo "  ✗ 缺失: $json"
    MISSING_JSON=1
  fi
done

if [ $MISSING_JSON -eq 0 ]; then
  echo "  ✓ 所有JSON数据存在 (${#JSON_FILES[@]}个)"
fi

# 3. 检查文件夹结构
echo ""
echo "✓ 检查文件夹结构..."
FOLDERS=(
  "Assets/Scripts"
  "Assets/Resources/Data"
  "Assets/Art"
  "Assets/Audio"
)

for folder in "${FOLDERS[@]}"; do
  if [ -d "$folder" ]; then
    echo "  ✓ $folder"
  else
    echo "  ✗ $folder (不存在)"
  fi
done

# 4. 统计
echo ""
echo "========================================"
echo "  统计"
echo "========================================"
echo "核心脚本: $(ls Assets/Scripts/*.cs 2>/dev/null | wc -l | tr -d ' ')个"
echo "JSON数据: $(ls Assets/Resources/Data/*.json 2>/dev/null | wc -l | tr -d ' ')个"
echo "Git提交: $(git log --oneline | wc -l | tr -d ' ')个"
echo ""

# 5. 最后检查
if [ $MISSING -eq 0 ] && [ $MISSING_JSON -eq 0 ]; then
  echo "✅ 项目完整性检查通过！"
  echo ""
  echo "👉 现在可以在Unity中测试："
  echo "   1. 在Unity中刷新资源 (Cmd+R)"
  echo "   2. 等待编译完成"
  echo "   3. 选中Hierarchy中的TestRunner"
  echo "   4. 添加RuntimeTests组件"
  echo "   5. 点击Play按钮"
  echo ""
  exit 0
else
  echo "❌ 项目不完整，请检查缺失文件"
  exit 1
fi
