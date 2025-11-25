using Arcube;
using Arcube.Animation;
using UnityEngine;
using UnityEngine.UI;

public class AnimationTesting : MonoBehaviour
{
    [SerializeField]
    DOTweenAnimator animator;
    [SerializeField] Button b_Click;


    private void Reset() {
        animator = GetComponent<DOTweenAnimator>();
        b_Click = gameObject.FindObject<Button>("b_Click"); 
    }

    

    private void Start() {
        //animator = gameObject.FindObject<DOTweenAnimator>("custom");
        b_Click.onClick.AddListener(OnButton1Click);
        animator.Play("Click");

    }

    void OnButton1Click() {
        Debug.Log("Check");

        b_Click.GetComponent<DOTweenAnimator>().Play("ScaleUp");
    }
}
