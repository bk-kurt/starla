using System;
using System.Collections.Generic;
using com.joyixir.starla.Agent;
using GamePlay.Agents;
using GamePlay.Agents.Components.Mover;
using GamePlay.Pickups;
using GamePlay.Weapon;
using Opsive.UltimateInventorySystem.Core;
using Opsive.UltimateInventorySystem.Core.AttributeSystem;
using UnityEngine;

namespace Agents
{
    public class Enemy : Agent
    {
        [SerializeField] private EnemyWeaponController _weaponController;
        [field: SerializeField] public Mover mover { get; private set; }
        [field: SerializeField] public DropPickup DropPickup { get; private set; }
        public override AgentType type => AgentType.Enemy;
        public Health Health => health;

        private Coroutine _weaponCoroutine;

        protected virtual void OnEnable()
        {
            health.Initialize();
            health.OnDeath += ListenToDeath;
        }

        protected virtual void OnDisable()
        {
            StopCoroutine(_weaponCoroutine);
            health.OnDeath -= ListenToDeath;
        }

        protected void ListenToDeath(float obj)
        {
            DropPickup.Drop();
            DropPickup.DropCurrency();
            healthBar.gameObject.SetActive(false);
        }

        public void StopFire()
        {
            _weaponController.StopFire();
        }

        private void Start()
        {
            _weaponController.SetOwner(this);
            _weaponCoroutine = StartCoroutine(_weaponController.StartFire());
        }

        public void AddChipDrop(ItemDefinition pickUpItemDef)
        {
            var obj = pickUpItemDef.GetAttribute<Attribute<GameObject>>("PickupObject").GetValue();
            DropPickup.AddPickUp(obj.GetComponent<Pickup>(), pickUpItemDef, 1);
        }

        public void UpgradeHealth(float percent)
        {
            health.IncreasePlayerMaxHealth(percent);
        }

        public void UpgradeDamage(float percent)
        {
            _weaponController.UpgradeDamage(percent);
        }
    }
}