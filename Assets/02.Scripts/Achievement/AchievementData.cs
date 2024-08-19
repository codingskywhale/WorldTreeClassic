using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementData
{
    //현재 몇 레벨까지 달성했는지의 여부만 저장하면 된다.
    // float 곱연산이 있으므로 float형으로 저장.
    public List<float> nowLevelList = new List<float>();

    public void SetachievementList()
    {

    }
}
