using JKFrame;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMono<EnemyManager>
{
    [SerializeField] private List<Enemy_Controller> enemies;
    [SerializeField] private Vector3[] spawnPostions;
    public void Init()
    {
        enemies = new List<Enemy_Controller>();
        spawnPostionsIdx = 0;
    }

    public void CreateEnemy(string prefabName, CombatEnemySpawnConfig config, Action<string> onDie, Vector3 position = default)
    {
        // go实例化
        GameObject enemyGo = PoolSystem.GetGameObject(prefabName);
        if (enemyGo == null)
        {
            enemyGo = ResSystem.InstantiateGameObject(prefabName, this.gameObject.transform);
            enemyGo.name = prefabName;
        }
        else
        {
            enemyGo.transform.SetParent(this.gameObject.transform, false);
        }

            // 初始化操作
        Enemy_Controller enemy_Controller = enemyGo.GetComponent<Enemy_Controller>();
        CharacterConfig characterConfig = ResSystem.LoadAsset<CharacterConfig>(enemy_Controller.characterConfigName);
        enemy_Controller.Init(characterConfig, onDie);
        if (position == default) // 若无指定则随机地点刷新
        {
            enemyGo.transform.position = GetSpawnPosition();
        }
        else
        {
            enemyGo.transform.position = position;
        }
        // 进场特效
        if(config.ShowUpEffect != null)
        {
            GameObject effectObj = PoolSystem.GetGameObject(config.ShowUpEffect.name);
            if (effectObj == null)
            {
                effectObj = GameObject.Instantiate(config.ShowUpEffect);
                effectObj.name = config.ShowUpEffect.name;
            }
            effectObj.transform.position = enemyGo.transform.position;
            effectObj.transform.rotation = Quaternion.Euler(enemyGo.transform.eulerAngles);
            StartCoroutine(AutoDestructEffectGameObject(3f, effectObj));
        }

        // 添加进链表记录
        enemies.Add(enemy_Controller);
    }

    public void RemoveEnemy(Enemy_Controller enemy_Controller)
    {
        enemies.Remove(enemy_Controller);
        PoolSystem.PushGameObject(enemy_Controller.gameObject);
    }

    [SerializeField] private int spawnPostionsIdx;
    private Vector3 GetSpawnPosition()
    {
        Vector3 pos = spawnPostions[spawnPostionsIdx];
        spawnPostionsIdx = (spawnPostionsIdx + 1) % spawnPostions.Length;
        return pos;
    }

    private IEnumerator AutoDestructEffectGameObject(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);
        obj.GameObjectPushPool();
    }

}
