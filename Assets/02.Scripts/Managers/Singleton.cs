using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                // 현재 씬에서 해당 타입의 인스턴스를 찾습니다.
                instance = FindObjectOfType<T>();

                if (instance == null)
                {
                    // 씬에 해당 타입의 오브젝트가 없으면 새로운 GameObject를 생성합니다.
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<T>();
                    singletonObject.name = typeof(T).ToString();

                    // 씬 전환 시에도 파괴되지 않도록 설정합니다.
                    DontDestroyOnLoad(singletonObject);
                }
            }

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            // 싱글톤 인스턴스가 이미 존재하면 새로 생성된 오브젝트를 파괴합니다.
            Destroy(gameObject);
        }
    }
}
