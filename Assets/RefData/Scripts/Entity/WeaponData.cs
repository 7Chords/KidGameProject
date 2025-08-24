using KidGame;
using KidGame.Core;
using System;
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
    public float soundRange;
    public int longOrShortPress;// ʹ�÷�ʽ�ǳ������Ƕ̰�
    public UseItemType UseItemType => UseItemType.weapon;
    public string Id => id;
}
