using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace RuneChronicles
{
    public class ChineseHandDisplay : MonoBehaviour
    {
        public Transform handContainer;
        private List<GameObject> displayedCards = new List<GameObject>();
        
        private void Update()
        {
            if (CardManager.Instance == null) return;
            
            if (displayedCards.Count != CardManager.Instance.hand.Count)
            {
                RefreshHand();
            }
        }
        
        private void RefreshHand()
        {
            foreach (var card in displayedCards)
            {
                if (card != null) Destroy(card);
            }
            displayedCards.Clear();
            
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
            rect.sizeDelta = new Vector2(180, 260);
            
            var bg = cardObj.AddComponent<Image>();
            bg.color = GetRarityColor(cardData.rarity);
            
            var button = cardObj.AddComponent<Button>();
            button.onClick.AddListener(() => OnCardClicked(cardData));
            
            // 卡牌名称
            CreateCardText(cardObj.transform, "Name", cardData.cardName, 
                new Vector2(0, 0.8f), new Vector2(1, 0.95f), 22, Color.black, FontStyle.Bold);
            
            // 攻击力
            CreateCardText(cardObj.transform, "Attack", $"攻击: {cardData.attack}", 
                new Vector2(0, 0.45f), new Vector2(1, 0.65f), 20, Color.red, FontStyle.Bold);
            
            // 费用
            CreateCardText(cardObj.transform, "Cost", $"费用: {cardData.manaCost}", 
                new Vector2(0, 0.15f), new Vector2(1, 0.35f), 18, new Color(0, 0.3f, 0.8f), FontStyle.Normal);
            
            return cardObj;
        }
        
        private void CreateCardText(Transform parent, string name, string text, 
            Vector2 anchorMin, Vector2 anchorMax, int fontSize, Color color, FontStyle style)
        {
            var textObj = new GameObject(name);
            textObj.transform.SetParent(parent, false);
            
            var rect = textObj.AddComponent<RectTransform>();
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.sizeDelta = Vector2.zero;
            
            var textComp = textObj.AddComponent<Text>();
            textComp.text = text;
            textComp.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            textComp.fontSize = fontSize;
            textComp.color = color;
            textComp.fontStyle = style;
            textComp.alignment = TextAnchor.MiddleCenter;
        }
        
        private Color GetRarityColor(CardRarity rarity)
        {
            switch (rarity)
            {
                case CardRarity.Common: return new Color(0.9f, 0.9f, 0.9f, 1f); // 灰白
                case CardRarity.Rare: return new Color(0.7f, 0.85f, 1f, 1f); // 蓝色
                case CardRarity.Epic: return new Color(0.8f, 0.6f, 1f, 1f); // 紫色
                case CardRarity.Legendary: return new Color(1f, 0.8f, 0.3f, 1f); // 金色
                default: return Color.white;
            }
        }
        
        private void OnCardClicked(CardData cardData)
        {
            // 检查游戏状态
            if (GameManager.Instance != null && GameManager.Instance.currentState != GameState.Playing)
            {
                Debug.Log("游戏已结束，无法使用卡牌！");
                return;
            }
            
            if (CardManager.Instance != null)
            {
                if (CardManager.Instance.PlayCard(cardData))
                {
                    Debug.Log($"使用了卡牌: {cardData.cardName}");
                    RefreshHand();
                }
            }
        }
    }
}
