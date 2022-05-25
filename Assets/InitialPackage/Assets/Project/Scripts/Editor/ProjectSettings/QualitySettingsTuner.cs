using UnityEditor;
using UnityEngine;

namespace Project
{
    public class QualitySettingsTuner : SettingsTuner
    {
        public override void Init()
        {
            
        }

        public override void DrawSettings()
        {
            //QualitySettings.antiAliasing = 0;

            base.DrawSettings();
        }

        protected override void ApplySettings()
        {
            QualitySettings.skinWeights = SkinWeights.OneBone;
            QualitySettings.vSyncCount = 0;
        }
    }
}