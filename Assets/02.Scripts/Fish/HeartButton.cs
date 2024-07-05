using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartButton : MonoBehaviour
{
    // 클릭하면 생명력을 얻을 수 있는 버블
    Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        LookCamera();
    }

    // 카메라를 보고 있도록 하지 않으면 버튼 자체가 회전해버림.
    private void LookCamera()
    {
        transform.LookAt(cam.transform);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,
                                              transform.rotation.eulerAngles.y + 180, transform.rotation.eulerAngles.z);
    }

    public void TouchHeartBubble()
    {
        // 재화를 획득한다.
        LifeManager.Instance.IncreaseWater(LifeManager.Instance.touchData.touchIncreaseAmount);
        // 사라진다.
        gameObject.SetActive(false);
    }
}
