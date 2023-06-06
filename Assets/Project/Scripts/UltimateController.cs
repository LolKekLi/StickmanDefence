using System;
using System.Linq;
using Project.UI;
using UnityEngine;

namespace Project
{
    public class UltimateController : MonoBehaviour
    {
        public static event Action KillAll = delegate {  };
        
        [SerializeField]
        private Ultimate[] _ultimates;

        private void OnEnable()
        {
            GameWindow.UltimateApplied += GameWindow_UltimateApplied;
        }

        private void OnDisable()
        {
            GameWindow.UltimateApplied -= GameWindow_UltimateApplied;
        }

        private void GameWindow_UltimateApplied(UltimateType ultimateType)
        {
            var ultimate = _ultimates.FirstOrDefault(x => x.UltimateType == ultimateType);

            if (!ultimate)
            {
                Debug.LogError($"Нет ульты пот тип {ultimateType}");
                return;
            }

            ultimate.Apply(() =>
            {
                KillAll();
            });

        }
    }
}