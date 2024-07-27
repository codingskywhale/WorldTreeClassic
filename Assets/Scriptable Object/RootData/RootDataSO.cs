using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;


[CreateAssetMenu(fileName = "RootDataSO", menuName = "RootData/Default", order = 0)]
public class RootDataSO : ScriptableObject
{
    public int unlockThreshold;
    public string baseLifeGenerationString;
    public string unlockCostString;
}
