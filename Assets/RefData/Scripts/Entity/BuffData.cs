using System;
using UnityEngine;

//buff����µĲ���ʱ��ʱ����·�ʽ
public enum BuffAddStackUpdateType
{
    Add,//��ԭ��ʱ��������
    Replace,//�滻Ϊ�µ�ʱ�䣨ˢ�£�
    Keep,//ʱ�䱣�ֲ���
}

//buff�Ƴ�ʱ�ĸ��·�ʽ
public enum BuffRemoveStackUpdateType
{
    Clear,//��������buff���� 
    Reduce,//����buff����
}

[Serializable]
public class BuffData
{
    //������Ϣ
    public string id;

    public string buffName;

    public string description;

    public Sprite icon;

    public int priority;//���ȼ� �����ж���������ִ����һ��

    public int maxStack;//���ɵ�������

    public string[] tags;//buff��һЩ������� �˺��͵� 

    //ʱ����Ϣ

    public bool isForever;//�Ƿ�������buff

    public float duration;//����ʱ��

    public float tickTime;//�������ʱ��

    //���·�ʽ

    public BuffAddStackUpdateType buffUpdateTime;

    public BuffRemoveStackUpdateType buffRemoveStackUpdate;

    //�����ص���
    public string OnCreateModuleId;

    public string OnRemoveModuleId;

    public string OnTickModuleId;

    //�˺��ص���

    public string OnHitModuleId;

    public string OnBeHurtModuleId;

    public string OnKillModuleId;

    public string OnBeKillModuleId;

    //etc...
}

