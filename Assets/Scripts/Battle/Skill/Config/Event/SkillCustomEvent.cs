public class SkillCustomEvent
{
    public SkillEventType EventType;
    public string CustomEventName;
    public int IntArg;
    public float FloatArg;
    public string StringArg;
    public UnityEngine.Object ObjectArg;
}

public enum SkillEventType
{
    Custom,
    CanSkillRelease,
    CanRotate,
    CanNotRotate,
    AddBuff,
}
