using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace RuneChronicles
{
    /// <summary>
    /// 简单的卡牌 UI 显示
    /// </summary>
    public class SimpleCardUI : MonoBehaviour
    {
        public CardData cardData;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI manaText;
        public Button cardButton;
        public Image background;
        
        [Header("Visual")]
        public Color normalColor = Color.white;
        public Color selectedColor = Color.yellow;
        
        private bool isSelected = false;
        
        private void Start()
        {
            if (cardButton != null)
            {
                cardButton.onClick.AddListener(OnCardClicked);
            }
            
            UpdateDisplay();
        }
        
        public void SetCard(CardData card)
        {
            cardData = card;
            UpdateDisplay();
        }
        
        private void UpdateDisplay()
        {
            if (cardData == null) return;
            
            if (nameText != null)
                nameText.text = cardData.cardName;
            
            if (attackText != null)
                attackText.text = $"ATK: {cardData.attack}";
            
            if (manaText != null)
                manaText.text = $"Cost: {cardData.manaCost}";
            
            UpdateVisual();
        }
        
        private void OnCardClicked()
        {
            // 检查是否在选择融合卡牌
            if (FusionSystem.Instance != null && 
                FusionSystem.Instance.selectedCards.Count < FusionSystem.Instance.maxFusionCards)
            {
                if (!isSelected)
                {
                    // 选择用于融合
                    if (FusionSystem.Instance.SelectCardForFusion(cardData))
                    {
                        isSelected = true;
                        UpdateVisual();
                    }
                }
                else
                {
                    // 取消选择
                    FusionSystem.Instance.DeselectCard(cardData);
                    isSelected = false;
                    UpdateVisual();
                }
            }
            else
            {
                // 直接使用卡牌
                if (CardManager.Instance != null)
                {
                    if (CardManager.Instance.PlayCard(cardData))
                    {
                        Debug.Log($"Played card: {cardData.cardName}");
                        // 刷新手牌显示
                        FindObjectOfType<SimpleHandDisplay>()?.RefreshHand();
                    }
                }
            }
        }
        
        private void UpdateVisual()
        {
            if (background != null)
            {
                background.color = isSelected ? selectedColor : normalColor;
            }
        }
        
        public void Deselect()
        {
            isSelected = false;
            UpdateVisual();
        }
    }
}
