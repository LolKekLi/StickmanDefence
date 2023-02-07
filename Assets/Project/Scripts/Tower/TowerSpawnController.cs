using System.Collections;
using Project.UI;
using UnityEngine;
using Zenject;

namespace Project
{
    public class TowerSpawnController : MonoBehaviour
    {
        private readonly string IgnoreRaycastLayer = "Tower";
        private readonly int MaxRayCastDistance = 1000;

        private int _layerMask = 1;

        private Vector3 _movePosition = Vector3.zero;
        private Quaternion _towerRotation = default;

        private TowerSettings _towerSettings = null;
        private CameraController _cameraController = null;
        private SpawnZoneController _spawnZoneController = null;
        private TowerController _towerController = null;

        private Coroutine _towerSpawnCor = null;

        public BaseTower CurrentTower
        {
            get;
            private set;
        }

        [Inject]
        private void Construct(TowerSettings towerSettings, CameraController cameraController,
            SpawnZoneController spawnZoneController, TowerController towerController)
        {
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
            _layerMask = 1 << LayerMask.NameToLayer(IgnoreRaycastLayer);
            _layerMask = ~_layerMask;

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

        private void SpawnTower()
        {
            if (CurrentTower.IsCanSpawn)
            {
                CurrentTower.Spawn();
                CurrentTower = null;

                StopTowerSpawn();
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

        private IEnumerator TowerSpawnCor(BaseTower refTower)
        {
            Ray screenPointToRay = default;
            RaycastHit hit = default;

            while (!Physics.Raycast(screenPointToRay, out hit, MaxRayCastDistance, _layerMask))
            {
                screenPointToRay = _cameraController.Camera.ScreenPointToRay(_movePosition);

                yield return null;
            }

            CurrentTower = _towerController.GetTower(refTower, hit.point, _towerRotation);

            CurrentTower.transform.parent = transform;

            _spawnZoneController.StartControlSpawn(CurrentTower);

            while (true)
            {
                yield return null;

                screenPointToRay = _cameraController.Camera.ScreenPointToRay(_movePosition);

                if (Physics.Raycast(screenPointToRay, out hit, MaxRayCastDistance, _layerMask))
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

                            SpawnTower();
                        }
                    }
                }
            }
        }

        private void UITowerSpawnController_PointerExited(TowerType currentTowerType)
        {
            var currentTowerPreset = _towerSettings.GetPresetByType(currentTowerType);

            StopTowerSpawn();

            StartCoroutine(TowerSpawnCor(currentTowerPreset.Tower));
        }

        private void UITowerSpawnController_PointerEntered()
        {
            StopTowerSpawn();
        }
    }
}