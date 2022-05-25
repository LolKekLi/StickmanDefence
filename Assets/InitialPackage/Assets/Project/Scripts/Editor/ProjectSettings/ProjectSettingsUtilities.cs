using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Project
{
    public class ProjectSettingsUtilities : EditorWindow
    {
        private static readonly string ProjectSettingsWindowName = "Project Tuner";

        private static readonly Dictionary<int, ProjectTunerTabs> _projectTunerTabs = new Dictionary<int, ProjectTunerTabs>()
        {
            { 0, ProjectTunerTabs.ProjectSettingsTuner },
            { 1, ProjectTunerTabs.QualitySettingsTuner }
        };

        private static readonly Dictionary<ProjectTunerTabs, SettingsTuner> _settingsTuners =
            new Dictionary<ProjectTunerTabs, SettingsTuner>();
        
        private static int _selectedTab = 0;
        
        [MenuItem("Tools/Project Tuner")]
        private static void ShowProjectSettingsTuner()
        {
            Init();
            
            ShowProjectTunerWindow();
        }

        private static void Init()
        {
            _settingsTuners.Clear();

            var tabs = _projectTunerTabs.Values.ToArray();

            foreach (var tab in tabs)
            {
                _settingsTuners.Add(tab, SetupSettingsTuner(tab));
            }
        }

        private static SettingsTuner SetupSettingsTuner(ProjectTunerTabs tab)
        {
            SettingsTuner settingsTuner = null;
            
            switch (tab)
            {
                case ProjectTunerTabs.ProjectSettingsTuner:
                    settingsTuner = new ProjectSettingsTuner();
                    break;
                
                case ProjectTunerTabs.QualitySettingsTuner:
                    settingsTuner = new QualitySettingsTuner();
                    break;
                
                default:
                    Debug.LogError($"Not found {nameof(ProjectTunerTabs)} SettingsTuner for tab: {tab}");
                    break;
            }
            
            settingsTuner?.Init();
            
            return settingsTuner;
        }

        private static void ShowProjectTunerWindow()
        {
            ProjectSettingsUtilities window =
                (ProjectSettingsUtilities)GetWindow(typeof(ProjectSettingsUtilities));
            window.titleContent.text = ProjectSettingsWindowName;
            window.Show();
        }

        private void OnGUI()
        {
            var values = _projectTunerTabs.Values.Select(t => SplitCamelCase(t.ToString())).ToArray();
            
            EditorGUILayout.BeginHorizontal();
            _selectedTab = GUILayout.Toolbar(_selectedTab, values);
            EditorGUILayout.EndHorizontal();

            if (_projectTunerTabs.TryGetValue(_selectedTab, out var tab))
            {
                if (_settingsTuners.TryGetValue(tab, out var tuner))
                {
                    tuner.DrawSettings();
                }
                else
                {
                    Init();
                }
            }
        }

        private void DrawProjectSettingsTab()
        {
            // compName = EditorGUILayout.TextField("Company Name:", compName);
            // prodName = EditorGUILayout.TextField("Product Name:", prodName);
            // EditorGUILayout.BeginHorizontal();
            // screenWidth = EditorGUILayout.IntField("Width:", screenWidth);
            // screenHeight = EditorGUILayout.IntField("Web Height:", screenHeight);
            // EditorGUILayout.EndHorizontal();
            // EditorGUILayout.Space();
            // EditorGUILayout.BeginHorizontal();
            // webScreenWidth = EditorGUILayout.IntField("Web Width:", webScreenWidth);
            // webScreenHeight = EditorGUILayout.IntField("Web Height:", webScreenHeight);
            // EditorGUILayout.EndHorizontal();
            // fullScreen = EditorGUILayout.Toggle("Full Screen:", fullScreen);
            // EditorGUILayout.BeginHorizontal();
            // if (GUILayout.Button("Save Values"))
            //     SaveSettings();
            // if (GUILayout.Button("Load Values"))
            //     LoadSettings();
            // EditorGUILayout.EndHorizontal();

            //fullScreen = EditorGUILayout.Toggle("Full Screen:", Pla);
        }
        
        private string SplitCamelCase(string source) 
        {
            return string.Join(" ", Regex.Split(source, @"(?<!^)(?=[A-Z](?![A-Z]|$))"));
        }
    }
}