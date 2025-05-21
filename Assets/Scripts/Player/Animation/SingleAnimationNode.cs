﻿using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// 单个动画的节点
/// </summary>
public class SingleAnimationNode : AnimationNodeBase
{
    AnimationClipPlayable clipPlayable;
    public void Init(PlayableGraph graph, AnimationMixerPlayable outputMixer, AnimationClip clip, float speed, int inputPort)
    {
        clipPlayable = AnimationClipPlayable.Create(graph, clip);
        clipPlayable.SetSpeed(speed);
        InputPort = inputPort;
        graph.Connect(clipPlayable, 0, outputMixer, inputPort);
    }

    public AnimationClip GetAnimationClip()
    {
        return clipPlayable.GetAnimationClip();
    }

    public override void SetSpeed(float speed)
    {
        clipPlayable.SetSpeed(speed);
    }
}
