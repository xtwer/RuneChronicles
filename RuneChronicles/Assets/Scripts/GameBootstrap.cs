using UnityEngine;

/// <summary>
/// 游戏启动器 - 自动初始化所有系统
/// 使用方法：创建空GameObject，添加此组件，点击Play
/// </summary>
public class GameBootstrap : MonoBehaviour
{
    void Start()
    {
        Debug.Log("=== 游戏启动中 ===");
        
        InitializeManagers();
        StartGame();
        
        Debug.Log("=== 游戏启动完成 ===");
    }
    
    void InitializeManagers()
    {
        Debug.Log("[Bootstrap] 初始化管理器...");
        
        // 创建所有Manager
        if (GameManager.Instance == null)
        {
            var gmObj = new GameObject("GameManager");
            gmObj.AddComponent<GameManager>();
        }
        
        if (CardManager.Instance == null)
        {
            var cmObj = new GameObject("CardManager");
            cmObj.AddComponent<CardManager>();
        }
        
        if (BattleManager.Instance == null)
        {
            var bmObj = new GameObject("BattleManager");
            bmObj.AddComponent<BattleManager>();
        }
        
        if (FusionManager.Instance == null)
        {
            var fmObj = new GameObject("FusionManager");
            fmObj.AddComponent<FusionManager>();
        }
        
        if (MapManager.Instance == null)
        {
            var mmObj = new GameObject("MapManager");
            mmObj.AddComponent<MapManager>();
        }
        
        if (CharacterManager.Instance == null)
        {
            var chObj = new GameObject("CharacterManager");
            chObj.AddComponent<CharacterManager>();
        }
        
        if (RelicManager.Instance == null)
        {
            var rmObj = new GameObject("RelicManager");
            rmObj.AddComponent<RelicManager>();
        }
        
        if (Player.Instance == null)
        {
            var plObj = new GameObject("Player");
            plObj.AddComponent<Player>();
        }
        
        if (AudioManager.Instance == null)
        {
            var amObj = new GameObject("AudioManager");
            amObj.AddComponent<AudioManager>();
        }
        
        if (DebugManager.Instance == null)
        {
            var dmObj = new GameObject("DebugManager");
            dmObj.AddComponent<DebugManager>();
        }
        
        if (ResourceManager.Instance == null)
        {
            var resObj = new GameObject("ResourceManager");
            resObj.AddComponent<ResourceManager>();
        }
        
        Debug.Log("[Bootstrap] 所有管理器初始化完成");
    }
    
    void StartGame()
    {
        Debug.Log("[Bootstrap] 启动主菜单...");
        
        // 创建主菜单
        var menuObj = new GameObject("MainMenuUI");
        menuObj.AddComponent<MainMenuUI>();
        
        // 创建EventSystem（如果不存在）
        if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            var eventSystemObj = new GameObject("EventSystem");
            eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }
    }
}
