using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Project
{
    public class Granatman : BaseTower
    {
        private CancellationTokenSource _checkTarget;

        public override TowerType TowerType
        {
            get => TowerType.Granatman;
        }

        public override bool IsAssTower
        {
            get => false;
        }

        private void OnDestroy()
        {
            UniTaskUtil.CancelToken(ref _checkTarget);
        }

        public override void Attack()
        {
            if (_checkTarget != null)
            {
                return;
            }

            IsAttack = true;
            
            if (_rotateToOrigineCor != null)
            {
                StopCoroutine(_rotateToOrigineCor);
                _rotateToOrigineCor = null;
            }

            _lookAtEnemyCor = StartCoroutine(LookAtTargetCor());
            _attackController.RefreshAttackDelay();
            
            _towerViewModel.OnFire();
            _towerViewModel.Fire += TowerViewModelOnFire;

            CheckTargetAsync(UniTaskUtil.RefreshToken(ref _checkTarget));
        }

        private async void CheckTargetAsync(CancellationToken refreshToken)
        {
            while (!refreshToken.IsCancellationRequested)
            {
                if (Target.IsDied)
                {
                    StopAttack();
                }

                await UniTask.Yield(refreshToken);
            }
        }

        private void TowerViewModelOnFire()
        {
            _attackController.Fire();
        }

        public override void StopAttack()
        {
            base.StopAttack();
            
            
            UniTaskUtil.CancelToken(ref _checkTarget);
            
            _towerViewModel.Fire -= TowerViewModelOnFire;
        }
    }
}