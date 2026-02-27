using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LevelSelector {
    public class StageItem : MonoBehaviour {
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Button button;
        public void Init(Stage stage, int index, Action<Stage> onClick, bool interactable) {
            if(iconImage != null) iconImage.sprite = stage.icon;
            if(label != null) label.text = "Stage " + (index + 1);
            if(button != null) {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => onClick?.Invoke(stage));
                button.interactable = interactable;
            }
            if(iconImage != null) iconImage.color = interactable ? Color.white : new Color(1f, 1f, 1f, 0.35f);
            if(label != null) label.color = interactable ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }
}
