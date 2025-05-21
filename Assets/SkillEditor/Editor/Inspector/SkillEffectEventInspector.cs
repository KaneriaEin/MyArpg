using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEffectEventInspector : SkillEventDataInspectorBase<EffectTrackItem, EffectTrack>
{
    private IntegerField effectDurationField;
    public override void OnDraw()
    {
        // 预制体
        ObjectField effectPrefabAssetField = new ObjectField("特效资源");
        effectPrefabAssetField.objectType = typeof(GameObject);
        effectPrefabAssetField.value = trackItem.SkillEffectEvent.Prefab;
        effectPrefabAssetField.RegisterValueChangedCallback(EffectPrefabAssetFieldValueChanged);
        root.Add(effectPrefabAssetField);

        // 坐标
        Vector3Field posField = new Vector3Field("坐标");
        posField.value = trackItem.SkillEffectEvent.Position;
        posField.RegisterValueChangedCallback(EffectPosFieldValueChanged);
        root.Add(posField);

        // 旋转
        Vector3Field rotaField = new Vector3Field("旋转");
        rotaField.value = trackItem.SkillEffectEvent.Rotation;
        rotaField.RegisterValueChangedCallback(EffectRotaFieldValueChanged);
        root.Add(rotaField);

        // 旋转
        Vector3Field scaleField = new Vector3Field("缩放");
        scaleField.value = trackItem.SkillEffectEvent.Scale;
        scaleField.RegisterValueChangedCallback(EffectScaleFieldValueChanged);
        root.Add(scaleField);

        // 自动销毁
        Toggle autoDestructToggle = new Toggle("自动销毁");
        autoDestructToggle.value = trackItem.SkillEffectEvent.AutoDestruct;
        autoDestructToggle.RegisterValueChangedCallback(EffectAutoDestructValueChanged);
        root.Add(autoDestructToggle);

        // 时间
        effectDurationField = new IntegerField("持续时间");
        effectDurationField.value = trackItem.SkillEffectEvent.Duration;
        effectDurationField.RegisterCallback<FocusInEvent>(EffectDurationFieldFocusIn);
        effectDurationField.RegisterCallback<FocusOutEvent>(EffectDurationFieldFocusOut);
        root.Add(effectDurationField);

        // 计算时间按钮(若改变了预制体里的duration,编辑器的item长度不会立刻更新，需要手动更新操作)
        Button calculateDurationButton = new Button(CalculateEffectDuration);
        calculateDurationButton.text = "重新计时";
        root.Add(calculateDurationButton);

        // 计算时间按钮(若改变了预制体里的duration,编辑器的item长度不会立刻更新，需要手动更新操作)
        Button applyModelTransformButton = new Button(ApplyModelTransformData);
        applyModelTransformButton.text = "引用模型Transform";
        root.Add(applyModelTransformButton);

        // 设置持续帧数至选中帧
        Button setFrameButton = new Button(SetEffectDurationFrameButtonClick);
        setFrameButton.text = "设置持续帧数至选中帧";
        root.Add(setFrameButton);
    }
    private void ApplyModelTransformData()
    {
        EffectTrackItem item = trackItem;
        item.ApplyModelTransformData();
        SkillEditorInspector.Instance.Show();
    }

    private void CalculateEffectDuration()
    {
        EffectTrackItem item = trackItem;
        ParticleSystem[] particleSystems = item.SkillEffectEvent.Prefab.GetComponentsInChildren<ParticleSystem>();
        float max = -1;
        float curr = -1;
        for (int i = 0; i < particleSystems.Length; i++)
        {
            if (particleSystems[i].main.duration > max)
            {
                max = particleSystems[i].main.duration;
                curr = i;
            }
        }

        item.SkillEffectEvent.Duration = (int)(max * SkillEditorWindow.Instance.SkillConfig.FrameRate);
        effectDurationField.value = (int)(max * SkillEditorWindow.Instance.SkillConfig.FrameRate);
        item.ResetView();
    }

    private void EffectPrefabAssetFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.SkillEffectEvent.Prefab = evt.newValue as GameObject;
        // 重新计时
        CalculateEffectDuration();
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectPosFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem item = trackItem;
        item.SkillEffectEvent.Position = evt.newValue;
        item.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }
    private void EffectRotaFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem item = trackItem;
        item.SkillEffectEvent.Rotation = evt.newValue;
        item.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectScaleFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        EffectTrackItem item = trackItem;
        item.SkillEffectEvent.Scale = evt.newValue;
        item.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void EffectAutoDestructValueChanged(ChangeEvent<bool> evt)
    {
        EffectTrackItem item = trackItem;
        item.SkillEffectEvent.AutoDestruct = evt.newValue;
        item.ResetView();
    }

    float oldEffectDuration;
    private void EffectDurationFieldFocusIn(FocusInEvent evt)
    {
        oldEffectDuration = effectDurationField.value;
    }

    private void EffectDurationFieldFocusOut(FocusOutEvent evt)
    {
        if (oldEffectDuration == effectDurationField.value) return;
        trackItem.SkillEffectEvent.Duration = effectDurationField.value;
        SkillEditorWindow.Instance.SaveConfig();
        trackItem.ResetView();
        SkillEditorWindow.Instance.TickSkill();
    }

    private void SetEffectDurationFrameButtonClick()
    {
        EffectDurationFieldFocusIn(null);
        effectDurationField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.FrameIndex;
        EffectDurationFieldFocusOut(null);
    }
}
