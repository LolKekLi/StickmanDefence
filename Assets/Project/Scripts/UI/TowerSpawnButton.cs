using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public enum TowerType
{
    None = 0,
    Gunner = 1,
}

namespace Project.UI
{
    public class TowerSpawnButton : MonoBehaviour
    {
        [SerializeField]
        private Button _button = null;

    }

    public class UITowerSpawnController : MonoBehaviour
    {
        private TowerSpawnButton[] _towerSpawnButtons = null;

        private void Start()
        {
            _towerSpawnButtons = GetComponentsInChildren<TowerSpawnButton>();

            for (int i = 0; i < _towerSpawnButtons.Length; i++)
            {
                //_towerSpawnButtons[i].Setup();
            }
        }
    }

    public class TowerSpawnController : MonoBehaviour
    {
        private UITowerSpawnController _uiTowerSpawnController = null;

        [Inject]
        private void Construct(UITowerSpawnController uiTowerSpawnController)
        {
            _uiTowerSpawnController = uiTowerSpawnController;
        }
    }
}

namespace Project
{
    public abstract class ToggleObject : MonoBehaviour
    {
        
    }

}

