using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class TrapEnitiy
{
    public String id;
    public string name;
    public int level;
    public trapType type;
    public string situation;
    public float setTime;
    public int damage;
    public float prepareTime;
    public float lastTime;
    public string detail;
}

public enum trapType
{
    initiative, // 主动触发
    passive, // 被动触发
    timing, // 定时
    mix, // 混合
}
