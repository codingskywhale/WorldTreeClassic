using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementData
{
    //현재 몇 레벨까지 달성했는지의 여부만 저장하면 된다.
    // float 곱연산이 있으므로 float형으로 저장.
    public List<float> nowLevelList;

    // 아래의 데이터는 별도로 저장이 필요한 값
    public int bubbleClickCount;
    public int ADCount;
    public int skillUseCount;
    public float playTime;

    public AchievementData() 
    {
        nowLevelList = new List<float>();

        for (int i = 0; i < DataManager.Instance.achievements.achievements.Length; i++)
        {
            nowLevelList.Add(1);
        }
    }
    public AchievementData(List<float> levelList)
    {
        nowLevelList = levelList;
    }

    public void SetachievementList(int idx, float nowLevel)
    {
        nowLevelList[idx] = nowLevel;
    }
}
