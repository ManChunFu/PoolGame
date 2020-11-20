using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(CustomCollider))]
public class CustomColliderEditor : Editor
{
    private CustomCollider _customCollider;

    private SerializedObject _serializedObject;
    private SerializedProperty _collisionPoints;
    private SerializedProperty _checkBoxes;
    private SerializedProperty _color;
    private SerializedProperty _pointRadius;
    private SerializedProperty _collisionType;
   
    private Tool _lastTool;
    private bool _showMessage;
    private const float _screenSize = 10f;

    private void OnEnable()
    {
        _lastTool = Tools.current;
        Tools.current = Tool.None;

        _customCollider = (CustomCollider)target;

        _serializedObject = serializedObject;
        _collisionPoints = _serializedObject.FindProperty("CollisionPoints");
        _checkBoxes = _serializedObject.FindProperty("CollisionPointCheckBoxes");
        _color = _serializedObject.FindProperty("PointColor");
        _pointRadius = _serializedObject.FindProperty("PointRadius");
        _collisionType = _serializedObject.FindProperty("CollisionType");

        Selection.selectionChanged += Repaint;
        SceneView.duringSceneGui += DuringSceneGUI;
    }

    private void OnDisable()
    {
        Tools.current = _lastTool;

        Selection.selectionChanged -= Repaint;
        SceneView.duringSceneGui -= DuringSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        _serializedObject.Update();

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(_pointRadius);

        GUILayout.Space(10);
        EditorGUILayout.PropertyField(_color);

        GUILayout.Space(20);
        if (GUILayout.Button("Add Collision Point"))
        {
            Undo.RegisterCompleteObjectUndo(_customCollider, "add point");
            _customCollider.AddCollisionPoint();
        }

        GUILayout.Space(10);
        if (_collisionPoints.arraySize > 0 && _checkBoxes.arraySize > 0)
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                EditorGUILayout.LabelField("CollisionType - CollisionXZ is checking if the given point is inside all of CollisionPoints' X and Z Axis. CollisionXY is checking X and Y Axis.", EditorStyles.wordWrappedMiniLabel);

                using (new GUILayout.HorizontalScope(EditorStyles.helpBox))
                {
                    ShowCollisionType();
                }

                GUILayout.Space(10);
                ReadCollisionPoints();
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Remove Selected Point"))
        {
            _showMessage = false;
            if (_checkBoxes.arraySize > 0)
            {
                if (_customCollider.IsAnySelected())
                {
                    Undo.RegisterCompleteObjectUndo(_customCollider, "selected point");
                    _customCollider.RemoveCollisionPoint();
                }
                else
                {
                    _showMessage = true;
                }
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Remove All"))
        {
            Undo.RegisterCompleteObjectUndo(_customCollider, "all the points");
            _customCollider.RemoveAll();
            _showMessage = false;
        }

        if (_showMessage)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Select at least one point to remove", MessageType.Warning);
        }

        if (_serializedObject.ApplyModifiedProperties())
            SceneView.RepaintAll();
    }

    private void ReadCollisionPoints()
    {
        for (int index = 0; index < _collisionPoints.arraySize; index++)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PropertyField(_checkBoxes.GetArrayElementAtIndex(index), new GUIContent(""), GUILayout.Width(20));
                GUILayout.Label("Point " + index.ToString(), GUILayout.Width(60));
                EditorGUILayout.PropertyField(_collisionPoints.GetArrayElementAtIndex(index), new GUIContent(""));
            }
        }
    }

    private void ShowCollisionType()
    {
        GUILayout.Space(10);
        var style = EditorStyles.wordWrappedLabel;
        style.fontStyle = FontStyle.Bold;
        EditorGUILayout.LabelField("Collilsion Type", style);
        EditorGUILayout.PropertyField(_collisionType, new GUIContent(""));

    }

    private void DuringSceneGUI(SceneView sceneView)
    {
        Selection.activeObject = _customCollider;

        _serializedObject.Update();
        if (_collisionPoints.arraySize > 0)
        {
            for (int i = 0; i < _collisionPoints.arraySize; i++)
            {
                SerializedProperty prop = _collisionPoints.GetArrayElementAtIndex(i);
                prop.vector3Value = Handles.PositionHandle(prop.vector3Value, Quaternion.identity);

                Handles.color = _color.colorValue;
                Handles.SphereHandleCap(-1, prop.vector3Value, Quaternion.identity, _pointRadius.floatValue, EventType.Repaint);

                Handles.Label(prop.vector3Value, "Point " + i.ToString());

                if (_collisionPoints.arraySize > 1)
                {
                    Handles.DrawDottedLine(prop.vector3Value, _collisionPoints.GetArrayElementAtIndex(_customCollider.GetNextIndex(i)).vector3Value, _screenSize);
                }
            }

        _serializedObject.ApplyModifiedProperties();
        }
    }
}
