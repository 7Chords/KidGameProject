using KidGame.Core;
using KidGame.Interface;
using KidGame.UI;
using System.Collections.Generic;
using UnityEngine;


namespace KidGame.Core
{
    /// <summary>
    /// �������
    /// </summary>
    public class TrapBase : MapItem,IInteractive
    {

        public List<string> RandomInteractSfxList { get => randomInteractSfxList; set { randomInteractSfxList = value; } }
        public GameObject InteractPartical { get => interactPartical; set { interactPartical = value; } }



        public override string itemName => _trapData.trapName;

        [Space(20)]


        //����public
        public TrapData _trapData;

        public TrapData trapData => _trapData;

        private Rigidbody rb;
        public Rigidbody Rb => rb;


        #region ʱ����������ز���

        protected bool _isTimeValid;
        protected float _validTime;
        protected float _validTimer;

        #endregion

        private CatalystBase _catalyst;

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        /// <summary>
        /// ��ʼ��
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

        #region �����ӿڷ���ʵ��

        public override void InteractPositive()
        {
            if (_trapData == null) return;
            //��Ҫ��ý
            if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
            //�������������ͽ���
            if (!_trapData.trapTypeList.Contains(TrapType.Positive))
                return;
            //�ж������Ƿ���Ч
            if (!GetValidState()) return;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            //todo:������Ч����Ч
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
            //��Ҫ��ý
            if (_trapData.triggerType == TrapTriggerType.Catalyst) return;
            //���Ǳ��������ͽ���
            if (!_trapData.trapTypeList.Contains(TrapType.Negative))
                return;
            //�ж������Ƿ���Ч
            if (!GetValidState()) return;
            PlayerController.Instance.RemoveInteractiveFromList(this);
            //todo:������Ч����Ч
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
            MonoManager.Instance.InstantiateGameObject(pickPartical, transform.position,Quaternion.identity,1f);
            UIHelper.Instance.ShowTip("�����" + itemName+"��1",gameObject);
            Destroy(gameObject);
        }

        public virtual void Recycle()
        {
            Pick();
        }

        #endregion

        #region ������

        /// <summary>
        /// ʱ���������ʱ����
        /// </summary>
        public virtual void TimeValidTick()
        {
            if (!_isTimeValid) return;
            if (GetValidState()) return;
            _validTimer += Time.deltaTime;
        }

        /// <summary>
        /// �����Ƿ���Ч
        /// </summary>
        /// <returns></returns>
        public virtual bool GetValidState()
        {
            if (!_isTimeValid) return true;
            if (_validTimer >= _validTime) return true;
            return false;
        }

        /// <summary>
        /// ���ô�ý
        /// </summary>
        /// <param name="catalyst"></param>
        public void SetCatalyst(CatalystBase catalyst)
        {
            _catalyst = catalyst;
        }

        /// <summary>
        /// ͨ����ý����
        /// </summary>
        public void TriggerByCatalyst(CatalystBase catalyst)
        {
            if (_trapData == null || _trapData.triggerType != TrapTriggerType.Catalyst) return;
            if (_catalyst == null || _catalyst != catalyst) return;
            Trigger();
        }

        /// <summary>
        /// ���崥����Ч������
        /// </summary>
        public virtual void Trigger()
        {
            Debug.Log(gameObject.name + "���崥����");
        }

        #endregion


        private void RemoveFormPlayerUsingList()
        {
            PlayerController.Instance.RemoveInteractiveFromList(this);
            PlayerController.Instance.RemovePickableFromList(this);
            UIHelper.Instance.RemoveBubbleInfoFromList(gameObject);
        }
    }
}