using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Arcube
{
    public class RandomizeChildrenPosition : MonoBehaviour, IRandomizeChildren
    {
        [SerializeField] private Transform[] children;
        private void Reset() => children = GetComponentsInChildren<Transform>();

        private void OnEnable() => StartCoroutine(Randomize());

        public IEnumerator Randomize()
        {
            if (children == null || children.Length <= 1)
                yield break;

            // Store original positions
            var positions = new Vector3[children.Length];
            for (var i = 0; i < children.Length; i++)
            {
                positions[i] = children[i].localPosition;
            }

            // Shuffle positions using Fisher–Yates algorithm
            for (var i = positions.Length - 1; i > 0; i--)
            {
                var randomIndex = Random.Range(0, i + 1);
                (positions[i], positions[randomIndex]) = (positions[randomIndex], positions[i]);
            }

            // Apply shuffled positions back to children
            for (var i = 0; i < children.Length; i++)
            {
                children[i].localPosition = positions[i];
            }

            yield return null;
        }
    }
}