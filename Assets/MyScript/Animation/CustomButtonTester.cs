using UnityEngine;
using UnityEngine.InputSystem.Composites;
using UnityEngine.UI;

public class CustomButtonTester : MonoBehaviour
{
    public Button btn;
    public bool disableBtnAfterPlay = false;
    public ParticleSystem particle;

    public AudioClip clip;
    public AudioSource audioSource;

    private Image img;
    private void Reset() {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null){
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        btn = GetComponentInChildren<Button>();
        particle = GetComponentInChildren<ParticleSystem>();
    }

    private void Start() {
        btn.onClick.AddListener(OnClick);
    }

    void OnClick() {
        particle.Play();
        btn.gameObject.SetActive(!disableBtnAfterPlay);
        if(audioSource && clip && !audioSource.isPlaying)
            audioSource.PlayOneShot(clip);
        else
            Debug.LogWarning("Audio Not Played");
    }
}
