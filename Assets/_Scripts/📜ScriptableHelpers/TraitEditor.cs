/*using System;
using Mind;
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
                var actionTagsProperty = behaviorProperty.FindPropertyRelative("_actionTags");

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

                // Draw ActionTags
                EditorGUI.LabelField(new Rect(rect.x, rect.y, rect.width, lineHeight), "Action Tags");
                rect.y += lineHeight + spacing;

                for (int i = 0; i < actionTagsProperty.arraySize; i++)
                {
                    SerializedProperty tagProperty = actionTagsProperty.GetArrayElementAtIndex(i);
                    EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width - 30, lineHeight), tagProperty, GUIContent.none);

                    if (GUI.Button(new Rect(rect.x + rect.width - 30, rect.y, 30, lineHeight), "-"))
                    {
                        actionTagsProperty.DeleteArrayElementAtIndex(i);
                        break;
                    }

                    rect.y += lineHeight + spacing;
                }

                if (GUI.Button(new Rect(rect.x, rect.y, 100, lineHeight), "Add Tag"))
                {
                    actionTagsProperty.arraySize++;
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
                SerializedProperty behaviorProperty = behaviorsProperty.GetArrayElementAtIndex(index);
                SerializedProperty conditionsProperty = behaviorProperty.FindPropertyRelative("_conditions");
                SerializedProperty actionTagsProperty = behaviorProperty.FindPropertyRelative("_actionTags");

                return EditorGUIUtility.singleLineHeight * 7 + // Name, Dialogue, Action, Priority
                       (actionTagsProperty.arraySize * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) + // ActionTags
                       (conditionsProperty.arraySize * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing)) + // Conditions
                       EditorGUIUtility.singleLineHeight + // Add space for "Add Condition" button
                       70; // Space for additional buttons
            }
        };
    }

    private Type GetEnumTypeForActionType(int actionIndex) =>
        // Map ActionType to corresponding Enum types
        ((Mind.ActionType)actionIndex) switch
        {
            Mind.ActionType.findCharacter => typeof(Mind.TargetType),
            Mind.ActionType.findObject => typeof(Mind.ObjectType),
            Mind.ActionType.kill => typeof(Mind.TargetType),
            Mind.ActionType.fullfillNeed => typeof(Mind.NeedType),
            Mind.ActionType.trader => typeof(Mind.TraderType),
            Mind.ActionType.farmer => typeof(Mind.FarmerType),
            Mind.ActionType.findKnowledge => typeof(Mind.KnowledgeType),
            Mind.ActionType.gotoLocation => typeof(Mind.TargetLocationType),
            Mind.ActionType.socialize => typeof(Mind.SocializeType),
            Mind.ActionType.useOneOfMyPossesions => typeof(Mind.ObjectType),
            Mind.ActionType.useObjectInInventory => typeof(Mind.ObjectType),
            Mind.ActionType.buyItem => typeof(Mind.ObjectType),
            Mind.ActionType.rentItem => typeof(Mind.ObjectType),
            Mind.ActionType.findOccupant => typeof(Mind.TraitType),
            Mind.ActionType.gotoOccupant => typeof(Mind.TraitType),
            _ => null
        };

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

    private Type GetEnumTypeForConditionType(Mind.ConditionType conditionType) =>
        // Map ConditionType to corresponding Enum types
        conditionType switch
        {
            Mind.ConditionType.hasTarget => typeof(Mind.TargetType),
            Mind.ConditionType.doesNotHaveTarget => typeof(Mind.TargetType),
            Mind.ConditionType.hasOccupant => typeof(Mind.TraitType),
            Mind.ConditionType.doesNotHaveOccupant => typeof(Mind.TraitType),
            Mind.ConditionType.hasObject => typeof(Mind.ObjectType),
            Mind.ConditionType.doesNotHaveObject => typeof(Mind.ObjectType),
            Mind.ConditionType.needsTo => typeof(Mind.NeedType),
            Mind.ConditionType.afterHour => typeof(Mind.TimeOfDayType),
            Mind.ConditionType.beforeHour => typeof(Mind.TimeOfDayType),
            Mind.ConditionType.hasKnowledge => typeof(Mind.KnowledgeType),
            Mind.ConditionType.doesNotHaveKnowledge => typeof(Mind.KnowledgeType),
            Mind.ConditionType.atLocation => typeof(Mind.TargetLocationType),
            Mind.ConditionType.notAtLocation => typeof(Mind.TargetLocationType),
            Mind.ConditionType.hasLocationTarget => typeof(Mind.TargetLocationType),
            Mind.ConditionType.doesNotHaveLocationTarget => typeof(Mind.TargetLocationType),
            Mind.ConditionType.reachedOccupant => typeof(Mind.TraitType),
            Mind.ConditionType.hasNotReachedOccupant => typeof(Mind.TraitType),
            Mind.ConditionType.hasEnoughCoin => typeof(Mind.CoinAmount),
            Mind.ConditionType.doesNotHaveEnoughCoin => typeof(Mind.CoinAmount),
            Mind.ConditionType.hasTrait => typeof(Mind.TraitType),
            Mind.ConditionType.doesNotHaveTrait => typeof(Mind.TraitType),
            Mind.ConditionType.beforeClosing => typeof(Mind.TargetLocationType),
            Mind.ConditionType.afterClosing => typeof(Mind.TargetLocationType),
            Mind.ConditionType.seesSomeoneWithTrait => typeof(Mind.TraitType),
            Mind.ConditionType.seesSomeoneWithoutTrait => typeof(Mind.TraitType),
            Mind.ConditionType.seesSomeoneRelatLevelAtOrAbove => typeof(Mind.ViewTowards),
            Mind.ConditionType.seesSomeoneRelatLevelAtOrBelow => typeof(Mind.ViewTowards),
            _ => null
        };

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
            EditorGUI.PropertyField(new Rect(rect.x, rect.y, rect.width * 0.4f, lineHeight), conditionTypeProperty, GUIContent.none);

            // Always reset parameter when ConditionType changes
            Mind.ConditionType currentConditionType = (Mind.ConditionType)conditionTypeProperty.enumValueIndex;
            Type enumType = GetEnumTypeForConditionType(currentConditionType);

            if (enumType != null)
            {
                if (parameterProperty.managedReferenceValue == null || parameterProperty.managedReferenceValue.GetType() != enumType)
                {
                    parameterProperty.managedReferenceValue = Enum.GetValues(enumType).GetValue(0);
                }

                var currentValue = (Enum)parameterProperty.managedReferenceValue;
                var newValue = EditorGUI.EnumPopup(new Rect(rect.x + rect.width * 0.45f, rect.y, rect.width * 0.5f, lineHeight), currentValue);
                parameterProperty.managedReferenceValue = newValue;
            }
            else
            {
                EditorGUI.LabelField(new Rect(rect.x + rect.width * 0.45f, rect.y, rect.width * 0.5f, lineHeight), "No parameters");
            }

            // Remove button
            if (GUI.Button(new Rect(rect.x + rect.width - 20, rect.y, 20, lineHeight), "-"))
            {
                conditionsProperty.DeleteArrayElementAtIndex(i);
                serializedObject.ApplyModifiedProperties();
                break;
            }

            rect.y += lineHeight + spacing;
        }

        // Add Condition button
        rect.y += spacing; // Small gap before the button
        if (GUI.Button(new Rect(rect.x, rect.y, rect.width, lineHeight), "Add Condition"))
        {
            conditionsProperty.arraySize++;
            var newCondition = conditionsProperty.GetArrayElementAtIndex(conditionsProperty.arraySize - 1);

            var conditionTypeProperty = newCondition.FindPropertyRelative("conditionType");
            conditionTypeProperty.enumValueIndex = 0;

            var parameterProperty = newCondition.FindPropertyRelative("parameter");
            Type parameterEnumType = GetEnumTypeForConditionType((Mind.ConditionType)0);

            if (parameterEnumType != null)
            {
                parameterProperty.managedReferenceValue = Enum.GetValues(parameterEnumType).GetValue(0);
            }
        }
    }

}
*/
