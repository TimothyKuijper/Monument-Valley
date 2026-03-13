#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Object = UnityEngine.Object;

[Overlay(typeof(SceneView), "Node Builder")]
public class NodeSceneBuilder : Overlay
{
    private const int SLICES = 8;
    private const float RADIUS = 120f;
    private const float INNER_RADIUS = 50f;
    private const float ICON_SIZE = 48f;
    private const float OPEN_SPEED = 8f;

    private static NodePalette palette;
    private static bool placementMode;

    private static bool radialActive;
    private static Vector2 radialCenter;
    private static float radialAnim;

    private static int selectedIndex;
    private static int currentPage;

    private static GameObject ghost;
    private static float gridSize = 1f;

    private static HashSet<Vector3> paintedPositions = new();


    public override VisualElement CreatePanelContent()
    {
        var root = new VisualElement();

        var toggle = new Toggle("Placement Mode");
        toggle.value = placementMode;
        toggle.RegisterValueChangedCallback(e => placementMode = e.newValue);
        root.Add(toggle);

        var paletteField = new UnityEditor.UIElements.ObjectField("Palette")
        {
            objectType = typeof(NodePalette),
            value = palette
        };
        paletteField.RegisterValueChangedCallback(e =>
        {
            palette = e.newValue as NodePalette;
            selectedIndex = 0;
            currentPage = 0;
            DestroyGhost();
        });
        root.Add(paletteField);

        var rebuildButton = new Button(() =>
        {
            RebuildGraph();
        })
        { text = "Rebuild Graph" };
        root.Add(rebuildButton);

        SceneView.duringSceneGui -= OnSceneGUI;
        SceneView.duringSceneGui += OnSceneGUI;

        EditorApplication.update -= Animate;
        EditorApplication.update += Animate;

        return root;
    }

    private static void Animate()
    {
        float target = radialActive ? 1f : 0f;
        radialAnim = Mathf.MoveTowards(radialAnim, target, Time.deltaTime * OPEN_SPEED);
        SceneView.RepaintAll();
    }


    private static void OnSceneGUI(SceneView view)
    {
        if (!placementMode || palette == null || palette.prefabs.Count == 0)
            return;

        Event e = Event.current;

        HandleRadialInput(e);

        if (radialAnim > 0.01f)
        {
            DrawRadial();
            return;
        }

        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        bool valid = UpdateGhost();

        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && valid)
        {
            PaintAtMouse();
            e.Use();
        }
    }



    private static void HandleRadialInput(Event e)
    {
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftShift && !radialActive)
        {
            radialActive = true;
            radialCenter = e.mousePosition;
            e.Use();
        }

        if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftShift)
        {
            radialActive = false;
            e.Use();
        }

        if (!radialActive) return;

        Vector2 dir = e.mousePosition - radialCenter;

        if (dir.magnitude < 30f) return; 
        
        float angle = Mathf.Atan2(dir.y, dir.x);

        if (angle < 0f) angle += Mathf.PI * 2f;


        int slice = Mathf.FloorToInt(angle / (Mathf.PI * 2f) * SLICES);
        int index = currentPage * SLICES + slice;

        if (index < palette.prefabs.Count && index != selectedIndex)
        {
            selectedIndex = index;
            DestroyGhost();
        }
    }

    private static void DrawRadial()
    {
        Handles.BeginGUI();

        float sliceAngle = 360f / SLICES;
        int start = currentPage * SLICES;

        for (int i = 0; i < SLICES; i++)
        {
            int index = start + i;
            if (index >= palette.prefabs.Count) break;

            float angleStart = i * sliceAngle;
            float angleEnd = angleStart + sliceAngle;

            bool selected = index == selectedIndex;

            float outer = RADIUS * radialAnim * (selected ? 1.1f : 1f);
            float inner = INNER_RADIUS * radialAnim;

            Color col = selected
                ? new Color(0.2f, 0.8f, 1f, 0.95f)
                : new Color(0f, 0f, 0f, 0.7f);

            Handles.color = col;

            Handles.DrawSolidArc(
                radialCenter,
                Vector3.forward,
                Quaternion.Euler(0, 0, angleStart) * Vector3.right,
                sliceAngle,
                outer);

            Handles.color = Color.black;
            Handles.DrawSolidArc(
                radialCenter,
                Vector3.forward,
                Quaternion.Euler(0, 0, angleStart) * Vector3.right,
                sliceAngle,
                inner);

            DrawIcon(index, angleStart + sliceAngle / 2f, selected);
        }

        DrawCenterPreview();

        Handles.EndGUI();
    }

    private static void DrawIcon(int index, float angleDeg, bool selected)
    {
        float rad = angleDeg * Mathf.Deg2Rad;
        float dist = (INNER_RADIUS + RADIUS) * 0.5f * radialAnim;

        Vector2 pos = radialCenter + new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)) * dist;

        float size = selected ? ICON_SIZE * 1.1f : ICON_SIZE * 0.85f;

        Rect rect = new Rect(pos.x - size / 2, pos.y - size / 2, size, size);

        Texture2D preview = AssetPreview.GetAssetPreview(palette.prefabs[index]);

        if (preview != null)
            GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit, true);
    }

    private static void DrawCenterPreview()
    {
        Texture2D preview = AssetPreview.GetAssetPreview(palette.prefabs[selectedIndex]);
        if (preview == null) return;

        float size = 64f * radialAnim;
        Rect rect = new Rect(radialCenter.x - size / 2, radialCenter.y - size / 2, size, size);
        GUI.DrawTexture(rect, preview, ScaleMode.ScaleToFit, true);
    }


    private static void PaintAtMouse()
    {
        Vector3 pos = GetSnappedPosition();
        if (pos == Vector3.positiveInfinity) return;

        var prefab = palette.prefabs[selectedIndex];
        var instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        Undo.RegisterCreatedObjectUndo(instance, "Paint Node");
        instance.transform.position = pos;

        RebuildGraph();
    }


    private static bool UpdateGhost()
    {
        Vector3 pos = GetSnappedPosition();

        if (Math.Abs(pos.x - -1) < 0.01f)
        {
            DestroyGhost();
            return false;
        }

        var prefab = palette.prefabs[selectedIndex];

        if (ghost == null)
        {
            ghost = Object.Instantiate(prefab);
            ghost.hideFlags = HideFlags.HideAndDontSave;

            foreach (var col in ghost.GetComponentsInChildren<Collider>())
                col.enabled = false;
        }

        ghost.transform.position = pos;

        foreach (var r in ghost.GetComponentsInChildren<Renderer>())
        {
            var block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);

            if (r.sharedMaterial.HasProperty("_Color"))
            {
                var c = r.sharedMaterial.color;
                c.a = 0.35f;
                block.SetColor("_Color", c);
            }

            r.SetPropertyBlock(block);
        }

        return true;
    }

    private static void DestroyGhost()
    {
        if (ghost != null)
        {
            Object.DestroyImmediate(ghost);
            ghost = null;
        }
    }


    private static Vector3 GetSnappedPosition()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            var node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                Vector3 basePos = Snap(node.transform.position);
                Vector3 normal = GetDominantAxis(hit.normal);
                return basePos + normal * gridSize;
            }

            return Snap(hit.point);
        }

        return new Vector3(-1, -1, -1);
    }

    private static Vector3 GetDominantAxis(Vector3 normal)
    {
        normal.Normalize();

        if (Mathf.Abs(normal.x) > Mathf.Abs(normal.y) &&
            Mathf.Abs(normal.x) > Mathf.Abs(normal.z))
            return new Vector3(Mathf.Sign(normal.x), 0, 0);

        if (Mathf.Abs(normal.y) > Mathf.Abs(normal.x) &&
            Mathf.Abs(normal.y) > Mathf.Abs(normal.z))
            return new Vector3(0, Mathf.Sign(normal.y), 0);

        return new Vector3(0, 0, Mathf.Sign(normal.z));
    }

    private static Vector3 Snap(Vector3 pos)
    {
        return new Vector3(
            Mathf.Round(pos.x / gridSize) * gridSize,
            Mathf.Round(pos.y / gridSize) * gridSize,
            Mathf.Round(pos.z / gridSize) * gridSize
        );
    }

    private static void RebuildGraph()
    {
        NodeBank.ResetNodeCache();
        NodeBank.RebuildGraph(Object.FindAnyObjectByType<Camera>());
    }
}
#endif
