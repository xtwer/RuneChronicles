using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RuneChronicles
{
    /// <summary>
    /// 融合系统 - 负责卡牌融合逻辑
    /// </summary>
    public class FusionSystem : MonoBehaviour
    {
        public static FusionSystem Instance { get; private set; }
        
        [Header("融合配方")]
        public List<CardData> fusionRecipes = new List<CardData>();
        
        [Header("当前选择")]
        public List<CardData> selectedCards = new List<CardData>();
        public int maxFusionCards = 2; // 每次融合需要的卡牌数量
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        /// <summary>
        /// 选择卡牌用于融合
        /// </summary>
        public bool SelectCardForFusion(CardData card)
        {
            if (selectedCards.Count >= maxFusionCards)
            {
                Debug.Log("Already selected max cards for fusion!");
                return false;
            }
            
            if (!selectedCards.Contains(card))
            {
                selectedCards.Add(card);
                Debug.Log($"Selected {card.cardName} for fusion ({selectedCards.Count}/{maxFusionCards})");
                return true;
            }
            
            return false;
        }
        
        /// <summary>
        /// 取消选择
        /// </summary>
        public void DeselectCard(CardData card)
        {
            if (selectedCards.Contains(card))
            {
                selectedCards.Remove(card);
                Debug.Log($"Deselected {card.cardName}");
            }
        }
        
        /// <summary>
        /// 清空选择
        /// </summary>
        public void ClearSelection()
        {
            selectedCards.Clear();
        }
        
        /// <summary>
        /// 尝试融合
        /// </summary>
        public CardData TryFusion()
        {
            if (selectedCards.Count != maxFusionCards)
            {
                Debug.Log("Need exactly 2 cards to fuse!");
                return null;
            }
            
            // 遍历所有融合配方，查找匹配的
            foreach (var recipe in fusionRecipes)
            {
                if (recipe.fusionRecipe == null || recipe.fusionRecipe.Length != maxFusionCards)
                    continue;
                
                // 检查配方是否匹配（顺序无关）
                if (IsRecipeMatch(recipe.fusionRecipe, selectedCards))
                {
                    Debug.Log($"Fusion successful! Created: {recipe.cardName}");
                    
                    // 从手牌中移除选中的卡牌
                    foreach (var card in selectedCards)
                    {
                        CardManager.Instance.hand.Remove(card);
                        CardManager.Instance.discard.Add(card);
                    }
                    
                    // 添加融合后的卡牌到手牌
                    CardManager.Instance.hand.Add(recipe);
                    
                    ClearSelection();
                    return recipe;
                }
            }
            
            Debug.Log("No matching fusion recipe found!");
            return null;
        }
        
        /// <summary>
        /// 检查配方是否匹配（顺序无关）
        /// </summary>
        private bool IsRecipeMatch(CardData[] recipe, List<CardData> selected)
        {
            if (recipe.Length != selected.Count)
                return false;
            
            var recipeList = recipe.ToList();
            var selectedCopy = new List<CardData>(selected);
            
            foreach (var card in recipeList)
            {
                if (!selectedCopy.Contains(card))
                    return false;
                selectedCopy.Remove(card);
            }
            
            return selectedCopy.Count == 0;
        }
        
        /// <summary>
        /// 获取可能的融合结果（用于 UI 提示）
        /// </summary>
        public CardData GetPossibleFusion()
        {
            if (selectedCards.Count != maxFusionCards)
                return null;
            
            foreach (var recipe in fusionRecipes)
            {
                if (recipe.fusionRecipe == null || recipe.fusionRecipe.Length != maxFusionCards)
                    continue;
                
                if (IsRecipeMatch(recipe.fusionRecipe, selectedCards))
                {
                    return recipe;
                }
            }
            
            return null;
        }
    }
}
