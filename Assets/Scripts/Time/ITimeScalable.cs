/// <summary>
/// 用于控制时间缩放的接口
/// </summary>
public interface ITimeScalable
{
    public TimeCategory TimeCategory { get; }
    public void SetTimeScale(float  timeScale);
}

public enum TimeCategory
{
    Default = 0,
    Player = 1,
    SmallEnemy = 2,
    EliteEnemy = 3,
    BossEnemy = 4,
    Projectile = 5,
}
