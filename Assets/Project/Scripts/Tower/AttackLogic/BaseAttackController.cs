using UnityEngine;

namespace Project
{
    public abstract class BaseAttackController
    {
        private BulletSettings _bulletSettings;
        protected BulletSettings _bulletPreset;
        protected Transform _firePosition;

        protected float _damageUpdateProcent = 0;
        protected float _fireSpeedUpdateProcent = 0;
        protected Transform _transform;
        protected FirePreset _firePreset;
        protected PoolManager _poolManager;

        public Enemy Target
        {
            get;
            set;
        }

        public WaitForSeconds AttackDelay
        {
            get;
            protected set;
        }

        public abstract void Fire();

        public virtual void Setup(BulletSettings bulletPreset, Transform firePosition, Transform transform,
            FirePreset firePreset, PoolManager poolManager)
        {
            _poolManager = poolManager;
            _firePreset = firePreset;
            _transform = transform;
            _firePosition = firePosition;
            _bulletPreset = bulletPreset;
        }

        public abstract void RefreshAttackDelay();

        public abstract void UpdateDamage(float updatePresetValue);
        public abstract void UpdateFireSpeed(float updatePresetValue);
    }

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