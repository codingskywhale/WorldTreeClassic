using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Achievement_Tab : MonoBehaviour
{
    public Slider conditionProgressSlider;
    public float needCount;
    public float nowCount;
    public int nowLevel;
    public Image[] starImages;
    public TextMeshProUGUI explainText;
    public TextMeshProUGUI rewardText;

    public Button claimRewardButton;
    public BigInteger claimRewardAmount = 200;

    private int nowStarImageIdx = 0;
    private int nowColorIdx;
    private Color[] colors = { Color.red, Color.yellow, Color.green, Color.blue, Color.cyan };

    private void Start()
    {
        for(int i = 0; i < nowLevel; i++) 
            ApplyIconColorChange();
    }

    // 도전 과제 달성 시 표시되는 이미지의 색상을 변경
    public void ApplyIconColorChange()
    {
        //인덱스 초과 시 일단 끄기.
        if (nowColorIdx >= colors.Length) return;

        starImages[nowStarImageIdx++].GetComponent<Image>().color = colors[nowColorIdx];

        // 마지막에 도달하면
        if(nowStarImageIdx == 5)
        {
            nowStarImageIdx = 0;
            nowColorIdx++;
        }
    }

    public void SetText(string text)
    {
        explainText.text = text;
    }

    public float NeedCount()
    {
        return needCount * (nowLevel + 1);
    }

    public void LevelUP()
    {
        nowLevel++;
    }

    // 버튼 클릭시 수행해 주기.
    public void SetButtonOnOff()
    {
        if (nowCount >= NeedCount())
        {
            claimRewardButton.interactable = true;
        }
        else
        {
            claimRewardButton.interactable = false;
        }

        ClaimReward();
    }

    public void ClaimReward()
    {
        if (nowLevel > 5)
        {
            rewardText.text = "400";
            claimRewardAmount = 400;
        }

        LifeManager.Instance.diamond.diamondAmount += claimRewardAmount;
    }
}
