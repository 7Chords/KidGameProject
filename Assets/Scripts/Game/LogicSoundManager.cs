using KidGame.Interface;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    public class LogicSoundManager : SingletonNoMono<LogicSoundManager>
    {
        private List<ISoundable> soundableList;
        public void Init()
        {
            soundableList = new List<ISoundable>();
        }
        public void Discard()
        {
            soundableList.Clear();
            soundableList = null;
        }
        public void RegSoundable(ISoundable soundable)
        {
            if (soundableList == null)
                return;
            if (!soundableList.Contains(soundable))
                soundableList.Add(soundable);
        }

        public void UnregSoundable(ISoundable soundable)
        {
            if (soundableList == null)
                return;
            if (soundableList.Contains(soundable))
                soundableList.Remove(soundable);
        }

        /// <summary>
        /// �����������������Ķ���
        /// </summary>
        public ISoundable GetNearestReceiver(ISoundable source, float range)
        {
            float nearest = float.MaxValue;
            float currentDis = 0;
            ISoundable nearestSoundable = null;
            foreach (var soundable in soundableList)
            {
                if (soundable == source)
                    continue;
                currentDis = Vector3.Distance(soundable.SoundGameObject.transform.position, 
                    source.SoundGameObject.transform.position);
                if (currentDis < range && currentDis< nearest)
                {
                    nearest = currentDis;
                    nearestSoundable = soundable;
                }
            }
            return nearestSoundable;
        }
        /// <summary>
        /// ����������������Ķ���
        /// </summary>
        public List<ISoundable> GetAllReceiver(ISoundable source, float range)
        {
            float currentDis = 0;
            List<ISoundable> receiverList = new List<ISoundable>();
            foreach (var soundable in soundableList)
            {
                if (soundable == source)
                    continue;
                currentDis = Vector3.Distance(soundable.SoundGameObject.transform.position,
                    source.SoundGameObject.transform.position);
                if (currentDis < range)
                {
                    receiverList.Add(soundable);
                }
            }
            return receiverList;
        }
    }
}
