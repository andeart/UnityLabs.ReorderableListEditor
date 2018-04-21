using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;



namespace UnityReorderableListEditor.V1.Editor
{

    public enum ReorderableListEditorFoldoutState
    {
        Open,
        Closed
    }

    public static class ReorderableListEditorUtils
    {

        private static Dictionary<ReorderableListEditorFoldoutState, GUIStyle> _foldoutStyles;

        static ReorderableListEditorUtils()
        {
            _foldoutStyles = new Dictionary<ReorderableListEditorFoldoutState, GUIStyle> ();
        }

        public static object[] GetPropertyAttributes (SerializedProperty property)
        {
            return GetPropertyAttributes<PropertyAttribute> (property);
        }

        public static object[] GetPropertyAttributes<T> (SerializedProperty property) where T : Attribute
        {
            const BindingFlags bindingFlags = BindingFlags.GetField
                                              | BindingFlags.GetProperty
                                              | BindingFlags.IgnoreCase
                                              | BindingFlags.Instance
                                              | BindingFlags.NonPublic
                                              | BindingFlags.Public;

            if (property.serializedObject.targetObject == null)
            {
                return null;
            }

            Type targetType = property.serializedObject.targetObject.GetType ();
            FieldInfo field = targetType.GetField (property.name, bindingFlags);
            return field == null ? null : field.GetCustomAttributes (typeof(T), true);
        }

        public static void DrawClosedFoldoutRect(Rect rect)
        {
            // Outermost border
            Color color = Color.white * 0.63f;
            color.a = 1f;
            EditorGUI.DrawRect(rect, color);

            // Inset border
            color = Color.white * 0.80f;
            color.a = 1f;
            rect.x += 1f;
            rect.width -= 2f;
            rect.y += 1f;
            rect.height -= 2f;
            EditorGUI.DrawRect(rect, color);

            // Main body color
            color = Color.white * 0.87f;
            color.a = 1f;
            rect.x += 1f;
            rect.width -= 1f;
            rect.y += 1f;
            rect.height -= 1f;
            EditorGUI.DrawRect(rect, color);

            // White top inset border
            color = Color.white * 0.95f;
            rect.y -= 1f;
            rect.height = 1f;
            EditorGUI.DrawRect(rect, color);
        }

        public static string GetPropertyDisplayNameFormatted(SerializedProperty property)
        {
            return string.Format("{0}  [{1}]", property.displayName, property.arraySize);
        }

        public static GUIStyle GetFoldoutStyle (ReorderableListEditorFoldoutState state)
        {
            ConfirmFoldoutGuiStyles ();
            return _foldoutStyles[state];
        }

        private static void ConfirmFoldoutGuiStyles()
        {
            if (_foldoutStyles.ContainsKey(ReorderableListEditorFoldoutState.Closed)
                && _foldoutStyles.ContainsKey(ReorderableListEditorFoldoutState.Open))
            {
                return;
            }

            GUIStyle guiStyle = new GUIStyle(EditorStyles.foldout);
            Color defaultTextColor = guiStyle.normal.textColor;
            guiStyle.hover.textColor = defaultTextColor;
            guiStyle.onHover.textColor = defaultTextColor;
            guiStyle.focused.textColor = defaultTextColor;
            guiStyle.onFocused.textColor = defaultTextColor;
            guiStyle.active.textColor = defaultTextColor;
            guiStyle.onActive.textColor = defaultTextColor;
            _foldoutStyles[ReorderableListEditorFoldoutState.Open] = guiStyle;

            guiStyle.margin.top += 1;
            guiStyle.margin.left += 11;
            _foldoutStyles[ReorderableListEditorFoldoutState.Closed] = guiStyle;
        }
    }

}