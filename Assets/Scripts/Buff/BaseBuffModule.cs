using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public abstract class BaseBuffModule : ScriptableObject
    {
        public abstract void Apply(BuffInfo buffInfo);
    }
}
