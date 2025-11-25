using JKFrame;
using System;
using System.Collections;
using UnityEngine;

public class GameCharacter_Controller : MonoBehaviour, IStateMachineOwner ,ICharacter, ITimeScalable
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
    [SerializeField] protected BehaviorDesigner.Runtime.BehaviorTree behaviorTree;
    [SerializeField] protected Enemy_Controller enemy_Controller;
    public CharacterController CharacterController { get => characterController; }
    public Enemy_Controller Enemy_Controller { get => enemy_Controller; }
    public GameCharacter_SkillBrainBase SkillBrain { get => skillBrain; }
    public CharacterConfig CharacterConfig { get => characterConfig; }
    public Animation_Controller Animation_Controller { get => view.Animation; }

    public Transform ModelTransform { get => view.transform; }
    public CharacterProperties CharacterProperties { get => characterProperties; }
    public BuffController BuffController { get => buffController; }
    public DamageController DamageController { get => damageController; }

    public float WalkSpeed { get => characterConfig.WalkSpeed; }
    public float RunSpeed { get => characterConfig.RunSpeed; }
    public float RotateSpeed { get => characterConfig.RotateSpeed; }
    public ICharacter Target { get => target; }
    public CommandControllerBase CommandController { get => commandController; }

    public HitTargetStatus HitTargetStatus { get => hitTargetStatus; set { hitTargetStatus = value; } }
    public GameCharacterState GameCharacterState { get => gameCharacterState; }

    public TimeCategory TimeCategory { get { return characterProperties.characterTimeCategory; } }
    protected float localTimeScale = 1f;
    public float LocalTimeScale { get { return localTimeScale; } }
    protected bool canChangeState = true;
    public bool CanChangeState { get { return canChangeState; } set { canChangeState = value; } }

    protected StateMachine stateMachine;
    protected GameCharacterState gameCharacterState;
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

        // 默认状态为Idle
        ChangeState(GameCharacterState.Idle);

        hitTargetStatus = HitTargetStatus.None;

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
        if(rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        view.Animation.PlaySingleAnimation(characterConfig.GetAnimationByName(animationClipName), speed * localTimeScale, refreshAnimation, transitionFixedTime);
    }

    /// <summary>
    /// 播放动画，在动画结束后执行Action
    /// </summary>
    public IEnumerator PlayAnimationSequentially(string animationClipName, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, bool refreshAnimation = false, float transitionFixedTime = 0.25f, Action action = null)
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
        Debug.Log(gameObject.name + $": 我被攻击了，来源是{attackData.source.ModelTransform.gameObject.name}，判定名称是{attackData.detectionEvent.TrackName}, 伤害是{attackData.attackValue}, 晕伤是{attackData.stunAttackValue}. ");
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

    public void AddBuff(BuffConfig buffConfig, int layer)
    {
        buffController.AddBuff(buffConfig, layer);
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

    public IEnumerator HitFreeze(float time)
    {
        // TEST Debug.Log($"我被打中了，需要顿{time}s");
        float oldspeed = Animation_Controller.Speed;
        Animation_Controller.SetAnimationSpeed(0);
        
        yield return new WaitForSeconds(time);
        Animation_Controller.SetAnimationSpeed(oldspeed);
    }

    private void GameCharacterBehaviorTreeInit()
    {
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
        behaviorTree.ExternalBehavior = null;
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

    public virtual void PropertyAddStun(float stun)
    {
        CharacterProperties.AddStun(stun);
    }

    public virtual void CharacterBattleEvent(CharacterBattleEventType eventType, CharacterBattleEventArg arg)
    {
        
    }

    public void SetTimeScale(float timeScale)
    {
        localTimeScale = timeScale;

        if(Animation_Controller != null)
            Animation_Controller.Speed = localTimeScale;

        if(skillBrain != null)
            skillBrain.Skill_Player.LocalTimeScale = timeScale;
    }
}
