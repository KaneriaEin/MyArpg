using JKFrame;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float destroyTime;
    private float destroyTimer;
    public bool autoDestroy = true;
    public bool isStartImme = true;
    private bool isFirstInit = true;
    [SerializeField] private ParticleSystem mainParticleSystem;
    [SerializeField] private ParticleSystem[] allParticleSystem;
    [SerializeField] private ParticleSystem[] needRotateParticleSystem;
    [SerializeField] private float[] originSimulationSpeed;
    public void Init(float rotation = 0, bool autoDestroy = true)
    {
        InitAllParticles();
        this.autoDestroy = autoDestroy;
        destroyTimer = destroyTime;
        if(needRotateParticleSystem != null && rotation != 0)
        {
            for(int i = 0; i < needRotateParticleSystem.Length; i++)
            {
                var main = needRotateParticleSystem[i].main;
                main.startRotation = rotation * Mathf.Deg2Rad;
                // Debug.Log($"设置了{rotation * Mathf.Deg2Rad}");
            }
            gameObject.transform.Rotate(0, 0, 180 - rotation, Space.Self);
        }
        if (mainParticleSystem != null && isStartImme)
        {
            mainParticleSystem.Stop();
            mainParticleSystem.Simulate(0.0001f, true, true, false);
            mainParticleSystem.Play();
        }
    }

    public void Init(float duration, float rotation = 0, bool autoDestroy = true)
    {
        InitAllParticles();
        destroyTime = duration;
        destroyTimer = destroyTime;
        this.autoDestroy = autoDestroy;
    }

    private void InitAllParticles()
    {
        if (isFirstInit == true)
        {
            // 初次init，缓存全particle引用和速度初值
            isFirstInit = false;
            allParticleSystem = GetComponentsInChildren<ParticleSystem>(true);
            originSimulationSpeed = new float[allParticleSystem.Length];
            for (int i = 0; i < allParticleSystem.Length; i++)
            {
                originSimulationSpeed[i] = allParticleSystem[i].main.simulationSpeed;
            }
        }
        else
        {
            ResetSimulationSpeed();
        }
    }

    private void Update()
    {
        if (autoDestroy)
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                this.GameObjectPushPool();
            }
        }
    }

    public void ResetSimulationSpeed()
    {
        for (int i = 0;i < allParticleSystem.Length; i++)
        {
            var main = allParticleSystem[i].main;
            main.simulationSpeed = originSimulationSpeed[i];
        }
    }
}
