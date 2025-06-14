using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//����ö�� �����
public enum TrapType
{
    Positive,//������
    Negative,//������
    Time_Valid,//�ȴ�һ��ʱ�����Ч
}

//���崥��ö��
public enum TrapTriggerType
{
    Self,//�Լ�������
    Catalyst,//��ý����
}





[CreateAssetMenu(fileName = "TrapData", menuName = "KidGameSO/Interactive/TrapData")]
public class TrapData : ScriptableObject
{
    public string ID;
    public string trapName;
    public float trapLevel;
    public float trapDamage;//����������˺��ǻ����˺� ���������˺�������������Ӱ�� ��trap��mono�ű��д������
    public float validTime = -1;//��Чʱ�� -1Ϊ����������Ч
    public List<TrapType> trapTypeList;
    public TrapTriggerType triggerType;

}
