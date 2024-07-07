public class Root1 : RootBase
{
    protected override void Start()
    {
        unlockThreshold = 5; // 5레벨이 되어야 해금
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
        return base.GetTotalLifeGeneration(); // 기본 클래스의 동작을 유지
    }
}