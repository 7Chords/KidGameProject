using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// 陷阱基类
    /// </summary>
    public class TrapBase : MapItem,IRecyclable
    {
        [SerializeField]
        protected List<string> randomRecycleSfxList;
        public List<string> RandomRecycleSfxList { get => randomRecycleSfxList; set { randomRecycleSfxList = value; } }

        [SerializeField]
        protected ParticleSystem recyclePartical;
        public ParticleSystem RecyclePartical { get => recyclePartical; set { recyclePartical = value; } }

        public override string itemName => _trapData.trapName;

        [Space(20)]


        //测试public
        public TrapData _trapData;

        public TrapData trapData => _trapData;


        #region 时间型陷阱相关参数

        protected bool _isTimeValid;
        protected float _validTime;
        protected float _validTimer;

        #endregion

        private CatalystBase _catalyst;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="trapData"></param>
        public virtual void Init(TrapData trapData)
        {
            _trapData = trapData;
            _isTimeValid = _trapData.trapTypeList.Contains(TrapType.Time_Valid);
            _validTime = _trapData.validTime;
            _validTimer = 0;
        }

        protected virtual void Update()
        {
            TimeValidTick();
        }

        #region 交互接口方法实现

        public override void InteractPositive()
        {
            if (_trapData == null) return;
            //需要触媒
            if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
            //不是主动触发型交互
            if (!_trapData.trapTypeList.Contains(TrapType.Positive))
                return;
            //判断陷阱是否有效
            if (!GetValidState()) return;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            //todo:播放音效和特效
            if(RandomInteractSfxList != null && RandomInteractSfxList.Count>0)
            {
                AudioManager.Instance.PlaySfx(RandomInteractSfxList[Random.Range(0, RandomInteractSfxList.Count)]);
            }
            if(interactPartical != null)
            {
                Instantiate(interactPartical, transform.position, Quaternion.identity);
            }
            RemoveFormPlayerUsingList();
            Trigger();
        }

        public override void InteractNegative()
        {
            if (_trapData == null) return;
            //需要触媒
            if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
            //不是被动触发型交互
            if (!_trapData.trapTypeList.Contains(TrapType.Negative))
                return;
            //判断陷阱是否有效
            if (!GetValidState()) return;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            //todo:播放音效和特效
            if (RandomInteractSfxList != null && RandomInteractSfxList.Count > 0)
            {
                AudioManager.Instance.PlaySfx(RandomInteractSfxList[Random.Range(0, RandomInteractSfxList.Count)]);
            }
            if (interactPartical != null)
            {
                Instantiate(interactPartical, transform.position, Quaternion.identity);
            }
            RemoveFormPlayerUsingList();
            Trigger();
        }

        public override void Pick()
        {
            RemoveFormPlayerUsingList();
            PlayerUtil.Instance.CallPlayerPickItem(this);
            Destroy(gameObject);
        }

        public virtual void Recycle()
        {
            Pick();
        }

        #endregion

        #region 功能性

        /// <summary>
        /// 时间型陷阱计时更新
        /// </summary>
        public virtual void TimeValidTick()
        {
            if (!_isTimeValid) return;
            if (GetValidState()) return;
            _validTimer += Time.deltaTime;
        }

        /// <summary>
        /// 陷阱是否有效
        /// </summary>
        /// <returns></returns>
        public virtual bool GetValidState()
        {
            if (!_isTimeValid) return true;
            if (_validTimer >= _validTime) return true;
            return false;
        }

        /// <summary>
        /// 设置触媒
        /// </summary>
        /// <param name="catalyst"></param>
        public void SetCatalyst(CatalystBase catalyst)
        {
            _catalyst = catalyst;
        }

        /// <summary>
        /// 通过触媒触发
        /// </summary>
        public void TriggerByCatalyst(CatalystBase catalyst)
        {
            if (_trapData == null || _trapData.triggerType != TrapTriggerType.Catalyst) return;
            if (_catalyst == null || _catalyst != catalyst) return;
            Trigger();
        }

        /// <summary>
        /// 陷阱触发的效果代码
        /// </summary>
        public virtual void Trigger()
        {
            Debug.Log(gameObject.name + "陷阱触发了");
        }

        #endregion


        private void RemoveFormPlayerUsingList()
        {
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemoveRecyclableFromList(this);
            BubbleManager.Instance.RemoveBubbleInfoFromList(gameObject);
        }
    }
}