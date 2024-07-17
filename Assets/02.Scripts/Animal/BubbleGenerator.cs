using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    public List<HeartButton> heartBubbleList = new List<HeartButton>();
    private List<HeartButton> nowBubbleList = new List<HeartButton>();
    private readonly int maxHeartCount = 2;
    private int nowOnHeartIndex;
    public readonly float heartGenerateDelay = 2f;
    private int buttonIdx = 0;

    public BubbleClickSkill bubbleClickSkill;

    private void Start()
    {
        bubbleClickSkill = FindObjectOfType<BubbleClickSkill>();
    }

    public List<HeartButton> GetNowBubbleList()
    {
        return nowBubbleList;
    }
    public void InitialBubbleSet()
    {
        if (heartBubbleList.Count > 0)
        {
            for (int i = 0; i < heartBubbleList.Count; i++)
            {
                GenerateNewHeart();
            }
        }
    }

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
            // 가장 첫번째에 띄워주기.
            if(nowBubbleList.Count == 0)
                nowBubbleList.Add(heartBubbleList[0].GetComponent<HeartButton>());
        }

        // 하트 버블이 2개 이상이라면. 전체 중 랜덤 Idx를 뽑아야 한다.
        else
        {
            // 현재 nowBubble이 2개라면 뽑을 필요가 없다. (이론상 나오면 안되는 경우긴함.)
            if (nowBubbleList.Count == 2) return;

            randomIdx = Random.Range(0, heartBubbleList.Count);
            // 현재 버블이 켜져 있는 오브젝트를 제외하기 위해.
            while (nowBubbleList.Contains(heartBubbleList[randomIdx].GetComponent<HeartButton>()))
            {
                randomIdx = Random.Range(0, heartBubbleList.Count);
            }
            // 중복되지 않은 값을 현재 버블 리스트에 넣어줌.
            nowBubbleList.Add(heartBubbleList[randomIdx].GetComponent<HeartButton>());
        }

        // 리스트에 없다?

        heartBubbleList[randomIdx].SetBubbleOn();
    }

    // nowBubbleList에서 해당 인덱스의 데이터를 제거. (터치한 경우 nowBubbleList에서 제거 시켜주는 기능)
    public void RemoveIdxFromNowBubbleList(int idx)
    {
       nowBubbleList.Remove(heartBubbleList[idx].GetComponent<HeartButton>());

        GenerateNewHeart();
    }
    
    // heartBubbleList에서 해당 인덱스의 데이터 제거 (넣기 기능 수행 시 heartBubbleList / nowBubbleList에서 모두 빼주는 기능)
    public void RemoveBubble(int idx)
    {
        HeartButton heartBubble = heartBubbleList[idx];     
        // 현재 버블이 떠 있는 오브젝트를 삭제하는 경우.
        if (nowBubbleList.Contains(heartBubble.GetComponent<HeartButton>()))
        {
            nowBubbleList.Remove(heartBubble.GetComponent<HeartButton>());

            GenerateNewHeart();
        }
        heartBubbleList.Remove(heartBubble);
        buttonIdx--;

        // 앞의 인덱스가 삭제되었을 때 해당 버튼의 인덱스도 당겨져야 함.
        foreach(HeartButton heartButton in heartBubbleList)
        {
            if(heartButton.heartIdx > idx)
                heartButton.heartIdx--;
        }
    }
}
