using System;
using UnityEngine;
using UnityEngine.Events;

namespace NumBalloonPop {

    public class BalloonPopGameManager : MonoBehaviour {

        public static BalloonPopGameManager Instance;

        public UnityEvent onAnyPop = new();
        public UnityEvent onRightPop = new();
        public UnityEvent onWrongPop = new();

        public UnityEvent<int> onLevelChange = new();


        private void Awake() {
            if(Instance == null) {
                Instance = this;
            } else if(Instance != this) {
                Destroy(gameObject);
                return;
            }
        }

        public void Invoke_ClickPop(bool wasNumbered) {
            onAnyPop?.Invoke();

            if(wasNumbered) {
                onRightPop?.Invoke();
            } else {
                onWrongPop?.Invoke();
            }
        }

        public void Invoke_OutOfBoundPop(bool wasNumbered) {
            if(wasNumbered)
                onWrongPop?.Invoke();
            else
                onAnyPop?.Invoke();
        }   

        public void Invoke_LevelChange(int i) {
            onLevelChange?.Invoke(i);
        }
    }
}
