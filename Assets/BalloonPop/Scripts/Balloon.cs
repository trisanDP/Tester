    using Arcube.Animation;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace NaniBabu.BalloonPop {
    public enum BalloonColor {
        Red,
        Green,
        Blue,
        Yellow,
    }

    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class Balloon : MonoBehaviour, IPointerClickHandler {

        #region Inspector
        public float speed = 200f;
        public float destroyOffset = 50f;
        [SerializeField] private Image image;
        [SerializeField] private BalloonColor balloonColor;

        [SerializeField] private TextMeshProUGUI numberTxt;
        #endregion

        #region State
        private RectTransform rectTransform;
        private RectTransform canvasRect;
        private BalloonPopGameManager manager;
        private Tween _moveTween;
        #endregion


        [SerializeField] private DOTweenAnimator animator;
        [SerializeField] private ParticleSystem explosionParticle;

        [Header("Sound")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;
        [SerializeField] private AudioClip hitClip;

        private bool _isNumbered = false;
        private int _hitsRemaining = 1;
        private bool clickAble = true;

        #region Unity
        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            if(image == null) image = GetComponent<Image>() ?? GetComponentInChildren<Image>();
            if(numberTxt != null) numberTxt.gameObject.SetActive(false);
        }

        private void OnDestroy() {
            _moveTween?.Kill();
        }
        #endregion


/*        public event Action OnLevelChange;*/

        #region API
        public void Initialize(BalloonSpawner.BalloonData data, RectTransform rect, BalloonPopGameManager mag, bool isNumbered = false, int hits = 1) {
            rectTransform ??= GetComponent<RectTransform>();
            if(rectTransform == null) { gameObject.SetActive(false); return; }

            if(image == null) GetComponentInChildren<Image>();
            if(image == null) { gameObject.SetActive(false); return; }

            canvasRect = rect;
            manager = mag ?? BalloonPopGameManager.Instance;

            image.sprite = data.sprite;
            balloonColor = data.color;

            _isNumbered = isNumbered;
            _hitsRemaining = Mathf.Max(1, hits);
            if(numberTxt != null) {
                numberTxt.gameObject.SetActive(_isNumbered);
                numberTxt.text = _isNumbered ? _hitsRemaining.ToString() : string.Empty;
            }

            if(canvasRect == null) {
                Debug.LogWarning($"[{name}] canvasRect is null — balloon will not move/destroy by boundary.");
                return;
            }

            if(speed <= 0f) speed = 200f;

            float topY = (canvasRect.rect.height / 2f) + destroyOffset;
            float distance = topY - rectTransform.anchoredPosition.y;
            float duration = Mathf.Max(0.01f, distance / speed);

            _moveTween = rectTransform.DOAnchorPosY(topY, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>  DestroyOutOfBound());
        }



        public void OnPointerClick(PointerEventData eventData) {
            if(!clickAble) return;
            if(_isNumbered && _hitsRemaining > 1) {
                clickAble = false;
                _hitsRemaining--;
                if(numberTxt != null) numberTxt.text = _hitsRemaining.ToString();
                if(source != null && hitClip != null) source.PlayOneShot(hitClip);
                animator.Play("Click");
                return;
            }
            DestroyBalloon_Right();
        }

        public void ResetClickable() {
            clickAble = true;
        }
        #endregion

        #region Life



        // Touch Destroy
        void DestroyBalloon_Right() {
            manager?.Invoke_ClickPop(_isNumbered);
            DestroyVisualAndSound();
        }


        void DestroyOutOfBound() {
            if(!_isNumbered) {
                DestroyBalloon_OutOfBound();
            } else {
                DestroyBalloon_NumberedWrong();
            }
        }

        // Numbered Touch
        void DestroyBalloon_NumberedWrong() {
            manager?.Invoke_OutOfBoundPop(_isNumbered);
            DestroyVisualAndSound();
        }

        //Pos Destroy
        void DestroyBalloon_OutOfBound() {
            DestroyVisualAndSound();
            manager?.Invoke_OutOfBoundPop(false);
        }

        void DestroyVisualAndSound() {
            transform.DOKill();
            clickAble = false;

            if(numberTxt != null) numberTxt.gameObject.SetActive(false);
            if(image != null) image.enabled = false;

            if(source != null && clip != null) source.PlayOneShot(clip);
            explosionParticle?.Play();

            float delay = (clip != null) ? clip.length : 0f;
            Destroy(gameObject, delay);
        }
        #endregion
    }
}
