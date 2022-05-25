using UnityEditor;
using UnityEngine;

namespace Project
{
    public abstract class SettingsTuner
    {
        public virtual void DrawSettings()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Setup Project"))
            {
                ApplySettings();
            }
        }
        
        public abstract void Init();
        protected abstract void ApplySettings();
    }
}