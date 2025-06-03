using BehaviorDesigner.Runtime.Tasks;

public class GameCharacterAction : Action
{
    protected GameCharacter_Controller controller;
    protected EnemyInputManager inputManager;
    public override void OnAwake()
    {
        controller = GetComponent<GameCharacter_Controller>();
        inputManager = GetComponent<EnemyInputManager>();
    }
}
