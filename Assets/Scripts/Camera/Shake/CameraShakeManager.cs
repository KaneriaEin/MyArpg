using Cinemachine;
using JKFrame;
using Sirenix.OdinInspector;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class CameraShakeManager : SingletonMono<CameraShakeManager>
{
    [Header("轻攻击")] public CinemachineImpulseSource lightAtkShakeSource;
    [Header("重攻击")] public CinemachineImpulseSource heavyAtkShakeSource;
    [Header("弹反")] public CinemachineImpulseSource perfectGuardShakeSource;
    [Header("蓄力")] public CinemachineImpulseSource chargeAtkShakeSource;
    [Header("受伤")] public CinemachineImpulseSource damageShakeSource;
    public void Init()
    {
    }

    public void TriggerShake(CameraShakeConfig config,Vector3 pos = default)
    {
        // TODO: 计算大小，方向
        if(pos == default) pos = PlayerManager.Instance.Player.ModelTransform.position;

        switch (config.shakeShape)
        {
            case CameraShakeShape.Light:
                lightAtkShakeSource.GenerateImpulseAtPositionWithVelocity(pos, config.screenDirectionBias * config.baseAmplitude);
                break;
            case CameraShakeShape.Heavy:
                heavyAtkShakeSource.GenerateImpulseAtPositionWithVelocity(pos, config.screenDirectionBias * config.baseAmplitude);
                break;
            case CameraShakeShape.PerfectGuard:
                perfectGuardShakeSource.GenerateImpulseAtPositionWithVelocity(pos, config.screenDirectionBias * config.baseAmplitude);
                break;
            case CameraShakeShape.Charge:
                chargeAtkShakeSource.GenerateImpulseAtPositionWithVelocity(pos, config.screenDirectionBias * config.baseAmplitude);
                break;
            case CameraShakeShape.Damage:
                damageShakeSource.GenerateImpulseAtPositionWithVelocity(pos, config.screenDirectionBias * config.baseAmplitude);
                break;
            default:
                break;
        }
    }
}

#region CameraShakeConfig
public class CameraShakeConfig
{
    [LabelText("震动名称")] public string shakeName = "_name";
    [LabelText("震动类型")] public CameraShakeShape shakeShape = CameraShakeShape.Light;
    [LabelText("震幅大小")] public float baseAmplitude = 0f;
    [LabelText("屏幕偏向")] public Vector2 screenDirectionBias = Vector2.one;
}

public enum CameraShakeShape
{
    Light,
    Heavy,
    PerfectGuard,
    Charge,
    Damage,
}
#endregion

