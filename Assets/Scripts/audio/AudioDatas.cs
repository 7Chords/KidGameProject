using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [System.Serializable]
    public class AudioData
    {
        public string audioName;

        public string audioPath;
    }


    [CreateAssetMenu(fileName = "New AudioDataListSO", menuName = "CustomizedSO/AudioDataListSO")]
    public class AudioDatas : ScriptableObject
    {
        public List<AudioData> audioDataList;
    }
}