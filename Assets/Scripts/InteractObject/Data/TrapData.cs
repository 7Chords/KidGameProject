using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
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

    //陷阱放置枚举
    public enum TrapPlacedType
    {
        Ground,
        Furniture,
    }

    //陷阱死亡枚举
    public enum TrapDeadType
    {
        Immediate,      // 立即死亡
        TimeDelay,      // 延迟死亡
        ExternalEvent   // 外部事件触发死亡
    }

    [System.Serializable]
    public class TrapDeadConfig
    {
        public List<TrapDeadType> conditions;

        [Tooltip("仅当包含TimeDelay时有效")]
        public float delayTime;
    }


    [CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TrapData")]
    public class TrapData : ScriptableObject
    {
        public string trapID;
        public string trapName;
        public string trapDesc;
        public Sprite trapIcon;
        public float trapLevel;
        public float trapDamage; //这里的陷阱伤害是基础伤害 部分陷阱伤害还受其他因素影响 在trap的mono脚本中处理计算
        public TrapTriggerType triggerType;
        public TrapValidType validType;
        public float validTime = -1; //有效时间 -1为可以立刻生效
        public TrapPlacedType placedType;
        public List<TrapDeadType> deadTypeList;
        public float deadDelayTime;//死亡延迟时间 deadTypeList包含TimeDelay才有效
        //public bool isCatalyst;//是否是触媒
    }
}