using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class AnimalGenerateWindow : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.CheckEnoughCost(0);
    }
}
