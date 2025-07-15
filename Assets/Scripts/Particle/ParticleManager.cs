using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.Core
{
    [Serializable]
    public class ParticleData
    {
        public string effectName;
        public string effectPath;
    }
    
    [Serializable]
    public class ParticleInfo
    {
        public string effectName;
        public ParticleSystem particleSystem;
        public Coroutine autoDestroyCoroutine;
    }
    
    public class ParticleManager : SingletonPersistent<ParticleManager>
    {
        public List<ParticleInfo> activeParticleEffects = new List<ParticleInfo>();
        public ParticleDatas particleDatas;
        private GameObject _particleRootGO;

        protected override void Awake()
        {
            base.Awake();
            
            _particleRootGO = new GameObject("PARTICLE_ROOT");
            _particleRootGO.transform.SetParent(transform);
        }

        #region 基础播放方法

        /// <summary>
        /// 播放粒子特效
        /// </summary>
        /// <param name="effectName">特效名称</param>
        /// <param name="position">生成位置</param>
        /// <param name="rotation">生成旋转</param>
        /// <param name="parent">父物体</param>
        /// <param name="autoDestroy">是否自动销毁</param>
        /// <param name="destroyDelay">销毁延迟时间</param>
        /// <param name="scale">缩放比例</param>
        public ParticleSystem PlayEffect(string effectName,
            Vector3 position,
            Quaternion rotation = default,
            Transform parent = null,
            bool autoDestroy = true,
            float destroyDelay = -1f,
            Vector3? scale = null)
        {
            // 从资源加载粒子特效
            ParticleData effectData = particleDatas.particleDataList.Find(x => x.effectName == effectName);

            if (effectData == null)
            {
                Debug.LogWarning("未找到粒子特效：" + effectName);
                return null;
            }

            // 创建粒子特效实例
            GameObject effectGO = new GameObject(effectName);
            effectGO.transform.SetParent(parent != null ? parent : _particleRootGO.transform);
            effectGO.transform.position = position;
            effectGO.transform.rotation = rotation == default ? Quaternion.identity : rotation;

            if (scale.HasValue)
            {
                effectGO.transform.localScale = scale.Value;
            }
            
            GameObject prefab = Resources.Load<GameObject>(effectData.effectPath);
            if (prefab == null)
            {
                Debug.LogWarning($"粒子特效预制体加载失败：{effectData.effectPath}");
                Destroy(effectGO);
                return null;
            }

            GameObject instance = Instantiate(prefab, effectGO.transform);
            ParticleSystem particleSystem = instance.GetComponent<ParticleSystem>();
            if (particleSystem == null)
            {
                particleSystem = instance.AddComponent<ParticleSystem>();
            }
            
            ParticleInfo info = new ParticleInfo
            {
                effectName = effectName,
                particleSystem = particleSystem
            };

            particleSystem.Play();
            
            if (autoDestroy)
            {
                float delay = destroyDelay >= 0 ? destroyDelay : particleSystem.main.duration;
                info.autoDestroyCoroutine = StartCoroutine(AutoDestroyEffect(info, delay));
            }

            activeParticleEffects.Add(info);
            return particleSystem;
        }

        #endregion

        #region 重载

        // 重载1：仅需特效名称和位置
        public ParticleSystem PlayEffect(string effectName, Vector3 position)
        {
            return PlayEffect(effectName, position, default, null, true, -1f, null);
        }

        // 重载2：指定位置和旋转
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Quaternion rotation)
        {
            return PlayEffect(effectName, position, rotation, null, true, -1f, null);
        }

        // 重载3：指定父物体
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Transform parent)
        {
            return PlayEffect(effectName, position, default, parent, true, -1f, null);
        }

        // 重载4：完全自定义参数
        public ParticleSystem PlayEffect(string effectName,
            Vector3 position,
            Quaternion rotation,
            Transform parent,
            bool autoDestroy,
            float destroyDelay)
        {
            return PlayEffect(effectName, position, rotation, parent, autoDestroy, destroyDelay, null);
        }

        // 重载5：自定义缩放
        public ParticleSystem PlayEffect(string effectName, Vector3 position, Vector3 scale)
        {
            return PlayEffect(effectName, position, default, null, true, -1f, scale);
        }

        #endregion

        #region 特效控制

        /// <summary>
        /// 停止粒子特效
        /// </summary>
        /// <param name="effectName">特效名称</param>
        /// <param name="immediate">是否立即销毁</param>
        public void StopEffect(string effectName, bool immediate = false)
        {
            ParticleInfo info = activeParticleEffects.Find(x => x.effectName == effectName);
            if (info == null)
            {
                Debug.LogWarning("未找到活跃的粒子特效：" + effectName);
                return;
            }

            if (info.autoDestroyCoroutine != null)
            {
                StopCoroutine(info.autoDestroyCoroutine);
            }

            if (immediate)
            {
                Destroy(info.particleSystem.gameObject);
                activeParticleEffects.Remove(info);
            }
            else
            {
                info.particleSystem.Stop();
                StartCoroutine(AutoDestroyEffect(info, info.particleSystem.main.duration));
            }
        }

        /// <summary>
        /// 暂停粒子特效
        /// </summary>
        /// <param name="effectName">特效名称</param>
        public void PauseEffect(string effectName)
        {
            ParticleInfo info = activeParticleEffects.Find(x => x.effectName == effectName);
            if (info == null)
            {
                Debug.LogWarning("未找到活跃的粒子特效：" + effectName);
                return;
            }

            info.particleSystem.Pause();
        }

        /// <summary>
        /// 恢复播放粒子特效
        /// </summary>
        /// <param name="effectName">特效名称</param>
        public void ResumeEffect(string effectName)
        {
            ParticleInfo info = activeParticleEffects.Find(x => x.effectName == effectName);
            if (info == null)
            {
                Debug.LogWarning("未找到活跃的粒子特效：" + effectName);
                return;
            }

            info.particleSystem.Play();
        }

        /// <summary>
        /// 停止所有粒子特效
        /// </summary>
        /// <param name="immediate">是否立即销毁</param>
        public void StopAllEffects(bool immediate = false)
        {
            foreach (var info in activeParticleEffects.ToArray())
            {
                StopEffect(info.effectName, immediate);
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 自动销毁粒子特效
        /// </summary>
        private IEnumerator AutoDestroyEffect(ParticleInfo info, float delay)
        {
            yield return new WaitForSeconds(delay);

            if (info.particleSystem != null)
            {
                Destroy(info.particleSystem.gameObject);
            }

            activeParticleEffects.Remove(info);
        }

        #endregion
    }
}