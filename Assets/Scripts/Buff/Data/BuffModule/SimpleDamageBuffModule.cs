using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    /// <summary>
    /// һ������������buff
    /// </summary>
    [CreateAssetMenu(fileName = "New SimpleDamageBuffModule", menuName = "KidGameSO/Buff/Module/SimpleDamageBuffModule")]
    public class SimpleDamageBuffModule : BaseBuffModule
    {
        public float damge;
        public override void Apply(BuffInfo buffInfo)
        {
            
        }
    }
}
