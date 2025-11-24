using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimations : MonoBehaviour
{
    [SerializeField] Button btn;

    [SerializeField] float speed;
    [SerializeField] Vector3 movePos;
    [SerializeField] int cycleTime;

    [SerializeField] Color color;

    private TextMeshProUGUI tmp;

    private void Awake() {
        tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
    }
    public enum AnimationType
    {
        Move,
        ColorChange,
        Shake,
        Fade
    }

    [SerializeField] AnimationType animationType;

    private void Start() {
        btn.onClick.AddListener(() => AnimateNoUIReferenced());
    }

    void AnimateNoUIReferenced() {
        Transform tf = btn.gameObject.transform;
        switch(animationType) {
            case AnimationType.Move:
                tf.DOLocalMove(movePos, speed).SetLoops(cycleTime, LoopType.Yoyo);
                break;
            case AnimationType.ColorChange:
            tf.GetComponentInChildren<TextMeshProUGUI>().DOColor(color, speed);
            break;

            case AnimationType.Fade:
            tf.GetComponentInChildren<TextMeshProUGUI>().DOFade(0, speed)
                .SetLoops(cycleTime, LoopType.Yoyo);
            break;

            case AnimationType.Shake:
                tf.DOShakePosition(speed, movePos).SetLoops(cycleTime, LoopType.Yoyo);
            break;


        }

    }
}
