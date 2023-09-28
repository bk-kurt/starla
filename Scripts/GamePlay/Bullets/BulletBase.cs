using System;
using com.joyixir.starla.Agent;
using GamePlay.Bullets.Components;
using UnityEngine;

namespace GamePlay.Bullets
{
    public abstract class BulletBase : MonoBehaviour
    {
        protected Agents.Agent _owner;
        protected Weapon.Weapon _weapon;
        [SerializeField] protected ExplosionDamage explosionDamage;
        [SerializeField] protected Ricochet ricochet;
        [SerializeField] protected LayerMask _layerMask;
        [SerializeField] protected bool hasPiercing;

        public BulletData data { get; private set; }

        public void InitBulletCommonData(BulletData bulletData, Agents.Agent owner, Weapon.Weapon weapon)
        {
            _owner = owner;
            _weapon = weapon;
            data = bulletData;
            if (explosionDamage)
                explosionDamage.Init(_layerMask, data.damage, weapon.HasExplosionDamage);
        }

        protected void Damage(Collider health)
        {
            if (_owner == null) return;
            var obj = health.GetComponent<Health>();
            if (obj == null) return;
            obj.TakeDamage(data.damage, data.isCritical, OnEnemyDeadWithDamage);
        }

        private void OnEnemyDeadWithDamage(int enemyHealth)
        {
            if (_owner == null) return;
            _owner.LifeStealEffect(enemyHealth);
        }
    }


    [Serializable]
    public class BulletData
    {
        public float damage;
        public float bulletSpeed;
        public float duration;
        [NonSerialized] public bool isCritical;
    }

    public class BulletSpecialDamageData
    {
        public float criticalChance;
        public float criticalCoeff;
    }
}