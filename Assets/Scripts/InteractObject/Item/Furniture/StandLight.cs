using KidGame.Interface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    public class StandLight : MapFurniture,IInteractive
    {

        public Light Light;

        private bool isLightOn;

        public override void Init(List<MaterialItem> materialList = null)
        {
            base.Init(materialList);
            isLightOn = true;
        }

        public void InteractNegative(GameObject interactor) { }

        public void InteractPositive(GameObject interactor)
        {
            isLightOn = !isLightOn;
            Light.gameObject.SetActive(isLightOn);
        }

    }

}
