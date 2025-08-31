using KidGame.Core;
using System;
using UnityEngine;

namespace KidGame.core
{
    public class Telescope : OnHandWeaponBase
    {
        [SerializeField] private FocusLinesScript _m_focusLinesScript;// 聚焦线脚本引用

        protected override void Update()
        {
            base.Update();
        }
        public override void _WeaponUseLogic()
        {
            
        }
    }
}