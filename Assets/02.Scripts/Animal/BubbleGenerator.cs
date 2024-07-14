using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    public List<HeartButton> heartBubbleList = new List<HeartButton>();
    private List<int> nowBubbleIdxList = new List<int>();
    private readonly int maxHeartCount = 2;
    public int selectedObjectIndex;
    private int nowOnHeartIndex;
    private readonly float heartGenerateDelay = 2f;
    private int buttonIdx = 0;

    public void AddAnimalHeartBubbleList(HeartButton bubble)
    {
        heartBubbleList.Add(bubble);
        bubble.heartIdx = buttonIdx++;
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

    public void HeartOnRandomAnimal()
    {
        // 동물이 없으면 돌아가지 않음.
        if (heartBubbleList.Count == 0) return;

        int randomIdx;

        // 1마리만 존재한다면 Idx는 0.
        if (heartBubbleList.Count == 1)
        {
            randomIdx = 0;
            nowBubbleIdxList.Add(randomIdx);
        }

        // 하트 버블이 2개 이상이라면. 전체 중 랜덤 Idx를 뽑아야 한다.
        else
        {
            if (nowBubbleIdxList.Count == 2) return;

            randomIdx = Random.Range(0, heartBubbleList.Count);
            // 현재 버블이 켜져 있는 오브젝트를 제외하기 위해.
            while (nowBubbleIdxList.Contains(randomIdx))
            {
                randomIdx = Random.Range(0, heartBubbleList.Count);
            }
            // 중복되지 않은 값을 현재 리스트에 넣어줌.
            nowBubbleIdxList.Add(randomIdx);
        }

        // 리스트에 없다?
        heartBubbleList[randomIdx].SetBubbleOn();
    }

    // 클릭한 동물의 인덱스를 가져와야 함.
    public void RemoveIdxFromNowBubbleList(int idx)
    {
        // 현재 버블이 켜져 있는 상태인 오브젝트일 때
        if (nowBubbleIdxList.Contains(idx))
        {
            nowBubbleIdxList.Remove(idx);
            GenerateNewHeart();
        }
        
        // 인덱스 밀림 방지
        for (int i = idx; i < nowBubbleIdxList.Count; i++)
        {
            if (nowBubbleIdxList[i] > idx)
            {
                nowBubbleIdxList[i]--;
            }
        }

        buttonIdx--;
    }
}
