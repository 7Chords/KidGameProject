using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// ����buff
    /// </summary>
    [CreateAssetMenu(fileName = "New AddForceBuffModule", menuName = "KidGameSO/Buff/Module/AddForceBuffModule")]
    public class AddForceBuffModule : BaseBuffModule
    {
        public override void Apply(BuffInfo buffInfo)
        {
            if (buffInfo.exParams.Length == 0) return;//Ӧ����Ҫ���⴫һ�����Ĳ����Ŷ�
            if (buffInfo.target == null) return;
            buffInfo.target.GetComponent<Rigidbody>().AddForce((Vector3)(buffInfo.exParams[0]),ForceMode.Impulse);
        }
    }
}
