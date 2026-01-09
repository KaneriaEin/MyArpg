using JKFrame;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float destroyTime;
    private float destroyTimer;
    [SerializeField] private ParticleSystem mainParticleSystem;
    [SerializeField] private ParticleSystem[] needRotateParticleSystem;
    public void Init(float rotation = 0)
    {
        destroyTimer = destroyTime;
        if(needRotateParticleSystem != null && rotation != 0)
        {
            for(int i = 0; i < needRotateParticleSystem.Length; i++)
            {
                var main = needRotateParticleSystem[i].main;
                main.startRotation = rotation * Mathf.Deg2Rad;
                // Debug.Log($"设置了{rotation * Mathf.Deg2Rad}");
            }
        }
        mainParticleSystem.Stop();
        mainParticleSystem.Simulate(0.0001f, true, true, false);
        mainParticleSystem.Play();
    }

    public void Init(float duration, float rotation = 0)
    {
        destroyTime = duration;
        destroyTimer = destroyTime;
    }

    private void Update()
    {
        destroyTimer -= Time.deltaTime;
        if( destroyTimer <= 0)
        {
            this.GameObjectPushPool();
        }
    }
}
