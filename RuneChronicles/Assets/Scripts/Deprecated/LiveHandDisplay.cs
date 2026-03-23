using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

namespace RuneChronicles
{
    /// <summary>
    /// 实时更新手牌显示
    /// </summary>
    public class LiveHandDisplay : MonoBehaviour
    {
        public Transform handContainer;
        public GameObject cardPrefabTemplate;
        
        private List<GameObject> displayedCards = new List<GameObject>();
        
        private void Update()
        {
            if (CardManager.Instance == null) return;
            
            // 检查手牌数量是否变化
            if (displayedCards.Count != CardManager.Instance.hand.Count)
            {
                RefreshHand();
            }
        }
        
        private void RefreshHand()
        {
            // 清空当前显示
            foreach (var card in displayedCards)
            {
                if (card != null)
                    Destroy(card);
            }
            displayedCards.Clear();
            
            // 显示手牌
            if (CardManager.Instance == null) return;
            
            foreach (var cardData in CardManager.Instance.hand)
            {
                var cardObj = CreateCardUI(cardData);
                if (cardObj != null)
                {
                    cardObj.transform.SetParent(handContainer, false);
                    displayedCards.Add(cardObj);
                }
            }
        }
        
        private GameObject CreateCardUI(CardData cardData)
        {
            var cardObj = new GameObject($"Card_{cardData.cardName}");
            
            var rect = cardObj.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(180, 240);
            
            // 背景
            var bg = cardObj.AddComponent<Image>();
            bg.color = Color.white;
            
            // 按钮
            var button = cardObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnCardClicked(cardData));
            
            // 卡牌名称
            var nameObj = new GameObject("Name");
            nameObj.transform.SetParent(cardObj.transform, false);
            var nameRect = nameObj.AddComponent<RectTransform>();
            nameRect.anchorMin = new Vector2(0, 0.75f);
            nameRect.anchorMax = new Vector2(1, 0.95f);
            nameRect.sizeDelta = Vector2.zero;
            var nameText = nameObj.AddComponent<TextMeshProUGUI>();
            nameText.text = cardData.cardName;
            nameText.fontSize = 24;
            nameText.alignment = TextAlignmentOptions.Center;
            nameText.color = Color.black;
            nameText.fontStyle = FontStyles.Bold;
            
            // 攻击力
            var atkObj = new GameObject("Attack");
            atkObj.transform.SetParent(cardObj.transform, false);
            var atkRect = atkObj.AddComponent<RectTransform>();
            atkRect.anchorMin = new Vector2(0, 0.4f);
            atkRect.anchorMax = new Vector2(1, 0.6f);
            atkRect.sizeDelta = Vector2.zero;
            var atkText = atkObj.AddComponent<TextMeshProUGUI>();
            atkText.text = $"ATK: {cardData.attack} (攻击)";
            atkText.fontSize = 18;
            atkText.alignment = TextAlignmentOptions.Center;
            atkText.color = Color.red;
            atkText.fontStyle = FontStyles.Bold;
            
            // 费用
            var costObj = new GameObject("Cost");
            costObj.transform.SetParent(cardObj.transform, false);
            var costRect = costObj.AddComponent<RectTransform>();
            costRect.anchorMin = new Vector2(0, 0.1f);
            costRect.anchorMax = new Vector2(1, 0.3f);
            costRect.sizeDelta = Vector2.zero;
            var costText = costObj.AddComponent<TextMeshProUGUI>();
            costText.text = $"Cost: {cardData.manaCost} (费用)";
            costText.fontSize = 16;
            costText.alignment = TextAlignmentOptions.Center;
            costText.color = Color.blue;
            
            return cardObj;
        }
        
        private void OnCardClicked(CardData cardData)
        {
            if (CardManager.Instance != null)
            {
                if (CardManager.Instance.PlayCard(cardData))
                {
                    Debug.Log($"Played: {cardData.cardName}");
                    RefreshHand();
                }
            }
        }
    }
}
