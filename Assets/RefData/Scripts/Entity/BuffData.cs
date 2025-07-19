using System;
using System.Collections.Generic;
using UnityEngine;

//buff添加新的层数时的时间更新方式
public enum BuffAddStackUpdateType
{
    Add,//在原有时间上增加
    Replace,//替换为新的时间（刷新）
    Keep,//时间保持不变
}

//buff移除时的更新方式
public enum BuffRemoveStackUpdateType
{
    Clear,//消除所有buff层数 
    Reduce,//减少buff层数
}

[Serializable]
public class BuffData
{
    //基本信息
    public string id;

    public string buffName;

    public string description;

    public string iconPath;

    public int priority;//优先级 例如中毒和烧伤先执行哪一个

    public int maxStack;//最大可叠层数量

    public List<string> tags;//buff的一些特征标记 伤害型等 

    //时间信息

    public bool isForever;//是否是永久buff

    public float duration;//持续时间

    public float tickTime;//间隔触发时间

    //更新方式

    public BuffAddStackUpdateType buffAddStackUpdateTime;

    public BuffRemoveStackUpdateType buffRemoveStackUpdate;

    //基础回调点
    public string onCreateModulePath;

    public string onRemoveModulePath;

    public string onTickModulePath;

    //伤害回调点

    public string onHitModulePath;

    public string onBeHurtModulePath;

    //etc...
}

