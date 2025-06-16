using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillAttackDetectionEventInspector : SkillEventDataInspectorBase<AttackDetectionTrackItem, AttackDetectionTrack>
{
    private IntegerField detectionDurationFrameField;
    private List<string> detectionChoiceList;
    public override void OnDraw()
    {
        DrawDetection();
        DrawHitConfig();
    }

    #region 检测

    private void DrawDetection()
    {
        // 持续帧数
        detectionDurationFrameField = new IntegerField("持续帧数");
        detectionDurationFrameField.value = trackItem.SkillAttackDetectionEvent.DurationFrame;
        detectionDurationFrameField.RegisterValueChangedCallback(DurationFrameFieldValueChanged);
        root.Add(detectionDurationFrameField);

        detectionChoiceList = new List<string>(Enum.GetNames(typeof(AttackDetectionType)));
        DropdownField atkDetectionDropDownField = new DropdownField("检测类型", detectionChoiceList, (int)trackItem.SkillAttackDetectionEvent.AttackDetectionType);
        atkDetectionDropDownField.RegisterValueChangedCallback(OnAtkDetectionDropDownFieldValueChanged);
        root.Add(atkDetectionDropDownField);

        // 根据检测类型进行绘制
        switch (trackItem.SkillAttackDetectionEvent.AttackDetectionType)
        {
            case AttackDetectionType.Weapon:
                AttackWeaponDetectionData weaponDetectionData = (AttackWeaponDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                DropdownField weaponDetectionDropdownField = new DropdownField("武器选择");
                if (SkillEditorWindow.Instance.PreviewCharacterObj != null)
                {
                    Skill_Player skill_Player = SkillEditorWindow.Instance.PreviewCharacterObj.GetComponent<Skill_Player>();
                    weaponDetectionDropdownField.choices = skill_Player.WeaponDic.Keys.ToList();
                }
                if (!string.IsNullOrEmpty(weaponDetectionData.weaponName))
                {
                    weaponDetectionDropdownField.value = weaponDetectionData.weaponName;
                }
                weaponDetectionDropdownField.RegisterValueChangedCallback(WeaponDetectionDropdownFieldValueChanged);
                root.Add(weaponDetectionDropdownField);
                break;
            case AttackDetectionType.Box:
                AttackBoxDetectionData boxDetectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                Vector3Field boxDetectionPositionField = new Vector3Field("坐标");
                Vector3Field boxDetectionRotationField = new Vector3Field("旋转");
                Vector3Field boxDetectionScaleField = new Vector3Field("缩放");
                boxDetectionPositionField.value = boxDetectionData.Position;
                boxDetectionRotationField.value = boxDetectionData.Rotation;
                boxDetectionScaleField.value = boxDetectionData.Scale;
                boxDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);
                boxDetectionRotationField.RegisterValueChangedCallback(BoxDetectionRotationFieldValueChanged);
                boxDetectionScaleField.RegisterValueChangedCallback(BoxDetectionScaleFieldValueChanged);
                root.Add(boxDetectionPositionField);
                root.Add(boxDetectionRotationField);
                root.Add(boxDetectionScaleField);
                break;
            case AttackDetectionType.Sphere:
                AttackSphereDetectionData sphereDetectionData = (AttackSphereDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                Vector3Field sphereDetectionPositionField = new Vector3Field("坐标");
                FloatField sphereDetectionRadiusField = new FloatField("半径");
                sphereDetectionPositionField.value = sphereDetectionData.Position;
                sphereDetectionRadiusField.value = sphereDetectionData.Radius;
                sphereDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);
                sphereDetectionRadiusField.RegisterValueChangedCallback(SphereDetectionRadiusFieldValueChanged);
                root.Add(sphereDetectionPositionField);
                root.Add(sphereDetectionRadiusField);
                break;
            case AttackDetectionType.Fan:
                AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
                Vector3Field fanDetectionPositionField = new Vector3Field("坐标");
                Vector3Field fanDetectionRotationField = new Vector3Field("旋转");
                FloatField fanInRadiusField = new FloatField("内半径");
                FloatField fanRadiusField = new FloatField("外半径");
                FloatField fanHeightField = new FloatField("高度");
                FloatField fanAngleField = new FloatField("角度");

                fanDetectionPositionField.value = fanDetectionData.Position;
                fanDetectionRotationField.value = fanDetectionData.Rotation;
                fanInRadiusField.value = fanDetectionData.InsideRadius;
                fanRadiusField.value = fanDetectionData.Radius;
                fanHeightField.value = fanDetectionData.Height;
                fanAngleField.value = fanDetectionData.Angle;

                fanDetectionPositionField.RegisterValueChangedCallback(ShapeDetectionPositionFieldValueChanged);
                fanDetectionRotationField.RegisterValueChangedCallback(FanDetectionRotationFieldValueChanged);
                fanInRadiusField.RegisterValueChangedCallback(FanInRadiusFieldValueChanged);
                fanRadiusField.RegisterValueChangedCallback(FanRadiusFieldValueChanged);
                fanHeightField.RegisterValueChangedCallback(FanHeightFieldValueChanged);
                fanAngleField.RegisterValueChangedCallback(FanAngleFieldValueChanged);

                root.Add(fanDetectionPositionField);
                root.Add(fanDetectionRotationField);
                root.Add(fanInRadiusField);
                root.Add(fanRadiusField);
                root.Add(fanHeightField);
                root.Add(fanAngleField);
                break;
            default:
                break;
        }

        // 设置持续帧数至选中帧
        Button setFrameButton = new Button(SetAttackDetectionDurationFrameButtonClick);
        setFrameButton.text = "设置持续帧数至选中帧";
        root.Add(setFrameButton);
    }
    private void WeaponDetectionDropdownFieldValueChanged(ChangeEvent<string> evt)
    {
        AttackWeaponDetectionData detectionData = (AttackWeaponDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.weaponName = evt.newValue;
    }

    private void DurationFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        trackItem.SkillAttackDetectionEvent.DurationFrame = evt.newValue;
        track.ResetView();
    }

    private void SetAttackDetectionDurationFrameButtonClick()
    {
        detectionDurationFrameField.value = SkillEditorWindow.Instance.CurrentSelectFrameIndex - trackItem.SkillAttackDetectionEvent.FrameIndex;
    }

    private void OnAtkDetectionDropDownFieldValueChanged(ChangeEvent<string> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackDetectionType = (AttackDetectionType)detectionChoiceList.IndexOf(evt.newValue);
        SkillEditorInspector.Instance.Show();
    }

    private void ShapeDetectionPositionFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackShapeDetectionDataBase shapeDetectionData = (AttackShapeDetectionDataBase)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        shapeDetectionData.Position = evt.newValue;
    }

    private void BoxDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackBoxDetectionData shapeDetectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        shapeDetectionData.Rotation = evt.newValue;
    }

    private void BoxDetectionScaleFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackBoxDetectionData shapeDetectionData = (AttackBoxDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        shapeDetectionData.Scale = evt.newValue;
    }

    private void SphereDetectionRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackSphereDetectionData shapeDetectionData = (AttackSphereDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        shapeDetectionData.Radius = evt.newValue;
    }

    private void FanDetectionRotationFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        fanDetectionData.Rotation = evt.newValue;
    }

    private void FanInRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        fanDetectionData.InsideRadius = evt.newValue;
        if (fanDetectionData.Radius <= fanDetectionData.InsideRadius)
        {
            fanDetectionData.InsideRadius = fanDetectionData.Radius - 0.01f;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanRadiusFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData fanDetectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        fanDetectionData.Radius = evt.newValue;
        if (fanDetectionData.Radius <= fanDetectionData.InsideRadius)
        {
            fanDetectionData.InsideRadius = fanDetectionData.Radius - 0.01f;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanHeightFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Height = evt.newValue;
        if (detectionData.Height <= 0)
        {
            detectionData.Height = 0.01f;
            SkillEditorInspector.Instance.Show();
        }
    }

    private void FanAngleFieldValueChanged(ChangeEvent<float> evt)
    {
        AttackFanDetectionData detectionData = (AttackFanDetectionData)trackItem.SkillAttackDetectionEvent.AttackDetectionData;
        detectionData.Angle = evt.newValue;
        if (detectionData.Angle < 0)
        {
            detectionData.Angle = 0.1f;
            SkillEditorInspector.Instance.Show();
        }
        else if (detectionData.Angle > 360)
        {
            detectionData.Angle = 360;
            SkillEditorInspector.Instance.Show();
        }
    }
    #endregion

    #region 命中部分
    private void DrawHitConfig()
    {
        root.Add(new Label());
        FloatField attackMultiplyField = new FloatField("攻击力系数");
        attackMultiplyField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.AttackMultiply;
        attackMultiplyField.RegisterValueChangedCallback(OnAttackMultiplyFieldValueChanged);
        root.Add(attackMultiplyField);

        Vector3Field repelStrengthField = new Vector3Field("击退强度");
        repelStrengthField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelStrength;
        repelStrengthField.RegisterValueChangedCallback(OnRepelStrengthFieldValueChanged);
        root.Add(repelStrengthField);

        FloatField repelTimeField = new FloatField("击退时间");
        repelTimeField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelTime;
        repelTimeField.RegisterValueChangedCallback(OnRepelTimeFieldValueChanged);
        root.Add(repelTimeField);

        ObjectField hitEffectPrefabField = new ObjectField("命中特效");
        hitEffectPrefabField.objectType = typeof(GameObject);
        hitEffectPrefabField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitEffectPrefab;
        hitEffectPrefabField.RegisterValueChangedCallback(OnHitEffectPrefabFieldValueChanged);
        root.Add(hitEffectPrefabField);

        ObjectField hitAudioClipField = new ObjectField("命中声音");
        hitAudioClipField.objectType = typeof(AudioClip);
        hitAudioClipField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitAudioClip;
        hitAudioClipField.RegisterValueChangedCallback(OnHitAudioClipFieldValueChanged);
        root.Add(hitAudioClipField);

        Vector3Field impulseVelField = new Vector3Field("相机抖动速度");
        impulseVelField.value = trackItem.SkillAttackDetectionEvent.AttackHitConfig.CameraImpulseVel;
        impulseVelField.RegisterValueChangedCallback(OnImpulseVelFieldValueChanged);
        root.Add(impulseVelField);
    }

    private void OnAttackMultiplyFieldValueChanged(ChangeEvent<float> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.AttackMultiply = evt.newValue;
    }

    private void OnRepelStrengthFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelStrength = evt.newValue;
    }

    private void OnRepelTimeFieldValueChanged(ChangeEvent<float> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.RepelTime = evt.newValue;
    }

    private void OnHitEffectPrefabFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitEffectPrefab = (GameObject)evt.newValue;
        SkillEditorWindow.Instance.SaveConfig();
        trackItem.ResetView();
    }

    private void OnHitAudioClipFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.HitAudioClip = (AudioClip)evt.newValue;
        SkillEditorWindow.Instance.SaveConfig();
        trackItem.ResetView();
    }

    private void OnImpulseVelFieldValueChanged(ChangeEvent<Vector3> evt)
    {
        trackItem.SkillAttackDetectionEvent.AttackHitConfig.CameraImpulseVel = evt.newValue;
    }
    #endregion
}
