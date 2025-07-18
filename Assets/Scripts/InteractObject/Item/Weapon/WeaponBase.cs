using KidGame.Interface;
using KidGame.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{

    public class WeaponBase : MapItem, IInteractive
    {
        //数据都去配表拿吧
        private WeaponData _weaponData;
        public override string EntityName { get => _weaponData.name;}
        public WeaponData weaponData
        {
            get
            {
                return _weaponData;
            }
        }

        protected virtual void Awake()
        {
            
        }

        protected virtual void Start()
        {
            //To do:生成的时候都要粘在角色手上
        }

        protected virtual void Update()
        {
            
        }
        protected virtual void InitWeaponData(string id, string dataName, string soName)
        {
            _weaponData = SoLoader.Instance.GetDataById(id, dataName, soName) as WeaponData;
        }
        /// <summary>
        /// 捡起物体的时候调用
        /// </summary>
        public override void Pick()
        {
            //他是通过Trigger类型Colider来增加物体的
            //如果捡起来的时候就移除
            PlayerController.Instance.RemovePickableFromList(this);
            //目前这个回调只注册了一个添加物品到背包的函数
            PlayerUtil.Instance.CallPlayerPickItem(this);
            //这个气泡也在上面这个回调加到列表里了   在这个地方移除
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);

            //TODO:武器捡起来可能还有什么要触发的东西 待定

            UIHelper.Instance.ShowOneTip(new TipInfo("获得了" + EntityName + "×1", gameObject));

            Destroy(gameObject);
        }

        public virtual void InteractPositive(GameObject interactor)
        {
            //To Do:武器可能会被用来附魔
        }

        //To Do:武器应该不需要被动互动 ？？  
        public virtual void InteractNegative(GameObject interactor)
        {

        }
    }

}
