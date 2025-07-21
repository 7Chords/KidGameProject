using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum RoomType
{
    Corridor, //走廊
    Bedroom, //卧室
    LivingRoom, //客厅
    DinningRoom, //餐厅
    Study, //书房
    NurseryRoom, //育婴室
    Bathroom, //洗手间
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

