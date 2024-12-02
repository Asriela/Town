using UnityEditor;
using UnityEditorInternal;
using static UnityEngine.GraphicsBuffer;
using UnityEngine;

public class TraitEditor : Editor
{
    private ReorderableList _reorderableList;
    private void OnEnable()
    {
        Trait trait = (Trait)target;

        _reorderableList = new(serializedObject, serializedObject.FindProperty("Behaviors"));
        _reorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Behaviors");
        _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            var element = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        _reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
