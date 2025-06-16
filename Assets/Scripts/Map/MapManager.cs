using KidGame.Core.Data;

namespace KidGame.Core
{
    public class MapManager : Singleton<MapManager>
    {
        private MapData _mapData;

        public void Init()
        {

        }

        public void GenerateMap(MapData mapData)
        {
            _mapData = mapData;
        }

        public void Discard()
        {

        }
    }
}
