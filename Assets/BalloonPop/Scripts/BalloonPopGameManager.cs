using System;
using UnityEngine;
using UnityEngine.Events;

namespace NaniBabu.BalloonPop {
    [Serializable]
    public class BalloonColorEvent : UnityEvent<BalloonColor> { }

    public class BalloonPopGameManager : MonoBehaviour {
        #region Fields
        public static BalloonPopGameManager Instance;
        

        public BalloonColorEvent onCorrectPop = new();     
        public BalloonColorEvent onWrongPop = new();
        public BalloonColorEvent onSafeColorChanged = new();
        public UnityEvent onAnyPop = new();
        #endregion

        [SerializeField] private BalloonSpawner spawner;
        #region State
        [SerializeField] private BalloonColor _currentSafeColor;
        public BalloonColor CurrentSafeColor => _currentSafeColor;
        #endregion

        [SerializeField] bool changeSafeOnlyInRightPop = true;
        #region Unity
        private void Awake() {
            if(Instance == null) {
                Instance = this;
            } else if(Instance != this) {
                Destroy(gameObject);
                return;
            }

            SetRandomSafeColor();
        }

        private void Start() {
            SetRandomSafeColor();
        }
        #endregion


        public void StartGame() {
            spawner.StartGame();
        }

        #region API
        public void HandleBalloonPopped(BalloonColor poppedColor) {
            onAnyPop?.Invoke();

            if(poppedColor == _currentSafeColor ) {
                onCorrectPop?.Invoke(poppedColor);
                SetRandomSafeColor();
            } else {
                onWrongPop?.Invoke(poppedColor);
                if(changeSafeOnlyInRightPop == false)
                    SetRandomSafeColor() ;
            }
        }

        public void SetRandomSafeColor() {
            var values = (BalloonColor[])Enum.GetValues(typeof(BalloonColor));
            if(values.Length == 0) return;

            BalloonColor next;
            if(values.Length == 1) {
                next = values[0];
            } else {
                do {
                    next = values[UnityEngine.Random.Range(0, values.Length)];
                } while(next.Equals(_currentSafeColor));
            }

            _currentSafeColor = next;
            onSafeColorChanged?.Invoke(_currentSafeColor);
        }

        public void SetSafeColor(BalloonColor color) {
            _currentSafeColor = color;
            onSafeColorChanged?.Invoke(_currentSafeColor);
        }

        public void RegisterOnCorrect(UnityAction<BalloonColor> action) => onCorrectPop.AddListener(action);
        public void UnregisterOnCorrect(UnityAction<BalloonColor> action) => onCorrectPop.RemoveListener(action);

        public void RegisterOnWrong(UnityAction<BalloonColor> action) => onWrongPop.AddListener(action);
        public void UnregisterOnWrong(UnityAction<BalloonColor> action) => onWrongPop.RemoveListener(action);
        #endregion
    }
}
