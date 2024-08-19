using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Achievements : MonoBehaviour
{
    public Achievement_Tab[] achievements;
    public AchievementData achievementData;

    private void Awake()
    {
        achievementData = new AchievementData();
    }
    //창을 켰을 때 데이터를 불러와서 적용해주자.
    private void OnEnable()
    {
        SetInfo();
    }

    public void SetInfo()
    {
        achievements[0].SetText($"세계수 {achievements[0].NeedCount()}레벨 달성하기");
        achievements[0].nowCount = DataManager.Instance.touchData.touchIncreaseLevel;
        achievements[0].conditionProgressSlider.value = Mathf.Min(1, achievements[0].nowCount / achievements[0].NeedCount());

        achievements[1].SetText($"동물 {achievements[1].NeedCount()}마리 달성");
        achievements[1].nowCount = DataManager.Instance.animalGenerateData.totalAnimalCount;
        achievements[1].conditionProgressSlider.value = Mathf.Min(1, achievements[1].nowCount / achievements[1].NeedCount());

        achievements[2].SetText($"식물 레벨 합계 {achievements[2].NeedCount()}레벨 달성");
        achievements[2].nowCount = GetFlowerTotalLevel();
        achievements[2].conditionProgressSlider.value = Mathf.Min(1, GetFlowerTotalLevel() / achievements[2].NeedCount());


        //achievements[3].SetText($"게임 접속시간 100분 달성");

        achievements[4].SetText($"버블 터치 {achievements[4].NeedCount()} 달성");
        achievements[4].nowCount = ResourceManager.Instance.bubbleGeneratorPool.bubbleClickCount;
        achievements[4].conditionProgressSlider.value = Mathf.Min(1, achievements[4].nowCount / achievements[4].NeedCount());

        achievements[5].SetText($"도감 해제 {achievements[5].NeedCount()}마리 달성");
        achievements[5].nowCount = DataManager.Instance.animalGenerateData.allTypeCountDic.Keys.Count;
        achievements[5].conditionProgressSlider.value = Mathf.Min(1, achievements[5].nowCount / achievements[5].NeedCount());

        /*
        achievements[6].SetText($"광고 10회 보기");
        achievements[7].SetText($"스킬 20회 사용");
        achievements[8].SetText($"게임 접속 3회");
        */

        CheckButtonCondition();
    }

    private float GetFlowerTotalLevel()
    {
        float totalLevel = 0; 
        foreach (var flower in AutoObjectManager.Instance.flowers)
        {
            totalLevel += flower.flowerLevel;
        }    

        return totalLevel;
    }

    private void CheckButtonCondition()
    {
        foreach(var achievement in achievements)
        {
            achievement.SetButtonOnOff();
        }
    }
}
