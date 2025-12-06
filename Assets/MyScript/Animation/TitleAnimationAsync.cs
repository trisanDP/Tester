using System;
using System.Threading;
using System.Threading.Tasks;
using Arcube.Animation;
using UnityEngine;
using UnityEngine.UI;

public class TitleAnimationAsync : MonoBehaviour {
    #region Fields
    [SerializeField] private AudioClip introClip;
    [SerializeField] private float offsetClip;

    [SerializeField] private AudioClip titleClip;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private DOTweenAnimator animator;

    private bool _isPlayingAsync;
    private CancellationTokenSource _cts;
    #endregion

    #region Unity
    private void Start() {
        GetComponent<RectTransform>().localScale = startScale;
        _cts = new CancellationTokenSource();
        _ = PlayClickWithAudioAsync(_cts.Token); // fire-and-forget intentionally
    }

    private void OnDisable() {
        CancelPending();
    }

    private void OnDestroy() {
        CancelPending();
    }

    private void CancelPending() {
        if(_cts == null) return;
        _cts.Cancel();
        _cts.Dispose();
        _cts = null;
    }
    #endregion

    #region Async Flow
    private async Task PlayClickWithAudioAsync(CancellationToken token) {
        if(_isPlayingAsync) return;
        _isPlayingAsync = true;

        try {
            PlayAnimation("Click");
            PlayAudio(introClip);

            // compute first wait: introClip.length - offsetClip (clamped to >= 0)
            var firstDelaySeconds = 0f;
            if(introClip != null) {
                firstDelaySeconds = Mathf.Max(0f, introClip.length - offsetClip);
            }

            // await with cancellation
            await Task.Delay(TimeSpan.FromSeconds(firstDelaySeconds), token).ConfigureAwait(true);

            // play title audio if available
            PlayAudio(titleClip);

            var secondDelaySeconds = (titleClip != null) ? titleClip.length : 0f;
            await Task.Delay(TimeSpan.FromSeconds(secondDelaySeconds), token).ConfigureAwait(true);

            // play follow-ups
            PlayFollowups();
        } catch(OperationCanceledException) {
            // cancelled — swallow silently
        } finally {
            _isPlayingAsync = false;
        }
    }
    #endregion

    #region Helpers
    private void PlayAudio(AudioClip clip) {
        if(clip == null || audioSource == null) return;
        audioSource.PlayOneShot(clip);
    }

    private void PlayFollowups() {
        animator?.PlaySimple("MoveUp");
        animator?.PlaySimple("ScaleUp");
    }

    private void PlayAnimation(string animationName) {
        animator?.PlaySimple(animationName);
    }
    #endregion
}
