using UnityEngine;

public abstract class BuffEffectResolverBase : MonoBehaviour
{
    public abstract void Resolve(Buff buff, BuffEffectDataBase effectData);
}
