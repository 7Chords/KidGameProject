using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New EnemyBaseData", menuName = "KidGameSO/Enemy/EnemyBaseData")]
public class EnemyBaseData : ScriptableObject
{
    [Header("身份&资源")]
    public string EnemyName;
    public float MaxSanity = 100f;

    [Header("移动&抵抗")]
    public float MoveSpeed = 2.0f;      // 普通移动速度
    public float Resistance = 0.5f;     // 状态抗性或韧性

    [Header("感知范围")]
    public float VisionRange = 8f;      // 视觉感知半径
    public float HearingRange = 4f;     // 听觉感知半径
}
