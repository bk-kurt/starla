using Agents;
using GameInventory.Pickups;
using UnityEngine;

namespace GamePlay.Agents.Enemies
{
    public class Boss : Enemy
    {
        [SerializeField] private HealthPickUp healthPickUpPrefab;
        [SerializeField] private int pickUpAmount = 2;

        protected override void OnEnable()
        {
            base.OnEnable();
            health.OnDeath += AddHealthPickUp;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            health.OnDeath -= AddHealthPickUp;
        }

        public void AddHealthPickUp(float deathDelay)
        {
            for (int n = 0; n < pickUpAmount; n++)
            {
                var drop = Instantiate(healthPickUpPrefab, transform.position, Quaternion.identity);
                drop.Drop(RandomPosInRadius());
            }
        }
        
        private Vector3 RandomPosInRadius()
        {
            var randomInCircle = Random.insideUnitCircle * 2;
            return transform.position + new Vector3(randomInCircle.x, 0, randomInCircle.y);
        }
    }
}