using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RoomType
{
    Corridor, //����
    Bedroom, //����
    LivingRoom, //����
    DinningRoom, //����
    Study, //�鷿
    NurseryRoom, //��Ӥ��
    Bathroom, //ϴ�ּ�
              //ect...
}
[Serializable]
public class RoomData
{
    public string id;
    public RoomType roomTyppe;
    public List<string> materialIdList;
}

