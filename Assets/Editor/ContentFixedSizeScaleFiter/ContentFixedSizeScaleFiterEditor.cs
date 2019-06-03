#if UNITY_EDITOR
using UnityEditor.UI;
using UnityEditor;
using UnityEngine.UI;

[CanEditMultipleObjects]
[CustomEditor(typeof(ContentFixedSizeScaleFiter), true)]
public class ContentFixedSizeScaleFiterEditor : SelfControllerEditor
{
    SerializedProperty m_HorizontalFit;
    SerializedProperty m_VerticalFit;
    SerializedProperty m_fixedSize;

    protected virtual void OnEnable()
    {
        m_fixedSize = serializedObject.FindProperty("m_fixedSize");
        m_HorizontalFit = serializedObject.FindProperty("m_HorizontalFit");
        m_VerticalFit = serializedObject.FindProperty("m_VerticalFit");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(m_fixedSize);
        EditorGUILayout.PropertyField(m_HorizontalFit, true);
        EditorGUILayout.PropertyField(m_VerticalFit, true);
        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }
}
#endif