using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    public class PathMover : ObjectMover{
        [Expandable]
        [SerializeField] PathMoveData moveData;
        [SerializeField] private List<Transform> points;
        public PathMover AddPoint(Transform transform)
        {
            if(!points.Contains(transform)) points.Add(transform);
            return this;
        }

        public override void Move() => StartCoroutine(MoveCR());

        protected IEnumerator MoveCR()
        {
            MoveStart();

            foreach (var path in points)
            {
                yield return MoveCR(path.position);
            }

            MoveStop();
        }

        public override void Move(Vector3 target) => StartCoroutine(MoveCR(target));

        protected IEnumerator MoveCR(Vector3 target)
        {

            var time = 0f;
            Vector3 initPosition = useRectTransform ? ((RectTransform)transform).anchoredPosition : transform.position;
            while (time < 1)
            {
                var pos = Vector3.Lerp(initPosition, target, time);
                time += Time.deltaTime * moveData.speed;
                var add = moveData.offset * moveData.offsetCurve.Evaluate(time);
                pos += add;
                if (useRectTransform)
                {
                    ((RectTransform)transform).anchoredPosition = pos;
                }
                else
                {
                    transform.position = pos;
                }

                if(lookAtTarget) transform.LookAt(target);

                yield return null;
            }
        }
    }
}