using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGeneratorPool : MonoBehaviour
{
    // 버블을 동적으로 동물에게 달아주는 역할을 수행한다.
    // ResourceManager.Instance.ObjectPool.SpwanFromPool을 통해서 오브젝트를 가져온다.
    // 기존의 방식과는 다르게 On 되어있는 상태로 활성화를 시켜준다.
    // 동물을 생성할 때 1마리거나 2마리일 때는 무조건 활성화를 시켜주도록 한다.
    // 3마리 이상일 경우에는 현재 버블이 없어질 때 랜덤으로 버블을 부여하도록 한다.
    public List<GameObject> nowHeartBubbleList = new List<GameObject>();
    private readonly int maxHeartCount = 2;
    public readonly float heartGenerateDelay = 3f;
    private readonly string bubbleTr = "BubblePosition";

    void Start()
    {
        LifeManager.Instance.bubbleGenratorPool = this;
    }
    public void GenerateNewHeart()
    {
        StartCoroutine(GenerateHeart());
    }

    IEnumerator GenerateHeart()
    {
        yield return new WaitForSeconds(heartGenerateDelay);
        HeartOnRandomAnimal();
    }

    public void HeartOnRandomAnimal()
    {
        // 2개 이상인 경우 더이상 표시해 줄 필요가 없음.
        if (nowHeartBubbleList.Count >= 2) return;

        // 동물이 없을 때는 실행되지 않음.
        // 동물이 한 마리밖에 없는 경우에는 추가로 생성하면 안됨.
        if (DataManager.Instance.spawnData.animalObjectList.Count == nowHeartBubbleList.Count) return;
        
        
        int randomIdx = 0;

        randomIdx = Random.Range(0, DataManager.Instance.spawnData.animalObjectList.Count);

        // 가져오는게 성공할 시에는 이미 있다는 걸 의미함.
        while (DataManager.Instance.spawnData.animalObjectList[randomIdx].GetComponentInChildren<HeartButton>() != null)
        {
            randomIdx = Random.Range(0, DataManager.Instance.spawnData.animalObjectList.Count);
        }

        //하트 버블을 랜덤한 동물에게 달아주는 작업.
        GameObject go = ResourceManager.Instance.objectPool.SpawnFromPool("Bubble");
        go.GetComponentInChildren<HeartButton>(true).heartIdx = randomIdx;
        go.GetComponentInChildren<HeartButton>(true).gameObject.SetActive(true);
        go.transform.SetParent(DataManager.Instance.spawnData.animalObjectList[randomIdx].transform.Find(bubbleTr).transform);
        go.GetComponent<RectTransform>().localPosition = Vector3.zero;
        nowHeartBubbleList.Add(go);
    }

    // 버블을 제거할 때 발생할 메서드
    public void RemoveBubble(int idx)
    {
        for(int i = 0; i < nowHeartBubbleList.Count; i++)
        {
            GameObject go = nowHeartBubbleList[i];
            if(go.GetComponentInChildren<HeartButton>().heartIdx == idx)
            {
                nowHeartBubbleList.Remove(go);
                StartCoroutine(WaitTimeForBubble(go));
            }
        }
        
        GenerateNewHeart();
    }

    IEnumerator WaitTimeForBubble(GameObject go)
    {
        yield return new WaitForSeconds(2f);
        go.GetComponentInChildren<HeartButton>(true).gameObject.SetActive(true);
        go.SetActive(false);
        go.transform.SetParent(ResourceManager.Instance.transform);
    }
}