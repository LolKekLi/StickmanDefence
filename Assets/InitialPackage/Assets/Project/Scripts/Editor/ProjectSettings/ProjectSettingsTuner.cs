using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class ProjectSettingsTuner : SettingsTuner
    {
        private const string DebugMenuDefine = "FORCE_DEBUG";
        private const string InAppEnableDefine = "INAPP_ENABLED";
        
        private bool _isDebugEnabled = false;
        private bool _isInAppEnabled = false;
        
        private string _companyName = string.Empty;
        private string _productName = string.Empty;
        private string _packageName = string.Empty;

        private int _selectedStore = 0;
        
        private readonly Dictionary<int, TargetStoreType> _targetStoreTypes = new Dictionary<int, TargetStoreType>();

        private readonly Dictionary<TargetStoreType, string[]> _manifestRequiredAssets =
            new Dictionary<TargetStoreType, string[]>()
            {
                { TargetStoreType.None, null },
                {
                    TargetStoreType.Amazon, new[]
                    {
                        "games.vaveda.publishing.amazon.onlycross", // paid
                        "games.vaveda.unity.build",
                        "games.jamba.purchasing"
                        //"games.vaveda.publishing.amazon"
                    }
                },
                { TargetStoreType.SayGames, null },
            };

        public override void Init()
        {
            _companyName = UnityEditor.PlayerSettings.companyName;
            _productName = UnityEditor.PlayerSettings.productName;
            _packageName = Application.identifier;
            
            var stores = (TargetStoreType[])Enum.GetValues(typeof(TargetStoreType));
            
            for (int i = 0; i < stores.Length; i++)
            {
                _targetStoreTypes.Add(i, stores[i]);
            }

            UnityEditor.PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, out var defines);

            DetectDebugMenuEnabled(defines);
            DetectInAppEnabled(defines);
            DetectTargetStore(defines);
        }

        private void DetectTargetStore(string[] defines)
        {
            if (defines != null && defines.Length > 0)
            {
                var stores = (TargetStoreType[])Enum.GetValues(typeof(TargetStoreType));

                for (int i = 0; i < stores.Length; i++)
                {
                    if (defines.Contains(StoreHelper.GetDefineByStore(stores[i])))
                    {
                        _selectedStore = i;

                        break;
                    }
                }
            }
        }

        private void DetectDebugMenuEnabled(string[] defines)
        {
            _isDebugEnabled = FindDefine(defines, DebugMenuDefine);
        }

        private void DetectInAppEnabled(string[] defines)
        {
            _isInAppEnabled = FindDefine(defines, InAppEnableDefine);
        }

        private bool FindDefine(string[] defines, string define)
        {
            if (defines != null && defines.Length > 0)
            {
                if (defines.Contains(define))
                {
                    return true;
                }
            }

            return false;
        }

        public override void DrawSettings()
        {
            _companyName = EditorGUILayout.TextField("Company Name:", _companyName);
            _productName = EditorGUILayout.TextField("Product Name:", _productName);
            _packageName = EditorGUILayout.TextField("Package Name:", _packageName);
            
            _selectedStore = EditorGUILayout.Popup("Target Store", _selectedStore,
                _targetStoreTypes.Values.Select(s => s.ToString()).ToArray());
 
            _isDebugEnabled = EditorGUILayout.Toggle("Debug Enabled: ", _isDebugEnabled);
            
            _isInAppEnabled = EditorGUILayout.Toggle("In App Enabled: ", _isInAppEnabled);

            base.DrawSettings();
            
            InitProject();
        }
        
        protected override void ApplySettings()
        {
            UnityEditor.PlayerSettings.companyName = _companyName;
            UnityEditor.PlayerSettings.productName = _productName;
            
            UnityEditor.PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, _packageName);
            UnityEditor.PlayerSettings.SetScriptingBackend(BuildTargetGroup.Android, ScriptingImplementation.IL2CPP);

            var targetStore = _targetStoreTypes[_selectedStore];

            SetupDefineSymbols(targetStore);
            VerifyManifest(targetStore);

            Debug.Log("Apply Settings");
        }
        
        private void InitProject()
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            
            if (GUILayout.Button("Init Project"))
            {
                if (Application.platform != RuntimePlatform.Android)
                {
                    EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
                }

                EditorSettings.spritePackerMode = SpritePackerMode.AlwaysOnAtlas;
            }
        }

        private void SetupDefineSymbols(TargetStoreType targetStore)
        {
            List<string> defines = new List<string>();
            
            if (_isDebugEnabled)
            {
                defines.Add(DebugMenuDefine);
            }
            
            if (_isInAppEnabled)
            {
                defines.Add(InAppEnableDefine);
            }

            defines.Add(StoreHelper.GetDefineByStore(targetStore));

            UnityEditor.PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android,
                defines.ToArray());
        }

        private void VerifyManifest(TargetStoreType targetStore)
        {
            var requiredAssets = _manifestRequiredAssets[targetStore];

            if (requiredAssets != null && requiredAssets.Length > 0)
            {
                var cachedRequiredAssets = requiredAssets.ToList(); 

                int removeIndex = Application.dataPath.IndexOf("Assets", StringComparison.Ordinal);
                var path = Application.dataPath.Remove(removeIndex);
                path += "Packages/manifest.json";
                
                if (File.Exists(path))
                {
                    using (StreamReader sr = File.OpenText(path))
                    {
                        string s = string.Empty;
                        
                        while ((s = sr.ReadLine()) != null)
                        {
                            for (int i = 0; i < requiredAssets.Length; i++)
                            {
                                if (s.Contains(requiredAssets[i]))
                                {
                                    cachedRequiredAssets.Remove(requiredAssets[i]);
                                }
                            }
                        }

                        if (cachedRequiredAssets.Count > 0)
                        {
                            Debug.LogException(new Exception($"Not added assets into manifest.json: "));

                            for (int i = 0; i < cachedRequiredAssets.Count; i++)
                            {
                                Debug.LogException(new Exception($"{cachedRequiredAssets[i]}"));
                            }
                        }
                    }
                }
                else
                {
                    Debug.LogException(new Exception($"Not found manifest.json on path - {path}"));
                }
            }
        }
    }
}