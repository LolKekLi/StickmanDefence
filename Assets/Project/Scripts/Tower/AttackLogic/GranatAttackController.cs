using UnityEngine;

namespace Project
{
    public class GranatAttackController : BaseAttackController
    {
        public override void Fire()
        {
            var bullet = _poolManager.Get<Granat>(_poolManager.PoolSettings.Granat, _firePosition.position,
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