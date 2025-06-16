using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
//陷阱枚举 可组合
    public enum TrapType
    {
        Positive, //主动的
        Negative, //被动的
        Time_Valid, //等待一段时间后生效
    }

//陷阱触发枚举
    public enum TrapTriggerType
    {
        Self, //自己能作用
        Catalyst, //触媒引发
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
        public float validTime = -1; //有效时间 -1为可以立刻生效
        public List<TrapType> trapTypeList;
        public TrapTriggerType triggerType;
    }
}