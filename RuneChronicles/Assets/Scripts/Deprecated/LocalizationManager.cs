using UnityEngine;
using System.Collections.Generic;

namespace RuneChronicles
{
    /// <summary>
    /// 多语言系统 - 单例模式
    /// </summary>
    public class LocalizationManager : MonoBehaviour
    {
        public static LocalizationManager Instance { get; private set; }
        
        public enum Language
        {
            Chinese,    // 简体中文
            English,    // 英语
            Japanese,   // 日语
            Korean      // 韩语
        }
        
        public Language currentLanguage = Language.Chinese;
        
        // 多语言文本数据库
        private Dictionary<string, Dictionary<Language, string>> textDatabase;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeDatabase();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeDatabase()
        {
            textDatabase = new Dictionary<string, Dictionary<Language, string>>
            {
                // UI 文本
                { "ui.title", new Dictionary<Language, string> {
                    { Language.Chinese, "符文编年史 - 测试场景" },
                    { Language.English, "Rune Chronicles - Test Scene" },
                    { Language.Japanese, "ルーンクロニクル - テストシーン" },
                    { Language.Korean, "룬 크로니클 - 테스트 장면" }
                }},
                
                { "ui.hp", new Dictionary<Language, string> {
                    { Language.Chinese, "生命" },
                    { Language.English, "HP" },
                    { Language.Japanese, "体力" },
                    { Language.Korean, "생명" }
                }},
                
                { "ui.mana", new Dictionary<Language, string> {
                    { Language.Chinese, "魔法" },
                    { Language.English, "Mana" },
                    { Language.Japanese, "魔力" },
                    { Language.Korean, "마나" }
                }},
                
                { "ui.enemy_hp", new Dictionary<Language, string> {
                    { Language.Chinese, "敌人生命" },
                    { Language.English, "Enemy HP" },
                    { Language.Japanese, "敵の体力" },
                    { Language.Korean, "적 생명" }
                }},
                
                { "ui.hand", new Dictionary<Language, string> {
                    { Language.Chinese, "手牌" },
                    { Language.English, "Hand" },
                    { Language.Japanese, "手札" },
                    { Language.Korean, "손패" }
                }},
                
                { "ui.cards", new Dictionary<Language, string> {
                    { Language.Chinese, "张" },
                    { Language.English, "cards" },
                    { Language.Japanese, "枚" },
                    { Language.Korean, "장" }
                }},
                
                // 按钮文本
                { "button.draw_card", new Dictionary<Language, string> {
                    { Language.Chinese, "抽卡" },
                    { Language.English, "Draw Card" },
                    { Language.Japanese, "カードを引く" },
                    { Language.Korean, "카드 뽑기" }
                }},
                
                { "button.fuse_cards", new Dictionary<Language, string> {
                    { Language.Chinese, "融合" },
                    { Language.English, "Fuse Cards" },
                    { Language.Japanese, "融合" },
                    { Language.Korean, "융합" }
                }},
                
                { "button.end_turn", new Dictionary<Language, string> {
                    { Language.Chinese, "结束回合" },
                    { Language.English, "End Turn" },
                    { Language.Japanese, "ターン終了" },
                    { Language.Korean, "턴 종료" }
                }},
                
                // 卡牌文本
                { "card.attack", new Dictionary<Language, string> {
                    { Language.Chinese, "攻击" },
                    { Language.English, "ATK" },
                    { Language.Japanese, "攻撃" },
                    { Language.Korean, "공격" }
                }},
                
                { "card.cost", new Dictionary<Language, string> {
                    { Language.Chinese, "费用" },
                    { Language.English, "Cost" },
                    { Language.Japanese, "コスト" },
                    { Language.Korean, "비용" }
                }},
                
                // 卡牌名称
                { "card.fire", new Dictionary<Language, string> {
                    { Language.Chinese, "火焰" },
                    { Language.English, "Fire" },
                    { Language.Japanese, "炎" },
                    { Language.Korean, "불" }
                }},
                
                { "card.ice", new Dictionary<Language, string> {
                    { Language.Chinese, "冰霜" },
                    { Language.English, "Ice" },
                    { Language.Japanese, "氷" },
                    { Language.Korean, "얼음" }
                }},
                
                { "card.steam", new Dictionary<Language, string> {
                    { Language.Chinese, "蒸汽" },
                    { Language.English, "Steam" },
                    { Language.Japanese, "蒸気" },
                    { Language.Korean, "증기" }
                }},
                
                // 系统消息
                { "msg.fusion_success", new Dictionary<Language, string> {
                    { Language.Chinese, "融合成功！创建了: {0}" },
                    { Language.English, "Fusion successful! Created: {0}" },
                    { Language.Japanese, "融合成功！作成: {0}" },
                    { Language.Korean, "융합 성공! 생성됨: {0}" }
                }},
                
                { "msg.not_enough_mana", new Dictionary<Language, string> {
                    { Language.Chinese, "魔法不足！" },
                    { Language.English, "Not enough mana!" },
                    { Language.Japanese, "魔力不足！" },
                    { Language.Korean, "마나 부족!" }
                }},
                
                { "msg.no_cards", new Dictionary<Language, string> {
                    { Language.Chinese, "没有更多卡牌可以抽取！" },
                    { Language.English, "No more cards to draw!" },
                    { Language.Japanese, "これ以上引けるカードがありません！" },
                    { Language.Korean, "더 이상 뽑을 카드가 없습니다!" }
                }}
            };
        }
        
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        public string GetText(string key)
        {
            if (textDatabase.ContainsKey(key) && textDatabase[key].ContainsKey(currentLanguage))
            {
                return textDatabase[key][currentLanguage];
            }
            
            Debug.LogWarning($"Localization key not found: {key}");
            return $"[{key}]";
        }
        
        /// <summary>
        /// 获取格式化文本（支持参数）
        /// </summary>
        public string GetText(string key, params object[] args)
        {
            string text = GetText(key);
            return string.Format(text, args);
        }
        
        /// <summary>
        /// 切换语言
        /// </summary>
        public void SetLanguage(Language language)
        {
            currentLanguage = language;
            // 触发事件，通知所有UI更新
            OnLanguageChanged?.Invoke();
            Debug.Log($"Language changed to: {language}");
        }
        
        // 语言切换事件
        public System.Action OnLanguageChanged;
    }
}
