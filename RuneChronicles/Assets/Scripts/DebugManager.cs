using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// DEBUG管理器 - 提供测试接口
/// Week 1 验收要求：至少5个DEBUG接口
/// </summary>
public class DebugManager : MonoBehaviour
{
    public static DebugManager Instance { get; private set; }

    // Debug Console 是否激活
    private bool consoleActive = false;
    private string consoleInput = "";
    private List<string> consoleLog = new List<string>();
    private const int MAX_LOG_LINES = 10;

    // GUI样式
    private GUIStyle consoleStyle;
    private GUIStyle inputStyle;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeStyles();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        // 按 ~ 键切换 Debug Console
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            consoleActive = !consoleActive;
            consoleInput = "";
        }

        // 在Console激活时处理回车
        if (consoleActive && Input.GetKeyDown(KeyCode.Return))
        {
            ProcessCommand(consoleInput);
            consoleInput = "";
        }
    }

    void OnGUI()
    {
        if (!consoleActive) return;

        // Debug Console窗口
        float width = Screen.width * 0.8f;
        float height = Screen.height * 0.4f;
        float x = Screen.width * 0.1f;
        float y = Screen.height * 0.3f;

        GUI.Box(new Rect(x, y, width, height), "Debug Console (按 ~ 关闭)", consoleStyle);

        // 显示日志
        float logY = y + 30;
        for (int i = 0; i < consoleLog.Count; i++)
        {
            GUI.Label(new Rect(x + 10, logY + i * 20, width - 20, 20), consoleLog[i], consoleStyle);
        }

        // 输入框
        GUI.Label(new Rect(x + 10, y + height - 50, 50, 30), ">", inputStyle);
        consoleInput = GUI.TextField(new Rect(x + 30, y + height - 50, width - 50, 30), consoleInput, inputStyle);
    }

    private void InitializeStyles()
    {
        consoleStyle = new GUIStyle();
        consoleStyle.normal.textColor = Color.green;
        consoleStyle.fontSize = 14;
        consoleStyle.normal.background = MakeTex(2, 2, new Color(0, 0, 0, 0.8f));

        inputStyle = new GUIStyle();
        inputStyle.normal.textColor = Color.white;
        inputStyle.fontSize = 16;
        inputStyle.normal.background = MakeTex(2, 2, new Color(0.2f, 0.2f, 0.2f, 0.8f));
    }

    private Texture2D MakeTex(int width, int height, Color col)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private void ProcessCommand(string command)
    {
        if (string.IsNullOrEmpty(command)) return;

        LogToConsole($"> {command}");

        string[] parts = command.ToLower().Split(' ');
        string cmd = parts[0];

        switch (cmd)
        {
            case "help":
                ShowHelp();
                break;
            case "hp":
                DebugSetHP(parts);
                break;
            case "energy":
                DebugSetEnergy(parts);
                break;
            case "add":
                DebugAddCard(parts);
                break;
            case "kill":
                DebugKillEnemy(parts);
                break;
            case "win":
                DebugWinBattle();
                break;
            case "clear":
                consoleLog.Clear();
                break;
            default:
                LogToConsole($"未知命令: {cmd}. 输入 'help' 查看帮助");
                break;
        }
    }

    private void LogToConsole(string message)
    {
        consoleLog.Add(message);
        if (consoleLog.Count > MAX_LOG_LINES)
            consoleLog.RemoveAt(0);
    }

    #region DEBUG接口实现

    /// <summary>
    /// DEBUG接口 #1: 显示帮助信息
    /// </summary>
    private void ShowHelp()
    {
        LogToConsole("=== 可用命令 ===");
        LogToConsole("hp [value] - 设置玩家生命值");
        LogToConsole("energy [value] - 设置玩家能量");
        LogToConsole("add [cardId] - 添加卡牌到手牌");
        LogToConsole("kill [index] - 杀死指定敌人");
        LogToConsole("win - 立即胜利");
        LogToConsole("clear - 清空日志");
    }

    /// <summary>
    /// DEBUG接口 #2: 设置玩家生命值
    /// 用法: hp 100
    /// </summary>
    private void DebugSetHP(string[] parts)
    {
        if (parts.Length < 2)
        {
            LogToConsole("用法: hp [value]");
            return;
        }

        if (int.TryParse(parts[1], out int hp))
        {
            if (Player.Instance != null)
            {
                Player.Instance.DEBUG_SetHP(hp);
                LogToConsole($"✓ 玩家HP设置为: {hp}");
            }
            else
            {
                LogToConsole("错误: Player未初始化");
            }
        }
        else
        {
            LogToConsole("错误: HP必须是数字");
        }
    }

    /// <summary>
    /// DEBUG接口 #3: 设置玩家能量
    /// 用法: energy 10
    /// </summary>
    private void DebugSetEnergy(string[] parts)
    {
        if (parts.Length < 2)
        {
            LogToConsole("用法: energy [value]");
            return;
        }

        if (int.TryParse(parts[1], out int energy))
        {
            if (BattleManager.Instance != null)
            {
                BattleManager.Instance.SetEnergy(energy);
                LogToConsole($"✓ 玩家能量设置为: {energy}");
            }
            else
            {
                LogToConsole("错误: BattleManager未初始化（需在战斗中使用）");
            }
        }
        else
        {
            LogToConsole("错误: 能量必须是数字");
        }
    }

    /// <summary>
    /// DEBUG接口 #4: 添加卡牌到手牌
    /// 用法: add ATK_001
    /// </summary>
    private void DebugAddCard(string[] parts)
    {
        if (parts.Length < 2)
        {
            LogToConsole("用法: add [cardId]");
            return;
        }

        string cardId = parts[1].ToUpper();
        if (CardManager.Instance != null && BattleManager.Instance != null)
        {
            var card = CardManager.Instance.GetCard(cardId);
            if (card != null)
            {
                BattleManager.Instance.DEBUG_AddCard(card);
                LogToConsole($"✓ 添加卡牌: {card.cardName}");
            }
            else
            {
                LogToConsole($"错误: 找不到卡牌 {cardId}");
            }
        }
        else
        {
            LogToConsole("错误: CardManager或BattleManager未初始化");
        }
    }

    /// <summary>
    /// DEBUG接口 #5: 杀死指定敌人
    /// 用法: kill 0
    /// </summary>
    private void DebugKillEnemy(string[] parts)
    {
        if (parts.Length < 2)
        {
            LogToConsole("用法: kill [index]");
            return;
        }

        if (int.TryParse(parts[1], out int index))
        {
            if (BattleManager.Instance != null)
            {
                var enemies = BattleManager.Instance.GetAliveEnemies();
                if (index >= 0 && index < enemies.Count)
                {
                    enemies[index].TakeDamage(enemies[index].currentHP);
                    LogToConsole($"✓ 杀死敌人 #{index}");
                }
                else
                {
                    LogToConsole($"错误: 没有索引为 {index} 的存活敌人");
                }
            }
            else
            {
                LogToConsole("错误: BattleManager未初始化（需在战斗中使用）");
            }
        }
        else
        {
            LogToConsole("错误: 索引必须是数字");
        }
    }

    /// <summary>
    /// DEBUG接口 #6 (额外): 立即胜利
    /// 用法: win
    /// </summary>
    private void DebugWinBattle()
    {
        if (BattleManager.Instance != null)
        {
            BattleManager.Instance.DEBUG_Win();
            LogToConsole("✓ 战斗胜利！");
        }
        else
        {
            LogToConsole("错误: BattleManager未初始化（需在战斗中使用）");
        }
    }

    #endregion

    #region 公开的DEBUG方法（供其他脚本调用）

    /// <summary>
    /// 公开DEBUG方法：设置HP
    /// </summary>
    public void SetPlayerHP(int hp)
    {
        DebugSetHP(new string[] { "hp", hp.ToString() });
    }

    /// <summary>
    /// 公开DEBUG方法：设置能量
    /// </summary>
    public void SetPlayerEnergy(int energy)
    {
        DebugSetEnergy(new string[] { "energy", energy.ToString() });
    }

    /// <summary>
    /// 公开DEBUG方法：添加卡牌
    /// </summary>
    public void AddCard(string cardId)
    {
        DebugAddCard(new string[] { "add", cardId });
    }

    /// <summary>
    /// 公开DEBUG方法：杀死敌人
    /// </summary>
    public void KillEnemy(int index)
    {
        DebugKillEnemy(new string[] { "kill", index.ToString() });
    }

    /// <summary>
    /// 公开DEBUG方法：胜利
    /// </summary>
    public void WinBattle()
    {
        DebugWinBattle();
    }

    #endregion
}
