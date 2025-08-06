using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerBaseData", menuName = "KidGameSO/Player/PlayerBaseData")]
public class PlayerBaseData : ScriptableObject
{
    public float WalkSpeed;
    public float RunSpeed;
    public int Hp;
    public float Sp;

    public float DashStaminaOneTime;
    public float RunStaminaPerSecond;
}