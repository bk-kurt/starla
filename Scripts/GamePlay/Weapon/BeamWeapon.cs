using GamePlay.Bullets;
using UnityEngine;

namespace GamePlay.Weapon
{
    public class BeamWeapon : Weapon
    {
        [SerializeField] private BeamBullet beamBullet;
        [SerializeField] private float beamSpeed;
        private BeamBullet _bullet;
        


        protected override void InstatiateBullet()
        {
            foreach (var socket in sockets)
            {
                _bullet = Instantiate(beamBullet, socket.position, socket.rotation, socket.transform);
                _bullet.InitBulletCommonData(ApplySpecialDamage(), _owner, this);

                _bullet.Initialize(socket, 40,beamSpeed);
            }
        }
    }
}