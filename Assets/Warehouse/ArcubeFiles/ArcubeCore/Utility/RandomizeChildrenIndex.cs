using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube
{
    public interface IRandomizeChildren
    {
        IEnumerator Randomize();
    }
    
    public class RandomizeChildrenIndex : MonoBehaviour, IRandomizeChildren
    {
        [SerializeField] private bool disableLayout = true;

        [SerializeField] private Transform[] children;
        private void Reset() => children = GetComponentsInChildren<Transform>();

        private void OnEnable() => StartCoroutine(Randomize());

        public IEnumerator Randomize()
        {
            if (!TryGetComponent(out LayoutGroup layout)) yield break;

            layout.enabled = true;
            if (children.Length > 0)
            {
                foreach (var t in children)
                {
                    t.SetSiblingIndex(Random.Range(0, children.Length));
                }
            }
            else
            {
                foreach (Transform t in transform)
                {
                    t.SetSiblingIndex(Random.Range(0, transform.childCount));
                }
            }

            yield return null;
            yield return null;

            if (disableLayout) layout.enabled = false;
        }
    }
}