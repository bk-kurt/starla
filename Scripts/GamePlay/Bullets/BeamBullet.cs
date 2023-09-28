using System;
using com.joyixir.starla.Agent;
using com.joyixir.starla.Utils.GameObjects;
using DG.Tweening;
using UnityEngine;

namespace GamePlay.Bullets
{
    public class BeamBullet : BulletBase
    {
        private Transform _startPoint;
        [SerializeField] private Transform endPoint;
        [SerializeField] private LineRenderer lineRenderer;
        [SerializeField] private bool isOneShot;
        [SerializeField] private BoxCollider boxCollider;
        private float _maxDistance = 20;
        private bool _hasHit;
        private bool _isInitialized;
        private float _animateSpeed;

        private void OnEnable()
        {
            _startPoint = transform;
        }

        public void Initialize(Transform startPosition, float maxDistance, float speed)
        {
            _animateSpeed = speed;
            endPoint.position = _startPoint.position;
            _maxDistance = maxDistance;
            _hasHit = false;
            _isInitialized = true;
            boxCollider.center = new Vector3(0, 0,
                Vector3.Distance(_startPoint.position, _startPoint.position + _startPoint.forward * _maxDistance) / 2);
            Invoke(nameof(DestroyProjectile), data.duration);
            boxCollider.size = new Vector3(boxCollider.size.x, boxCollider.size.y,
                Vector3.Distance(_startPoint.position, _startPoint.position + _startPoint.forward * _maxDistance));
        }

        private void SetAsRichochet(Transform startPoint, Transform newEndPoint)
        {
            _startPoint = startPoint;
            endPoint = newEndPoint;
            Invoke(nameof(DestroyProjectile), data.duration);
        }

        private void FixedUpdate()
        {
            if (_startPoint == null || endPoint == null)
            {
                DestroyProjectile();
                return;
            }

            AnimateEndPoint();
            // CheckRay();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_hasHit) return;
            // var direction = (endPoint.position - _startPoint.position);
            // var hasHit = Physics.BoxCast(endPoint.position, new Vector3(0.5f, 2, _maxDistance * 5),
            //     direction.normalized, out var hit,
            //     Quaternion.identity, _maxDistance * 2, _layerMask);
            var hasHit = CompareLayer.IsInLayerMask(other.gameObject, _layerMask);

            if (hasHit)
            {
                var health = other.GetComponent<Health>();
                if (health)
                {
                    Damage(other);
                    if (isOneShot)
                    {
                        Destroy(gameObject);
                        return;
                    }
                }

                endPoint.position = other.ClosestPoint(endPoint.position);
                if (ricochet.isAvaible && _weapon.HasRicochet)
                {
                    var newTarget = ricochet.FindRandomTarget(endPoint.position, _layerMask);

                    if (!newTarget)
                    {
                        Destroy(gameObject);
                        return;
                    }

                    var newQuaternion = Quaternion.LookRotation(newTarget.position - _startPoint.position);
                    var newChain = Instantiate(this, endPoint.position, newQuaternion, transform.parent);
                    newChain.InitBulletCommonData(data, _owner, _weapon);
                    newChain.SetAsRichochet(endPoint, newTarget);
                    _hasHit = true;
                    DestroyProjectile();
                }

                if (!hasPiercing && !_weapon.HasPiercingShot)
                {
                    DestroyProjectile();
                    _hasHit = true;
                }
            }
        }


        private void OnDrawGizmos()
        {
            Gizmos.DrawCube(endPoint.position, new Vector3(0.5f, 2, _maxDistance * 5));
        }

        private void DestroyProjectile()
        {
            Destroy(gameObject, 0.1f);
        }

        private void AnimateEndPoint()
        {
            if (_hasHit || !_isInitialized) return;
            if (Vector3.Distance(_startPoint.position, endPoint.position) >= _maxDistance) return;
            endPoint.position += transform.forward * Time.fixedDeltaTime * _animateSpeed;
            lineRenderer.SetPosition(1, endPoint.position);
        }
    }
}