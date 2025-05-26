using JKFrame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_View : MonoBehaviour
{
    [SerializeField] new Animation_Controller animation;
    public Animation_Controller Animation { get => animation; }


    /// <summary>
    /// 游戏中的初始化
    /// </summary>
    public void InitOnGame()
    {
        animation.Init();
    }
}
