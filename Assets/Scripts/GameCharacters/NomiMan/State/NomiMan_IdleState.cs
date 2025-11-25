using UnityEngine;

public class NomiMan_IdleState : GameCharacterStateBase
{
    public override void Enter()
    {
        animation.AddAnimationEvent("IntoStunIdle", IntoStunIdle);
        animation.AddAnimationEvent("IntoIdle", IntoIdle);
        if (gameCharacter.CharacterProperties.InStun())
        {
            gameCharacter.PlayAnimation("StunIdle_Start");
        }
        else
        {
            gameCharacter.PlayAnimation("Idle");
        }
    }

    public override void Exit()
    {
        base.Exit();
        animation.RemoveAnimationEvent("IntoStunIdle", IntoStunIdle);
        animation.RemoveAnimationEvent("IntoIdle", IntoIdle);
    }

    public override void Update()
    {
        if (gameCharacter.CharacterProperties.InStun()) return;
        if (CheckAndEnterSkillState()) return;
        gameCharacter.CharacterController.Move(new Vector3(0, -9.8f * Time.deltaTime, 0));
        // ¼ì²âÍæ¼ÒµÄÊäÈë
        Vector2 cmdInput = gameCharacter.CommandController.GetMoveInput();
        float h = cmdInput.x;
        float v = cmdInput.y;

        if (h != 0 || v != 0)
        {
            // ÇÐ»»×´Ì¬
            gameCharacter.ChangeState(GameCharacterState.Move);
        }
    }

    private void IntoStunIdle()
    {
        gameCharacter.PlayAnimation("StunIdle", null, 1, true, 0.1f);
        gameCharacter.CanChangeState = true;
    }

    private void IntoIdle()
    {
        gameCharacter.PlayAnimation("Idle", null, 1, true, 0.1f);
        gameCharacter.CanChangeState = true;
    }
}
