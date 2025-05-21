using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ���� ��menu��xx���� ��track�����
/// </summary>
public class AnimationTrack : SkillTrackBase
{
    private SkillSingleLineTrackStyle trackStyle;
    // <ÿ��item��ʼ֡��item>
    private Dictionary<int, AnimationTrackItem> trackItemDic = new Dictionary<int, AnimationTrackItem>();

    public SkillAnimationData AnimationData {get =>SkillEditorWindow.Instance.SkillConfig.SkillAnimationData;}
    public override void Init(VisualElement menuParent, VisualElement trackParent, float frameWidth)
    {
        base.Init(menuParent, trackParent, frameWidth);
        trackStyle = new SkillSingleLineTrackStyle();
        trackStyle.Init(menuParent, trackParent, "��������");
        trackStyle.contentRoot.RegisterCallback<DragUpdatedEvent>(OnDragUpdate);
        trackStyle.contentRoot.RegisterCallback<DragExitedEvent>(OnDragExited);
        ResetView();
    }

    public override void ResetView(float frameWidth)
    {
        base.ResetView(frameWidth);


        // ���ٵ�ǰ����
        foreach (var item in trackItemDic)
        {
            trackStyle.DeleteItem(item.Value.itemStyle.root);
        }
        trackItemDic.Clear();

        if (SkillEditorWindow.Instance.SkillConfig == null) return;

        // �������ݻ���TrackItem
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

    #region ��ק��Դ
    private void OnDragUpdate(DragUpdatedEvent evt)
    {
        // �����û���ק���Ƿ��Ƕ���
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
        }
    }

    private void OnDragExited(DragExitedEvent evt)
    {
        // �����û���ק���Ƿ��Ƕ���
        UnityEngine.Object[] objs = DragAndDrop.objectReferences;
        AnimationClip clip = objs[0] as AnimationClip;
        if (clip != null)
        {
            // ���ö�����Դ
            // ��ǰѡ�е�λ���ܷ���ö���
            int selectFrameIndex = SkillEditorWindow.Instance.GetFrameIndexByPos(evt.localMousePosition.x);

            bool canPlace = true;
            int durationFrame = -1;      // -1���������ԭ��AnimationCLip�ĳ���ʱ��
            int clipFrameCount = (int)(clip.length * clip.frameRate);
            int nextTrackItem = -1;
            int currentOffset = int.MaxValue;

            foreach (var item in AnimationData.FrameData)
            {
                // ������ѡ��֡��TrackItem�м�
                if (selectFrameIndex > item.Key && selectFrameIndex < item.Key + item.Value.DurationFrame)
                {
                    canPlace = false;
                    break;
                }
                // �ҵ��Ҳ�������trackitem
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
            // ʵ�ʷ���
            if (canPlace)
            {
                // �ұ�������trackitem,Ҫ����track�����ص������� 
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

                // ������������
                SkillAnimationEvent animationEvent = new SkillAnimationEvent()
                {
                    AnimationClip = clip,
                    DurationFrame = durationFrame,
                    TransitionTime = 0.25f
                };

                // ���������Ķ�������
                AnimationData.FrameData.Add(selectFrameIndex, animationEvent);
                SkillEditorWindow.Instance.SaveConfig();

                // ͬ���޸ı༭����ͼ
                CreateItem(selectFrameIndex, animationEvent);
            }
        }
    }
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetIndex">��֡����֡֡��Ŀ��λ��֡</param>
    /// <param name="selfIndex">��֡�ĳ�ʼλ��</param>
    /// <returns></returns>
    public bool CheckFrameIndexOnDrag(int targetIndex, int selfIndex, bool isleft)
    {
        foreach (var item in AnimationData.FrameData)
        {
            // �����קʱ��������
            if (selfIndex == item.Key && isleft) continue;

            // ���� && ԭ�������� && Ŀ��֡λ������
            if (isleft && selfIndex > item.Key && targetIndex < item.Key + item.Value.DurationFrame)
            {
                return false;
            }
            // ���� && ԭ�������� && Ŀ��֡λ������
            else if (!isleft && selfIndex < item.Key && targetIndex > item.Key)
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// ��ԭ��oldIndex�����ݱ�ΪnewINdex
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
    /// ��ȡ��ɫ�ڸ��˶�����ĳһ֡ʱ��λ��
    /// </summary>
    public Vector3 GetPositionForRootMotion(int frameIndex, bool recover = false)
    {
        GameObject previewObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewObject.GetComponentInChildren<Animator>();
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        // ���������ֵ�(��key������)
        SortedDictionary<int, SkillAnimationEvent> frameDataSortedDic = new SortedDictionary<int, SkillAnimationEvent>(frameData);
        int[] keys = frameDataSortedDic.Keys.ToArray();
        Vector3 rootMotionTotalPos = Vector3.zero;

        for (int i = 0; i < keys.Length; i++)
        {
            int key = keys[i];
            SkillAnimationEvent animationEvent = frameDataSortedDic[key];
            // ֻ���Ǹ��˶����õĶ���
            if (animationEvent.ApplyRootMotion == false) continue;
            int nextKeyFrame = 0; // ��һ����������ʼ֡
            if (i + 1 < keys.Length)
                nextKeyFrame = keys[i + 1];
            else
                nextKeyFrame = SkillEditorWindow.Instance.SkillConfig.FrameCount;

            bool isBreak = false; // ��������һ�β���
            if (nextKeyFrame > frameIndex)
            {
                nextKeyFrame = frameIndex;
                isBreak = true;
            }
            // ������֡�� = ��һ��������֡�� - ��������Ŀ�ʼʱ��
            int durationFrameCount = nextKeyFrame - key;
            if (durationFrameCount > 0)
            {
                // ������Դ����֡��
                float clipFrameCount = animationEvent.AnimationClip.length * SkillEditorWindow.Instance.SkillConfig.FrameRate;
                // �����ܵĲ��Ž���
                float totalProgress = durationFrameCount / clipFrameCount;
                // ���Ŵ���
                int playTimes = 1;
                // ���ղ�������һ�β���
                float lastProgress = 0;
                // ֻ��ѭ����������Ҫ�������
                if (animationEvent.AnimationClip.isLooping)
                {
                    playTimes = (int)totalProgress;
                    lastProgress = totalProgress - (int)totalProgress;
                }
                // ��ѭ���Ķ�����������Ž���С��1����ô����������һ�����Ž���
                else
                {
                    if (totalProgress < 1f)
                    {
                        playTimes = 0;
                        lastProgress = totalProgress;
                    }
                }
                // ��������
                animator.applyRootMotion = true;
                if (playTimes >= 1)
                {
                    // ����һ�ζ�������������
                    animationEvent.AnimationClip.SampleAnimation(previewObject, animationEvent.AnimationClip.length);
                    Vector3 samplePos = previewObject.transform.position;
                    rootMotionTotalPos += samplePos * playTimes;
                }
                if (lastProgress > 0)
                {
                    // ����һ�ζ����Ĳ���������(��ǰ���ŵĲ���)
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
    /// ������̬
    /// </summary>
    private  void UpdatePosture(int frameIndex)
    {
        GameObject previewObject = SkillEditorWindow.Instance.PreviewCharacterObj;
        Animator animator = previewObject.GetComponent<Animator>();
        Dictionary<int, SkillAnimationEvent> frameData = AnimationData.FrameData;

        // �ҵ�������һ֡��������һ������������ǰ�ڲ��ŵĶ���
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
            // ������֡��
            float clipFrameCount = animationEvent.AnimationClip.length * animationEvent.AnimationClip.frameRate;
            // ���㵱ǰ���Ž���
            float progress = currentOffset / clipFrameCount;
            // ѭ�������Ĵ���
            if (progress > 1 && animationEvent.AnimationClip.isLooping)
            {
                progress -= (int)progress; // ֻ��С������
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
