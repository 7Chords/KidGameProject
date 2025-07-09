using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class StandLight : MapFurniture
    {

        public Light Light;

        private bool isLightOn;

        public override void Init(List<MaterialItem> materialList = null)
        {
            base.Init(materialList);
            isLightOn = true;
        }
        public override void InteractPositive(GameObject interactor)
        {
            isLightOn = !isLightOn;
            Light.gameObject.SetActive(isLightOn);
        }

    }

}
