using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerLevelData
{
    public int level;
    public int maxHp;
    public int maxMp;
    public int maxExp;
}

public class GlobalData : SingletonMonoBehavior<GlobalData>
{
    [SerializeField] List<PlayerLevelData> playerDatas;
    public Dictionary<int, PlayerLevelData> playerDataMap = new Dictionary<int, PlayerLevelData>();
    protected override void OnInit()
    {
        playerDataMap = playerDatas.ToDictionary(x => x.level);
    }
}
