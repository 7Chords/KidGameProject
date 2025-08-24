using KidGame;
using KidGame.Core;
using System;
[Serializable]
public class WeaponData : BagItemInfoBase
{
    public string id;
    public string name;
    public int useType;// 消耗品 0 | 手持品 1  
    public float maxDistance;// 距离
    public int weaponType;// 近战 0 | 远程 1
    public float damage;
    public float impactForce;// 冲击力，击退力
    public float soundRange;
    public int longOrShortPress;// 使用方式是长按还是短按
    public UseItemType UseItemType => UseItemType.weapon;
    public string Id => id;
}
