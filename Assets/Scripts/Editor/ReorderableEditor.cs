using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;



namespace UnityReorderableEditor.V1.Editor
{

    [CustomEditor (typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ReorderableEditor : UnityEditor.Editor
    {
        private Dictionary<string, ReorderableListProperty> _reorderableLists;

        protected virtual void OnEnable ()
        {
            _reorderableLists = new Dictionary<string, ReorderableListProperty> ();
        }

        ~ReorderableEditor ()
        {
            _reorderableLists.Clear ();
            _reorderableLists = null;
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
            bool isdefaultScriptProperty = property.name.Equals ("m_Script")
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
                Rect rect = new Rect (indentSpace - 1f, lastRect.y + lastRect.height + 2f, Screen.width - 19f, 18f);

                ReorderableEditorUtils.DrawClosedFoldoutRect (rect);

                property.isExpanded = EditorGUILayout.Foldout (property.isExpanded,
                                                               ReorderableEditorUtils.GetPropertyDisplayNameFormatted (property),
                                                               true,
                                                               ReorderableEditorUtils.GetFoldoutStyle (ReorderableEditorFoldoutState.Closed));

                EditorGUI.indentLevel--;
            } else
            {
                listData.DoLayoutList ();
            }

            EditorGUILayout.GetControlRect (true, 2f);
        }

        private ReorderableListProperty GetReorderableList (SerializedProperty property)
        {
            ReorderableListProperty reorderableListProperty;
            if (_reorderableLists.TryGetValue (property.name, out reorderableListProperty))
            {
                reorderableListProperty.Property = property;
                return reorderableListProperty;
            }

            reorderableListProperty = new ReorderableListProperty (property);
            _reorderableLists[property.name] = reorderableListProperty;
            return reorderableListProperty;
        }


        #region ReorderableListProperty

        private class ReorderableListProperty
        {
            private SerializedProperty _property;
            private ReorderableList _list;

            public SerializedProperty Property
            {
                get { return _property; }
                set
                {
                    _property = value;
                    _list.serializedProperty = _property;
                }
            }

            public ReorderableListProperty (SerializedProperty property)
            {
                _property = property;
                CreateList ();
            }

            ~ReorderableListProperty ()
            {
                _property = null;
                _list = null;
            }

            private void CreateList ()
            {
                _list = new ReorderableList (Property.serializedObject, Property, true, true, true, true);
                _list.drawHeaderCallback += OnListDrawHeader;
                _list.onCanRemoveCallback += GetListCanRemove;
                _list.drawElementCallback += OnListDrawElement;
                _list.elementHeightCallback += GetListElementHeight;
            }

            private void OnListDrawHeader(Rect rect)
            {
                _property.isExpanded = EditorGUI.Foldout(new Rect(rect.x + 10, rect.y, rect.width, rect.height),
                                                         _property.isExpanded,
                                                         _property.displayName,
                                                         true,
                                                         ReorderableEditorUtils.GetFoldoutStyle (ReorderableEditorFoldoutState.Open));
            }

            private bool GetListCanRemove(ReorderableList list)
            {
                return _list.count > 0;
            }

            private void OnListDrawElement(Rect rect, int index, bool active, bool focused)
            {
                rect.x += 8;
                rect.width -= 8;
                rect.y += 2;
                rect.height = EditorGUI.GetPropertyHeight(_property.GetArrayElementAtIndex(index), GUIContent.none, true);

                if (_property.GetArrayElementAtIndex(index).propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUI.LabelField(rect, _property.GetArrayElementAtIndex(index).displayName);
                }

                EditorGUI.PropertyField(rect, _property.GetArrayElementAtIndex(index), GUIContent.none, true);
                _list.elementHeight = rect.height + 4.0f;
            }

            private float GetListElementHeight (int index)
            {
                return Mathf.Max (EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight (_property.GetArrayElementAtIndex (index), GUIContent.none, true)) + 4.0f;
            }

            public void DoLayoutList ()
            {
                _list.DoLayoutList ();
            }
        }

        #endregion ReorderableListProperty
    }

}