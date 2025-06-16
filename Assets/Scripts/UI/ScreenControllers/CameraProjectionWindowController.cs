using System;
using System.Collections.Generic;
using UnityEngine;

namespace KidGame.UI.Game
{
    [Serializable]
    public class CameraProjectionWindowProperties : WindowProperties
    {
        public readonly Camera WorldCamera;
        public readonly Transform TransformToFollow;

        public CameraProjectionWindowProperties(Camera worldCamera, Transform toFollow) {
            WorldCamera = worldCamera;
            TransformToFollow = toFollow;
        }
    }

    public class CameraProjectionWindowController : WindowController<CameraProjectionWindowProperties>
    {
        [SerializeField] 
        private UIFollowComponent followTemplate = null;

        private List<UIFollowComponent> allElements = new List<UIFollowComponent>();

        protected override void OnPropertiesSet() {
            CreateNewLabel(Properties.TransformToFollow,"看这里!", null);
        }

        protected override void WhileHiding() {
            foreach (var element in allElements) {
                Destroy(element.gameObject);
            }
            allElements.Clear();
            Properties.TransformToFollow.parent.gameObject.SetActive(false);
        }

        private void LateUpdate() {
            for (int i = 0; i < allElements.Count; i++) {
                allElements[i].UpdatePosition(Properties.WorldCamera);
            }
        }

        private void CreateNewLabel(Transform target, string label, Sprite icon) {
            var followComponent = Instantiate(followTemplate, followTemplate.transform.parent, false);
            followComponent.LabelDestroyed += OnLabelDestroyed;
            followComponent.gameObject.SetActive(true);
            followComponent.SetFollow(target);
            followComponent.SetText(label);
            
            if (icon != null) {
                followComponent.SetIcon(icon);
            }

            allElements.Add(followComponent);
        }

        private void OnLabelDestroyed(UIFollowComponent destroyedLabel) {
            allElements.Remove(destroyedLabel);
        }
    }
}
