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


    public class MapFurniture : MapItem, IInteractive
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;


        [SerializeField]
        private List<string> randomInteractSfxList;
        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }


        [SerializeField]
        private GameObject interactPartical;
        public GameObject InteractPartical { get => interactPartical; set{ interactPartical = value; } }


        public override string itemName { get => mapFurnitureData.furnitureData.furnitureName; set { } }


        public GameObject TrapDependPos;//陷阱放在家具上的放置位置

        protected TrapBase takeTrap;//家具上有的陷阱

        protected bool canInteract;
        public void SetData(MapFurnitureData data)
        {
            mapFurnitureData = data;
        }

        public virtual void SetTrap(GameObject trap)
        {
            trap.transform.position = TrapDependPos.transform.position;
            trap.transform.localScale *= 0.5f;
            takeTrap = trap.GetComponent<TrapBase>();
        }

        public virtual void Init(List<MaterialItem> materialList = null)
        {
            canInteract = mapFurnitureData.furnitureData.canInteract;
            if(materialList!= null)
            {
                materialHoldList = new List<MaterialItem>();
                foreach (var item in materialList)
                {
                    materialHoldList.Add(item);
                }
            }
            if (canInteract)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactive");
            }
        }

        public virtual void Discard()
        {
            materialHoldList.Clear();
            materialHoldList = null;
        }

        public override void Pick()
        {
            if (materialHoldList == null || materialHoldList.Count == 0)
            {
                UIHelper.Instance.ShowTipImmediate(new TipInfo("空空如也",gameObject));
            }
            else
            {
                MaterialBase tmpMat = new MaterialBase();
                foreach (var item in materialHoldList)
                {
                    for(int i =0;i<item.amount;i++)
                    {
                        tmpMat.Init(item.data);
                        PlayerUtil.Instance.CallPlayerPickItem(tmpMat);
                        UIHelper.Instance.ShowTipByQueue(new TipInfo("获得了" + item.data.materialName + "×1", gameObject,0.5f));
                    }
                }
                materialHoldList.Clear();
            }
        }

        public virtual void InteractPositive(GameObject interactor)
        {
        }

        public virtual void InteractNegative(CatalystBase catalyst, GameObject interactor)
        {
        }
    }
}
