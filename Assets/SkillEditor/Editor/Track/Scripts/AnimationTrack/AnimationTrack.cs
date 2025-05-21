using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 包括 左menu：xx配置 右track：轨道
/// </summary>
public class AnimationTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    // <每个item起始帧，item>
    private Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();

    public SkillAnimationData AnimationData {get =>SkillEditorWindow.Instance.SkillConfig.SkillAnimationData;}
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "动画配置");
        trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExited);
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);


        // 销毁当前已有
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }
        trackItemDic.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // 根据数据绘制TrackItem
        foreach (var item in AnimationData.FrameData)
        {
            CreateItem(item.Key, item.Value);
        }
    }

    private void CreateItem(int frameIndex, SkillAnimationEvent skillAnimationEvent)
    {
        AnimationTrackItem trackItem = new AnimationTrackItem();
        trackItem.Init(this, trackStyle, frameIndex, frameWidth, skillAnimationEvent);
        trackItemDic.Add(frameIndex, trackItem);
    }

    #region 拖拽资源
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExited(DragExitedEvent evt)
    {
        // 监听用户拖拽的是否是动画
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            // 放置动画资源
            // 当前选中的位置能否放置动画
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);

            bool canPlace = true;
            int durationFrame = -1;      // -1代表可以用原本AnimationCLip的持续时间
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in AnimationData.FrameData)
            {
                // 不允许选中帧在TrackItem中间
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Key + item.Value.DurationFrame)
                {
                    canPlace = false;
                    break;
                }
                // 找到右侧的最近的trackitem
                if (item.Key > selectFrameIndex)
                {
                    int tempOffset = item.Key - selectFrameIndex;
                    if (tempOffset < currentOffset)
                    {
                        currentOffset = tempOffset;
                        nextTrackItem = item.Key;
                    }
                }
            }
            // 实际放置
            if (canPlace)
            {
                // 右边有其他trackitem,要考虑track不能重叠的问题 
                if (nextTrackItem != -1)
                {
                    int offset = clipFrameCount - currentOffset;
                    if (offset < 0)
                        durationFrame = clipFrameCount;
                    else
                        durationFrame = currentOffset;
                }
                else
                    durationFrame = clipFrameCount;

                // 构建动画数据
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clip,
                    DurationFrame = durationFrame,
                    TransitionTime = 0.25f
                };

                // 保存新增的动画数据
                AnimationData.FrameData.Add(selectFrameIndex, animationEvent);
                SkillEditorWindow.Instance.SaveConfig();

                // 同步修改编辑器视图
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetIndex">左帧或右帧帧的目标位置帧</param>
    /// <param name="selfIndex">左帧的初始位置</param>
    /// <returns></returns>
    public bool CheckFrameIndexOnDrag(int targetIndex, int selfIndex, bool isleft)
    {
        foreach (var item in AnimationData.FrameData)
        {
            // 规避拖拽时考虑自身
            if (selfIndex == item.Key && isleft) continue;

            // 向左 && 原先在其右 && 目标帧位在其右
            if (isleft && selfIndex > item.Key && targetIndex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
            // 向右 && 原先在其左 && 目标帧位在其左
            else if (!isleft && selfIndex < item.Key && targetIndex > item.Key)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 将原本oldIndex的数据变为newINdex
    /// </summary>
    /// <param name="oldIndex"></param>
    /// <param name="newIndex"></param>
    public void SetFrameIndex(int oldIndex, int newIndex)
    {
        if (AnimationData.FrameData.Remove(oldIndex, out SkillAnimationEvent animationEvent))
        {
            AnimationData.FrameData.Add(newIndex, animationEvent);
            trackItemDic.Remove(oldIndex, out AnimationTrackItem animationTrackItem);
            trackItemDic.Add(newIndex, animationTrackItem);
            SkillEditorWindow.Instance.SaveConfig();
        }
    }
    
    public override void DeleteTrackItem(int frameIndex)
    {
        AnimationData.FrameData.Remove(frameIndex);
        if (trackItemDic.Remove(frameIndex, out AnimationTrackItem item))
        {
            trackStyle.DeleteItem(item.itemStyle.root);
        }
        SkillEditorWindow.Instance.SaveConfig();
    }

    public override void OnConfigChanged()
    {
        foreach (var item in trackItemDic.Values)
        {
            item.OnConfigChanged();
        }
    }

    /// <summary>
    /// 获取角色在根运动动画某一帧时的位置
    /// </summary>
    public Vector3 GetPositionForRootMotion(int frameIndex, bool recover = false)
    {
        GameObject previewObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewObject.GetComponentInChildren<Animator>();
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        // 利用排序字典(对key的排序)
        SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameData);
        int[] keys = frameDataSortedDic.Keys.ToArray();
        Vector3 rootMotionTotalPos = Vector3.zero;

        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i];
            SkillAnimationEvent animationEvent = frameDataSortedDic[key];
            // 只考虑根运动配置的动画
            if (animationEvent.ApplyRootMotion == false) continue;
            int nextKeyFrame = 0; // 下一个动画的起始帧
            if (i + 1 < keys.Length)
                nextKeyFrame = keys[i + 1];
            else
                nextKeyFrame = SkillEditorWindow.Instance.SkillConfig.FrameCount;

            bool isBreak = false; // 标记是最后一次采样
            if (nextKeyFrame > frameIndex)
            {
                nextKeyFrame = frameIndex;
                isBreak = true;
            }
            // 持续的帧数 = 下一个动画的帧数 - 这个动画的开始时间
            int durationFrameCount = nextKeyFrame - key;
            if (durationFrameCount > 0)
            {
                // 动画资源的总帧数
                float clipFrameCount = animationEvent.AnimationClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate;
                // 计算总的播放进度
                float totalProgress = durationFrameCount / clipFrameCount;
                // 播放次数
                int playTimes = 1;
                // 最终不完整的一次播放
                float lastProgress = 0;
                // 只有循环动画才需要采样多次
                if (animationEvent.AnimationClip.isLooping)
                {
                    playTimes = (int)totalProgress;
                    lastProgress = totalProgress - (int)totalProgress;
                }
                // 不循环的动画，如果播放进度小于1，那么本身就是最后一个播放进度
                else
                {
                    if (totalProgress < 1f)
                    {
                        playTimes = 0;
                        lastProgress = totalProgress;
                    }
                }
                // 采样计算
                animator.applyRootMotion = true;
                if (playTimes >= 1)
                {
                    // 采样一次动画的完整进度
                    animationEvent.AnimationClip.SampleAnimation(previewObject, animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewObject.transform.position;
                    rootMotionTotalPos += samplePos * playTimes;
                }
                if (lastProgress > 0)
                {
                    // 采样一次动画的不完整进度(当前播放的部分)
                    animationEvent.AnimationClip.SampleAnimation(previewObject, animationEvent.AnimationClip.length * lastProgress);
                    Vector3 samplePos = previewObject.transform.position;
                    rootMotionTotalPos += samplePos;
                }
            }
            if (isBreak)
                break;
        }
        if (recover)
        {
            //UpdatePosture(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
            TickView(SkillEditorWindow.Instance.CurrentSelectFrameIndex);
        }
        return rootMotionTotalPos;
    }

    /// <summary>
    /// 更新姿态
    /// </summary>
    private  void UpdatePosture(int frameIndex)
    {
        GameObject previewObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewObject.GetComponent<Animator>();
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        // 找到距离这一帧左边最近的一个动画，即当前在播放的动画
        int currentOffset = int.MaxValue;
        int animationEventIndex = -1;
        foreach (var item in frameData)
        {
            if (item.Key < frameIndex && frameIndex - item.Key < currentOffset)
            {
                currentOffset = frameIndex - item.Key;
                animationEventIndex = item.Key;
            }
        }
        if (animationEventIndex != -1)
        {
            SkillAnimationEvent animationEvent = frameData[animationEventIndex];
            // 动画总帧数
            float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
            // 计算当前播放进度
            float progress = currentOffset / clipFrameCount;
            // 循环动画的处理
            if (progress > 1 && animationEvent.AnimationClip.isLooping)
            {
                progress -= (int)progress; // 只留小数部分
            }
            animator.applyRootMotion = animationEvent.AnimationClip.hasRootCurves;
            Skill_Player skillplayer = previewObject.GetComponent<Skill_Player>();
            skillplayer.SetMainWeaponHand(animationEvent.MainWeaponOnLeftHand);
            animationEvent.AnimationClip.SampleAnimation(previewObject, progress * animationEvent.AnimationClip.length);
        }

    }

    public override void TickView(int frameIndex)
    {
        GameObject previewObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        if (previewObject == null) return;
        UpdatePosture(frameIndex);
        previewObject.transform.position = GetPositionForRootMotion(frameIndex);
    }

    public override void Destroy()
    {
        trackStyle.Destroy();
    }
}
