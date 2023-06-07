using UnityEngine;

namespace Project
{
    public abstract class BulletBase : PooledBehaviour
    {
        protected int _damageAbility = 0;
        
        protected float _damage = 1;
        protected float _lifeTime = 0f;
        protected float _speed = 0f;
        protected float _damageUpdateProcent;

        public DamageType DamageType
        {
            get;
            private set;
        }

        public void Setup(BulletSettings bulletSettings, float damageUpdateProcent)
        {
            _damageUpdateProcent = damageUpdateProcent;
            _damage = bulletSettings.BaseDamage;
            _lifeTime = bulletSettings.LifeTime;
            _speed = bulletSettings.Speed;
            DamageType = bulletSettings.DamageType;
        }

        public abstract void Shoot(Vector3 direction);

        public abstract float GetDamage();
    }
}