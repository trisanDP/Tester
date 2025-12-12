using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

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
        #endregion

        #region State
        private RectTransform rectTransform;
        private RectTransform canvasRect;
        private BalloonPopGameManager manager;
        private Tween _moveTween;
        #endregion

        [Header("Sound")]
        [SerializeField] private AudioSource source;
        [SerializeField] private AudioClip clip;


        #region Unity
        private void Awake() {
            rectTransform = GetComponent<RectTransform>();
            if(image == null) image = GetComponent<Image>() ?? GetComponentInChildren<Image>();
        }

        private void OnDestroy() {
            _moveTween?.Kill();
        }
        #endregion

        #region API
        public void Initialize(BalloonSpawner.BalloonData data, RectTransform rect, BalloonPopGameManager mag) {
            rectTransform ??= GetComponent<RectTransform>();
            if(rectTransform == null) { gameObject.SetActive(false); return; }

            if(image == null) image = GetComponent<Image>() ?? GetComponentInChildren<Image>();
            if(image == null) { gameObject.SetActive(false); return; }

            canvasRect = rect;
            manager = mag ?? BalloonPopGameManager.Instance;

            image.sprite = data.sprite;
            balloonColor = data.color;

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
                .OnComplete(() => DestroyBalloon());
        }

        public void OnPointerClick(PointerEventData eventData) {
            DestroyBalloon();
            manager?.HandleBalloonPopped(balloonColor);
        }
        #endregion

        #region Life
        void DestroyBalloon() {
            source.PlayOneShot(clip);
            _moveTween?.Kill();
            gameObject.GetComponent<Image>().enabled = false ;
            Destroy(gameObject, clip.length);
        }
        #endregion
    }
}
