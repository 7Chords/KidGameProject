using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class DamageInfo
    {
        public GameObject creator;

        public float damage;

        public BuffInfo buffInfo;

        public DamageInfo(GameObject creator, float damage, BuffInfo info = null)
        {
            this.creator = creator;
            this.damage = damage;
            buffInfo = info;
        }
    }
}