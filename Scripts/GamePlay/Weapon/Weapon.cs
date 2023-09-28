using System;
using System.Collections;
using System.Collections.Generic;
using GamePlay.Agents;
using GamePlay.Bullets;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GamePlay.Weapon
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected BulletData data;
        private BulletData _data;
        protected Agent _owner;
        [SerializeField] protected List<Transform> sockets = new List<Transform>();

        public BulletData Data => _data;
        public bool HasPiercingShot { get; private set; }
        [SerializeField] private bool _hasRichochet;
        public bool HasRicochet => _hasRichochet;
        public bool HasExplosionDamage { get; private set; }
        private BulletSpecialDamageData _specialDamageData;
        [SerializeField] protected ShootData shootData;
        public ShootData ShootData => shootData;
        protected WaitForSeconds _burstSeconds;
        protected bool _isFiring;
        public bool IsFiring => _isFiring;
        protected int _onFire;
        [SerializeField] protected Animator animator;
        protected readonly int OnIdle = Animator.StringToHash("OnIdle");
        [SerializeField] protected string shotAnimationTrigger;
        [SerializeField] private Aiming aiming;

        public Action onFireFinish;
        public Action onFireStart;
        public Action onPreprationStart;
        private int OnPrepration;
        [SerializeField] protected string preprationAnimationTrigger = "OnPrepration";
        

        private void Awake()
        {
            Initialize();
        }

        public void ModifyDefaultDataDamage(int damageAmount)
        {
            _data.damage += damageAmount;
        }

        public void Initialize()
        {
            _burstSeconds = new WaitForSeconds(shootData.TimeBetweenShot);
            _onFire = Animator.StringToHash(shotAnimationTrigger);
            OnPrepration = Animator.StringToHash(preprationAnimationTrigger);

            _data = data;
        }

        public void SetDefaultDamage()
        {
            _data = data;
        }

        public void SetDefaultSpecialDamage()
        {
            _specialDamageData = null;
        }

        public void IncreaseDamageByPercent(float damagePercent)
        {
            var newAmount = _data.damage * damagePercent;
            _data.damage += newAmount;
        }

        public void IncreaseDamageByPercent(int amount)
        {
            _data.damage += amount;
        }

        private void Update()
        {
            HandleAiming();
        }

        private void HandleAiming()
        {
            if (!aiming) return;
            if (_isFiring)
                aiming.Rotate();
            else
                aiming.ResetRotation();
        }

        public IEnumerator Fire()
        {
            if (animator)
            {
                animator.ResetTrigger(OnIdle);
                // animator.SetTrigger(_onFire);
            }

            _isFiring = true;
            if (animator)
                animator.SetTrigger(OnPrepration);
            onPreprationStart?.Invoke();
            yield return new WaitForSeconds(shootData.preprationTime);

            onFireStart?.Invoke();
            for (int i = 0; i < shootData.countPerSecond; i++)
            {

                if (animator)
                    animator.SetTrigger(_onFire);
                InstatiateBullet();
                yield return _burstSeconds;
            }

            _isFiring = false;

            if (animator)
            {
                animator.SetTrigger(OnIdle);
                animator.ResetTrigger(_onFire);

            }
            onFireFinish?.Invoke();
        }

        public void ManualFire()
        {
            InstatiateBullet();
        }

        public void SetSpecialDamageData(BulletSpecialDamageData specialDamageData)
        {
            _specialDamageData = specialDamageData;
        }

        protected BulletData ApplySpecialDamage()
        {
            BulletData newData = new BulletData
            {
                damage = _data.damage,
                duration = _data.duration,
                bulletSpeed = _data.bulletSpeed
            };
            var randomChance = Random.value;
            if (_specialDamageData == null || randomChance > _specialDamageData.criticalChance) return newData;
            var newAmount = newData.damage * _specialDamageData.criticalCoeff;
            newData.damage += (int)newAmount;
            newData.isCritical = true;
            return newData;
        }

        public void SetDamage(int damageAmount)
        {
            _data.damage = damageAmount;
        }

        public void SetOwner(Agent owner)
        {
            _owner = owner;
        }

        public void SetPiercingShot(bool status)
        {
            HasPiercingShot = status;
        }

        public void SetRichochet(bool status)
        {
            _hasRichochet = status;
        }

        public void SetExplosionDamage(bool status)
        {
            HasExplosionDamage = status;
        }

        protected abstract void InstatiateBullet();
    }

    [Serializable]
    public class ShootData
    {
        public float countPerSecond;
        public float TimeBetweenShot;
        public float preprationTime;
    }
}