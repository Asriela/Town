﻿using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(Trait))]
public class TraitEditor : Editor
{
    private ReorderableList _behaviorList;

    private void OnEnable()
    {
        SerializedProperty behaviorsProperty = serializedObject.FindProperty("Behaviors");

        _behaviorList = new ReorderableList(serializedObject, behaviorsProperty)
        {
            drawHeaderCallback = rect =>
            {
                EditorGUI.LabelField(rect, "Behaviors");

                // Add custom button to add new behaviors
                if (GUI.Button(new Rect(rect.x + rect.width - 100, rect.y, 100, EditorGUIUtility.singleLineHeight), "Add Behavior"))
                {
                    behaviorsProperty.arraySize++;
                    var newBehavior = behaviorsProperty.GetArrayElementAtIndex(behaviorsProperty.arraySize - 1);
                    // Initialize new behavior properties here (optional)
                }
            },

            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                SerializedProperty behaviorProperty = behaviorsProperty.GetArrayElementAtIndex(index);

                var nameProperty = behaviorProperty.FindPropertyRelative("_name");
                var dialogueProperty = behaviorProperty.FindPropertyRelative("_dialogue");
                var actionProperty = behaviorProperty.FindPropertyRelative("_action");
                var actionParameterProperty = behaviorProperty.FindPropertyRelative("_actionParameter");
                var conditionsProperty = behaviorProperty.FindPropertyRelative("_conditions");
                var priorityProperty = behaviorProperty.FindPropertyRelative("_priority");

                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                rect.height = lineHeight;

                // Draw the horizontal divider line
                Rect dividerRect = new Rect(rect.x, rect.y + lineHeight, rect.width, 1); // Divider is 1px high
                EditorGUI.DrawRect(dividerRect, Color.gray); // You can change the color of the divider here

                rect.y += lineHeight + spacing + 2; // Adjust position of next element to avoid overlap with the divider

                // Draw Name, Dialogue, Action
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineHeight), nameProperty, new GUIContent("Name"));
                rect.y += lineHeight + spacing;

                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineHeight), dialogueProperty, new GUIContent("Dialogue"));
                rect.y += lineHeight + spacing;

                // ActionType - Draw and detect changes
                Mind.ActionType actionType = (Mind.ActionType)actionProperty.enumValueIndex;
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineHeight), actionProperty, new GUIContent("Action"));
                rect.y += lineHeight + spacing;

                // Apply modified properties to reflect changes
                serializedObject.ApplyModifiedProperties();

                // Check if the action type has changed
                if (actionType != (Mind.ActionType)actionProperty.enumValueIndex)
                {
                    // Reset ActionParameter when ActionType changes
                    actionParameterProperty.managedReferenceValue = Enum.GetValues(GetEnumTypeForActionType(actionProperty.enumValueIndex)).GetValue(0);
                }

                // Draw ActionParameter dynamically based on ActionType
                var enumType = GetEnumTypeForActionType(actionProperty.enumValueIndex);
                if (enumType != null)
                {
                    var currentValue = (Enum)actionParameterProperty.managedReferenceValue;
                    var newValue = EditorGUI.EnumPopup(new Rect(rect.x, rect.y, rect.width, lineHeight), currentValue);
                    actionParameterProperty.managedReferenceValue = newValue;
                }
                else
                {
                    EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "No Parameters");
                }

                rect.y += lineHeight + spacing;

                // Draw the Priority field
                EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width, lineHeight), priorityProperty, new GUIContent("Priority"));
                rect.y += lineHeight + spacing;

                // Draw Conditions list
                DrawConditionsList(new Rect(rect.x, rect.y, rect.width, lineHeight), conditionsProperty);
            },

            elementHeightCallback = index =>
            {
                // Compute the height based on the number of conditions + space for priority
                SerializedProperty behaviorProperty = behaviorsProperty.GetArrayElementAtIndex(index);
                SerializedProperty conditionsProperty = behaviorProperty.FindPropertyRelative("_conditions");
                int numConditions = conditionsProperty.arraySize;

                // Adjust the height for the conditions list, behavior details, and priority field
                return EditorGUIUtility.singleLineHeight * 7 + (numConditions * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) + 30; // Adding extra space (30) for the new controls and divider
            }
        };
    }

    private Type GetEnumTypeForActionType(int actionIndex)
    {
        // Map ActionType to corresponding Enum types
        return ((Mind.ActionType)actionIndex) switch
        {
            Mind.ActionType.find => typeof(Mind.TargetType),
            Mind.ActionType.kill => typeof(Mind.TargetType),
            Mind.ActionType.fullfillNeed => typeof(Mind.NeedType),
            _ => null,
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Draw Trait Type
        SerializedProperty typeProperty = serializedObject.FindProperty("Type");
        EditorGUILayout.PropertyField(typeProperty, new GUIContent("Trait Type"));

        // Draw the ReorderableList for behaviors
        _behaviorList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }

    private Type GetEnumTypeForConditionType(Mind.ConditionType conditionType)
    {
        // Map ConditionType to corresponding Enum types
        return conditionType switch
        {
            Mind.ConditionType.hasTarget => typeof(Mind.TargetType),
            Mind.ConditionType.needsTo => typeof(Mind.NeedType),
            _ => null,
        };
    }

    private void DrawConditionsList(Rect rect, SerializedProperty conditionsProperty)
    {
        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        // Draw the "Conditions" label above the list
        EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "Conditions");
        rect.y += lineHeight + spacing;

        // Loop through the conditions and draw them
        for (int i = 0; i < conditionsProperty.arraySize; i++)
        {
            SerializedProperty conditionProperty = conditionsProperty.GetArrayElementAtIndex(i);
            SerializedProperty conditionTypeProperty = conditionProperty.FindPropertyRelative("conditionType");
            SerializedProperty parameterProperty = conditionProperty.FindPropertyRelative("parameter");

            // Draw condition type
            EditorGUI.PropertyField(new Rect(rect.x, rect.y + (lineHeight + spacing) * i, rect.width * 0.4f, lineHeight), conditionTypeProperty, GUIContent.none);

            // Always reset parameter when ConditionType changes
            Mind.ConditionType currentConditionType = (Mind.ConditionType)conditionTypeProperty.enumValueIndex;
            Type enumType = GetEnumTypeForConditionType(currentConditionType);

            if (enumType != null)
            {
                // Reset the parameter when the condition type changes
                if (parameterProperty.managedReferenceValue == null || parameterProperty.managedReferenceValue.GetType() != enumType)
                {
                    parameterProperty.managedReferenceValue = Enum.GetValues(enumType).GetValue(0); // Set default value
                }

                // Draw parameter field dynamically based on condition type
                var currentValue = (Enum)parameterProperty.managedReferenceValue;
                var newValue = EditorGUI.EnumPopup(new Rect(rect.x + rect.width * 0.45f, rect.y + (lineHeight + spacing) * i, rect.width * 0.5f, lineHeight), currentValue);
                parameterProperty.managedReferenceValue = newValue;
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.45f, rect.y + (lineHeight + spacing) * i, rect.width * 0.5f, lineHeight), "No parameters");
            }

            // Add a "Remove" button (represented by a "-" icon) to the left of the condition row
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y + (lineHeight + spacing) * i, 20, lineHeight), "-"))
            {
                // Remove the condition from the list
                conditionsProperty.DeleteArrayElementAtIndex(i);
                // Reapply properties after deleting
                serializedObject.ApplyModifiedProperties();
            }
        }

        // Add button for adding conditions at the bottom of the list
        if (GUI.Button(new Rect(rect.x + rect.width - 100, rect.y + (lineHeight + spacing) * conditionsProperty.arraySize, 100, EditorGUIUtility.singleLineHeight), "Add Condition"))
        {
            conditionsProperty.arraySize++;
            var newCondition = conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1);

            var conditionTypeProperty = newCondition.FindPropertyRelative("conditionType");
            conditionTypeProperty.enumValueIndex = 0; // Default to the first condition type

            var parameterProperty = newCondition.FindPropertyRelative("parameter");
            Type parameterEnumType = GetEnumTypeForConditionType((Mind.ConditionType)0);

            // Set default value only if enumType is valid
            if (parameterEnumType != null)
            {
                parameterProperty.managedReferenceValue = Enum.GetValues(parameterEnumType).GetValue(0);
            }
        }
    }
}
