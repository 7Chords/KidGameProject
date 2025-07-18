using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "New ParticleDataListSO", menuName = "CustomizedSO/ParticleDataListSO")]
    public class ParticleDatas : ScriptableObject
    {
        public List<ParticleData> particleDataList;
    }
}