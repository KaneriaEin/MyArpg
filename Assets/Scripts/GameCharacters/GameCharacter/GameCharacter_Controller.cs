using JKFrame;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

public class GameCharacter_Controller : MonoBehaviour, IStateMachineOwner ,ICharacter
{
    [SerializeField] private GameCharacter_SkillBrainBase skillBrain;
    [SerializeField] private GameCharacter_View view;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private CharacterProperties characterProperties;
    [SerializeField] private BuffController buffController;
    [SerializeField] private DamageController damageController;
    [SerializeField] private ICharacter target;
    [SerializeField] private CommandControllerBase commandController;
    [SerializeField] private HitTargetStatus hitTargetStatus;
    [SerializeField] protected HitTargetStatus defaultHitStatus;
    [SerializeField] protected int armorLevel;
    [SerializeField] protected int defaultArmorLevel;
    [SerializeField] protected BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
    [SerializeField] protected Enemy_Controller enemy_Controller;
    [SerializeField] private RimLightController rimLightController;

    public CharacterController CharacterController { get => characterController; }
    public Enemy_Controller Enemy_Controller { get => enemy_Controller; }
    public GameCharacter_SkillBrainBase SkillBrain { get => skillBrain; }
    public CharacterConfig CharacterConfig { get => characterConfig; }
    public Animation_Controller Animation_Controller { get => view.Animation; }

    public Transform ModelTransform { get => view.transform; }
    public Transform ModelCenter { get => view.pelvisTransform; }
    public CharacterProperties CharacterProperties { get => characterProperties; }
    public BuffController BuffController { get => buffController; }
    public DamageController DamageController { get => damageController; }
    public RimLightController RimLightController { get { return rimLightController; } }

    public float WalkSpeed { get => characterConfig.WalkSpeed; }
    public float RunSpeed { get => characterConfig.RunSpeed; }
    public float RotateSpeed { get => characterConfig.RotateSpeed; }
    public ICharacter Target { get => target; }
    public CommandControllerBase CommandController { get => commandController; }

    public HitTargetStatus HitTargetStatus { get => hitTargetStatus; set { hitTargetStatus = value; } }

    public void SetDefaultHitTargetStatus() { hitTargetStatus = defaultHitStatus; }
    public void SetDefaultArmorLevel() { armorLevel = defaultArmorLevel; }

    public GameCharacterState GameCharacterState { get => gameCharacterState; }

    public TimeCategory TimeCategory { get { return characterProperties.characterTimeCategory; } }
    protected float localTimeScale = 1f;
    public float LocalTimeScale { get { return localTimeScale; } }
    protected bool canChangeState = true;
    public bool CanChangeState { get { return canChangeState; } set { canChangeState = value; } }
    public BehaviorDesigner.Runtime.BehaviorTree BehaviorTree { get { return behaviorTree; } }

    public int ArmorLevel { get { return armorLevel; } set { armorLevel = value; } }

    protected StateMachine stateMachine;
    [ShowInInspector] protected GameCharacterState gameCharacterState;
    private CharacterConfig characterConfig;
    public Action<string> OnDieAction;

    public virtual void Init(CharacterConfig characterConfig)
    {
        this.characterConfig = characterConfig;
        view.InitOnGame();
        characterProperties.Init(characterConfig);
        skillBrain.Init(this);

        // 初始化状态机
        stateMachine = ResSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);

        // 初始化
        damageController.Init(this);

        if(rimLightController != null) rimLightController.Init(this);

        // 默认状态为Idle
        ChangeState(GameCharacterState.Idle);

        SetDefaultHitTargetStatus();
        SetDefaultArmorLevel();

        TimeManager.Instance.RegisterObject(this);
    }

    /// <summary>
    /// 给敌人角色用的初始化
    /// </summary>
    /// <param name="characterConfig"></param>
    /// <param name="enemy_Controller"></param>
    public virtual void Init(CharacterConfig characterConfig, Enemy_Controller enemy_Controller)
    {
        this.Init(characterConfig);
        this.enemy_Controller = enemy_Controller;
        if (gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            target = GameObject.FindWithTag("Player").GetComponent<GameCharacter_Controller>();
            GameCharacterBehaviorTreeInit();
        }

        TimeManager.Instance.RegisterObject(this);

    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="newState"></param>
    public virtual void ChangeState(GameCharacterState newState, bool reCurrstate = false)
    {
        this.gameCharacterState = newState;

    }

    /// <summary>
    /// 播放动画
    /// </summary>
    public void PlayAnimation(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        if (playAnim != null) StopCoroutine(playAnim);
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAnimation(characterConfig.GetAnimationByName(animationClipName), speed * localTimeScale, refreshAnimation, transitionFixedTime);
    }

    Coroutine playAnim = null;
    /// <summary>
    /// 播放动画，在动画结束后执行Action
    /// </summary>
    public void PlayAnimationSequentially(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f, Action action = null)
    {
        if (playAnim != null) StopCoroutine(playAnim);
        playAnim = StartCoroutine(PlayAnimationSequentially_Coroutine(animationClipName, rootMotionAction, speed, refreshAnimation, transitionFixedTime, action));
    }

    public IEnumerator PlayAnimationSequentially_Coroutine(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f, Action action = null)
    {
        if(rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAnimation(characterConfig.GetAnimationByName(animationClipName), speed * localTimeScale, refreshAnimation, transitionFixedTime);

        // 等待第一个动画播放完毕
        yield return new WaitForSeconds(characterConfig.GetAnimationByName(animationClipName).length);

        action?.Invoke();
    }

    /// <summary>
    /// 在Layer1播放动画
    /// </summary>
    public void PlayAnimation_Layer1(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAnimation_Layer1(characterConfig.GetAnimationByName(animationClipName), speed * localTimeScale, refreshAnimation, transitionFixedTime);
    }

    public void PlayBlendAnimation(string clip1Name, string clip2Name, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        AnimationClip clip1 = characterConfig.GetAnimationByName(clip1Name);
        AnimationClip clip2 = characterConfig.GetAnimationByName(clip2Name);

        view.Animation.PlayBlendAnimation(clip1, clip2, speed * localTimeScale, transitionFixedTime);
    }

    public void SetAnimationLayerWeight(int layer, float weight)
    {
        view.Animation.SetLayerWeight(layer, weight);
    }

    public void Rotate(Vector3 input, float rotateSpeed = 0)
    {
        if (rotateSpeed == 0) rotateSpeed = RotateSpeed;
        // 获取相机的y旋转值
        float y = Camera.main.transform.rotation.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;            // 让input也旋转y角度    --四元数与向量相乘：表示这个向量按照这个四元数进行旋转后得到的新的向量
                                                                        // 处理旋转
        ModelTransform.rotation = Quaternion.Slerp(ModelTransform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotateSpeed * localTimeScale);

    }

    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent)
    {
        return characterProperties.atk.Total * detectionEvent.AttackHitConfig.AttackMultiply;
    }

    public virtual void BeHit(AttackData attackData)
    {
        // 受击表现
        if (hitTargetStatus == HitTargetStatus.Invincibility) return;
        // Debug.Log(gameObject.name + $": 我被攻击了，来源是{attackData.source.ModelTransform.gameObject.name}，判定名称是{attackData.detectionEvent.TrackName}, 伤害是{attackData.attackValue}, 晕伤是{attackData.stunAttackValue}. ");
        damageController.TakeDamage(attackData);
    }

    public void OnSkillRotate()
    {
        Vector2 moveInput = commandController.GetMoveInput();
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            Rotate(new Vector3(moveInput.x, 0, moveInput.y));
        }
    }

    public virtual void AddBuff(BuffConfig buffConfig, int layer)
    {
        buffController.AddBuff(buffConfig, layer);
    }

    public virtual void RemoveBuff(BuffConfig buffConfig, int layer)
    {
        buffController.RemoveBuff(buffConfig, layer);
    }

    public virtual int GetBuffLayer(BuffConfig buffConfig)
    {
        return buffController.GetBuffLayer(buffConfig);
    }

    public void ChangeToIdleState()
    {
        ChangeState(GameCharacterState.Idle);
    }

    public void OnSkillMove(Vector3 deltaPosition)
    {
        CharacterController.Move(deltaPosition);
    }

    public void OnSkillRotate(Quaternion deltaRotation)
    {
        ModelTransform.rotation *= deltaRotation;
    }

    public void LockOnTarget(ICharacter hitTarget)
    {
        this.target = hitTarget;
    }

    public void UnLockOnTarget()
    {
        this.target = null;
    }

    #region 顿帧相关
    private Coroutine beHitFreezeCoroutine;
    public Action targetHitFreezeStart = null;
    public Action targetHitFreezeFinish = null;
    public Action<float> targetHitFreezeEvents = null;
    public void TargetHitFreeze(float time)
    {
        if (beHitFreezeCoroutine != null)
        {
            StopCoroutine(beHitFreezeCoroutine);
        }
        beHitFreezeCoroutine = StartCoroutine(TargetHitFreezeWait(time));
        targetHitFreezeEvents?.Invoke(time);
    }

    public virtual IEnumerator TargetHitFreezeWait(float time)
    {
        // TEST Debug.Log($"我被打中了，需要顿{time}s");
        float oldspeed = Animation_Controller.Speed;

        Animation_Controller.SetAnimationSpeed(0);
        targetHitFreezeStart?.Invoke();

        yield return new WaitForSeconds(time);

        targetHitFreezeFinish?.Invoke();
        Animation_Controller.SetAnimationSpeed(oldspeed * LocalTimeScale);
    }

    public void AddHitFreezeAction(Action startAction, Action finishAction)
    {
        targetHitFreezeStart += startAction;
        targetHitFreezeFinish += finishAction;
    }

    public void RemoveHitFreezeAction(Action startAction, Action finishAction)
    {
        targetHitFreezeStart -= startAction;
        targetHitFreezeFinish -= finishAction;
    }
    #endregion

    private void GameCharacterBehaviorTreeInit()
    {
        behaviorTree.ExternalBehavior = null;
        behaviorTree.ExternalBehavior = characterConfig.behaviorTree;
        
        behaviorTree.EnableBehavior();
    }

    public virtual void OnDie(string name)
    {
        UnLockOnTarget();
        if ((GameCharacter_Controller)PlayerManager.Instance.Player.Target == this)
        {
            CameraManager.Instance.LockOn();
        }
        behaviorTree.DisableBehavior();
        // behaviorTree.ExternalBehavior = null;
        TimeManager.Instance.UnregisterObject(this);
    }

    public virtual void PropertyAddHP(float hp)
    {
        CharacterProperties.AddHP(hp);
    }

    public virtual void PropertyAddMP(float mp)
    {
        CharacterProperties.AddMP(mp);
    }

    public virtual void PropertyAddSP(float sp)
    {
        CharacterProperties.AddSP(sp);
    }

    public virtual void PropertyAddULT(float sp)
    {
        CharacterProperties.AddULT(sp);
    }

    public virtual void PropertyAddStun(float stun)
    {
        CharacterProperties.AddStun(stun);
    }

    public virtual void PropertyAddThunderDebuff(float value, BuffConfig buff) { }

    public virtual void PropertyAddThunderExplo(float value, BuffConfig buff) { }

    public virtual void CharacterBattleEvent(CharacterBattleEventType eventType, CharacterBattleEventArg arg)
    {
        
    }

    public void SetTimeScale(float timeScale)
    {
        float oldScale = localTimeScale;
        localTimeScale = timeScale;

        if(Animation_Controller != null)
        {
            if(oldScale == 0)
                Animation_Controller.Speed = localTimeScale;
            else
                Animation_Controller.Speed = Animation_Controller.Speed / oldScale * localTimeScale;
        }

        if(skillBrain != null)
            skillBrain.Skill_Player.LocalTimeScale = timeScale;
    }
}
