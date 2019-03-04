using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Andeart.ReorderableListEditor
{

    internal static class ReorderableListEditorUtils
    {
        private static readonly Dictionary<bool, GUIStyle> _foldoutStyles;

        static ReorderableListEditorUtils ()
        {
            _foldoutStyles = new Dictionary<bool, GUIStyle> ();
        }

        public static void DrawClosedFoldoutRect (Rect rect)
        {
            // Outermost border
            Color color = Color.white * 0.63f;
            color.a = 1f;
            EditorGUI.DrawRect (rect, color);

            // Inset border
            color = Color.white * 0.80f;
            color.a = 1f;
            rect.x += 1f;
            rect.width -= 2f;
            rect.y += 1f;
            rect.height -= 2f;
            EditorGUI.DrawRect (rect, color);

            // Main body color
            color = Color.white * 0.87f;
            color.a = 1f;
            rect.x += 1f;
            rect.width -= 1f;
            rect.y += 1f;
            rect.height -= 1f;
            EditorGUI.DrawRect (rect, color);

            // White top inset border
            color = Color.white * 0.95f;
            rect.y -= 1f;
            rect.height = 1f;
            EditorGUI.DrawRect (rect, color);
        }

        public static string GetPropertyDisplayNameFormatted (SerializedProperty property)
        {
            return $"{property.displayName}  [{property.arraySize}]";
        }

        /// <summary>
        /// Returns GuiStyle for the foldout.
        /// </summary>
        /// <param name="isOpenStyle">
        /// True, if the GuiStyle being requested is for an open foldout. Else, (false) returns GuiStyle
        /// for closed foldout.
        /// </param>
        public static GUIStyle GetFoldoutStyle (bool isOpenStyle)
        {
            ConfirmFoldoutGuiStyles ();
            return _foldoutStyles[isOpenStyle];
        }

        private static void ConfirmFoldoutGuiStyles ()
        {
            if (_foldoutStyles.ContainsKey (true) && _foldoutStyles.ContainsKey (false))
            {
                return;
            }

            GUIStyle guiStyle = new GUIStyle (EditorStyles.foldout);
            Color defaultTextColor = guiStyle.normal.textColor;
            guiStyle.hover.textColor = defaultTextColor;
            guiStyle.onHover.textColor = defaultTextColor;
            guiStyle.focused.textColor = defaultTextColor;
            guiStyle.onFocused.textColor = defaultTextColor;
            guiStyle.active.textColor = defaultTextColor;
            guiStyle.onActive.textColor = defaultTextColor;
            _foldoutStyles[true] = guiStyle;

            guiStyle.margin.top += 1;
            guiStyle.margin.left += 11;
            _foldoutStyles[false] = guiStyle;
        }
    }

}