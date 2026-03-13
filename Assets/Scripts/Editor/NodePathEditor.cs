using Framework.Enemies;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NodePath))]
public class NodePathEditor : UnityEditor.Editor
{
    private enum PickMode
    {
        None,
        Start,
        End
    }

    private PickMode pickMode;

    private void OnEnable()
    {
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        NodePath path = (NodePath)target;

        serializedObject.Update();

        EditorGUILayout.Space();

        DrawNodeField("Start Node", ref path.startNode, PickMode.Start);
        DrawNodeField("End Node", ref path.endNode, PickMode.End);

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawNodeField(string label, ref Node node, PickMode mode)
    {
        EditorGUILayout.BeginHorizontal();

        EditorGUILayout.ObjectField(label, node, typeof(Node), true);

        if (GUILayout.Button("Pick", GUILayout.Width(45)))
        {
            pickMode = mode;
        }

        EditorGUILayout.EndHorizontal();
    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        if (pickMode == PickMode.None)
            return;

        Event e = Event.current;

        if (e.type != EventType.MouseDown || e.button != 0)
            return;

        Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Node node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                NodePath path = (NodePath)target;

                Undo.RecordObject(path, "Assign Node");

                if (pickMode == PickMode.Start)
                    path.startNode = node;
                else if (pickMode == PickMode.End)
                    path.endNode = node;

                EditorUtility.SetDirty(path);

                pickMode = PickMode.None;

                e.Use(); // stops selection change
                Repaint();
            }
        }
    }
}
