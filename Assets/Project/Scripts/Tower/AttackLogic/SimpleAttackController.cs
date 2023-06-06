using UnityEngine;

namespace Project
{
    public class SimpleAttackController : BaseAttackController
    {
        public override void Setup(BulletSettings bulletPreset, Transform firePosition, Transform transform, FirePreset firePreset,
            PoolManager poolManager)
        {
            base.Setup(bulletPreset, firePosition, transform, firePreset, poolManager);

            RefreshAttackDelay();
        }

        public override void Fire()
        {
            var bullet = _poolManager.Get<Bullet>(_poolManager.PoolSettings.Bullet, _firePosition.position,
                Quaternion.identity);

            bullet.Setup(_bulletPreset, _damageUpdateProcent);
            bullet.Shoot((Target.transform.position - _transform.position).normalized);
        }

        public override void RefreshAttackDelay()
        {
            var fireRate = 1 / _firePreset.FireRate;
            fireRate -= fireRate * (_fireSpeedUpdateProcent / 100);
            AttackDelay = new WaitForSeconds(fireRate);
        }

        public override void UpdateDamage(float updatePresetValue)
        {
            _damageUpdateProcent = updatePresetValue;
        }

        public override void UpdateFireSpeed(float updatePresetValue)
        {
            _fireSpeedUpdateProcent = updatePresetValue;
        }
    }
}