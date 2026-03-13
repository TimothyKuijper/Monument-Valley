using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class SceneCameraInspectorWindow : EditorWindow
    {
        private SceneView sceneView;

        [MenuItem("Window/Scene Camera Inspector")]
        public static void ShowWindow()
        {
            GetWindow<SceneCameraInspectorWindow>("Scene Camera Inspector");
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
            UpdateSceneView();
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnSceneGUI(SceneView view)
        {
            UpdateSceneView();
        }

        private void UpdateSceneView()
        {
            sceneView = SceneView.lastActiveSceneView;
        }

        private void OnGUI()
        {
            if (sceneView == null)
            {
                EditorGUILayout.HelpBox("No active Scene View found.", MessageType.Warning);
                return;
            }

            EditorGUILayout.LabelField("Scene View Camera", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            EditorGUI.BeginChangeCheck();

            // Position (pivot)
            Vector3 pivot = EditorGUILayout.Vector3Field("Position", sceneView.pivot);

            // Rotation
            Vector3 rotationEuler = EditorGUILayout.Vector3Field(
                "Rotation",
                sceneView.rotation.eulerAngles
            );

            // Orthographic toggle
            bool ortho = EditorGUILayout.Toggle("Orthographic", sceneView.orthographic);

            if (EditorGUI.EndChangeCheck())
            {
                sceneView.pivot = pivot;
                sceneView.rotation = Quaternion.Euler(rotationEuler);
                sceneView.orthographic = ortho;

                sceneView.Repaint();
            }
        }
    }
}