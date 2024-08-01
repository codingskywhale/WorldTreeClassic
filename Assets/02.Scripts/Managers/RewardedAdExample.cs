using System;
using GoogleMobileAds.Api;
using UnityEngine;

public class RewardedAdExample : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string adUnitId;

    public static RewardedAdExample Instance { get; private set; } // 싱글톤 인스턴스

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 게임 오브젝트가 씬 전환 시에도 파괴되지 않도록 설정
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { });

#if UNITY_ANDROID
        adUnitId = "ca-app-pub-3940256099942544/5224354917"; // Test ID
#elif UNITY_IPHONE
        adUnitId = "ca-app-pub-3940256099942544/1712485313"; // Test ID
#else
        adUnitId = "unexpected_platform";
#endif

        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // Create a new AdRequest
        AdRequest adRequest = new AdRequest();

        // Load a new rewarded ad
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            if (ad == null)
            {
                Debug.LogError("Rewarded ad failed to load.");
                return;
            }

            rewardedAd = ad;
            RegisterEventHandlers(rewardedAd);
        });
    }

    public void ShowRewardedAd(Action<Reward> onUserEarnedReward)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("User earned reward: " + reward.Type + ", amount: " + reward.Amount);
                onUserEarnedReward?.Invoke(reward);
                // 여기에 사용자에게 보상을 제공하는 로직을 추가합니다.
            });
        }
        else
        {
            Debug.Log("Rewarded ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Rewarded ad received a paid event.");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad impression recorded.");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd(); // 광고가 닫힌 후 새 광고를 로드합니다.
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad full screen content failed to open: " + error);
        };
    }
}
