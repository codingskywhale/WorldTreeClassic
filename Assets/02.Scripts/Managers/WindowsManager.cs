using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowsManager : MonoBehaviour
{
    public static WindowsManager Instance { get; private set; } // 싱글톤 인스턴스
    public AnimalInfoWindow animalInfoWnd;
    public CreateAnimalWindow createAnimalWindow;
    public PictorialBookWindow bookWindow;

    private void Awake()
    {
        // 싱글톤 인스턴스 설정
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 이미 인스턴스가 존재하면 중복 생성된 객체 파괴
        }
    }
}
