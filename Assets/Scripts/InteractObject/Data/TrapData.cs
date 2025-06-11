using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//陷阱触发枚举 可组合
public enum TrapTriggerType
{
    Positive,//主动的
    Negative,//被动的
    Time_Valid,//等待一段时间后生效

}


[CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TileData")]
public class TrapData : ScriptableObject
{
    public string ID;
    public string trapName;
    public float trapLevel;
    public float trapDamage;//这里的陷阱伤害是基础伤害 部分陷阱伤害还受其他因素影响 在trap的mono脚本中处理计算
    public float validTime;//有效时间 -1为可以立刻生效
    public List<TrapTriggerType> triggerTypeList;


}
