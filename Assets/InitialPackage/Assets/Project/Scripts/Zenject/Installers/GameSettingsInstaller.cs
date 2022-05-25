using UnityEngine;
using Zenject;

namespace Project
{
    [CreateAssetMenu(fileName = "GameSettingsInstaller", menuName = "Scriptable/Zenject/Level Settings Installer")]
    public class GameSettingsInstaller : ScriptableObjectInstaller<GameSettingsInstaller>
    {
        public override void InstallBindings()
        {
            
        }
    }
}