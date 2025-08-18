using KidGame;
using KidGame.Core;
using System;
using System.Collections.Generic;

[Serializable]
public class TrapData : BagItemInfoBase
{
    public string id;
    public string trapName;
    public string trapDesc;
    public string trapIconPath;
    public int trapLevel;
    public float trapDamage; //这里的陷阱伤害是基础伤害 部分陷阱伤害还受其他因素影响 在trap的mono脚本中处理计算
    public int trapScore;
    public TrapTriggerType triggerType;
    public TrapValidType validType;
    public float validTime = -1; //有效时间 -1为可以立刻生效
    public List<TrapDeadType> deadTypeList;
    public float deadDelayTime;//死亡延迟时间 deadTypeList包含TimeDelay才有效
    public float placeTime;//放置所需要的时间
    public bool hasCollider;//是否有碰撞体
    public string interactSoundName;
    public string pickSoundName;
    public string workSoundName;
    public string deadSoundName;
    public string interactParticleName;
    public string pickParticleName;
    public string workParticleName;
    public string deadParticleName;
    public float soundRange;
    public UseItemType UseItemType => UseItemType.trap;
    public string Id => id;
}

//陷阱触发枚举
public enum TrapTriggerType
{
    Positive, //主动的
    Negative, //被动的
}

//陷阱生效枚举
public enum TrapValidType
{
    Immediate,//立刻生效
    TimeDelay, //等待一段时间后生效
}

//陷阱死亡枚举
public enum TrapDeadType
{
    Immediate,      // 立即死亡
    TimeDelay,      // 延迟死亡
    ExternalEvent   // 外部事件触发死亡
}