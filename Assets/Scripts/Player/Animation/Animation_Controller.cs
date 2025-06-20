using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// 动画控制器
/// </summary>
public class Animation_Controller : MonoBehaviour
{
    [SerializeField] Animator animator;
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;

    private AnimationNodeBase previousNode; // 上一个节点
    private AnimationNodeBase currentNode; // 当前节点
    private int inputPort0 = 0;
    private int inputPort1 = 1;

    private Coroutine transitionCoroutine = null;

    private float speed;
    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            currentNode.SetSpeed(speed);
        }
    }


    public void Init()
    {
        // 创建图
        graph = PlayableGraph.Create("Animation_Controller");
        // 设置图的时间模式
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        // 创建混合器
        mixer = AnimationMixerPlayable.Create(graph, 3);

        //
        var playoutput = AnimationPlayableOutput.Create(graph,"Animation",animator);

        //
        playoutput.SetSourcePlayable(mixer);
    }

    public void DestroyNode(AnimationNodeBase node)
    {
        if(node != null)
        {
            graph.Disconnect(mixer, node.InputPort);
            node.PushPool();
        }
    }

    private void StartTransitionAnimation(float fixedTime)
    {
        if (transitionCoroutine != null) StopCoroutine(transitionCoroutine);
        transitionCoroutine = StartCoroutine(TransitionAnimation(fixedTime));
    }
    private IEnumerator TransitionAnimation(float fixedTime)
    {
        // 交换端口号
        inputPort0 = inputPort0 ^ inputPort1;
        inputPort1 = inputPort0 ^ inputPort1;
        inputPort0 = inputPort0 ^ inputPort1;

        // 硬切判断
        if (fixedTime == 0)
        {
            mixer.SetInputWeight(inputPort1, 0);
            mixer.SetInputWeight(inputPort0, 1);
        }

        float currentWeight = 1;
        float speed = 1 / fixedTime;
        while (currentWeight > 0)
        {
            currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
            mixer.SetInputWeight(inputPort1, currentWeight);
            mixer.SetInputWeight(inputPort0, 1 - currentWeight);
            yield return null;
        }
        transitionCoroutine = null;
    }

    public void PlaySingleAnimation(AnimationClip animationClip, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        SingleAnimationNode singleAnimationNode = null;
        if(currentNode == null) // 首次播放
        {
            singleAnimationNode = ResSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort0);
            mixer.SetInputWeight(0, 1);
        }
        else
        {
            SingleAnimationNode preNode = currentNode as SingleAnimationNode;
            if (!refreshAnimation && preNode != null && animationClip == preNode.GetAnimationClip()) return;
            // 销毁掉当前可能被占用的Node
            DestroyNode(previousNode);

            singleAnimationNode = ResSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer, animationClip, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = singleAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation(List<AnimationClip> clips, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = ResSystem.GetOrNew<BlendAnimationNode>();
        if (currentNode == null) // 首次播放
        {
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestroyNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clips, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void PlayBlendAnimation(AnimationClip clip1, AnimationClip clip2, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = ResSystem.GetOrNew<BlendAnimationNode>();
        if (currentNode == null) // 首次播放
        {
            blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort0);
            mixer.SetInputWeight(inputPort0, 1);
        }
        else
        {
            DestroyNode(previousNode);
            blendAnimationNode.Init(graph, mixer, clip1, clip2, speed, inputPort1);
            previousNode = currentNode;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed = speed;
        currentNode = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void SetBlendWeight(List<float> weightList)
    {
        (currentNode as BlendAnimationNode).SetBlendWeight(weightList);
    }

    public void SetBlendWeight(float clip1Weight)
    {
        (currentNode as BlendAnimationNode).SetBlendWeight(clip1Weight);
    }

    public void SetAnimationSpeed(float spd)
    {
        Speed = spd;
    }

    private void OnDestroy()
    {
        graph.Destroy();
    }

    private void OnDisable()
    {
        graph.Stop();
    }

    #region RootMotion
    private Action<Vector3, Quaternion> rootMotionAction;
    private void OnAnimatorMove()
    {
        rootMotionAction?.Invoke(animator.deltaPosition, animator.deltaRotation);
    }
    public void SetRootMotionAction(Action<Vector3, Quaternion> rootMotionAction)
    {
        this.rootMotionAction = rootMotionAction;
    }
    public void ClearRootMotionAction()
    {
        rootMotionAction = null;
    }
    #endregion

    #region 动画事件
    private Dictionary<string, Action> eventDic = new Dictionary<string, Action>();
    // Animator 实际触发的事件函数
    private void AnimationEvent(string eventName)
    {
        if (eventDic.TryGetValue(eventName, out Action action))
        {
            action?.Invoke();
        }
    }

    public void AddAnimationEvent(string eventName, Action action)
    {
        if (eventDic.TryGetValue(eventName, out Action _action))
        {
            _action += action;
        }
        else
        {
            eventDic.Add(eventName, action);
        }
    }

    public void RemoveAnimationEvent(string eventName)
    {
        eventDic.Remove(eventName);
    }

    public void RemoveAnimationEvent(string eventName, Action action)
    {
        if (eventDic.TryGetValue(eventName, out Action _action))
        {
            _action -= action;
        }
    }

    public void CleanAllActionEvent()
    {
        eventDic.Clear();
    }
    #endregion
}
