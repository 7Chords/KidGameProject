using KidGame.Core.Data;
using UnityEngine;

namespace KidGame.Core
{
    public class MapWall : MonoBehaviour
    {
        public MapWallData mapWallData;

        public MapWall(MapWallData data)
        {
            mapWallData = data;
        }
        public void SetData(MapWallData data)
        {
            mapWallData = data;
        }
    }
}
