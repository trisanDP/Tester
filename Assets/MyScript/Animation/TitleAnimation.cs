using System.Collections;
using Arcube.Animation;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimation : MonoBehaviour {
    #region Fields
    [SerializeField] private AudioClip clip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private DOTweenAnimator animator;

    private Coroutine _waitRoutine;
    #endregion

    #region Unity
    private void Start() {

        GetComponent<RectTransform>().localScale = startScale;
        PlayClickWithAudio();
    }

    private void OnDisable() {
        if(_waitRoutine != null) StopCoroutine(_waitRoutine);
        _waitRoutine = null;
    }

    private void OnDestroy() {
        if(_waitRoutine != null) StopCoroutine(_waitRoutine);
        _waitRoutine = null;
    }
    #endregion

    #region Public API
    public void PlayClickWithAudio() {
        if(_waitRoutine != null) {
            StopCoroutine(_waitRoutine);
            _waitRoutine = null;
        }

        animator?.PlaySimple("Click");

        if(clip == null || audioSource == null) {
            animator?.PlaySimple("MoveUp");
            animator?.PlaySimple("ScaleUp");
            return;
        }

        audioSource.PlayOneShot(clip);
        _waitRoutine = StartCoroutine(WaitForAudioThenPlayNext(clip.length));
    }

    #endregion

    #region Coroutines
    private IEnumerator WaitForAudioThenPlayNext(float duration) {
        var end = Time.realtimeSinceStartup + duration;
        yield return new WaitUntil(() => Time.realtimeSinceStartup >= end);

        _waitRoutine = null;

        animator?.PlaySimple("MoveUp");
        animator?.PlaySimple("ScaleUp");
    }
    #endregion
}
