using System.Collections;
using System.Collections.Generic;
using Project;
using UnityEngine;

public class SpawnZoneController : MonoBehaviour
{
    [SerializeField]
    private List<CantSpawnZone> _cantSpawnZones = null;

    [SerializeField]
    private bool _isNeedVisual = true; 
    
    private Coroutine _controlSpawnCor = null;

    public void StartControlSpawn(Tower currentTower)
    {
        _controlSpawnCor = StartCoroutine(ControlSpawnCor(currentTower));
    }

    [ContextMenu("AddAllSpawnZones")]
    public void AddAllSpawnZones()
    {
        _cantSpawnZones.Clear();
        _cantSpawnZones = GetComponentsInChildren<CantSpawnZone>().ToList();
    }
    
    public void AddSpawnZone(CantSpawnZone cantSpawnZone)
    {
        _cantSpawnZones.Add(cantSpawnZone);
    }

    private IEnumerator ControlSpawnCor(Tower currentTower)
    {
        bool isZoneContainsTower = false;

        while (!currentTower.IsSpawned)
        {
            yield return null;

            for (int i = 0; i < _cantSpawnZones.Count; i++)
            {
                var cantSpawnZone = _cantSpawnZones[i];

                if (cantSpawnZone.Contains(currentTower.transform.position))
                {
                    isZoneContainsTower = true;

                    break;
                }
            }

            currentTower.ToggleSpawnAbility(!isZoneContainsTower);
            isZoneContainsTower = false;
        }
    }
    
    private void ToggleVisual()
    {
        for (int i = 0; i < _cantSpawnZones.Count; i++)
        {
            _cantSpawnZones[i].ToggleVisual(_isNeedVisual);
        }
    }

    private void OnDrawGizmos()
    {
        ToggleVisual();
    }
}