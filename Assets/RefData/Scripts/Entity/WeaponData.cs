using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class WeaponData
{
    public string id;
    public string name;
    public int useType;// 消耗品 0 | 手持品 1  
    public float maxDistance;// 距离
    public int weaponType;// 近战 0 | 远程 1
    public float damage;
    public float impactForce;// 冲击力，击退力
}
