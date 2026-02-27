using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LevelSelector {
    [RequireComponent(typeof(RectTransform))]
    public class LevelItem : MonoBehaviour {
        [SerializeField] private Image stateImage;
        [SerializeField] private Image diffIconImg;
        [SerializeField] private GameObject starsContainer;
        [SerializeField] private List<Image> starImages;
        [SerializeField] private TextMeshProUGUI levelLabel;
        public LevelDataSO levelData;

        public void SetUI(Level level) {
            LevelState state = level.State;
            SetDifficultyIcon(level.difficulty);
            levelLabel.text = level.levelID.ToString();

            switch(state) {
                case LevelState.Locked:
                if(stateImage != null && levelData != null && levelData.lockedImg != null)
                    stateImage.sprite = levelData.lockedImg;
                SetStarsVisible(false);
                break;

                case LevelState.Active:
                if(stateImage != null && levelData != null && levelData.activeImg != null)
                    stateImage.sprite = levelData.activeImg;

                if(level.StarCount > 0) {
                    SetStarsVisible(true);
                    UpdateStarsImmediate(level.StarCount);
                } else {
                    SetStarsVisible(false);
                }
                break;

                case LevelState.Completed:
                if(stateImage != null && levelData != null && levelData.completedImg != null)
                    stateImage.sprite = levelData.completedImg;

                int stars = level.StarCount;
                if(stars <= 0 && level.Score > 0 && LevelManager.Instance != null) {
                    stars = LevelManager.Instance.CalculateStars(level);
                    level.StarCount = stars;
                }

                if(stars <= 0) {
                    SetStarsVisible(false);
                } else {
                    SetStarsVisible(true);
                    UpdateStarsImmediate(stars);
                }
                break;
                case LevelState.Unlocked:
                if(stateImage != null && levelData != null && levelData.activeImg != null)
                    stateImage.sprite = levelData.activeImg;

                if(level.StarCount > 0) {
                    SetStarsVisible(true);
                    UpdateStarsImmediate(level.StarCount);
                } else {
                    SetStarsVisible(false);
                }
                break;
            }
        }

        public void SetDifficultyIcon(LevelDifficulty diff) {
            if(levelData == null) return;
            switch(diff) {
                case LevelDifficulty.Easy:
                diffIconImg.sprite = levelData.easyDiffImg;
                break;
                case LevelDifficulty.Medium:
                diffIconImg.sprite = levelData.midDiffImg;
                break;
                case LevelDifficulty.Hard:
                diffIconImg.sprite = levelData.hardDiffImg;
                break;
            }
        }

        public Sequence PlayStarsAnimation(int stars, float fadeDuration = 0.22f, float between = 0.12f) {
            var seq = DOTween.Sequence();
            if(starImages == null) return seq;
            for(int i = 0; i < Mathf.Min(stars, starImages.Count); i++) {
                var img = starImages[i];
                if(img == null) continue;
                var startColor = img.color;
                startColor.a = 0f;
                img.color = startColor;
                img.rectTransform.localScale = Vector3.one * 0.6f;
                img.enabled = true;
                seq.Append(img.DOFade(1f, fadeDuration).SetEase(Ease.OutCubic));
                seq.Join(img.rectTransform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack));
                seq.AppendInterval(between);
            }
            return seq;
        }

        void SetStarsVisible(bool v) {
            if(starsContainer != null) starsContainer.SetActive(v);
        }

        void UpdateStarsImmediate(int stars) {
            if(starImages == null) return;
            for(int i = 0; i < starImages.Count; i++) {
                var img = starImages[i];
                if(img == null) continue;
                img.enabled = i < stars;
                var c = img.color;
                c.a = 1f;
                img.color = c;
                img.rectTransform.localScale = Vector3.one;
            }
        }
    }
}
