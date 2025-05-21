using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoomData", menuName = "KidGameSO/Map/RoomData")]
public class RoomData : ScriptableObject
{
    public string RoomName;
    public List<MapItem> itemList = new List<MapItem>();
}
