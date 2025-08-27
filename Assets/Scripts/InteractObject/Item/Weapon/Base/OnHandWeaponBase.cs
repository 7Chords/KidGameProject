using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class OnHandWeaponBase : WeaponBase
    {
        protected override void Update()
        {
            WeaponUseLogic();
        }

        public override void _WeaponUseLogic() { }

    }
}
