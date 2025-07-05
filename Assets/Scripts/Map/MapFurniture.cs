using KidGame.Core.Data;
using UnityEngine;
using KidGame.Interface;
using System.Collections.Generic;
using KidGame.UI;

namespace KidGame.Core
{
    [System.Serializable]
    public class MaterialItem
    {
        public int amount;
        public MaterialData data;

        public MaterialItem(MaterialData data,int amount)
        {
            this.amount = amount;
            this.data = data;
        }
    }


    public class MapFurniture : MonoBehaviour, IPickable,IInteractive
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;

        [SerializeField]
        private List<string> randomPickSfxList;
        public List<string> RandomPickSfxList { get => randomPickSfxList; set { } }

        [SerializeField]
        private GameObject pickPartical;
        public GameObject PickPartical { get => pickPartical; set { } }


        [SerializeField]
        private List<string> randomInteractSfxList;
        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }

        [SerializeField]
        public GameObject interactPartical;
        public GameObject InteractPartical { get => interactPartical; set { interactPartical = value; } }

        public string itemName { get => mapFurnitureData.furnitureData.furnitureName; set { } }

        public GameObject TrapDependPos;//陷阱放在家具上的放置位置

        private TrapBase takeTrap;//家具上有的陷阱

        private bool canInteract;
        public MapFurniture(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }
        public void SetData(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }

        public void SetTrap(TrapBase trap)
        {
            takeTrap = trap;
        }

        public void Init(bool canInteract, List<MaterialItem> materialList = null)
        {
            this.canInteract = canInteract;
            materialHoldList = materialList;
            if (canInteract)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactive");
            }
        }

        public void Discard()
        {
            materialHoldList.Clear();
            materialHoldList = null;
        }

        public virtual void Pick()
        {

        }

        public virtual void InteractPositive(GameObject interactor) { }

        public virtual void InteractNegative(CatalystBase catalyst, GameObject interactor) { }

    }
}
