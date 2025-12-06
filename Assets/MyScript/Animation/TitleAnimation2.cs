using System.Collections;
using Arcube.Animation;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TitleAnimation2 : MonoBehaviour {
    #region Fields

    [SerializeField] private List<AudioClip>introClip;
    
    [SerializeField] private int i;
    [SerializeField] private float introOffset;
    [SerializeField] private AudioClip titleClip;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Vector3 startScale;
    [SerializeField] private DOTweenAnimator animator;

    #endregion

    #region Unity
    private void Start() {
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        GetComponent<RectTransform>().localScale = startScale;
        StartCoroutine(WaitForAudio());
    }

    private void OnDestroy() {
        gameObject.transform.DOKill();
    }

    #endregion



    IEnumerator WaitForAudio() {
        PlayAnimations("Click");
        if(introClip != null) {
            PlayAudio(introClip[i]);
            yield return new WaitForSeconds(introClip[i].length -introOffset);
        }
        PlayAudio(titleClip);
        yield return new WaitForSeconds(titleClip.length);
        PlayAnimations("MoveUp");
       // PlayAnimations();
    }
    void PlayAudio(AudioClip clip) {
        if(clip == null || audioSource == null) {
            PlayAnimations();
            return;
        }
        audioSource.PlayOneShot(clip);
    }

    #region Public API

    public void PlayAnimations() {
        animator.PlaySimple("MoveUp");
        animator.PlaySimple("ScaleUp");
    }

    public void PlayAnimations(string clip) {
        animator.PlaySimple(clip);
    }

    #endregion
}
