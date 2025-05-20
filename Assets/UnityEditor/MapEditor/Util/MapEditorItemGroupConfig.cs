using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapEditorItemGroupConfig",menuName = "KidGameSO/Editor/MapEditorItemGroupConfig")]
public class MapEditorItemGroupConfig : ScriptableObject
{
    [Header("Grid")]
    public List<GridData> gridList;

    [Header("Blueprint")]
    public List<RoomData> RoomList;
    public List<MapData> MapListList;

}
