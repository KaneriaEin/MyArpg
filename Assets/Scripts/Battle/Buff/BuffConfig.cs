using JKFrame;
using UnityEngine;

public enum BuffEffectType
{
    Hp,
    AtkValueMultipiler
}

[CreateAssetMenu(menuName ="Config/BuffConfig")]
public class BuffConfig : ConfigBase
{
    public string buffName;                          // 名称
    [Multiline] public string description;           // 描述
    public Sprite icon;                              // 图标
    public int maxLayer;                        // 最大堆叠数
    public bool canStack => maxLayer > 1;           // 能否堆叠
    public float duration;                           // 持续时间
    public float periodicTime;                           // 驱动周期 每x秒驱动一次
    public BuffEffectDataBase startEffect;           // 开始效果
    public BuffEffectDataBase periodicEffect;            // 驱动效果
    public BuffEffectDataBase endEffect;             // 结束效果

}

public abstract class BuffEffectDataBase
{
}

public class SimpleBuffEffectData : BuffEffectDataBase
{
    public BuffEffectType type;
    public float value;
}
