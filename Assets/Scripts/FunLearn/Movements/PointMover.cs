using NaughtyAttributes;
using System.Collections;
using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    public class PointMover : ObjectMover
    {
        [Expandable]
        [SerializeField] PointMoveData moveData;
        public override void Move() => StartCoroutine(MoveCR());
        protected IEnumerator MoveCR()
        {
            MoveStart();

            foreach (var target in moveData.points)
            {
                yield return MoveCR(target);
            }

            MoveStop();
        }

        private void Start() {
            Move();
        }

        public override void Move(Vector3 target) => StartCoroutine(MoveCR(target));

        private float _speed;

        public void SetSpeed(float value)
        {
            _speed = value;
            Debug.Log("Speed set to: " + _speed);
        }
        
        protected IEnumerator MoveCR(Vector3 target)
        {
            _speed = moveData.speed;
            var time = 0f;
            var isRectTransform = TryGetComponent(out RectTransform rt);
            Vector3 initPosition = isRectTransform ? ((RectTransform)transform).anchoredPosition : transform.localPosition;
            while (time < 1)
            {
                var pos = Vector3.Lerp(initPosition, target, time);
                time += Time.deltaTime * _speed;
                var add = moveData.offset * moveData.offsetCurve.Evaluate(time);
                pos += add;
                if (isRectTransform)
                {
                    ((RectTransform)transform).anchoredPosition = pos;
                }
                else
                {
                    transform.localPosition = pos;
                }

                yield return null;
            }
        }
    }
}