using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 受力buff
    /// </summary>
    [CreateAssetMenu(fileName = "New AddForceBuffModule", menuName = "KidGameSO/Buff/Module/AddForceBuffModule")]
    public class AddForceBuffModule : BaseBuffModule
    {
        public override void Apply(BuffInfo buffInfo)
        {
            if (buffInfo.exParams.Length == 0) return;//应该是要额外传一个力的参数才对
            if (buffInfo.target == null) return;
            buffInfo.target.GetComponent<Rigidbody>().AddForce((Vector3)(buffInfo.exParams[0]),ForceMode.Impulse);
        }
    }
}
