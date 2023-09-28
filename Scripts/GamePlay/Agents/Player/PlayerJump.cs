using com.joyixir.starla.Controller.MoveSystem;
using GamePlay.Background;
using GamePlay.Controllers;
using UnityEngine;

namespace GamePlay.Agents.Player
{
    public class PlayerJump : MonoBehaviour
    {
        [SerializeField] private PlayerSpinner _spinner;

        private JumpState _state;
        private Vector3 initialPos;
        private float _jumpDistance;
        private Vector3 _jumpPlatformPos;
        private float _jumpedDistance;
        private float _jumpAngle;
        private float _jumpVelocity;

        private void Awake()
        {
            initialPos = transform.position;
        }

        public void Jump(Vector3 jumpPlatformPos, float angle, float velocity, float distance)
        {
            _jumpPlatformPos = jumpPlatformPos;
            _jumpDistance = distance;
            _jumpAngle = angle;
            _jumpVelocity = velocity;
            // CameraController.Instance.ChangeCameraDampingZ(1f, 1);
            _state = JumpState.jumping;
        }

        private void FixedUpdate()
        {
            if (_state == JumpState.None) return;
            if (_state == JumpState.jumping)
            {
                if (_jumpDistance <= 0)
                    _state = JumpState.Cooldowning;
            }

            if (_state == JumpState.Cooldowning)
            {
                if (transform.position.y <= initialPos.y || _jumpedDistance >= _jumpDistance)
                {
                    _state = JumpState.None;
                    _jumpedDistance = 0;
                    _spinner.directionY = 0;
                    return;
                }
            }

            var deltaY = YByPos() - transform.position.y;

            transform.position += deltaY * Vector3.up;
            _jumpedDistance = transform.position.z - _jumpPlatformPos.z;
            var slop = -SlopByPos();
            _spinner.directionY = slop < 0 ? slop : slop/10f;
        }

        private float YByPos()
        {
            var z = transform.position.z - _jumpPlatformPos.z;
            if (z <= 0) return initialPos.y;
            if (z >= _jumpDistance) return initialPos.y;

            return _jumpPlatformPos.y + z * Mathf.Tan(_jumpAngle * Mathf.Deg2Rad) - (-Physics.gravity.y) * z * z /
                (2 * _jumpVelocity * _jumpVelocity * Mathf.Pow(Mathf.Cos(_jumpAngle * Mathf.Deg2Rad), 2));
        }

        private float SlopByPos()
        {
            var z = transform.position.z - _jumpPlatformPos.z;
            if (z <= 0) return 0;
            if (z >= _jumpDistance) return 0;

            return Mathf.Tan(_jumpAngle * Mathf.Deg2Rad) - (-Physics.gravity.y) * z /
                (_jumpVelocity * _jumpVelocity * Mathf.Pow(Mathf.Cos(_jumpAngle * Mathf.Deg2Rad), 2));
        }

        enum JumpState
        {
            None,
            jumping,
            Cooldowning,
        }
    }
}