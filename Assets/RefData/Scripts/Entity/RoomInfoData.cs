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
public class RoomInfoData
{
    public string id;
    public RoomType roomTyppe;
    public List<string> furnitureIdList;
    public List<string> materialIdList;
}

