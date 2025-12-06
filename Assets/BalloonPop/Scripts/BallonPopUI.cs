using TMPro;
using UnityEngine;

namespace NaniBabu.BalloonPop {
    public class BallonPopUI : MonoBehaviour {
        [SerializeField] private TextMeshProUGUI txt_activeColor;
        [SerializeField] private TextMeshProUGUI txt_result;

        private void OnEnable() {
            if(txt_activeColor == null) {
                Debug.LogWarning("BalloonPop: Active Color Text Not Found!");
                return;
            }

            if(BalloonPopGameManager.Instance != null) {
                BalloonPopGameManager.Instance.onSafeColorChanged.AddListener(OnSafeColorChanged);
                UpdateTxt(BalloonPopGameManager.Instance.CurrentSafeColor);
            } else {
                var mgr = FindFirstObjectByType<BalloonPopGameManager>();
                if(mgr != null) {
                    mgr.onSafeColorChanged.AddListener(OnSafeColorChanged);
                    UpdateTxt(mgr.CurrentSafeColor);
                } else {
                    Debug.LogWarning("BallonPopUI: BalloonPopGameManager not found in scene.");
                }
            }
        }

        private void OnDisable() {
            if(BalloonPopGameManager.Instance != null) {
                BalloonPopGameManager.Instance.onSafeColorChanged.RemoveListener(OnSafeColorChanged);
            }
        }

        private void OnSafeColorChanged(BalloonColor color) => UpdateTxt(color);

        private void UpdateTxt(BalloonColor color) {
            if(txt_activeColor == null) return;
            txt_activeColor.text = $"Pop: {color} Balloons";
        }

        public void UpdateResultTxt(bool istrue) {
            if(istrue)
                txt_result.text = "Correct";
            else
                txt_result.text = "Wrong";
        }
    }
}