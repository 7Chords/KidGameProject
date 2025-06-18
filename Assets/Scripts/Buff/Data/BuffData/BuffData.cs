using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [CreateAssetMenu(fileName = "New BuffData", menuName = "KidGameSO/Buff/BuffData")]
    public class BuffData : ScriptableObject
    {
        //������Ϣ
        public string id;

        public string buffName;

        public string description;

        public Sprite icon;

        public int priority;//���ȼ� �����ж���������ִ����һ��

        public int maxStack;//���ɵ�������

        public string[] tags;//buff��һЩ������� �˺��͵� 

        //ʱ����Ϣ

        public bool isForever;//�Ƿ�������buff

        public float duration;//����ʱ��

        public float tickTime;//�������ʱ��

        //���·�ʽ

        public BuffUpdateTimeEnum buffUpdateTime;

        public BuffRemoveStackUpdateEnum buffRemoveStackUpdate;

        //�����ص���
        public BaseBuffModule OnCreate;

        public BaseBuffModule OnRemove;

        public BaseBuffModule OnTick;

        //�˺��ص���

        public BaseBuffModule OnHit;

        public BaseBuffModule OnBeHurt;

        public BaseBuffModule OnKill;

        public BaseBuffModule OnBeKill;

        //etc...


    }
}
