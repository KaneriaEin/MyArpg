using UnityEngine;

public class PersonBSSkill2Behaviour : GameCharacter_SkillBehaviourBase
{
    [SerializeField] private int attackIndex = -1;
    [SerializeField] private int clipCount = 2;
    [SerializeField] private AnimationCurve Clip2Curve;
    [SerializeField] private float Clip2MaxDistance;

    public override SkillBehaviourBase DeepCopy()
    {
        return new PersonBSSkill2Behaviour()
        {
            attackIndex = -1,
            clipCount = 2,
            Clip2Curve = new AnimationCurve(Clip2Curve.keys),
            Clip2MaxDistance = 4.2f
        };
    }

    public override void Release()
    {
        base.Release();
        attackIndex += 1;
        if (attackIndex >= clipCount)
        {
            attackIndex = 0;
        }
        
        skill_Player.StartPlayerSkillConfig(this);
        skill_Player.PlaySkillClip(skillConfig.Clips[attackIndex]);
    }

    public override bool CheckRelease()
    {
        bool rc = true;
        if(attackIndex == -1)
        {
            rc = EnemyManager.Instance.EnemyController_GetSharedData(character.Enemy_Controller, "Skill2_UnionAtttack_Signal");
            if (rc) { Debug.Log("Skill2_UnionAtttack可以释放"); } else { Debug.Log("Skill2_UnionAtttack不允许"); }
        }
        return (rc && base.CheckRelease());
    }

    public override void OnRootMotion(Vector3 deltaPosition, Quaternion deltaRotation)
    {
        deltaPosition.y -= 9.8f * Time.deltaTime;
        float speedMultiplier = 1;
        if (attackIndex == 1)
        {
            if (character.Target != null)
            {
                float distance = Vector3.Distance(character.ModelTransform.position, character.Target.ModelTransform.position);
                float normalizedDistance = Mathf.Clamp01(distance / Clip2MaxDistance);
                speedMultiplier = Clip2Curve.Evaluate(normalizedDistance);
            }
        }
        owner.OnSkillMove(deltaPosition * speedMultiplier);
        owner.OnSkillRotate(deltaRotation);
    }

    public override void OnSkillClipEnd()
    {
        base.OnSkillClipEnd();
        owner.ChangeToIdleState();
    }

    public override void OnClipEndOrReleaseNewSkill()
    {
        base.OnClipEndOrReleaseNewSkill();
        if (attackIndex == 1) { EnemyManager.Instance.EnemyController_ReleaseSharedData(character.Enemy_Controller, "Skill2_UnionAtttack_Signal"); }
        attackIndex = -1;
    }

    public override void AfterSkillCustomEvent(SkillCustomEvent customEvent)
    {
        base.AfterSkillCustomEvent(customEvent);
        if (customEvent.EventType == SkillEventType.EnemyRPC)
        {
            character.Enemy_Controller.inRPC = true;
            RPC_DataInfo info = new RPC_DataInfo();
            info.source = character.Enemy_Controller;
            info.desPoses = new Vector3[2];
            info.desPoses[0] = character.transform.TransformPoint(new Vector3(-3f, 0, 2));
            info.desPoses[1] = character.transform.TransformPoint(new Vector3(3f, 0, 2));
            EnemyManager.Instance.EnemyController_RPC_Client(GameCharacter_RPCService.RPC_PersonBS_Skill_2, character.Enemy_Controller, info, GameCharacterType.PersonBS, 2);
        }
    }
}
