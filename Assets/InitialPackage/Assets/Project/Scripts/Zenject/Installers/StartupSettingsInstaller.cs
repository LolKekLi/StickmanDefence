using UnityEngine;
using Zenject;

namespace Project
{
    [CreateAssetMenu(fileName = "StartupSettingsInstaller", menuName = "Scriptable/Zenject/Startup Settings Installer")]
    public class StartupSettingsInstaller : ScriptableObjectInstaller<StartupSettingsInstaller>
    {
        public override void InstallBindings()
        {
            
        }
    }
}