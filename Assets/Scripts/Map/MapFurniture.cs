using KidGame.Core.Data;
using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;

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


    public class MapFurniture : MapEntity
    {
        public MapFurnitureData mapFurnitureData;

        public List<MaterialItem> materialHoldList;


        public override string EntityName { get => mapFurnitureData.furnitureData.furnitureName; set { } }


        public GameObject TrapDependPos;//������ڼҾ��ϵķ���λ��

        protected TrapBase takeTrap;//�Ҿ����е�����

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
            if(materialList!= null)
            {
                materialHoldList = new List<MaterialItem>();
                foreach (var item in materialList)
                {
                    materialHoldList.Add(item);
                }
            }
            if(this is IInteractive)
            {
                gameObject.layer = LayerMask.NameToLayer("Interactive");
            }
        }

        public virtual void Discard()
        {
            materialHoldList.Clear();
            materialHoldList = null;
        }

    }
}
