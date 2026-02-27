
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;


namespace LevelSelector {
    public class SimpleScroller : MonoBehaviour {

        #region Inspector
        public float dragSpeed = 1f;
        public float inertia = 5f;


        [Header("Background")]
        [SerializeField] private GameObject bg;
        [SerializeField] private float value;
        private Material material;
        private Vector2 offset;

        #endregion


        #region State
        [SerializeField] LevelUI lm;
        [SerializeField] StageUI sm;
        Vector2 velocity;
        Vector2 prevPointerPos;
        bool dragging;
        #endregion


        #region Unity
        void Awake() {
            if(lm == null) lm = FindFirstObjectByType<LevelUI>();
            if(sm== null) sm = FindFirstObjectByType<StageUI>();
            material = bg.GetComponent<Image>().material;
        }
        private void Reset() {
            if(lm == null) lm = FindFirstObjectByType<LevelUI>();
            if(sm== null) sm = FindFirstObjectByType<StageUI>();
        }
        void OnEnable() { EnhancedTouchSupport.Enable(); }
        void OnDisable() { EnhancedTouchSupport.Disable(); }

        void Update() {
            var touches = UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches;
            if(touches.Count > 0) {
                var t = touches[0];
                if(t.phase == UnityEngine.InputSystem.TouchPhase.Began) {
                    dragging = true;
                    prevPointerPos = t.screenPosition;
                    velocity = Vector2.zero;
                } else if(t.phase == UnityEngine.InputSystem.TouchPhase.Moved && dragging) {
                    var delta = (Vector2)t.screenPosition - prevPointerPos;
                    DragBy(delta.x);
                    velocity = new Vector2(delta.x / Time.deltaTime, 0);
                    prevPointerPos = t.screenPosition;
                } else if((t.phase == UnityEngine.InputSystem.TouchPhase.Ended || t.phase == UnityEngine.InputSystem.TouchPhase.Canceled) && dragging) {
                    dragging = false;
                }
            } else {
                if(Mouse.current != null && Mouse.current.leftButton.isPressed) {
                    if(!dragging) {
                        dragging = true;
                        prevPointerPos = Mouse.current.position.ReadValue();
                        velocity = Vector2.zero;
                    } else {
                        var cur = Mouse.current.position.ReadValue();
                        var delta = cur - prevPointerPos;
                        DragBy(delta.x);
                        velocity = new Vector2(delta.x / Time.deltaTime, 0);
                        prevPointerPos = cur;
                    }
                } else if(dragging) dragging = false;
            }

            if(!dragging && velocity.magnitude > 0.1f) {
                DragBy(velocity.x * Time.deltaTime);
                velocity = Vector2.Lerp(velocity, Vector2.zero, inertia * Time.deltaTime);
            }
        }
        #endregion


        #region Helpers
        void DragBy(float deltaScreenX) {
            if(lm == null || lm.contentParent == null) return;
            float unitsPerPixel = 0.02f;
            float dx = deltaScreenX * unitsPerPixel * dragSpeed;
            lm.contentParent.anchoredPosition += new Vector2(dx, 0);

            if(sm == null || sm.contentParent == null) return;
            sm.contentParent.anchoredPosition += new Vector2(dx, 0);

            offset += new Vector2(dx/value, 0)/* * Time.deltaTime*/;
            material.mainTextureOffset = offset;

        }
        #endregion
    }
}