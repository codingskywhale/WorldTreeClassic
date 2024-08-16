using UnityEngine;
using System.Collections.Generic;

public class ObjectPool : MonoBehaviour
{
    // 오브젝트 풀 데이터를 정의할 데이터 모음 정의
    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    public List<Pool> Pools;
    public Dictionary<string, List<GameObject>> PoolDictionary;
    public Transform objectPoolTr;

    private void Awake()
    {
        // 인스펙터창의 Pools를 바탕으로 오브젝트풀을 만들 것. 
        // 오브젝트풀은 오브젝트마다 따로이며, pool 개수를 넘어가면 강제로 끄고 새로운 오브젝트에게 할당.
        PoolDictionary = new Dictionary<string, List<GameObject>>();
        foreach (var pool in Pools)
        {
            // List로 풀 생성
            List<GameObject> objectPool = new List<GameObject>();
            for (int i = 0; i < pool.size; i++)
            {
                // Awake하는 순간 오브젝트풀에 들어갈 Instantitate 일어나기 때문에 터무니없는 사이즈 조심
                GameObject obj = Instantiate(pool.prefab, objectPoolTr);
                obj.SetActive(false);
                objectPool.Add(obj);
            }
            // 접근이 편한 Dictionary에 등록
            PoolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag)
    {
        // 애초에 Pool이 존재하지 않는 경우
        if (!PoolDictionary.ContainsKey(tag))
            return null;

        // 제일 오래된 객체를 재활용
        List<GameObject> objectPool = PoolDictionary[tag];

        // 사용 가능한 객체를 찾음
        GameObject obj = objectPool.Find(o => !o.activeInHierarchy);

        if (obj != null)
        {
            obj.SetActive(true);
            return obj;
        }

        // 모든 객체가 사용 중이라면 제일 오래된 객체를 재활용
        obj = objectPool[0];
        objectPool.RemoveAt(0);
        objectPool.Add(obj);

        obj.SetActive(true);
        return obj;
    }
}