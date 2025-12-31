using System.Collections;
using TMPro;
using UnityEngine;

namespace NaniBabu.BalloonPop {
    public class BallonPopUI : MonoBehaviour {

        [SerializeField] private TextMeshProUGUI txt_activeColor;
        [SerializeField] private TextMeshProUGUI txt_result;
        [SerializeField] private TextMeshProUGUI txt_level;
        [SerializeField] private TextMeshProUGUI txt_Spawned;


        [Tooltip("Optional: assign the spawner in inspector. If left empty, the script will find the first BalloonSpawner in the scene.")]
        [SerializeField] private BalloonSpawner spawner;

        // local state
        private int _numberedPoppedCount = 0;
        private Coroutine _clearResultCoroutine;

        #region Unity Callbacks
        private void OnEnable() {
            if(txt_activeColor == null) {
                Debug.LogWarning("BallonPopUI: Active Color / Progress Text not assigned.");
            }

            if(txt_result == null) {
                Debug.LogWarning("BallonPopUI: Result Text not assigned.");
            }

            UpdateProgressTxt();
            ClearResultTextImmediate();
        }

        private void OnDisable() {
            if(_clearResultCoroutine != null) {
                StopCoroutine(_clearResultCoroutine);
                _clearResultCoroutine = null;
            }
        }

        public void Update() {
            SpawnedThisLevel();
        }
        #endregion

        #region Event Linked

        public void OnRightPop() {
            _numberedPoppedCount++;
            UpdateProgressTxt();
            ShowResultTemporary(success: true);
        }


        public void SpawnedThisLevel() {
             txt_Spawned.text = spawner.GetSpawnedThisLevel() + " Spawned";
        }

        public void OnAnyPop() {
            ShowResultTemporary(success: null);
        }

        #endregion

        public void UpdateLevelText(int levelLabel) {
            if(txt_level == null) return;
            txt_level.text = $"Level: {levelLabel}";
            _numberedPoppedCount = 0;
            UpdateProgressTxt();
        }


        private void UpdateProgressTxt() {
            if(txt_activeColor == null) return;
            txt_activeColor.text = $"Numbered popped: {_numberedPoppedCount}";
        }


        public void UpdateResultTxt_Correct() {
            if(txt_result == null) return;
            txt_result.text = "Correct";

            if(_clearResultCoroutine != null) {
                StopCoroutine(_clearResultCoroutine);
            }
            _clearResultCoroutine = StartCoroutine(ClearResultAfterSeconds(1.0f));
        }

        public void UpdateResultTxt_Wrong() {
            if(txt_result == null) return;
            txt_result.text = "Wrong";
            if(_clearResultCoroutine != null) {
                StopCoroutine(_clearResultCoroutine);
            }
            _clearResultCoroutine = StartCoroutine(ClearResultAfterSeconds(1.0f));  
        }


        #region Backend
        private void ShowResultTemporary(bool? success) {
            if(txt_result == null) return;

            if(_clearResultCoroutine != null) {
                StopCoroutine(_clearResultCoroutine);
            }

            if(success == true) txt_result.text = "Correct";
            else if(success == false) txt_result.text = "Wrong";
            else txt_result.text = "Pop";

            _clearResultCoroutine = StartCoroutine(ClearResultAfterSeconds(1.0f));
        }

        private IEnumerator ClearResultAfterSeconds(float secs) {
            yield return new WaitForSeconds(secs);
            ClearResultTextImmediate();
            _clearResultCoroutine = null;
        }

        private void ClearResultTextImmediate() {
            if(txt_result != null) txt_result.text = string.Empty;
        }
        #endregion

    }
}
