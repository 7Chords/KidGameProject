using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    //���䳤������ί��ģ��
    public delegate void MsgRecAction(params object[] _objs);

    

    /// <summary>
    /// ��Ϣ����(����.Instance���� ���Ǿ�̬����) ui�õ���Signal 
    /// ע����Ҫע���вλ����޲εģ��޲εĴ�Act��׺ ������Ϣ��ʱ��ҲҪע�����������
    /// </summary>
    public class MsgCenter : SingletonNoMono<MsgCenter>
    {
        //�в��¼��ֵ�
        private Dictionary<int, List<MsgRecAction>> _m_broadcastDict = new Dictionary<int, List<MsgRecAction>>();
        //�޲��¼��ֵ�
        private Dictionary<int, List<Action>> _m_broadcastActDict = new Dictionary<int, List<Action>>();

        private List<MsgRecAction> _m_cacheBroadcastList;
        private List<Action> _m_cacheBroadcastActList;


        /// <summary>
        /// �㲥�¼����ص��޲Σ�
        /// </summary>
        /// <param name="_msg"></param>
        public static void SendMsgAct(int _msg)
        {
            Instance.sendMsgAct(_msg);
        }
        private void sendMsgAct(int _msg)
        {
            List<Action> srcList;

            if (_m_broadcastActDict.TryGetValue(_msg, out srcList) && srcList.Count > 0)
            {
                if (_m_cacheBroadcastActList == null)
                    _m_cacheBroadcastActList = new List<Action>();
                else
                    _m_cacheBroadcastActList.Clear();

                _m_cacheBroadcastActList.AddRange(srcList);
                for (int i = 0; i < _m_cacheBroadcastActList.Count; ++i)
                {
                    _m_cacheBroadcastActList[i]();
                }
            }
        }


        /// <summary>
        /// �㲥�¼����ص��вΣ�
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_obj">�ɱ����</param>
        public static void SendMsg(int _msg, params object[] _obj)
        {
            Instance.sendMsg(_msg, _obj);
        }

        private void sendMsg(int _msg, params object[] _obj)
        {
            List<MsgRecAction> srcList;
            if (_m_broadcastDict.TryGetValue(_msg, out srcList) && srcList.Count > 0)
            {
                if (_m_cacheBroadcastList == null)
                    _m_cacheBroadcastList = new List<MsgRecAction>();
                else
                    _m_cacheBroadcastList.Clear();

                _m_cacheBroadcastList.AddRange(srcList);
                for (int i = 0; i < _m_cacheBroadcastList.Count; ++i)
                {
                    _m_cacheBroadcastList[i](_obj);
                }
            }
        }




        /// <summary>
        /// ע���¼����ص��޲Σ�
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_callback"></param>
        public static void RegisterMsgAct(int _msg, Action _callback)
        {
            Instance.registerMsgAct(_msg, _callback);

        }

        private void registerMsgAct(int _msg, Action _callback)
        {
            List<Action> broadcast;
            if (!_m_broadcastActDict.TryGetValue(_msg, out broadcast))
            {
                broadcast = new List<Action>();
                _m_broadcastActDict[_msg] = broadcast;
            }

            if (!broadcast.Contains(_callback))
            {
                broadcast.Add(_callback);
            }
        }




        /// <summary>
        /// ע���¼����ص��вΣ�
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_callback"></param>
        public static void RegisterMsg(int _msg, MsgRecAction _callback)
        {
            Instance.registerMsg(_msg, _callback);
        }

        private void registerMsg(int _msg, MsgRecAction _callback)
        {
            List<MsgRecAction> broadcast;
            if (!_m_broadcastDict.TryGetValue(_msg, out broadcast))
            {
                broadcast = new List<MsgRecAction>();
                _m_broadcastDict[_msg] = broadcast;
            }

            if (!broadcast.Contains(_callback))
            {
                broadcast.Add(_callback);
            }
        }





        /// <summary>
        /// ��ע���¼����ص��޲Σ�
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_callback"></param>
        public static void UnregisterMsgAct(int _msg, Action _callback)
        {
            Instance.unregisterMsgAct(_msg, _callback);
        }

        private void unregisterMsgAct(int _msg, Action _callback)
        {
            List<Action> broadcast;
            if (!_m_broadcastActDict.TryGetValue(_msg, out broadcast))
            {
                return;
            }

            broadcast.Remove(_callback);
        }



        /// <summary>
        /// ��ע���¼����ص��вΣ�
        /// </summary>
        /// <param name="_msg"></param>
        /// <param name="_callback"></param>
        public static void UnregisterMsg(int _msg, MsgRecAction _callback)
        {
            Instance.unregisterMsg(_msg, _callback);
        }

        private void unregisterMsg(int _msg, MsgRecAction _callback)
        {
            List<MsgRecAction> broadcast;
            if (!_m_broadcastDict.TryGetValue(_msg, out broadcast))
            {
                return;
            }

            broadcast.Remove(_callback);

        }


    }
}
