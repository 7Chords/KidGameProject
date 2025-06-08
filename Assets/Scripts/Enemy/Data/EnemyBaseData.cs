using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New EnemyBaseData", menuName = "KidGameSO/Enemy/EnemyBaseData")]
public class EnemyBaseData : ScriptableObject
{
    [Header("���&��Դ")]
    public string EnemyName;
    public float MaxSanity = 100f;

    [Header("�ƶ�&�ֿ�")]
    public float MoveSpeed = 2.0f;      // ��ͨ�ƶ��ٶ�
    public float Resistance = 0.5f;     // ״̬���Ի�����

    [Header("��֪��Χ")]
    public float VisionRange = 8f;      // �Ӿ���֪�뾶
    public float HearingRange = 4f;     // ������֪�뾶
}
