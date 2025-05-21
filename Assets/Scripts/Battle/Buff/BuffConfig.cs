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
    public string buffName;                          // ����
    [Multiline] public string description;           // ����
    public Sprite icon;                              // ͼ��
    public int maxLayer;                        // ���ѵ���
    public bool canStack => maxLayer > 1;           // �ܷ�ѵ�
    public float duration;                           // ����ʱ��
    public float periodicTime;                           // �������� ÿx������һ��
    public BuffEffectDataBase startEffect;           // ��ʼЧ��
    public BuffEffectDataBase periodicEffect;            // ����Ч��
    public BuffEffectDataBase endEffect;             // ����Ч��

}

public abstract class BuffEffectDataBase
{
}

public class SimpleBuffEffectData : BuffEffectDataBase
{
    public BuffEffectType type;
    public float value;
}
