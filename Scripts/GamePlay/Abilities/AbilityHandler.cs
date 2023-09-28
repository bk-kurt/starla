using System;
using System.Collections.Generic;
using Abilities.Data;
using GamePlay.Abilities;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Abilities
{
    public class AbilityHandler : MonoBehaviour
    {
        public AbilityData Data;

        private Dictionary<AbilityType, Type> abilitiesReferences = new Dictionary<AbilityType, Type>
        {
            { AbilityType.criticalDamage, typeof(IncreaseSpecialDamageAbility) },
            { AbilityType.DamageUp, typeof(IncreaseDamageAbility) },
            { AbilityType.AttackSpeedUp, typeof(IncreaseAttackSpeedAbility) },
            { AbilityType.HealthRewind, typeof(HealthRewindAbility) },
            { AbilityType.RestoreHealth, typeof(RestoreHealthAbility) },
            { AbilityType.IncreaseMaxHealth, typeof(IncreaseMaxHpAbility) },
            { AbilityType.PiercingShot, typeof(PiercingShotAbility) },
            { AbilityType.RichochetShot, typeof(RichochetAbility) },
            { AbilityType.ExplosionDamage, typeof(ExplosionDamageAbility) },
            { AbilityType.LifeSteal, typeof(LifeStealAbility) },
            { AbilityType.FrostNova, typeof(FrostNovaAbility) },
            // { AbilityType.ChainLightning, typeof(LightningChainAbility) },
            { AbilityType.ShockWave, typeof(ShockWaveAbility) },
            { AbilityType.SummonDrone, typeof(SummonDroneAbility) },
            { AbilityType.Revive, typeof(ReviveAbility) },
            { AbilityType.Invulnerability, typeof(InvulnerabilityAbility) },
        };

        private Dictionary<AbilityType, int> _abilityLevels = new Dictionary<AbilityType, int>();

        private List<IAbility> _pickedAbilities = new List<IAbility>();

        private List<AbilityType> _abilityTypes = new List<AbilityType>();

        public static Action<List<AbilityDataModel>, List<int>> onAbilitiesInitiate;
        public static Action<AbilityDataModel> onAbilityPickedCallback;
        public static Action onAbilityPicked;
        public static AbilityHandler Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(this);
            foreach (var reference in abilitiesReferences)
            {
                _abilityTypes.Add(reference.Key);
            }
        }

        [Button]
        public void UseAbility(AbilityType type)
        {
            var reference = FindAbilityByType(type);
            var level = _abilityLevels.GetValueOrDefault(type, 0);
            IAbility com;
            Ability ability;
            var data = FindAbilityDataModelByType(type);
            if (level == 0)
            {
                com = gameObject.AddComponent(reference) as IAbility;
                ability = (Ability)com;
                ability.SetMaxLevel(data.maxLevel);
                _abilityLevels.Add(type, level + 1);
            }
            else
            {
                com = gameObject.GetComponent(reference) as IAbility;
                ability = (Ability)com;
                ability.LevelUp();
                _abilityLevels[type] = level + 1;
            }

            com.Use();
            _pickedAbilities.Add(com);
            onAbilityPicked?.Invoke();
            onAbilityPickedCallback?.Invoke(data);
        }

        public void ResetAllAbilities()
        {
            foreach (var ability in _pickedAbilities)
            {
                Destroy((MonoBehaviour)ability);
            }

            _pickedAbilities.Clear();
            _abilityLevels.Clear();
        }

        private Type FindAbilityByType(AbilityType type)
        {
            foreach (var reference in abilitiesReferences)
            {
                if (type != reference.Key) continue;
                return reference.Value;
            }

            return null;
        }

        private AbilityDataModel FindAbilityDataModelByType(AbilityType type)
        {
            foreach (var dataModel in Data.abilities)
            {
                if (type != dataModel.type) continue;
                return dataModel;
            }

            return null;
        }

        private int FindAbilityLevelByType(AbilityType type)
        {
            return _abilityLevels.GetValueOrDefault(type, 0);
        }

        public void EnableAbilityState()
        {
            var abilityLists = new List<AbilityDataModel>();
            var abilityLevels = new List<int>();
            var temoAbilityTypeCount = new List<AbilityType>(_abilityTypes);
            int count = 3;
            while (count > 0)
            {
                var randomIndex = Random.Range(0, temoAbilityTypeCount.Count - 1);
                if (FindAbilityLevelByType(temoAbilityTypeCount[randomIndex]) >=
                    FindAbilityDataModelByType(temoAbilityTypeCount[randomIndex]).maxLevel)
                {
                    temoAbilityTypeCount.RemoveAt(randomIndex);
                    continue;
                }
                abilityLists.Add(FindAbilityDataModelByType(temoAbilityTypeCount[randomIndex]));
                abilityLevels.Add(FindAbilityLevelByType(temoAbilityTypeCount[randomIndex]));
                temoAbilityTypeCount.RemoveAt(randomIndex);
                count--;
            }

            onAbilitiesInitiate?.Invoke(abilityLists, abilityLevels);
        }
    }
}