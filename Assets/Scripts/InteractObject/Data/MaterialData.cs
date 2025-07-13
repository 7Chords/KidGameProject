using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "MaterialData", menuName = "KidGameSO/Interactive/MaterialData")]
    public class MaterialData : ScriptableObject
    {
        public string materialID;
        public string materialName;
        public string materialDesc;
        public string materialIconPath;
        public string pickSoundPath;
        public string pickParticalPath;

    }
}