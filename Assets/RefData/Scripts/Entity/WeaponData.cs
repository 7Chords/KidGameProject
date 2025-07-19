using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public class WeaponData
{
    public string id;
    public string name;
    public string useType;// 消耗品 | 手持品
    public float maxDistance;// 距离
    public string weaponType;// 近战 | 远程
    public float damage;
    public float impactForce;// 冲击力，击退力
}
