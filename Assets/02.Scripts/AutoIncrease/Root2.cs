using System;
using TMPro;
using UnityEngine;

public class Root2 : RootBase
{
    protected override void Start()
    {
        baseLifeGeneration = 3;
        base.Start();
        LifeManager.Instance.RegisterRoot(this);
        UpdateUI();
    }

    protected override void GenerateLife()
    {
        float generatedLife = GetTotalLifeGeneration();
        InvokeLifeGenerated(generatedLife);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();
    }

    public override float GetTotalLifeGeneration()
    {
        return baseLifeGeneration;
    }
}
