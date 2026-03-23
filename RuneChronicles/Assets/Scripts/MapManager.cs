using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// 地图管理器
/// Week 3: 简单地图系统
/// </summary>
public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    
    [Header("地图配置")]
    public int totalFloors = 15; // 总层数
    public int nodesPerFloor = 3; // 每层节点数
    
    [Header("当前状态")]
    public int currentFloor = 0;
    public MapNode currentNode = null;
    
    // 地图数据
    private List<List<MapNode>> mapData = new List<List<MapNode>>();
    
    // 事件
    public event Action<MapNode> OnNodeEntered;
    public event Action<int> OnFloorChanged;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        GenerateMap();
    }
    
    #region 地图生成
    
    /// <summary>
    /// 生成地图
    /// </summary>
    public void GenerateMap()
    {
        mapData.Clear();
        
        for (int floor = 0; floor < totalFloors; floor++)
        {
            List<MapNode> floorNodes = new List<MapNode>();
            
            // 每层生成节点
            for (int i = 0; i < nodesPerFloor; i++)
            {
                MapNode node = GenerateNode(floor, i);
                floorNodes.Add(node);
            }
            
            mapData.Add(floorNodes);
        }
        
        Debug.Log($"[MapManager] 生成地图：{totalFloors}层，每层{nodesPerFloor}个节点");
    }
    
    /// <summary>
    /// 生成单个节点
    /// </summary>
    private MapNode GenerateNode(int floor, int index)
    {
        MapNode node = new MapNode
        {
            floor = floor,
            nodeIndex = index,
            nodeType = DetermineNodeType(floor)
        };
        
        return node;
    }
    
    /// <summary>
    /// 决定节点类型
    /// </summary>
    private MapNodeType DetermineNodeType(int floor)
    {
        // BOSS层（第5、10、15层）
        if ((floor + 1) % 5 == 0)
        {
            return MapNodeType.Boss;
        }
        
        // 精英层（第4、9、14层）
        if ((floor + 1) % 5 == 4)
        {
            return MapNodeType.Elite;
        }
        
        // 商店层（第3、8、13层）
        if ((floor + 1) % 5 == 3)
        {
            return MapNodeType.Shop;
        }
        
        // 宝箱层（第2、7、12层）
        if ((floor + 1) % 5 == 2)
        {
            return MapNodeType.Treasure;
        }
        
        // 其他层为普通战斗
        return MapNodeType.Battle;
    }
    
    #endregion
    
    #region 地图导航
    
    /// <summary>
    /// 进入节点
    /// </summary>
    public void EnterNode(MapNode node)
    {
        currentNode = node;
        currentFloor = node.floor;
        
        Debug.Log($"[MapManager] 进入节点：第{currentFloor + 1}层，类型：{node.nodeType}");
        
        OnNodeEntered?.Invoke(node);
        OnFloorChanged?.Invoke(currentFloor);
        
        // 执行节点事件
        ExecuteNodeEvent(node);
    }
    
    /// <summary>
    /// 执行节点事件
    /// </summary>
    private void ExecuteNodeEvent(MapNode node)
    {
        switch (node.nodeType)
        {
            case MapNodeType.Battle:
                StartBattle(false);
                break;
            
            case MapNodeType.Elite:
                StartBattle(true);
                break;
            
            case MapNodeType.Boss:
                StartBossBattle();
                break;
            
            case MapNodeType.Shop:
                OpenShop();
                break;
            
            case MapNodeType.Treasure:
                OpenTreasure();
                break;
        }
    }
    
    /// <summary>
    /// 开始战斗
    /// </summary>
    private void StartBattle(bool isElite)
    {
        Debug.Log($"[MapManager] 开始{(isElite ? "精英" : "普通")}战斗");
        
        // TODO: 生成敌人并开始战斗
        // BattleManager.Instance.StartBattle(...);
    }
    
    /// <summary>
    /// 开始BOSS战
    /// </summary>
    private void StartBossBattle()
    {
        Debug.Log("[MapManager] 开始BOSS战");
        
        // TODO: 生成BOSS并开始战斗
    }
    
    /// <summary>
    /// 打开商店
    /// </summary>
    private void OpenShop()
    {
        Debug.Log("[MapManager] 打开商店");
        
        // TODO: 显示商店UI
    }
    
    /// <summary>
    /// 打开宝箱
    /// </summary>
    private void OpenTreasure()
    {
        Debug.Log("[MapManager] 打开宝箱");
        
        // TODO: 给予奖励
        GiveReward();
    }
    
    /// <summary>
    /// 前往下一层
    /// </summary>
    public void MoveToNextFloor(int nodeIndex = 0)
    {
        if (currentFloor < totalFloors - 1)
        {
            currentFloor++;
            
            if (nodeIndex >= 0 && nodeIndex < nodesPerFloor)
            {
                EnterNode(mapData[currentFloor][nodeIndex]);
            }
        }
        else
        {
            Debug.Log("[MapManager] 已到达最后一层");
        }
    }
    
    /// <summary>
    /// 获取当前层的所有节点
    /// </summary>
    public List<MapNode> GetCurrentFloorNodes()
    {
        if (currentFloor >= 0 && currentFloor < mapData.Count)
        {
            return mapData[currentFloor];
        }
        
        return new List<MapNode>();
    }
    
    #endregion
    
    #region 奖励系统
    
    /// <summary>
    /// 给予奖励
    /// </summary>
    private void GiveReward()
    {
        // 随机奖励类型
        int rewardType = UnityEngine.Random.Range(0, 3);
        
        switch (rewardType)
        {
            case 0:
                // 金币
                int gold = UnityEngine.Random.Range(40, 80);
                Debug.Log($"[MapManager] 获得 {gold} 金币");
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.AddGold(gold);
                }
                break;
            
            case 1:
                // 卡牌
                Debug.Log("[MapManager] 选择一张卡牌加入牌库");
                // TODO: 显示卡牌选择UI
                break;
            
            case 2:
                // 融合点
                int fusionPoints = UnityEngine.Random.Range(1, 3);
                Debug.Log($"[MapManager] 获得 {fusionPoints} 融合点");
                FusionManager.Instance.GainFusionPoints(fusionPoints);
                break;
        }
    }
    
    #endregion
    
    #region DEBUG接口
    
    /// <summary>
    /// DEBUG: 跳转到指定层
    /// </summary>
    public void DEBUG_JumpToFloor(int floor)
    {
        if (floor >= 0 && floor < totalFloors)
        {
            currentFloor = floor;
            Debug.Log($"[MapManager] 跳转到第 {floor + 1} 层");
            OnFloorChanged?.Invoke(currentFloor);
        }
    }
    
    /// <summary>
    /// DEBUG: 打印地图
    /// </summary>
    public void DEBUG_PrintMap()
    {
        Debug.Log("=== 地图结构 ===");
        for (int i = 0; i < mapData.Count; i++)
        {
            string floorInfo = $"第{i + 1}层: ";
            foreach (var node in mapData[i])
            {
                floorInfo += $"[{node.nodeType}] ";
            }
            Debug.Log(floorInfo);
        }
    }
    
    #endregion
}

/// <summary>
/// 地图节点
/// </summary>
[Serializable]
public class MapNode
{
    public int floor;
    public int nodeIndex;
    public MapNodeType nodeType;
    public bool isCompleted = false;
}

/// <summary>
/// 地图节点类型
/// </summary>
public enum MapNodeType
{
    Battle,     // 普通战斗
    Elite,      // 精英战斗
    Boss,       // BOSS战
    Shop,       // 商店
    Treasure    // 宝箱
}
