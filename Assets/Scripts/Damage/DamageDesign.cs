using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class DamageInfo
    {
        public GameObject creator;

        public float damage;

        public DamageInfo(GameObject creator, float damage)
        {
            this.creator = creator;
            this.damage = damage;
        }
    }
}