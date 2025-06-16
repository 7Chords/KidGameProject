using System.Collections.Generic;

namespace KidGame.Core
{
    public interface IStateMachineOwner
    {
    }

    /// <summary>
    /// ״̬��������
    /// </summary>
    public class StateMachine
    {
        // ��ǰ״̬
        public int CurrStateType { get; private set; } = -1;

        // ��ǰ��Ч�е�״̬
        private StateBase currStateObj;

        // ����
        private IStateMachineOwner owner;

        // ���е�״̬ Key:״̬ö�ٵ�ֵ Value:�����״̬
        private Dictionary<int, StateBase> stateDic = new Dictionary<int, StateBase>();

        /// <summary>
        /// ��ʼ��
        /// </summary>
        /// <param name="owner">����</param>
        public void Init(IStateMachineOwner owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// �л�״̬
        /// </summary>
        /// <typeparam name="T">����Ҫ�л�����״̬�ű�����</typeparam>
        /// <param name="newState">��״̬</param>
        /// <param name="reCurrstate">��״̬�͵�ǰ״̬һ�µ�����£��Ƿ�ҲҪ�л�</param>
        /// <returns></returns>
        public bool ChangeState<T>(int newStateType, bool reCurrstate = false) where T : StateBase, new()
        {
            // ״̬һ�£����Ҳ���Ҫˢ��״̬�����л�ʧ��
            if (newStateType == CurrStateType && !reCurrstate) return false;

            // �˳���ǰ״̬
            if (currStateObj != null)
            {
                currStateObj.Exit();
                currStateObj.RemoveUpdate(currStateObj.Update);
                currStateObj.RemoveLateUpdate(currStateObj.LateUpdate);
                currStateObj.RemoveFixedUpdate(currStateObj.FixedUpdate);
            }

            // ������״̬
            currStateObj = GetState<T>(newStateType);
            CurrStateType = newStateType;
            currStateObj.Enter();
            currStateObj.OnUpdate(currStateObj.Update);
            currStateObj.OnLateUpdate(currStateObj.LateUpdate);
            currStateObj.OnFixedUpdate(currStateObj.FixedUpdate);

            return true;
        }

        /// <summary>
        /// �Ӷ���ػ�ȡһ��״̬
        /// </summary>
        private StateBase GetState<T>(int stateType) where T : StateBase, new()
        {
            if (stateDic.ContainsKey(stateType)) return stateDic[stateType];
            //TODO:�����
            StateBase state = PoolManager.Instance.GetObject<T>();
            state.Init(owner, stateType, this);
            stateDic.Add(stateType, state);
            return state;
        }

        /// <summary>
        /// ֹͣ����
        /// ������״̬���ͷţ�����StateMachineδ�������Թ���
        /// </summary>
        public void Stop()
        {
            // ����ǰ״̬�Ķ����߼�
            currStateObj.Exit();
            currStateObj.RemoveUpdate(currStateObj.Update);
            currStateObj.RemoveLateUpdate(currStateObj.LateUpdate);
            currStateObj.RemoveFixedUpdate(currStateObj.FixedUpdate);
            CurrStateType = -1;
            currStateObj = null;
            // ������������״̬���߼�
            var enumerator = stateDic.GetEnumerator();
            while (enumerator.MoveNext())
            {
                enumerator.Current.Value.UnInit();
            }

            stateDic.Clear();
        }

        /// <summary>
        /// ���٣�����Ӧ���ͷŵ�StateMachin������
        /// </summary>
        public void Destory()
        {
            // ��������״̬
            Stop();
            // ����������Դ������
            owner = null;

            // �Ž������
            this.ObjectPushPool();
        }
    }
}