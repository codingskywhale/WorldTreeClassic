using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animal : MonoBehaviour
{
    [SerializeField] private HeartButton heart;
    // 테스트용 
    public float heartOnDelay = 30f;

    public void HeartTouch()
    {
        heart.TouchHeartBubble();
        StartCoroutine(heartGenerateDelay());
    }
    IEnumerator heartGenerateDelay()
    {
        yield return new WaitForSeconds(heartOnDelay);
        heart.gameObject.SetActive(true);
    }
}
