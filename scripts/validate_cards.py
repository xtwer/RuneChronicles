#!/usr/bin/env python3
"""
卡牌配置数据校验脚本
用途：在提交代码前自动校验 JSON 配置合法性
"""

import json
import sys
from pathlib import Path

# 数值平衡公式
ENERGY_TO_VALUE = 6.5  # 1能量 = 6.5点数值
BALANCE_TOLERANCE = 0.2  # ±20%容差

# 稀有度修正系数
RARITY_MULTIPLIERS = {
    "普通": 1.0,
    "罕见": 1.3,
    "史诗": 1.6,
    "传说": 2.0
}

# AOE修正系数
AOE_MULTIPLIERS = {
    1: 1.0,   # 单目标
    2: 0.7,   # 2目标
    3: 0.6,   # 3目标
    99: 0.5   # 全体
}

def calculate_card_value(card):
    """计算卡牌理论价值"""
    value = 0
    
    # 基础伤害/护盾
    if 'damage' in card:
        value += card['damage']
    if 'shield' in card:
        value += card['shield']
    if 'heal' in card:
        value += card['heal']
    
    # 抽牌价值（1抽牌 = 3数值）
    if 'draw' in card:
        value += card['draw'] * 3
    
    # 特殊效果价值
    effects = card.get('effects', [])
    for effect in effects:
        if '灼烧' in effect:
            value += 4  # 灼烧2层 = 4数值
        if '冻结' in effect:
            value += 6  # 冻结1回合 = 6数值
        if '虚弱' in effect:
            value += 4
        if '易伤' in effect:
            value += 5
    
    return value

def validate_cards(json_path):
    """验证卡牌配置"""
    errors = []
    warnings = []
    
    try:
        with open(json_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
    except FileNotFoundError:
        return [f"错误：文件不存在 - {json_path}"], []
    except json.JSONDecodeError as e:
        return [f"错误：JSON 格式错误 - {e}"], []
    
    cards = data.get('cards', [])
    
    if not cards:
        return ["错误：卡牌列表为空"], []
    
    # 1. 检查必填字段
    required_fields = ['id', 'name', 'energy', 'rarity', 'type']
    for i, card in enumerate(cards):
        for field in required_fields:
            if field not in card:
                errors.append(f"卡牌 #{i+1} ({card.get('name', '???')}) 缺少字段: {field}")
    
    # 2. 检查数值合法性
    for card in cards:
        card_name = card.get('name', '未知卡牌')
        
        # 能量不能为负
        if card.get('energy', -1) < 0:
            errors.append(f"卡牌 {card_name} 能量为负: {card['energy']}")
        
        # 稀有度必须合法
        if card.get('rarity') not in ['普通', '罕见', '史诗', '传说']:
            errors.append(f"卡牌 {card_name} 稀有度非法: {card.get('rarity')}")
        
        # 类型必须合法
        valid_types = ['攻击', '技能', '能力', '诅咒']
        if card.get('type') not in valid_types:
            errors.append(f"卡牌 {card_name} 类型非法: {card.get('type')}")
    
    # 3. 检查 ID 唯一性
    ids = [c['id'] for c in cards if 'id' in c]
    if len(ids) != len(set(ids)):
        errors.append("存在重复的卡牌 ID")
        # 找出重复的ID
        seen = set()
        duplicates = set()
        for card_id in ids:
            if card_id in seen:
                duplicates.add(card_id)
            seen.add(card_id)
        if duplicates:
            errors.append(f"  重复的ID: {', '.join(duplicates)}")
    
    # 4. 检查数值平衡（基于公式）
    for card in cards:
        if card.get('type') not in ['攻击', '技能']:
            continue  # 跳过能力卡
        
        card_name = card.get('name', '未知卡牌')
        energy = card.get('energy', 0)
        
        if energy == 0:
            continue  # 跳过0费卡（特殊平衡）
        
        # 计算期望值
        rarity = card.get('rarity', '普通')
        rarity_mult = RARITY_MULTIPLIERS.get(rarity, 1.0)
        expected_value = energy * ENERGY_TO_VALUE * rarity_mult
        
        # 计算实际值
        actual_value = calculate_card_value(card)
        
        # 检查是否在容差范围内
        tolerance = expected_value * BALANCE_TOLERANCE
        if abs(actual_value - expected_value) > tolerance:
            severity = "警告" if abs(actual_value - expected_value) < expected_value * 0.3 else "错误"
            msg = (
                f"{severity}: 卡牌 {card_name} 数值可能失衡\n"
                f"  能量: {energy}费 | 稀有度: {rarity}\n"
                f"  期望值: {expected_value:.1f} | 实际值: {actual_value:.1f}\n"
                f"  差异: {actual_value - expected_value:+.1f} ({((actual_value/expected_value - 1) * 100):+.1f}%)"
            )
            if severity == "错误":
                errors.append(msg)
            else:
                warnings.append(msg)
    
    return errors, warnings

def validate_enemies(json_path):
    """验证敌人配置"""
    errors = []
    warnings = []
    
    try:
        with open(json_path, 'r', encoding='utf-8') as f:
            data = json.load(f)
    except FileNotFoundError:
        return [f"错误：文件不存在 - {json_path}"], []
    except json.JSONDecodeError as e:
        return [f"错误：JSON 格式错误 - {e}"], []
    
    enemies = data.get('enemies', [])
    
    if not enemies:
        return ["错误：敌人列表为空"], []
    
    # 1. 检查必填字段
    required_fields = ['id', 'name', 'hp', 'type']
    for i, enemy in enumerate(enemies):
        for field in required_fields:
            if field not in enemy:
                errors.append(f"敌人 #{i+1} ({enemy.get('name', '???')}) 缺少字段: {field}")
    
    # 2. 检查数值合法性
    for enemy in enemies:
        enemy_name = enemy.get('name', '未知敌人')
        
        # HP 必须为正数
        if enemy.get('hp', 0) <= 0:
            errors.append(f"敌人 {enemy_name} HP 非法: {enemy.get('hp')}")
        
        # 类型必须合法
        valid_types = ['普通', '精英', 'BOSS']
        if enemy.get('type') not in valid_types:
            errors.append(f"敌人 {enemy_name} 类型非法: {enemy.get('type')}")
        
        # 行为模式必须存在
        if 'behavior' not in enemy or not enemy['behavior']:
            errors.append(f"敌人 {enemy_name} 缺少行为模式")
    
    # 3. 检查 ID 唯一性
    ids = [e['id'] for e in enemies if 'id' in e]
    if len(ids) != len(set(ids)):
        errors.append("存在重复的敌人 ID")
    
    return errors, warnings

def main():
    """主函数"""
    print("=" * 60)
    print("  卡牌数据校验工具")
    print("=" * 60)
    print()
    
    # 获取项目根目录
    script_dir = Path(__file__).parent
    project_root = script_dir.parent
    data_dir = project_root / "Assets" / "Data"
    
    total_errors = 0
    total_warnings = 0
    
    # 校验卡牌配置
    cards_json = data_dir / "cards.json"
    print(f"📋 检查卡牌配置: {cards_json}")
    
    if cards_json.exists():
        errors, warnings = validate_cards(cards_json)
        
        if errors:
            print(f"  ❌ 发现 {len(errors)} 个错误:")
            for err in errors:
                print(f"    {err}")
            total_errors += len(errors)
        
        if warnings:
            print(f"  ⚠️  发现 {len(warnings)} 个警告:")
            for warn in warnings:
                print(f"    {warn}")
            total_warnings += len(warnings)
        
        if not errors and not warnings:
            print("  ✅ 所有卡牌配置验证通过")
    else:
        print(f"  ⏭  文件不存在（跳过）")
    
    print()
    
    # 校验敌人配置
    enemies_json = data_dir / "enemies.json"
    print(f"👹 检查敌人配置: {enemies_json}")
    
    if enemies_json.exists():
        errors, warnings = validate_enemies(enemies_json)
        
        if errors:
            print(f"  ❌ 发现 {len(errors)} 个错误:")
            for err in errors:
                print(f"    {err}")
            total_errors += len(errors)
        
        if warnings:
            print(f"  ⚠️  发现 {len(warnings)} 个警告:")
            for warn in warnings:
                print(f"    {warn}")
            total_warnings += len(warnings)
        
        if not errors and not warnings:
            print("  ✅ 所有敌人配置验证通过")
    else:
        print(f"  ⏭  文件不存在（跳过）")
    
    print()
    print("=" * 60)
    print(f"总计: {total_errors} 个错误, {total_warnings} 个警告")
    print("=" * 60)
    
    # 如果有错误，返回非零退出码（阻止Git提交）
    if total_errors > 0:
        sys.exit(1)
    else:
        sys.exit(0)

if __name__ == '__main__':
    main()
