using JKFrame;
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
    [SerializeField] private ICharacter target;
    [SerializeField] private CommandControllerBase commandController;
    [SerializeField] private HitTargetStatus hitTargetStatus;
    [SerializeField] private AttackData curAttackData;
    public CharacterController CharacterController { get => characterController; }
    public GameCharacter_SkillBrainBase SkillBrain { get => skillBrain; }
    public CharacterConfig CharacterConfig { get => characterConfig; }
    public Animation_Controller Animation_Controller { get => view.Animation; }

    public Transform ModelTransform { get => view.transform; }
    public CharacterProperties CharacterProperties { get => characterProperties; }
    public BuffController BuffController { get => buffController; }

    public float WalkSpeed { get => characterConfig.WalkSpeed; }
    public float RunSpeed { get => characterConfig.RunSpeed; }
    public float RotateSpeed { get => characterConfig.RotateSpeed; }
    public ICharacter Target { get => target; }
    public CommandControllerBase CommandController { get => commandController; }

    public HitTargetStatus HitTargetStatus { get => hitTargetStatus; set { hitTargetStatus = value; } }
    public GameCharacterState GameCharacterState { get => gameCharacterState; }
    public AttackData CurAttackData { get => curAttackData; set { curAttackData = value; } }

    protected StateMachine stateMachine;
    protected GameCharacterState gameCharacterState;
    private CharacterConfig characterConfig;
    public Action<string> OnDieAction;

    public void Init(CharacterConfig characterConfig)
    {
        this.characterConfig = characterConfig;
        view.InitOnGame();
        characterProperties.Init(characterConfig);
        skillBrain.Init(this);

        // 初始化状态机
        stateMachine = ResSystem.GetOrNew<StateMachine>();
        stateMachine.Init(this);

        // 默认状态为Idle
        ChangeState(GameCharacterState.Idle);
        if(gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            target = GameObject.FindWithTag("Player").GetComponent<GameCharacter_Controller>();
        }

        hitTargetStatus = HitTargetStatus.None;

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
        view.Animation.PlaySingleAnimation(characterConfig.GetAnimationByName(animationClipName), speed, refreshAnimation, transitionFixedTime);
    }

    public void PlayBlendAnimation(string clip1Name, string clip2Name, Action<Vector3, Quaternion> rootMotionAction = null, float speed = 1, float transitionFixedTime = 0.25f)
    {
        if (rootMotionAction != null)
        {
            view.Animation.SetRootMotionAction(rootMotionAction);
        }
        AnimationClip clip1 = characterConfig.GetAnimationByName(clip1Name);
        AnimationClip clip2 = characterConfig.GetAnimationByName(clip2Name);

        view.Animation.PlayBlendAnimation(clip1, clip2, speed, transitionFixedTime);
    }

    public void Rotate(Vector3 input, float rotateSpeed = 0)
    {
        if (rotateSpeed == 0) rotateSpeed = RotateSpeed;
        // 获取相机的y旋转值
        float y = Camera.main.transform.rotation.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0, y, 0) * input;            // 让input也旋转y角度    --四元数与向量相乘：表示这个向量按照这个四元数进行旋转后得到的新的向量
                                                                        // 处理旋转
        ModelTransform.rotation = Quaternion.Slerp(ModelTransform.rotation, Quaternion.LookRotation(moveDir), Time.deltaTime * rotateSpeed);

    }

    public float GetAttackValue(SkillAttackDetectionEvent detectionEvent)
    {
        return characterProperties.atk.Total * detectionEvent.AttackHitConfig.AttackMultiply;
    }

    public virtual void BeHit(AttackData attackData)
    {
        // 受击表现
        if (hitTargetStatus == HitTargetStatus.Invincibility) return;
        Debug.Log(gameObject.name + $": 我被攻击了，来源是{attackData.source.ModelTransform.gameObject.name}, 伤害是{attackData.attackValue}. ");
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
}
