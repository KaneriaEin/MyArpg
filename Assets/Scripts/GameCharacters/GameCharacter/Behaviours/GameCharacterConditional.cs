using BehaviorDesigner.Runtime.Tasks;

public class GameCharacterConditional : Conditional
{
    protected GameCharacter_Controller controller;
    public override void OnAwake()
    {
        controller = GetComponent<GameCharacter_Controller>();
    }
}
