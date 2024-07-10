using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    private List<HeartButton> heartBubbleList = new List<HeartButton>();
    private List<int> nowBubbleIdx = new List<int>();
    private readonly int maxHeartCount = 2;
    private int nowOnHeartIndex;
    private readonly float heartGenerateDelay = 2f;
    private int buttonIdx = 0;

    public void StartGenerateHeart()
    {
        StartCoroutine(GenerateHeart());
    }
    public void AddAnimalHeartBubbleList(HeartButton bubble)
    {
        heartBubbleList.Add(bubble);
        bubble.heartIdx = buttonIdx++;
    }

    public void HeartOnRandomAnimal()
    {
        foreach (int i in nowBubbleIdx)
        {
            Debug.Log(i);
        }
        
        // 동물이 없으면 돌아가지 않음.
        if (heartBubbleList.Count == 0) return;

        int randomIdx;

        // 1마리만 존재한다면 Idx는 0.
        if(heartBubbleList.Count == 1)
        {
            randomIdx = 0;
        }

        // 2마리 이상이라면. 랜덤 Idx
        else
        {
            randomIdx = Random.Range(0, buttonIdx);
            // 이미 있는 경우
            while (nowBubbleIdx.Contains(randomIdx))
            {
                Debug.Log(randomIdx);
                randomIdx = Random.Range(0, buttonIdx);
            }
        }
        nowBubbleIdx.Add(randomIdx);

        heartBubbleList[randomIdx].SetBubbleOn();
    }

    // 클릭한 동물의 인덱스를 가져와야 함.
    public void RemoveIdxFromBubbleList(int idx)
    {
        nowBubbleIdx.Remove(idx);
    }

    // 최초 실행을 위해.
    public void GenerateNewHeart()
    {
        StartCoroutine(GenerateHeart());
    }

    IEnumerator GenerateHeart()
    {
        yield return new WaitForSeconds(heartGenerateDelay);

        HeartOnRandomAnimal();
    }
}
