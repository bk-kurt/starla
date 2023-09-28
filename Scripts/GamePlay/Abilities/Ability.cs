using Abilities.Data;
using Controllers;
using UnityEngine;

namespace Abilities
{
    public abstract class Ability : MonoBehaviour, IAbility
    {
        private int _level;
        public int Level => _level;
        private int _maxLevel;
        public bool isMaxLevel => _level < _maxLevel;

        public void SetMaxLevel(int maxLevel)
        {
            _maxLevel = maxLevel;
        }

        public virtual void LevelUp()
        {
            _level++;
            RaisePlayerUpgraded();
        }

        public virtual void Use()
        {
            RaisePlayerUpgraded();
        }

        public abstract void OnDestroy();

        protected void RaisePlayerUpgraded()
        {
            KinjaGameController.Player.Upgrade();
        }

        public virtual AbilityType Type { get; }
    }
}