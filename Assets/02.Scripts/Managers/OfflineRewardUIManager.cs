using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;
using System;

public class OfflineRewardUIManager : MonoBehaviour
{
    public GameObject offlineUIPanel; // 오프라인 UI 패널
    public TextMeshProUGUI offlineRewardText; // 오프라인 보상 텍스트
    public TextMeshProUGUI accumulatedLifeText; // 모은 생명력 텍스트
    public TextMeshProUGUI offlineDurationText; // 오프라인 기간 텍스트
    public Image fillAmountImage; // FillAmount 이미지

    private OfflineRewardManager offlineRewardManager;
    private BigInteger pendingLifeIncrease; // 확인 버튼을 눌렀을 때 지급될 생명력

    public void Initialize(OfflineRewardManager rewardManager)
    {
        offlineRewardManager = rewardManager;
    }

    public void ShowOfflineRewardUI(BigInteger totalLifeIncrease, double offlineDurationInSeconds, double maxOfflineDurationInSeconds)
    {
        pendingLifeIncrease = totalLifeIncrease; // 지급할 생명력 저장
        offlineRewardText.text = $"자리를 비운 사이에 ({BigIntegerUtils.FormatBigInteger(totalLifeIncrease)}) 생명력을 모았습니다.";
        accumulatedLifeText.text = BigIntegerUtils.FormatBigInteger(totalLifeIncrease);

        // 오프라인 기간을 분/최대 분으로 나타내기
        double offlineDurationInMinutes = offlineDurationInSeconds / 60;
        double maxOfflineDurationInMinutes = maxOfflineDurationInSeconds / 60;
        offlineDurationText.text = $"오프라인 기간: {Math.Floor(offlineDurationInMinutes)}/{Math.Floor(maxOfflineDurationInMinutes)} 분";

        fillAmountImage.fillAmount = (float)(offlineDurationInSeconds / maxOfflineDurationInSeconds);
        offlineUIPanel.SetActive(true);
    }

    public void HideOfflineRewardUI()
    {
        offlineUIPanel.SetActive(false);
    }

    public void ClaimOfflineReward()
    {
        if (pendingLifeIncrease > 0)
        {
            LifeManager.Instance.IncreaseWater(pendingLifeIncrease);
            pendingLifeIncrease = 0; // 보상 지급 후 초기화
            HideOfflineRewardUI();
        }
    }
}
