using NaughtyAttributes;
using UnityEngine;

namespace Arcube.FunLearn.Movements
{
    public class PositionDeltaMover : ObjectMover
    {
        [Expandable]
        [SerializeField] private PositionDeltaMoveData moveData;

        [SerializeField] private bool move;
        private Vector3 _speed;
        private void OnEnable()
        {
            _speed = moveData.speed + Utils.RandomVector(moveData.randomOffset);
        }

        public void StartMove() => move = true;

        public void StopMove() => move = false;

        private void Update()
        {
            if(!move) return;
            Move();
        }

        public override void Move()
        {
            var pos = transform.position;
            pos += Time.deltaTime * _speed;
            transform.position = pos;
        }

        public override void Move(Vector3 target) { }
    }
}