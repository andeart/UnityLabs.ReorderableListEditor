using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;



namespace UnityReorderableListEditor.V1.Editor
{

    /// <summary>
    /// Custom editor to draw object arrays in Unity Editor by default.
    /// This custom editor allows you re-order array elements (unlike Unity's default behaviour).
    /// This is inspired by the research in Valentin Simonov's blog article here: http://va.lent.in/unity-make-your-lists-functional-with-reorderablelist/ ,
    /// along with some UI/UX tweaks.
    /// The majority of functionality here is protected/virtual, making it easy to write custom editors that derive from this (in order to use the reordering feature).
    /// </summary>
    [CustomEditor (typeof(Object), true)]
    [CanEditMultipleObjects]
    public class ReorderableListEditor : UnityEditor.Editor
    {
        private Dictionary<string, ReorderableListProperty> _reorderableLists;

        protected virtual void OnEnable ()
        {
            _reorderableLists = new Dictionary<string, ReorderableListProperty> ();
        }

        protected virtual void OnDestroy ()
        {
            _reorderableLists.Clear ();
            _reorderableLists = null;
        }

        public override void OnInspectorGUI ()
        {
            Color currentGuiColor = GUI.color;
            serializedObject.Update ();
            SerializedProperty property = serializedObject.GetIterator ();
            bool next = property.NextVisible (true);
            if (next)
            {
                do
                {
                    GUI.color = currentGuiColor;
                    DrawProperty (property);
                } while (property.NextVisible (false));
            }

            serializedObject.ApplyModifiedProperties ();
        }

        protected void DrawProperty (SerializedProperty property)
        {
            bool isPropertyMonobehaviourId = property.name.Equals ("m_Script")
                                             && property.type.Equals ("PPtr<MonoScript>")
                                             && property.propertyType == SerializedPropertyType.ObjectReference
                                             && property.propertyPath.Equals ("m_Script");

            bool wasGuiEnabled = GUI.enabled;
            if (isPropertyMonobehaviourId)
            {
                GUI.enabled = false;
            }

            if (property.isArray && property.propertyType != SerializedPropertyType.String)
            {
                DrawListProperty (property);
            } else
            {
                EditorGUILayout.PropertyField (property, property.isExpanded);
            }

            if (isPropertyMonobehaviourId)
            {
                GUI.enabled = wasGuiEnabled;
            }
        }

        private void DrawListProperty (SerializedProperty property)
        {
            ReorderableListProperty reorderableListProperty = GetReorderableList (property);
            if (!reorderableListProperty.Property.isExpanded)
            {
                EditorGUI.indentLevel++;

                int defaultFoldoutIndent = EditorGUI.indentLevel * 14;
                Rect lastRect = GUILayoutUtility.GetLastRect ();
                Rect rect = new Rect (defaultFoldoutIndent, lastRect.y + lastRect.height + 2f, Screen.width - 19f, 18f);
                ReorderableListEditorUtils.DrawClosedFoldoutRect (rect);

                property.isExpanded = EditorGUILayout.Foldout (property.isExpanded,
                                                               ReorderableListEditorUtils.GetPropertyDisplayNameFormatted (property),
                                                               true,
                                                               ReorderableListEditorUtils.GetFoldoutStyle (false));

                EditorGUI.indentLevel--;
            } else
            {
                reorderableListProperty.DoLayoutList ();
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
            private const float _elementVerticalMargin = 4f;

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
                _list = new ReorderableList (_property.serializedObject, _property, true, true, true, true);
                _list.drawHeaderCallback += OnListDrawHeader;
                _list.onCanRemoveCallback += GetListCanRemove;
                _list.drawElementCallback += OnListDrawElement;
                _list.elementHeightCallback += GetListElementHeight;
            }

            private void OnListDrawHeader (Rect rect)
            {
                _property.isExpanded = EditorGUI.Foldout (new Rect (rect.x + 10, rect.y, rect.width, rect.height),
                                                          _property.isExpanded,
                                                          _property.displayName,
                                                          true,
                                                          ReorderableListEditorUtils.GetFoldoutStyle (true));
            }

            private bool GetListCanRemove (ReorderableList list)
            {
                return _list.count > 0;
            }

            private void OnListDrawElement (Rect rect, int index, bool active, bool focused)
            {
                rect.x += 8;
                rect.width -= 8;
                rect.y += 2;
                SerializedProperty propertyChild = _property.GetArrayElementAtIndex (index);
                rect.height = EditorGUI.GetPropertyHeight (propertyChild, GUIContent.none, true);

                if (propertyChild.propertyType == SerializedPropertyType.Generic)
                {
                    EditorGUI.LabelField (rect, propertyChild.displayName);
                }

                EditorGUI.PropertyField (rect, propertyChild, GUIContent.none, true);
                _list.elementHeight = rect.height + _elementVerticalMargin;
            }

            private float GetListElementHeight (int index)
            {
                return Mathf.Max (EditorGUIUtility.singleLineHeight, EditorGUI.GetPropertyHeight (_property.GetArrayElementAtIndex (index), GUIContent.none, true))
                       + _elementVerticalMargin;
            }

            public void DoLayoutList ()
            {
                _list.DoLayoutList ();
            }
        }

        #endregion ReorderableListProperty
    }

}