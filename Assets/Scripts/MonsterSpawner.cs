using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [System.Serializable]
    public class MonsterPool
    {
        public string name;       // 식별용 이름
        public ObjectPool pool;   // 각 몬스터 풀
    }

    public List<MonsterPool> monsterPools;

    [Header("스폰 범위")]
    public Vector2 spawnRangeMin; // 방의 좌하단
    public Vector2 spawnRangeMax; // 방의 우상단

    // 몬스터 이름으로 꺼내기
    public GameObject Spawn(string monsterName)
    {
        MonsterPool mp = monsterPools.Find(x => x.name == monsterName);
        if (mp == null) return null;

        GameObject obj = mp.pool.Get();
        if (obj != null)
        {
            obj.transform.position = GetRandomPosition();
        }
        return obj;

    }
    Vector2 GetRandomPosition()
    {
        float x = Random.Range(spawnRangeMin.x, spawnRangeMax.x);
        float y = Random.Range(spawnRangeMin.y, spawnRangeMax.y);
        return new Vector2(x, y);
    }
}
