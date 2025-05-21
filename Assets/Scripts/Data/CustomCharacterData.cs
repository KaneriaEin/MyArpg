using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using JKFrame;

/// <summary>
/// �Զ���ɫ��ȫ������
/// </summary>
[Serializable]
public class CustomCharacterData
{
    public ProfessionType ProfessionType;
    public Serialized_Dic<int, CustomCharacterPartData> CustomPartDataDic;
}

/// <summary>
/// �Զ����ɫ��λ������
/// </summary>
[Serializable]
public class CustomCharacterPartData
{
    public int Index;
    public float Size;
    public float Height;
    public Serialized_Color Color1;
    public Serialized_Color Color2;
}

