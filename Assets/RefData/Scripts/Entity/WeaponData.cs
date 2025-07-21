using System;
using System.Collections;
using System.Collections.Generic;
using KidGame.Core;
using UnityEngine;
[Serializable]
public class WeaponData : BagItemInfoBase
{
    public string id;
    public string name;
    public int useType;// ����Ʒ 0 | �ֳ�Ʒ 1  
    public float maxDistance;// ����
    public int weaponType;// ��ս 0 | Զ�� 1
    public float damage;
    public float impactForce;// �������������
    public UseItemType UseItemType => UseItemType.weapon;
}
