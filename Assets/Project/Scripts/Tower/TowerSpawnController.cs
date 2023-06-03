using System;
using System.Collections;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerSpawnController : MonoBehaviour
    {
        private readonly int MaxRayCastDistance = 1000;
        public static event Action TowerSpawned = delegate { };

        [SerializeField]
        private LayerMask _ignoreRaycastLayerMask;

        private Vector3 _movePosition = Vector3.zero;
        private Quaternion _towerRotation = default;

        private TowerSettings _towerSettings = null;
        private CameraController _cameraController = null;
        private SpawnZoneController _spawnZoneController = null;
        private TowerController _towerController = null;

        private Coroutine _towerSpawnCor = null;
        private TowerUpgradeController _towerUpgradeController;
        private AttackControllerFactory _attackControllerFactory;
        private TowerSettings.TowerPreset _currentTowerPreset;


        public BaseTower CurrentTower
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(TowerSettings towerSettings, CameraController cameraController,
            SpawnZoneController spawnZoneController, TowerController towerController,
            TowerUpgradeController towerUpgradeController)
        {
            _towerUpgradeController = towerUpgradeController;
            _towerSettings = towerSettings;
            _cameraController = cameraController;
            _spawnZoneController = spawnZoneController;
            _towerController = towerController;
        }

        private void OnEnable()
        {
            UITowerSpawnController.PointerExited += UITowerSpawnController_PointerExited;
            UITowerSpawnController.PointerEntered += UITowerSpawnController_PointerEntered;
        }

        private void OnDisable()
        {
            UITowerSpawnController.PointerExited -= UITowerSpawnController_PointerExited;
            UITowerSpawnController.PointerEntered -= UITowerSpawnController_PointerEntered;
        }

        private void Start()
        {
            _attackControllerFactory = new AttackControllerFactory();
            _ignoreRaycastLayerMask = ~_ignoreRaycastLayerMask;
            _towerRotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }

        private void Update()
        {
            _movePosition = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                StopTowerSpawn();
            }
        }

        private void BuildTower()
        {
            if (CurrentTower.IsCanSpawn)
            {
                TowerSpawned();

                CurrentTower.OnBuild(_attackControllerFactory.GetAttackController(_currentTowerPreset.FirePreset.FireType));
                CurrentTower = null;

                StopTowerSpawn();

#if UNITY_EDITOR
                _towerController.RefreshName();
#endif
            }
        }

        private void StopTowerSpawn()
        {
            if (_towerSpawnCor != null)
            {
                StopCoroutine(_towerSpawnCor);
                _towerSpawnCor = null;
            }

            if (CurrentTower != null)
            {
                CurrentTower.Free();
                CurrentTower = null;
            }
        }

        private IEnumerator TowerSpawnCor(TowerType towerType, TowerViewModelType towerViewModelType)
        {
            Ray screenPointToRay = default;
            RaycastHit hit = default;

            while (!Physics.Raycast(screenPointToRay, out hit, MaxRayCastDistance, _ignoreRaycastLayerMask))
            {
                screenPointToRay = _cameraController.Camera.ScreenPointToRay(_movePosition);

                yield return null;
            }

            CurrentTower = _towerController.GetTower(towerType, towerViewModelType, hit.point, _towerRotation);

            CurrentTower.transform.parent = transform;

            _spawnZoneController.StartControlSpawn(CurrentTower);

            while (true)
            {
                yield return null;

                screenPointToRay = _cameraController.Camera.ScreenPointToRay(_movePosition);


                if (Physics.Raycast(screenPointToRay, out hit, MaxRayCastDistance, _ignoreRaycastLayerMask))
                {
                    if (CurrentTower != null)
                    {
                        CurrentTower.Move(hit.point);
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        if (CurrentTower != null)
                        {
                            var cantSpawnZone = CurrentTower.CantSpawnZone;
                            cantSpawnZone.ChangeBorderCenter(CurrentTower.transform.position);

                            _spawnZoneController.AddSpawnZone(cantSpawnZone);

                            BuildTower();
                        }
                    }
                }
            }
        }

        private void UITowerSpawnController_PointerExited(TowerType currentTowerType)
        {
             _currentTowerPreset = _towerSettings.GetTowerPresetByType(currentTowerType);

            StopTowerSpawn();

            if (_towerUpgradeController.CurrentTower != null)
            {
                _towerUpgradeController.UnselectedCurrentTower();
            }

            StartCoroutine(
                TowerSpawnCor(_currentTowerPreset.TowerPrefab.TowerType, _currentTowerPreset.BaseViewModelType));
        }

        private void UITowerSpawnController_PointerEntered()
        {
            StopTowerSpawn();
        }
    }
}