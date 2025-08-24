// Đặt file này trong thư mục Editor
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuffSkillData))]
public class BuffSkillDataEditor : Editor
{
    // Tạo các biến để tham chiếu đến các thuộc tính
    SerializedProperty skillNameProp, descriptionProp, iconProp, skillTypeProp, skillAnimationProp;
    SerializedProperty cooldownProp, manaCostProp;
    SerializedProperty isPermanentProp, statToBuffProp, buffValueProp, durationProp;

    private void OnEnable()
    {
        // Lấy các thuộc tính khi Editor được bật
        skillNameProp = serializedObject.FindProperty("skillName");
        descriptionProp = serializedObject.FindProperty("description");
        iconProp = serializedObject.FindProperty("icon");
        skillTypeProp = serializedObject.FindProperty("skillType");
        skillAnimationProp = serializedObject.FindProperty("skillAnimation");
        cooldownProp = serializedObject.FindProperty("cooldown");
        manaCostProp = serializedObject.FindProperty("manaCost");

        isPermanentProp = serializedObject.FindProperty("isPermanent");
        statToBuffProp = serializedObject.FindProperty("statToBuff");
        buffValueProp = serializedObject.FindProperty("buffValue");
        durationProp = serializedObject.FindProperty("duration");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // --- Vẽ các trường chung ---
        EditorGUILayout.LabelField("General Info", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(skillNameProp);
        EditorGUILayout.PropertyField(descriptionProp);
        EditorGUILayout.PropertyField(iconProp);
        EditorGUILayout.PropertyField(skillTypeProp);

        SkillType currentSkillType = (SkillType)skillTypeProp.enumValueIndex;

        // --- Vẽ các trường của Buff Skill ---
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Buff Settings", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(statToBuffProp);
        EditorGUILayout.PropertyField(buffValueProp);

        // Passive skill luôn là vĩnh viễn
        if (currentSkillType == SkillType.Passive)
        {
            isPermanentProp.boolValue = true;
        }

        // Chỉ hiển thị isPermanent cho skill Active
        if (currentSkillType == SkillType.Active)
        {
            EditorGUILayout.PropertyField(isPermanentProp);
        }

        // --- Logic hiển thị có điều kiện ---
        bool isPermanent = isPermanentProp.boolValue;
        if (currentSkillType == SkillType.Active && !isPermanent)
        {
            EditorGUILayout.PropertyField(durationProp);
        }

        // Chỉ hiển thị animation, cooldown và mana cost nếu skill là Active
        if (currentSkillType == SkillType.Active)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Active Skill Stats", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(skillAnimationProp);
            EditorGUILayout.PropertyField(cooldownProp);
            EditorGUILayout.PropertyField(manaCostProp);
        }

        serializedObject.ApplyModifiedProperties();
    }
}