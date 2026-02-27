using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InteractableObjects : MonoBehaviour, IPointerClickHandler {
    public UnityEvent onInteract;

    private AudioSource audioSource;

    public AudioClip birdClip; // assign via Inspector

    private void Awake() {
        audioSource = GetComponent<AudioSource>();
        if(!audioSource) {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Check");
        onInteract?.Invoke();
        PlayBirdSound();
    }

    public void OnMouseDown() {
        Debug.Log("Check2");
        onInteract?.Invoke();
    }

    public void PlayBirdSound() {
        if(audioSource && birdClip) {
            audioSource.PlayOneShot(birdClip);
        } else
            Debug.LogWarning("Not Played");
    }
}
