using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Project
{
    public abstract class BaseFactory<T> : ZenjectManager<BaseFactory<T>> where T : MonoBehaviour
    {
        protected abstract T Create(Vector3 position, Quaternion rotation, Transform parent = null);
    }

    public class TowerModelFactory : BaseFactory<TowerViewModel>
    {
        [Inject]
        private TowerViewModelSettings _towerViewModelSettings = null;

        private List<TowerViewModel> _towerViewModels = null;

        protected override void Init()
        {
            base.Init();

            _towerViewModels = new List<TowerViewModel>(15);
            
            PrepareTowerViewModels();
        }

        private void PrepareTowerViewModels()
        {
            var towerViewModelPresets = _towerViewModelSettings.TowerViewModelPresets;

            for (var i = 0; i < towerViewModelPresets.Length; i++)
            {
                var towerViewModel = Instantiate(towerViewModelPresets[i].TowerViewModel, Vector3.zero,
                    Quaternion.identity, transform);
                
                _towerViewModels.Add(towerViewModel);
            }
        }

        public TowerViewModel CreateTowerViewModel(TowerMeshType towerMeshType, Vector3 position, Quaternion rotation,
            Transform parent = null)
        {
            return null;
        }

        protected override TowerViewModel Create(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            return null;
        }
    }
}