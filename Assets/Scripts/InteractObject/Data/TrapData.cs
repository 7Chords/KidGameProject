using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//���崥��ö�� �����
public enum TrapTriggerType
{
    Positive,//������
    Negative,//������
    Time_Valid,//�ȴ�һ��ʱ�����Ч

}


[CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TileData")]
public class TrapData : ScriptableObject
{
    public string ID;
    public string trapName;
    public float trapLevel;
    public float trapDamage;//����������˺��ǻ����˺� ���������˺�������������Ӱ�� ��trap��mono�ű��д������
    public float validTime;//��Чʱ�� -1Ϊ����������Ч
    public List<TrapTriggerType> triggerTypeList;


}
