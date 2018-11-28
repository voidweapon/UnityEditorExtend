using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ParallelogramLayoutGroup), true)]
    [CanEditMultipleObjects]
    public class ParallelogramLayoutGroupEditor : HorizontalOrVerticalLayoutGroupEditor
    {

        SerializedProperty m_OffsetZ;
        protected override void OnEnable()
        {
            base.OnEnable();
            m_OffsetZ = serializedObject.FindProperty("m_offsetZ");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.PropertyField(m_OffsetZ, true);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

