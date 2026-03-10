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
    private AnimationLayerMixerPlayable layerMixer;

    private AnimationMixerPlayable mixer;
    private AnimationNodeBase previousNode; // 上一个节点
    private AnimationNodeBase currentNode; // 当前节点
    private int inputPort0 = 0;
    private int inputPort1 = 1;

    private AnimationMixerPlayable mixer_Layer1;
    private AnimationNodeBase previousNode_Layer1; // 上一个节点
    private AnimationNodeBase currentNode_Layer1; // 当前节点
    private int inputPort0_Layer1 = 0;
    private int inputPort1_Layer1 = 1;

    private Coroutine transitionCoroutine = null;
    private Coroutine transitionCoroutine_Layer1 = null;

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

    private float speed_Layer1;
    public float Speed_Layer1
    {
        get => speed_Layer1;
        set
        {
            speed_Layer1 = value;
            currentNode_Layer1.SetSpeed(speed_Layer1);
        }
    }


    public void Init()
    {
        // 创建图
        if(graph.IsValid()) graph.Destroy();
        graph = PlayableGraph.Create("Animation_Controller");

        // 设置图的时间模式
        graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        // 创建混合器作为layer0
        mixer = AnimationMixerPlayable.Create(graph, 3);
        // 创建混合器作为layer1
        mixer_Layer1 = AnimationMixerPlayable.Create(graph, 3);
        // 创建分层混合器
        layerMixer = AnimationLayerMixerPlayable.Create(graph, 2);

        // 两个mixer混合器连接到分层混合器的层0层1
        layerMixer.ConnectInput(0, mixer, 0);
        layerMixer.SetInputWeight(0, 1f);

        layerMixer.ConnectInput(1, mixer_Layer1, 0);
        layerMixer.SetLayerAdditive(1, true);
        layerMixer.SetInputWeight(1, 0f); // 层1一般关闭

        //
        var playoutput = AnimationPlayableOutput.Create(graph,"Animation",animator);

        // 分层混合器作为输出节点
        playoutput.SetSourcePlayable(layerMixer);
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
        // Debug.Log($"角色 {gameObject.name} 要播放动画名称:{animationClip.name}");
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

    #region Layer1相关接口
    public void DestroyNode_Layer1(AnimationNodeBase node)
    {
        if (node != null)
        {
            graph.Disconnect(mixer_Layer1, node.InputPort);
            node.PushPool();
        }
    }

    private void StartTransitionAnimation_Layer1(float fixedTime)
    {
        if (transitionCoroutine_Layer1 != null) StopCoroutine(transitionCoroutine_Layer1);
        transitionCoroutine_Layer1 = StartCoroutine(TransitionAnimation_Layer1(fixedTime));
    }
    private IEnumerator TransitionAnimation_Layer1(float fixedTime)
    {
        // 交换端口号
        inputPort0_Layer1 = inputPort0_Layer1 ^ inputPort1_Layer1;
        inputPort1_Layer1 = inputPort0_Layer1 ^ inputPort1_Layer1;
        inputPort0_Layer1 = inputPort0_Layer1 ^ inputPort1_Layer1;

        // 硬切判断
        if (fixedTime == 0)
        {
            mixer_Layer1.SetInputWeight(inputPort1_Layer1, 0);
            mixer_Layer1.SetInputWeight(inputPort0_Layer1, 1);
        }

        float currentWeight = 1;
        float speed = 1 / fixedTime;
        while (currentWeight > 0)
        {
            currentWeight = Mathf.Clamp01(currentWeight - Time.deltaTime * speed);
            mixer_Layer1.SetInputWeight(inputPort1_Layer1, currentWeight);
            mixer_Layer1.SetInputWeight(inputPort0_Layer1, 1 - currentWeight);
            yield return null;
        }
        transitionCoroutine_Layer1 = null;
    }

    public void PlaySingleAnimation_Layer1(AnimationClip animationClip, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        // Debug.Log($"角色 {gameObject.name} 要播放动画名称:{animationClip.name}");
        SingleAnimationNode singleAnimationNode = null;
        if (currentNode_Layer1 == null) // 首次播放
        {
            singleAnimationNode = ResSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer_Layer1, animationClip, speed, inputPort0_Layer1);
            mixer_Layer1.SetInputWeight(0, 1);
        }
        else
        {
            SingleAnimationNode preNode = currentNode_Layer1 as SingleAnimationNode;
            if (!refreshAnimation && preNode != null && animationClip == preNode.GetAnimationClip()) return;
            // 销毁掉当前可能被占用的Node
            DestroyNode_Layer1(previousNode_Layer1);

            singleAnimationNode = ResSystem.GetOrNew<SingleAnimationNode>();
            singleAnimationNode.Init(graph, mixer_Layer1, animationClip, speed, inputPort1_Layer1);
            previousNode_Layer1 = currentNode_Layer1;
            StartTransitionAnimation_Layer1(transitionFixedTime);
        }
        this.speed_Layer1 = speed;
        currentNode_Layer1 = singleAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    /// <summary>
    /// 播放混合动画
    /// </summary>
    public void PlayBlendAnimation_Layer1(List<AnimationClip> clips, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = ResSystem.GetOrNew<BlendAnimationNode>();
        if (currentNode_Layer1 == null) // 首次播放
        {
            blendAnimationNode.Init(graph, mixer_Layer1, clips, speed, inputPort0_Layer1);
            mixer_Layer1.SetInputWeight(inputPort0_Layer1, 1);
        }
        else
        {
            DestroyNode_Layer1(previousNode_Layer1);
            blendAnimationNode.Init(graph, mixer_Layer1, clips, speed, inputPort1_Layer1);
            previousNode_Layer1 = currentNode_Layer1;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed_Layer1 = speed;
        currentNode_Layer1 = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void PlayBlendAnimation_Layer1(AnimationClip clip1, AnimationClip clip2, float speed = 1, float transitionFixedTime = 0.25f)
    {
        BlendAnimationNode blendAnimationNode = ResSystem.GetOrNew<BlendAnimationNode>();
        if (currentNode_Layer1 == null) // 首次播放
        {
            blendAnimationNode.Init(graph, mixer_Layer1, clip1, clip2, speed, inputPort0_Layer1);
            mixer_Layer1.SetInputWeight(inputPort0_Layer1, 1);
        }
        else
        {
            DestroyNode_Layer1(previousNode_Layer1);
            blendAnimationNode.Init(graph, mixer_Layer1, clip1, clip2, speed, inputPort1_Layer1);
            previousNode_Layer1 = currentNode_Layer1;
            StartTransitionAnimation(transitionFixedTime);
        }
        this.speed_Layer1 = speed;
        currentNode_Layer1 = blendAnimationNode;
        if (graph.IsPlaying() == false) graph.Play();
    }

    public void SetBlendWeight_Layer1(List<float> weightList)
    {
        (currentNode_Layer1 as BlendAnimationNode).SetBlendWeight(weightList);
    }

    public void SetBlendWeight_Layer1(float clip1Weight)
    {
        (currentNode_Layer1 as BlendAnimationNode).SetBlendWeight(clip1Weight);
    }

    public void SetLayerWeight(int layer, float weight)
    {
        layerMixer.SetInputWeight(layer, weight);
    }
    #endregion

    public void SetAnimationSpeed(float spd, int layer = 0)
    {
        if(layer == 0) Speed = spd;
        if(layer == 1) Speed_Layer1 = spd;
    }

    private void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }

    private void OnDisable()
    {
        if (graph.IsValid())
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
