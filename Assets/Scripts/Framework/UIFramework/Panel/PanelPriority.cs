using System.Collections.Generic;
using UnityEngine;

namespace KidGame.UI
{
    /// <summary>
    /// 规定面板属于哪个层的，便于管理
    /// </summary>
    public enum PanelPriority {
        None = 0,
        Prioritary = 1,
        Tutorial = 2,
        Blocker = 3,
    }
    /// <summary>
    /// 因为panel里要处理优先级关系，所以自己又封装了个类
    /// </summary>
    [System.Serializable] 
    public class PanelPriorityLayerListEntry {
        [SerializeField] 
        [Tooltip("指定下面板层的优先级")]
        private PanelPriority priority;
        [SerializeField] 
        [Tooltip("此优先级下所有面板的父节点")]
        private Transform targetParent;

        public Transform TargetParent {
            get { return targetParent; }
            set { targetParent = value; }
        }

        public PanelPriority Priority {
            get { return priority; }
            set { priority = value; }
        }

        public PanelPriorityLayerListEntry(PanelPriority prio, Transform parent) {
            priority = prio;
            targetParent = parent;
        }
    }

    [System.Serializable] 
    public class PanelPriorityLayerList {
        [SerializeField] 
        [Tooltip("根据面板的优先级查找并存储对应的GameObject。渲染优先级由这些GameObject在层级结构中的顺序决定")]
        private List<PanelPriorityLayerListEntry> paraLayers = null;

        private Dictionary<PanelPriority, Transform> lookup;

        public Dictionary<PanelPriority, Transform> ParaLayerLookup {
            get {
                if (lookup == null || lookup.Count == 0) {
                    CacheLookup();
                }

                return lookup;
            }
        }
        //用这个函数自动根据paralayrs填充字典，方便之后查找
        private void CacheLookup() {
            lookup = new Dictionary<PanelPriority, Transform>();
            for (int i = 0; i < paraLayers.Count; i++) {
                lookup.Add(paraLayers[i].Priority, paraLayers[i].TargetParent);
            }
        }
        //用这个构造函数初始化
        public PanelPriorityLayerList(List<PanelPriorityLayerListEntry> entries) {
            paraLayers = entries;
        }
    }
}
