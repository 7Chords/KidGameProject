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
    主动触发,
    被动触发,
    定时,
    混合
}
