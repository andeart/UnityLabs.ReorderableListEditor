using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using Object = UnityEngine.Object;



namespace UnityReorderableEditor.V1.Editor
{

    [CustomEditor (typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ReorderableEditor : UnityEditor.Editor
    {

        private Dictionary<string, ReorderableListProperty> _reorderableLists;

        private Dictionary<ReorderableEditorFoldoutState, GUIStyle> _foldoutStates;

        protected virtual void OnEnable ()
        {
            _reorderableLists = new Dictionary<string, ReorderableListProperty> ();
            _foldoutStates = new Dictionary<ReorderableEditorFoldoutState, GUIStyle> ();
        }

        ~ReorderableEditor ()
        {
            _reorderableLists.Clear ();
            _reorderableLists = null;
            _foldoutStates = null;
        }

        public override void OnInspectorGUI ()
        {
            Color cachedGuiColor = GUI.color;
            serializedObject.Update ();
            SerializedProperty property = serializedObject.GetIterator ();
            bool next = property.NextVisible (true);
            if (next)
            {
                do
                {
                    GUI.color = cachedGuiColor;
                    DrawProperty (property);
                } while (property.NextVisible (false));
            }

            serializedObject.ApplyModifiedProperties ();
        }

        protected void DrawProperty (SerializedProperty property)
        {
            bool isdefaultScriptProperty = property.name.Equals
                                               ("m_Script")
                                           && property.type.Equals ("PPtr<MonoScript>")
                                           && property.propertyType == SerializedPropertyType.ObjectReference
                                           && property.propertyPath.Equals ("m_Script");

            bool cachedGuiEnabled = GUI.enabled;
            if (isdefaultScriptProperty)
            {
                GUI.enabled = false;
            }

            // // TODO: Use attributes to see if it's a list.
            // object[] attr = GetPropertyAttributes(property);
            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                DrawArrayProperty (property);
            } else
            {
                EditorGUILayout.PropertyField (property, property.isExpanded);
            }

            if (isdefaultScriptProperty)
            {
                GUI.enabled = cachedGuiEnabled;
            }
        }

        protected void DrawArrayProperty (SerializedProperty property)
        {
            ReorderableListProperty listData = GetReorderableList (property);
            if (!listData.Property.isExpanded)
            {
                EditorGUI.indentLevel++;

                int indentSpace = EditorGUI.indentLevel * 15;
                Rect lastRect = GUILayoutUtility.GetLastRect ();

                // Full rect size: Rect rect = new Rect(indentSpace + 1f, lastRect.y + lastRect.height + 2f, Screen.width - 23f, 16f);


                Rect rect = new Rect (indentSpace - 1f, lastRect.y + lastRect.height + 2f, Screen.width - 19f, 18f);

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

                GUIStyle headerGuiStyle = new GUIStyle (EditorStyles.foldout);
                Color defaultTextColor = headerGuiStyle.normal.textColor;
                headerGuiStyle.hover.textColor = defaultTextColor;
                headerGuiStyle.onHover.textColor = defaultTextColor;
                headerGuiStyle.focused.textColor = defaultTextColor;
                headerGuiStyle.onFocused.textColor = defaultTextColor;
                headerGuiStyle.active.textColor = defaultTextColor;
                headerGuiStyle.onActive.textColor = defaultTextColor;
                headerGuiStyle.margin.top += 1;
                headerGuiStyle.margin.left += 11;

                // headerGuiStyle.margin.bottom = 8;
                property.isExpanded = EditorGUILayout.Foldout
                    (property.isExpanded, GetPropertyHeader (property), true, headerGuiStyle);

                // GUIStyle subtitleGuiStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
                // subtitleGuiStyle.alignment = TextAnchor.UpperLeft;
                // EditorGUILayout.LabelField (property.arraySize == 0 ? "Empty." : string.Format ("Contains {0} {1}s.", property.arraySize, property.arrayElementType), subtitleGuiStyle);

                EditorGUI.indentLevel--;
            } else
            {
                // if (EditorGUILayout.BeginFadeGroup (listData.IsExpanded.faded))
                listData.List.DoLayoutList ();

                // EditorGUILayout.EndFadeGroup ();
            }

            EditorGUILayout.GetControlRect (true, 2f);
        }

        private string GetPropertyHeader (SerializedProperty property)
        {
            return string.Format ("{0}  [{1}]", property.displayName, property.arraySize);

            //property.arraySize == 0 ? "Empty" : string.Format("Contains {0} {1}s", property.arraySize, property.arrayElementType)
        }

        protected object[] GetPropertyAttributes (SerializedProperty property)
        {
            return GetPropertyAttributes<PropertyAttribute> (property);
        }

        protected object[] GetPropertyAttributes<T> (SerializedProperty property) where T : Attribute
        {
            BindingFlags bindingFlags = BindingFlags.GetField
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
            if (field != null)
            {
                return field.GetCustomAttributes (typeof(T), true);
            }

            return null;
        }

        private ReorderableListProperty GetReorderableList (SerializedProperty property)
        {
            ReorderableListProperty ret;
            if (_reorderableLists.TryGetValue (property.name, out ret))
            {
                ret.Property = property;
                return ret;
            }

            ret = new ReorderableListProperty (property);
            _reorderableLists.Add (property.name, ret);
            return ret;
        }


        private enum ReorderableEditorFoldoutState
        {

            Open,
            Closed

        }


        #region Inner-class ReorderableListProperty

        private class ReorderableListProperty
        {

            private SerializedProperty _property;

            // public AnimBool IsExpanded { get; private set; }

            public ReorderableList List { get; private set; }

            public SerializedProperty Property
            {
                get { return _property; }
                set
                {
                    _property = value;
                    List.serializedProperty = _property;
                }
            }

            public ReorderableListProperty (SerializedProperty property)
            {
                // IsExpanded = new AnimBool (property.isExpanded);
                // IsExpanded.speed = 1f;
                _property = property;
                CreateList ();
            }

            ~ReorderableListProperty ()
            {
                _property = null;
                List = null;
            }

            private void CreateList ()
            {
                const bool draggable = true;
                const bool header = true;
                const bool add = true;
                const bool remove = true;
                List = new ReorderableList (Property.serializedObject, Property, draggable, header, add, remove);

                //List.drawHeaderCallback += rect => _property.isExpanded = EditorGUI.ToggleLeft (rect, _property.displayName, _property.isExpanded, EditorStyles.boldLabel);
                List.drawHeaderCallback += OnListDrawHeaderCallback;
                List.onCanRemoveCallback += list => { return List.count > 0; };
                List.drawElementCallback += drawElement;
                List.elementHeightCallback += index =>
                                              {
                                                  return Mathf.Max
                                                             (EditorGUIUtility.singleLineHeight,
                                                              EditorGUI.GetPropertyHeight
                                                                  (_property.GetArrayElementAtIndex
                                                                       (index),
                                                                   GUIContent.none,
                                                                   true))
                                                         + 4.0f;
                                              };
            }

            private void OnListDrawHeaderCallback (Rect rect)
            {
                GUIStyle headerGuiStyle = new GUIStyle (EditorStyles.foldout);
                Color defaultTextColor = headerGuiStyle.normal.textColor;
                headerGuiStyle.hover.textColor = defaultTextColor;
                headerGuiStyle.onHover.textColor = defaultTextColor;
                headerGuiStyle.focused.textColor = defaultTextColor;
                headerGuiStyle.onFocused.textColor = defaultTextColor;
                headerGuiStyle.active.textColor = defaultTextColor;
                headerGuiStyle.onActive.textColor = defaultTextColor;
                _property.isExpanded = EditorGUI.Foldout
                    (new Rect (rect.x + 10, rect.y, rect.width, rect.height),
                     _property.isExpanded,
                     _property.displayName,
                     true,
                     headerGuiStyle);
            }

            private void drawElement (Rect rect, int index, bool active, bool focused)
            {
                //rect.height = 16;
                rect.x += 8;
                rect.width -= 8;
                rect.y += 2;
                rect.height = EditorGUI.GetPropertyHeight
                    (_property.GetArrayElementAtIndex (index), GUIContent.none, true);

                if (_property.GetArrayElementAtIndex (index).propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUI.LabelField (rect, _property.GetArrayElementAtIndex (index).displayName);
                }

                EditorGUI.PropertyField (rect, _property.GetArrayElementAtIndex (index), GUIContent.none, true);
                List.elementHeight = rect.height + 4.0f;
            }

        }

        #endregion

    }

}