using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartButton : MonoBehaviour
{
    // 클릭하면 생명력을 얻을 수 있는 버블
    Camera cam;
    public int heartIdx;

    private BubbleClickSkill bubbleClickSkill;
    private void Awake()
    {
        cam = Camera.main;
    }
    private void Start()
    {
        bubbleClickSkill = FindObjectOfType<BubbleClickSkill>();
    }
    private void Update()
    {
        LookCamera();
    }


    // 카메라를 보고 있도록 하지 않으면 버튼 자체가 회전해버림.
    private void LookCamera()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(-transform.rotation.eulerAngles.x,
                                              transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
    }

    public void SetBubbleOn()
    {
        gameObject.SetActive(true);
    }
    public void SetBubbleOff()
    {
        gameObject.SetActive(false);
    }

    public void TouchHeartBubble()
    {
        // 화면 터치시 효과음 재생
        SoundManager.instance.PlaySFX(SoundManager.instance.sfxClips[0]);
        // 재화를 획득한다.
        LifeManager.Instance.IncreaseWater(LifeManager.Instance.touchData.touchIncreaseAmount);

        LifeManager.Instance.bubbleGenerator.RemoveIdxFromNowBubbleList(heartIdx);
        if (bubbleClickSkill != null)
        {
            bubbleClickSkill.RemoveBubbleFromQueue(gameObject);
        }
        // 사라진다.
        gameObject.SetActive(false);
        // 일정 시간 후 다시 활성화
        Invoke("ReactivateBubble", 5f);

    }
    private void ReactivateBubble()
    {
        gameObject.SetActive(true);
        if (bubbleClickSkill != null)
        {
            Debug.Log("ReactivateBubble: Adding bubble to queue");
            bubbleClickSkill.AddBubbleToQueue(gameObject);
        }
        else
        {
            Debug.LogError("ReactivateBubble: BubbleClickSkill not found");
        }
    }
}
