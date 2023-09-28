using System;
using System.Collections.Generic;
using Components;
using DG.Tweening;
using GamePlay.Agents.Components;
using GamePlay.Agents.Drone;
using GamePlay.Bullets;
using GamePlay.Weapon;
using GamePlay.Weapon.Player;
using Joyixir.Utils;
using UnityEngine;

namespace GamePlay.Agents.Player
{
    public class Player : Agent
    {
        [SerializeField] private SlideMovement slideMovement;
        [field: SerializeField] public CountBasePlayerWeaponController WeaponController { get; private set; }
        [field: SerializeField] public PlayerPeriodicWeaponController FrostNovaWeaponController { get; private set; }
        [field: SerializeField] public PlayerPeriodicWeaponController LightningWeaponController { get; private set; }
        [SerializeField] private List<SpaceshipWingTrailHandler> trals;
        private List<PlayerWeaponController> _weaponControllers = new List<PlayerWeaponController>();

        [SerializeField] private ShockWave shockWaveObject;
        [SerializeField] private Revive revive;
        [SerializeField] private DroneController droneObject;
        [SerializeField] private Invulnerability invulnerability;
        [SerializeField] private ForwardMovement _forwardMovement;
        [SerializeField] private Spaceship spaceship;
        private DroneController _droneReference;
        private ShockWave _shockWaveRefernce;
        private Collider _playerCollider;
        private List<int> _repairs = new List<int>();
        public override AgentType type => AgentType.Player;
        private List<Coroutine> _weaponCoroutines = new List<Coroutine>();


        public Action onPlayerReachedFightArena;
        public Action onPlayerReachedEnd;
        public Action onPlayerReachedPortal;

        private bool isInFight;
        public Action onPlayerUpgraded;
        public Action OnPlayerDead;

        private void Awake()
        {
            _playerCollider = GetComponent<SphereCollider>();
            healthBar.gameObject.SetActive(false);
        }


        public void Initialize()
        {
            _weaponControllers.Add(WeaponController);
            revive.onOutOfLife += ListenToDeath;
            revive.onRevive += ListenToRevive;
            slideMovement.Initialize();
            _playerCollider.enabled = true;
            healthBar.gameObject.SetActive(true);
            WeaponController.SetDefaultAttackSpeed();
            foreach (var tral in trals)
            {
                tral.Show();
            }

            RefreshPlayer();
        }

        public void InitRunnerPlayer()
        {
            _forwardMovement.IsEnable = true;
            slideMovement.ChangeVerticalInputStatus(false);
            foreach (var tral in trals)
            {
                tral.ChangeFakeRendererStatus(false);
            }
        }

        public void InitAbilityPart()
        {
            slideMovement.ChangeVerticalInputStatus(false);
        }

        public void InitFighterPlayer()
        {
            _forwardMovement.IsEnable = false;
            slideMovement.ChangeVerticalInputStatus(true);
            foreach (var tral in trals)
            {
                tral.ChangeFakeRendererStatus(true);
            }

            ApplyRepairs();
        }

        private void ListenToRevive()
        {
            healthBar.gameObject.SetActive(true);
        }

        public void DeInitialize()
        {
            StopFire();
            _weaponControllers.Clear();
            revive.onOutOfLife -= ListenToDeath;
            revive.onRevive -= ListenToRevive;
            revive.DeInitialize();
            healthBar.gameObject.SetActive(false);
            if (_shockWaveRefernce)
                _shockWaveRefernce.StopShockWave();
            _playerCollider.enabled = false;
            foreach (var tral in trals)
            {
                tral.Hide();
            }

            slideMovement.DeInitialize();
        }

        private void ListenToDeath()
        {
            StopFire();
            slideMovement.DeInitialize();
            healthBar.gameObject.SetActive(false);
            OnPlayerDead?.Invoke();
        }

        public void StartFire()
        {
            isInFight = true;
            _weaponCoroutines = new List<Coroutine>();
            foreach (var weaponController in _weaponControllers)
            {
                var cor = StartCoroutine(weaponController.StartFire());
                _weaponCoroutines.Add(cor);
            }

            if (_shockWaveRefernce)
                _shockWaveRefernce.StartShockWave();
            if (_droneReference)
                _droneReference.Summon();
        }

        public void StopFire()
        {
            isInFight = false;
            foreach (var weaponCoroutine in _weaponCoroutines)
                StopCoroutine(weaponCoroutine);
            if (_shockWaveRefernce)
                _shockWaveRefernce.StopShockWave();
            if (_droneReference)
                _droneReference.UnSummon();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("FightCollider"))
            {
                if (!isInFight)
                    onPlayerReachedFightArena?.Invoke();
            }

            if (other.CompareTag("EndCollider"))
            {
                onPlayerReachedEnd?.Invoke();
            }

            if (other.CompareTag("GoingToPortalCollider"))
            {
                slideMovement.DeInitialize();
                transform.DOMoveX(0, 1f);
                onPlayerReachedPortal?.Invoke();
            }
        }

        private void RefreshPlayer()
        {
            health.Initialize();
            health.ReHeal(100);
        }

        public void AddMaxHeal(int amount)
        {
            spaceship.IncreaseHp(amount);
            // health.IncreasePlayerMaxHealth(amount);
        }

        public void UpgradeWeaponDamage(float amount)
        {
            WeaponController.UpgradeWeaponDamage(amount);
        }

        public void UpgradeWeaponDamage(int amount)
        {
            spaceship.IncreaseDamage(amount);
            // WeaponController.UpgradeWeaponDamage(amount);
        }


        public void UpgradeWeaponAttackSpeed(float amount)
        {
            WeaponController.IncreaseAttackSpeed(amount);
        }

        public void SetSpecialDamageData(BulletSpecialDamageData specialDamageData)
        {
            WeaponController.SetSpecialDamageData(specialDamageData);
        }

        public void SetHighSpeed()
        {
            foreach (var spaceshipWingTrailHandler in trals)
            {
                spaceshipWingTrailHandler.SetHighSpeed();
            }
        }

        public void SetNormalSpeed()
        {
            foreach (var spaceshipWingTrailHandler in trals)
            {
                spaceshipWingTrailHandler.SetNormalSpeed();
            }
        }

        public void EnableFrostNova()
        {
            _weaponControllers.Add(FrostNovaWeaponController);
        }

        public void DisableFrostNova()
        {
            _weaponControllers.Remove(FrostNovaWeaponController);
        }

        public void EnableLightningChain()
        {
            _weaponControllers.Add(LightningWeaponController);
        }

        public void DisableLightningChain()
        {
            _weaponControllers.Remove(LightningWeaponController);
        }

        public void EnableShockWave()
        {
            _shockWaveRefernce = shockWaveObject;
        }

        public void DisableShockWave()
        {
            if (_shockWaveRefernce)
                _shockWaveRefernce.StopShockWave();
            _shockWaveRefernce = null;
        }

        public void Revive()
        {
            revive.IncreaseRevive();
        }

        public void ResetRevive()
        {
            revive.ResetRevive();
        }

        public void EnableDrone()
        {
            _droneReference = Instantiate(droneObject);
            _droneReference.SetOwner(this);
        }

        public void DisableDrone()
        {
            Destroy(_droneReference.gameObject);
        }


        public void ActiveInvulnerability()
        {
            invulnerability.Initialize();
        }

        public void LevelUpInvulnerability(int level)
        {
            invulnerability.LevelUp(level);
        }

        public void DeActiveInvulnerability()
        {
            invulnerability.DeInitialize();
        }

        public void AddRepair(int effect)
        {
            _repairs.Add(effect);
        }

        private void ApplyRepairs()
        {
            foreach (var repair in _repairs)
            {
                health.ReHeal(repair);
            }
        }

        public void EndState()
        {
            healthBar.gameObject.SetActive(false);
            transform.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InCirc);
            foreach (var tral in trals)
            {
                tral.Hide();
            }

            _forwardMovement.IsEnable = false;
        }

        public void BackToMenuState()
        {
            transform.localScale = Vector3.one;
        }

        public void Upgrade()
        {
            onPlayerUpgraded?.Invoke();
        }
    }
}