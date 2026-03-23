using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RuneChronicles
{
    /// <summary>
    /// 简单的手牌显示
    /// </summary>
    public class SimpleHandDisplay : MonoBehaviour
    {
        public GameObject cardPrefab;
        public Transform handContainer;
        
        private List<SimpleCardUI> displayedCards = new List<SimpleCardUI>();
        
        private void Start()
        {
            // 每秒刷新一次手牌显示
            InvokeRepeating(nameof(RefreshHand), 0.5f, 1f);
        }
        
        public void RefreshHand()
        {
            if (CardManager.Instance == null) return;
            
            // 清空当前显示
            foreach (var card in displayedCards)
            {
                if (card != null)
                    Destroy(card.gameObject);
            }
            displayedCards.Clear();
            
            // 显示手牌
            foreach (var cardData in CardManager.Instance.hand)
            {
                if (cardPrefab != null && handContainer != null)
                {
                    var cardObj = Instantiate(cardPrefab, handContainer);
                    var cardUI = cardObj.GetComponent<SimpleCardUI>();
                    if (cardUI != null)
                    {
                        cardUI.SetCard(cardData);
                        displayedCards.Add(cardUI);
                    }
                }
            }
        }
    }
}
