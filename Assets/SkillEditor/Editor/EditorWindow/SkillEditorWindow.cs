using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SkillEditorWindow : EditorWindow
{
    public static SkillEditorWindow Instance;
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("SkillEditor/SkillEditorWindow")]
    public static void ShowExample()
    {
        SkillEditorWindow wnd = GetWindow<SkillEditorWindow>();
        wnd.titleContent = new GUIContent("���ܱ༭��");
    }
    private VisualElement root;
    public void CreateGUI()
    {
        SkillClip.SetValidateAction(ResetView);

        Instance = this;
        root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        InitTopMenu();
        InitTimerShaft();
        InitConsole();
        InitContent();
        if (skillConfig != null) // ��Ȼ�༭������ܻ�����skillconfig�ģ�����Ϊ���±����ԭ����Щ���ݣ�����֡���ݿ򣩿��ܶ����ˣ�������¸���ֵ
        {
            SkillConfigObjectField.value = skillConfig;
            CurrentFrameCount = skillConfig.FrameCount;
        }
        else
        {
            CurrentFrameCount = 100;
        }
        if (currentPreviewCharacterPrefab != null) 
        {
            PreviewCharacterPrefabObjectField.value = currentPreviewCharacterPrefab;
        }
        if (currentPreviewCharacterObj != null) 
        {
            PreviewCharacterGameObjectField.value = currentPreviewCharacterObj ;
        }
        CurrentSelectFrameIndex = 0;
    }

    private void ResetView()
    {
        SkillClip temp = skillConfig;
        SkillConfigObjectField.value = null;
        SkillConfigObjectField.value = temp;
    }

    // �������ٻ���ã��ر�unity������ã������OnDisable
    //private void OnDestroy()

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }
    private void OnDisable()
    {
        SceneView.duringSceneGui -= OnSceneGUI;
        if (SkillConfig != null) SaveConfig();
    }

    #region TopMenu
    private const string skillEditorScenePath = "Assets/SkillEditor/SkillEditorScene.unity";
    private const string previewCharacterParentPath = "PreviewCharacterRoot";
    private string oldScenePath;
    private Button LoadEditorSceneButton;
    private Button LoadOldSceneButton;
    private Button SkillBasicButton;

    private ObjectField PreviewCharacterPrefabObjectField;
    private ObjectField PreviewCharacterGameObjectField;
    private ObjectField SkillConfigObjectField;
    private GameObject currentPreviewCharacterObj;
    private GameObject currentPreviewCharacterPrefab;
    public GameObject PreviewCharacterObj { get { return currentPreviewCharacterObj; } }
    private void InitTopMenu()
    {
        LoadEditorSceneButton = root.Q<Button>(nameof(LoadEditorSceneButton));
        LoadEditorSceneButton.clicked += LoadEditorSceneButtonClick;

        LoadOldSceneButton = root.Q<Button>("LoadOldSceneButton");
        LoadOldSceneButton.clicked += LoadOldSceneButtonClick;

        SkillBasicButton = root.Q<Button>("SkillBasicButton");
        SkillBasicButton.clicked += SkillBasicButtonClick;

        PreviewCharacterGameObjectField = root.Q<ObjectField>(nameof(PreviewCharacterGameObjectField));
        PreviewCharacterGameObjectField.RegisterValueChangedCallback(PreviewCharacterGameObjectFieldValueChanged);

        PreviewCharacterPrefabObjectField = root.Q<ObjectField>(nameof(PreviewCharacterPrefabObjectField));
        PreviewCharacterPrefabObjectField.RegisterValueChangedCallback(PreviewCharacterPrefabObjectFieldValueChanged);

        SkillConfigObjectField = root.Q<ObjectField>(nameof(SkillConfigObjectField));
        SkillConfigObjectField.objectType = typeof(SkillClip);
        SkillConfigObjectField.RegisterValueChangedCallback(SkillConfigObjectFieldValueChanged);
    }

    public bool OnEditorScene
    {
        get
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            return currentScenePath == skillEditorScenePath;
        }
    }

    // ���ر༭������
    private void LoadEditorSceneButtonClick()
    {
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        if (currentScenePath == skillEditorScenePath) return;
        oldScenePath = currentScenePath;
        EditorSceneManager.OpenScene(skillEditorScenePath);
    }

    // �ع�ɳ���
    private void LoadOldSceneButtonClick()
    {
        if (!string.IsNullOrEmpty(oldScenePath))
        {
            string currentScenePath = EditorSceneManager.GetActiveScene().path;
            if(currentScenePath == oldScenePath) return;
            EditorSceneManager.OpenScene(oldScenePath);
        }
        else Debug.LogWarning("����������");
    }

    // �鿴���ܻ�����Ϣ
    private void SkillBasicButtonClick()
    {
        if(skillConfig != null)
        {
            Selection.activeObject = skillConfig;
        }
    }
    // ��ɫԤ�����޸�
    private void PreviewCharacterPrefabObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {

        // ��������������ʵ����
        string currentScenePath = EditorSceneManager.GetActiveScene().path;
        if (currentScenePath != skillEditorScenePath)
        {
            PreviewCharacterPrefabObjectField.value = null;
            return;
        }
        if (evt.newValue == currentPreviewCharacterPrefab)
            return; 

        currentPreviewCharacterPrefab = (GameObject)evt.newValue;

        // ���پɵ�
        if (currentPreviewCharacterObj != null) DestroyImmediate(currentPreviewCharacterObj);

        Transform parent = GameObject.Find(previewCharacterParentPath).transform;
        if (parent!=null && parent.childCount > 0)
        {
            DestroyImmediate(parent.GetChild(0).gameObject);
        }

        // ʵ�����µ�
        if (evt.newValue != null)
        {
            currentPreviewCharacterObj = Instantiate(evt.newValue as GameObject, Vector3.zero, Quaternion.identity, parent);
            currentPreviewCharacterObj.transform.localRotation = Quaternion.Euler(0,0,0);
            PreviewCharacterGameObjectField.value = currentPreviewCharacterObj;
            if (currentPreviewCharacterObj.GetComponent<Skill_Player>() == null)
            {
                currentPreviewCharacterObj.AddComponent<Skill_Player>();
            }
        }

    }

    // ��ɫԤ�������޸�
    private void PreviewCharacterGameObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        currentPreviewCharacterObj = (GameObject)evt.newValue;
    }

    // ���������޸�
    private void SkillConfigObjectFieldValueChanged(ChangeEvent<UnityEngine.Object> evt)
    {
        SaveConfig();
        skillConfig = evt.newValue as SkillClip;
        CurrentSelectFrameIndex = 0;

        if (skillConfig == null)
        {
            CurrentFrameCount = 100;
        }
        else
        {
            CurrentFrameCount = skillConfig.FrameCount;
        }
        // ���»���:ˢ�¹��
        ResetTrack();
    }
    #endregion

    #region TimerShaft
    private IMGUIContainer timerShaft;
    private IMGUIContainer selectLine;
    private VisualElement contentContainer;
    private VisualElement contentViewPort;

    private int currentSelectFrameIndex = -1;
    public int CurrentSelectFrameIndex
    {
        get =>currentSelectFrameIndex;
        private set
        {
            int old = currentSelectFrameIndex;
            // ���������Χ���������֡
            if(value > currentFrameCount) CurrentFrameCount = value;

            currentSelectFrameIndex = Mathf.Clamp(value, 0, CurrentFrameCount);
            CurrentFrameField.value = currentSelectFrameIndex;

            if (old != currentSelectFrameIndex)
            {
                UpdateTimerShaftView();
                TickSkill();
            }
        }
    }

    /// <summary>
    /// һ�����ܵ���֡������Ӧskillconfig.framecount
    /// </summary>
    private int currentFrameCount;
    public int CurrentFrameCount
    {
        get => currentFrameCount;
        set
        {
            currentFrameCount = value;
            FrameCountField.value = currentFrameCount;
            if (skillConfig != null)
            {
                skillConfig.FrameCount = currentFrameCount;
            }
            // Content����ĳߴ�仯
            UpdateContentSize();
        }
    }

    // ��ǰ���������ƫ������
    private float contentOffsetPos { get => Mathf.Abs(contentContainer.transform.position.x); }
    // ѡ��֡��λ��(����ƫ������Ĭ��λ��)
    private float currentSelectFramePos { get => currentSelectFrameIndex * skillEditorConfig.frameUnitWidth; }
    private bool timerShaftIsOnMouseEnter = false;
    private void InitTimerShaft()
    {
        ScrollView MainContentView = root.Q<ScrollView>("MainContentView");
        contentContainer = MainContentView.Q<VisualElement>("unity-content-container");
        contentViewPort = MainContentView.Q<VisualElement>("unity-content-viewport");
        
        timerShaft = root.Q<IMGUIContainer>("TimerShaft");
        timerShaft.onGUIHandler = DrawTimerShaft;
        timerShaft.RegisterCallback<WheelEvent>(TimerShaftWheel);
        timerShaft.RegisterCallback<MouseDownEvent>(TimerShaftMouseDown);
        timerShaft.RegisterCallback<MouseUpEvent>(TimerShaftMouseUp);
        timerShaft.RegisterCallback<MouseMoveEvent>(TimerShaftMouseMove);
        timerShaft.RegisterCallback<MouseOutEvent>(TimerShaftMouseOut);

        selectLine = root.Q<IMGUIContainer>("SelectLine");
        selectLine.onGUIHandler = DrawSelectLine;
    }

    private void DrawTimerShaft()
    {
        Handles.BeginGUI();
        Handles.color = Color.white;
        Rect rect = timerShaft.contentRect;
        
        // ��ʼ����
        int index = Mathf.CeilToInt(contentOffsetPos / skillEditorConfig.frameUnitWidth);
        // �����������ƫ��
        float startOffset = 0;
        if(index > 0) startOffset = skillEditorConfig.frameUnitWidth - (contentOffsetPos % skillEditorConfig.frameUnitWidth);

        int tickStep = SkillEditorConfig.MaxFrameWidthLv + 1 - (skillEditorConfig.frameUnitWidth / SkillEditorConfig.StandardFrameUnitWidth);
        tickStep = Mathf.Clamp(tickStep/2, 1, tickStep);

        for (float i = startOffset; i < rect.width; i+=skillEditorConfig.frameUnitWidth)
        {
            // ���Ƴ��������ı�
            if (index % tickStep == 0)
            {
                Handles.DrawLine(new Vector3(i, rect.height - 10), new Vector3(i, rect.height));
                string indexStr = index.ToString();
                GUI.Label(new Rect(i - indexStr.Length * 4.5f, 0, 35, 20), indexStr);
            }
            else
                Handles.DrawLine(new Vector3(i, rect.height - 5), new Vector3(i, rect.height));
            index += 1;
        }

        Handles.EndGUI();
    }

    private void TimerShaftWheel(WheelEvent evt)
    {
        int delta = (int)evt.delta.y;
        skillEditorConfig.frameUnitWidth = Mathf.Clamp(skillEditorConfig.frameUnitWidth - delta, SkillEditorConfig.StandardFrameUnitWidth, SkillEditorConfig.MaxFrameWidthLv * SkillEditorConfig.StandardFrameUnitWidth);
        UpdateTimerShaftView();
        UpdateContentSize();
        ResetTrack();
    }

    private void TimerShaftMouseDown(MouseDownEvent evt)
    {
        // �� ѡ���ߵ�λ�� ���� ֡ ��λ����
        timerShaftIsOnMouseEnter = true;
        IsPlaying = false;
        int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
        if(CurrentSelectFrameIndex != newValue)
            CurrentSelectFrameIndex = newValue;
    }

    private void TimerShaftMouseUp(MouseUpEvent evt)
    {
        timerShaftIsOnMouseEnter =false;
    }

    private void TimerShaftMouseMove(MouseMoveEvent evt)
    {
        if (timerShaftIsOnMouseEnter)
        {
            int newValue = GetFrameIndexByMousePos(evt.localMousePosition.x);
            if (CurrentSelectFrameIndex != newValue)
                CurrentSelectFrameIndex = newValue;
        }
    }

    private void TimerShaftMouseOut(MouseOutEvent evt)
    {
        timerShaftIsOnMouseEnter = false;
    }

    /// <summary>
    /// ������������ȡ֡����
    /// </summary>
    public int GetFrameIndexByMousePos(float x)
    {
        return GetFrameIndexByPos(x + contentOffsetPos);
    }

    public int GetFrameIndexByPos(float x)
    {
        return Mathf.RoundToInt(x / skillEditorConfig.frameUnitWidth);
    }

    private void DrawSelectLine()
    {
        // �жϵ�ǰѡ��֡�Ƿ�����ͼ��Χ��
        if (currentSelectFramePos >= contentOffsetPos)
        {
            Handles.BeginGUI();
            Handles.color = Color.white;
            float x = currentSelectFramePos - contentOffsetPos;
            Handles.DrawLine(new Vector3(x, 0), new Vector3(x, contentViewPort.contentRect.height + timerShaft.contentRect.height));
            Handles.EndGUI();
        }
    }

    private void UpdateTimerShaftView()
    {
        timerShaft.MarkDirtyLayout();
        selectLine.MarkDirtyLayout();
    }
    #endregion

    #region Console
    private Button PreviousFrameButton;
    private Button PlayButton;
    private Button NextFrameButton;
    private IntegerField CurrentFrameField;
    private IntegerField FrameCountField;

    private void InitConsole()
    {
        PreviousFrameButton = root.Q<Button>(nameof(PreviousFrameButton));
        PreviousFrameButton.clicked += PreviousFrameButtonClick;
        PlayButton = root.Q<Button>(nameof(PlayButton));
        PlayButton.clicked += PlayButtonButtonClick;
        NextFrameButton = root.Q<Button>(nameof(NextFrameButton));
        NextFrameButton.clicked += NextFrameButtonClick;

        CurrentFrameField = root.Q<IntegerField>(nameof(CurrentFrameField));
        CurrentFrameField.RegisterValueChangedCallback(CurrentFrameFieldValueChanged);
        FrameCountField = root.Q<IntegerField>(nameof(FrameCountField));
        FrameCountField.RegisterValueChangedCallback(FrameCountFieldValueChanged);
    }

    private void PreviousFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex -= 1;
    }

    private void PlayButtonButtonClick()
    {
        if (CurrentSelectFrameIndex == CurrentFrameCount)
            CurrentSelectFrameIndex = 0;
        IsPlaying = !IsPlaying;
    }

    private void NextFrameButtonClick()
    {
        IsPlaying = false;
        CurrentSelectFrameIndex += 1;
    }

    private void CurrentFrameFieldValueChanged(ChangeEvent<int> evt)
    {
        if (CurrentSelectFrameIndex != evt.newValue)
            CurrentSelectFrameIndex = evt.newValue;
    }

    private void FrameCountFieldValueChanged(ChangeEvent<int> evt)
    {
        if (CurrentFrameCount != evt.newValue)
            CurrentFrameCount = evt.newValue;
    }
    #endregion

    #region Config
    private SkillClip skillConfig;
    public SkillClip SkillConfig { get { return skillConfig; } }
    private SkillEditorConfig skillEditorConfig = new SkillEditorConfig();

    public void SaveConfig()
    {
        if (skillConfig != null)
        {
            EditorUtility.SetDirty(skillConfig);
            AssetDatabase.SaveAssetIfDirty(skillConfig);

            ResetTrackData();
        }
    }

    private void ResetTrackData()
    {
        // ��������һ������
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].OnConfigChanged();
        }
    }
    #endregion

    #region Track
    private VisualElement contentListView;
    private VisualElement trackMenuParent;
    private List<SkillTrackBase> trackList = new List<SkillTrackBase>();
    private void InitContent()
    {
        contentListView = root.Q<VisualElement>("ContentListView");
        trackMenuParent = root.Q<VisualElement>("TrackMenuList");

        ScrollView trackMenuScrollView = root.Q<ScrollView>("TrackMenuScrollView");
        ScrollView mainContentView = root.Q<ScrollView>("MainContentView");
        trackMenuScrollView.verticalScroller.valueChanged += value => 
        {
            mainContentView.verticalScroller.value = value;
        };
        mainContentView.verticalScroller.valueChanged += value => 
        {
            trackMenuScrollView.verticalScroller.value = value;
        };
        UpdateContentSize();
        InitTrack();
    }

    private void InitTrack()
    {
        if (skillConfig == null) return;
        InitEventTrack();
        InitAnimationTrack();
        InitAudioTrack();
        InitEffectTrack();
        InitAttackDetectionTrack();
    }

    private void InitAnimationTrack()
    {
        AnimationTrack animationTrack = new AnimationTrack();
        animationTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(animationTrack);
        getPositionForRootMotion = animationTrack.GetPositionForRootMotion;
    }

    private void InitAudioTrack()
    {
        AudioTrack audioTrack = new AudioTrack();
        audioTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(audioTrack);
    }

    private void InitEffectTrack()
    {
        EffectTrack effectTrack = new EffectTrack();
        effectTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(effectTrack);
    }

    private void InitAttackDetectionTrack()
    {
        AttackDetectionTrack atkDetectionTrack = new AttackDetectionTrack();
        atkDetectionTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(atkDetectionTrack);
    }

    private void InitEventTrack()
    {
        EventTrack eventTrack = new EventTrack();
        eventTrack.Init(trackMenuParent, contentListView, skillEditorConfig.frameUnitWidth);
        trackList.Add(eventTrack);
    }

    /// <summary>
    /// ���»��ƣ����»�ȡframeWidth���������²�ַ�
    /// </summary>
    private void ResetTrack()
    {
        // �������ļ������ڣ���������й��
        if(skillConfig == null)
        {
            DestroyTracks();
        }
        else
        {
            // �������б���û�����ݣ�˵��û�й��������ʱ�û�������skillCOnfig,������Ҫinit���ƣ�
            if(trackList.Count == 0)
            {
                InitTrack();
            }
            // ������ͼ
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].ResetView(skillEditorConfig.frameUnitWidth);
            }
        }
    }

    private void DestroyTracks()
    {
        for (int i = 0; i < trackList.Count; i++)
        {
            trackList[i].Destroy();
        }
        trackList.Clear();
    }

    private void UpdateContentSize()
    {
        contentListView.style.width = skillEditorConfig.frameUnitWidth * CurrentFrameCount;
    }

    public void ShowTrackItemOnInspector(TrackItemBase trackItem, SkillTrackBase skilltrack)
    {
        SkillEditorInspector.SetTrackItem(trackItem, skilltrack);
        Selection.activeObject = this;
    }
    #endregion

    #region Preview
    private bool isPlaying;
    public bool IsPlaying 
    { 
        get { return isPlaying; }
        set
        {
            isPlaying = value;
            if (isPlaying)
            {
                startTime = DateTime.Now;
                startFrameIndex = currentSelectFrameIndex;

                // OnPlay
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnPlay(currentSelectFrameIndex);
                }
            }
            else
            {
                // OnStop
                for (int i = 0; i < trackList.Count; i++)
                {
                    trackList[i].OnStop();
                }
            }
        }
    }
    private DateTime startTime;
    private int startFrameIndex;
    private void Update()
    {
        if (isPlaying)
        {
            // �õ�ʱ���
            float time = (float)DateTime.Now.Subtract(startTime).TotalSeconds;

            // ȷ��ʱ�����֡��
            float frameRate;
            if (skillConfig != null)
                frameRate = skillConfig.FrameRate;
            else
                frameRate = skillEditorConfig.defaultFrameRate;

            // ����ʱ�����㵱ǰ��ѡ��֡
            CurrentSelectFrameIndex = (int)(time * frameRate + startFrameIndex);

            // �������һ֡�Զ���ͣ
            if(CurrentSelectFrameIndex == CurrentFrameCount)
            {
                IsPlaying = false;
            }
        }
    }

    public void TickSkill()
    {
        // �������ܱ���
        if (skillConfig != null && currentPreviewCharacterObj != null)
        {
            for (int i = 0; i < trackList.Count; i++)
            {
                trackList[i].TickView(currentSelectFrameIndex);
            }
        }
    }

    private Func<int, bool, Vector3> getPositionForRootMotion;
    public Vector3 GetPositionForRootMotion(int frameIndex, bool recover = false) => getPositionForRootMotion(frameIndex, recover);

    #endregion

    #region Gizmo��SceneGUI
    [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Selected)]
    private static void DrawGizmos(Skill_Player skill_Player, GizmoType gizmoType)
    {
        if(Instance == null || Instance.currentPreviewCharacterObj == null) return;
        if (Instance == null || Instance.currentPreviewCharacterObj.GetComponent<Skill_Player>() != skill_Player) return;
        for (int i = 0; i < Instance.trackList.Count; i++)
        {
            Instance.trackList[i].DrawGizmos();
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (Instance == null || Instance.currentPreviewCharacterObj == null) return;
        for (int i = 0; i < Instance.trackList.Count; i++)
        {
            Instance.trackList[i].OnSceneGUI();
        }
    }
    #endregion
}

public class SkillEditorConfig
{
    public const int StandardFrameUnitWidth = 10; // Ĭ��֡��λ���
    public const int MaxFrameWidthLv = 10;     // ���֡��λ��ȼ���
    public int frameUnitWidth = 10;            // ��ǰ֡��λ���
    public float defaultFrameRate = 10;
}

