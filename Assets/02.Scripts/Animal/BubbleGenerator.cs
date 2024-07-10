using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

public class BubbleGenerator : MonoBehaviour
{
    private List<HeartButton> heartBubbleList = new List<HeartButton>();
    public int nowHeartCount = 0;
    private readonly int maxHeartCount = 2;
    private int nowOnHeartIndex;

    public void AddAnimalHeartBubbleList(HeartButton bubble)
    {
        heartBubbleList.Add(bubble);
    }

    public void HeartOnRandomAnimal()
    {
        // 동물이 없거나 1마리면 돌아가지 않음.
        if (LifeManager.Instance.animalGenerateData.nowAnimalCount > 2)
        {
            // 하트가 이미 다 생성되었거나 랜덤으로 돌렸을 때 인덱스가 같지 않으면
            while (nowHeartCount != 2 && nowOnHeartIndex != Random.Range(0, heartBubbleList.Count))
            {
                nowOnHeartIndex = Random.Range(0, heartBubbleList.Count);
            }

            for (int i = nowHeartCount; i < maxHeartCount;)
            {
                heartBubbleList[Random.Range(0, heartBubbleList.Count)].SetBubbleOn();
                nowHeartCount++;
            }
        }
        else
        {
            heartBubbleList[Random.Range(0, heartBubbleList.Count)].SetBubbleOn();
        }
    }
}
